using System;
using System.Threading.Tasks;
using AutoMapper;
using GranSteL.Helpers.Redis;
using NLog;
using NovgorodBot.Services;
using NovgorodBot.Services.Extensions;
using Yandex.Dialogs.Models;
using Yandex.Dialogs.Models.Input;
using Request = NovgorodBot.Models.Request;
using Response = NovgorodBot.Models.Response;

namespace NovgorodBot.Messengers.Yandex
{
    public class YandexService : MessengerService<InputModel, OutputModel>, IYandexService
    {
        private const string PingCommand = "ping";
        private const string PongResponse = "pong";

        private const string IsOldUserKey = "ISOLDUSER";

        private const string ErrorAnswer = "Прости, у меня какие-то проблемы... Давай попробуем ещё раз";

        private readonly Logger _log = LogManager.GetLogger(nameof(YandexService));

        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cache;

        public YandexService(IConversationService conversationService, IMapper mapper, IRedisCacheService cache) : base(conversationService, mapper)
        {
            _mapper = mapper;
            _cache = cache;
        }

        protected override Request Before(InputModel input)
        {
            var request = base.Before(input);

            _cache.TryGet($"{IsOldUserKey}:{input.Session.UserId}", out bool isOldUser);

            request.IsOldUser = isOldUser;

            return request;
        }

        protected override Response ProcessCommand(Request request)
        {
            Response response = null;

            if (PingCommand.Equals(request.Text, StringComparison.InvariantCultureIgnoreCase))
            {
                response = new Response { Text = PongResponse };
            }

            return response;
        }

        public override async Task<OutputModel> ProcessIncomingAsync(InputModel input)
        {
            OutputModel result;

            try
            {
                result = await base.ProcessIncomingAsync(input);
            }
            catch (Exception e)
            {
                _log.Error(e);

                var response = new Response { Text = ErrorAnswer };

                result = await AfterAsync(input, response);
            }

            return result;
        }

        protected override async Task<OutputModel> AfterAsync(InputModel input, Response response)
        {
            var output = await base.AfterAsync(input, response);

            _mapper.Map(input, output);

            if(response?.RequestGeolocation == true)
            {
                output.InitRequestGeolocation();
            }

            _cache.AddAsync($"{IsOldUserKey}:{input.Session.UserId}", true).Forget();

            return output;
        }
    }
}
