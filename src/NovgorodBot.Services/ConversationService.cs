using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NovgorodBot.Models;
using NovgorodBot.Models.Internal;

namespace NovgorodBot.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IDialogflowService _dialogflowService;
        private readonly IGeolocationService _geolocationService;
        private readonly ISkillsService _skillsService;

        public ConversationService(
            IDialogflowService dialogflowService,
            IGeolocationService geolocationService,
            ISkillsService skillsService)
        {
            _dialogflowService = dialogflowService;
            _geolocationService = geolocationService;
            _skillsService = skillsService;
        }

        public async Task<Response> GetResponseAsync(Request request)
        {
            //TODO: processing commands, invoking external services, and other cool asynchronous staff to generate response

            var response = new Response();

            if (request.Geolocation != null)
            {
                response = await GetResponseByLocationAsync(request);

                if (response != null)
                {
                    if (request.NewSession == true && request.IsOldUser)
                    {
                        response.Text = $"С возвращением! {response.Text}";
                    }

                    return response;
                }
            }

            var dialog = await _dialogflowService.GetResponseAsync(request);

            if (dialog?.Action?.Equals("REQUESTLOCATION", System.StringComparison.InvariantCultureIgnoreCase) == true)
            {
                response.RequestGeolocation = true;
            }

            response.Text = dialog.Response;

            if (request.NewSession == true && request.IsOldUser)
            {
                response.Text = $"С возвращением! {response.Text}";
            }

            response.Finished = dialog.EndConversation;

            return response;
        }

        private async Task<Response> GetResponseByLocationAsync(Request request)
        {
            Response response;

            var area = _geolocationService.GetArea(request.Geolocation);

            if (area == null)
            {
                response = new Response
                {
                    Text = "Чем тебе хотелось бы заняться?",
                    Buttons = new[]
                    {
                        new Button { Text = "Экскурсии"},
                        new Button { Text = "Квесты"},
                        new Button { Text = "Бары"},
                    }
                };

                return response;
            }

            var parameters = new Dictionary<string, string>
            {
                { "areaName", area.Name}
            };

            request.Text = "RelevantToLocation";

            var dialog = await _dialogflowService.GetResponseAsync(request, parameters);

            var names = _skillsService.GetSkills(area.Id);

            var buttons = names.Select(skill => new Button { Text = skill.Name, Url = skill.Link }).ToArray();

            response = new Response
            {
                Text = dialog.Response,
                Buttons = buttons
            };

            return await Task.FromResult(response);
        }
    }
}
