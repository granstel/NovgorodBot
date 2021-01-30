using System;
using System.Threading.Tasks;
using AutoMapper;
using GranSteL.Helpers.Redis;
using NovgorodBot.Services;
using NovgorodBot.Services.Extensions;
using Yandex.Dialogs.Models;
using Yandex.Dialogs.Models.Input;
using InternalModels = NovgorodBot.Models.Internal;

namespace NovgorodBot.Messengers.Yandex
{
    public class YandexService : MessengerService<InputModel, OutputModel>, IYandexService
    {
        private const string PingCommand = "ping";
        private const string PongResponse = "pong";

        private const string IsOldUserKey = "ISOLDUSER";

        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cache;

        public YandexService(IConversationService conversationService, IMapper mapper, IRedisCacheService cache) : base(conversationService, mapper)
        {
            _mapper = mapper;
            _cache = cache;
        }

        protected override InternalModels.Request Before(InputModel input)
        {
            var request = base.Before(input);

            _cache.TryGet($"{IsOldUserKey}:{input.Session.UserId}", out bool isOldUser);

            request.IsOldUser = isOldUser;

            if (isOldUser)
            {
                request.Text = IsOldUserKey;
            }

            return request;
        }

        protected override InternalModels.Response ProcessCommand(InternalModels.Request request)
        {
            InternalModels.Response response = null;

            if (PingCommand.Equals(request.Text, StringComparison.InvariantCultureIgnoreCase))
            {
                response = new InternalModels.Response { Text = PongResponse };
            }

            return response;
        }

        protected override async Task<OutputModel> AfterAsync(InputModel input, InternalModels.Response response)
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
