using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NovgorodBot.Models;
using NovgorodBot.Models.Internal;
using Enum = System.Enum;

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

                if (request.NewSession == true && request.IsOldUser)
                {
                    response.Text = $"С возвращением! {response.Text}";
                }

                return response;
            }

            var dialog = await _dialogflowService.GetResponseAsync(request);

            if (dialog?.Action?.Equals("REQUESTLOCATION", System.StringComparison.InvariantCultureIgnoreCase) == true)
            {
                response.RequestGeolocation = true;
            }

            if (dialog?.Action?.Equals("GETSKILLSBYCATEGORIES", System.StringComparison.InvariantCultureIgnoreCase) == true)
            {
                var categories = new List<ActionsCategories>();

                if (dialog.Parameters.TryGetValue("ActionsCategories", out string[] categoriesNames))
                {
                    foreach (var name in categoriesNames)
                    {
                        if (!Enum.TryParse(name, true, out ActionsCategories category))
                        {
                            continue;
                        }

                        categories.Add(category);
                    }

                }

                var buttons = GetSkillsButtons(categories);
                response.Buttons = buttons;
            }

            if (dialog?.Buttons?.Any() == true)
            {
                response.Buttons = dialog.Buttons;
            }

            response.Text = dialog.Response;
            response.Finished = dialog.EndConversation;

            return response;
        }

        private async Task<Response> GetResponseByLocationAsync(Request request)
        {
            Response response;

            var area = _geolocationService.GetArea(request.Geolocation);

            if (area == null)
            {
                request.Text = "Start";

                var dialog1 = await _dialogflowService.GetResponseAsync(request);

                response = new Response
                {
                    Text = dialog1.Response,
                    Buttons = dialog1.Buttons
                };

                return response;
            }

            var parameters = new Dictionary<string, string>
            {
                { "areaName", area.Name}
            };

            request.Text = "RelevantToLocation";

            var dialog = await _dialogflowService.GetResponseAsync(request, parameters);

            var buttons = GetSkillsButtons(area);

            response = new Response
            {
                Text = dialog.Response,
                Buttons = buttons
            };

            return await Task.FromResult(response);
        }

        private Button[] GetSkillsButtons(GeoArea area)
        {
            var skills = _skillsService.GetSkills(area?.Id);

            var buttons = skills.Select(skill => new Button { Text = skill.Name, Url = skill.Link }).ToArray();

            return buttons;
        }

        private Button[] GetSkillsButtons(ICollection<ActionsCategories> categories)
        {
            var skills = _skillsService.GetSkills(categories);

            var buttons = skills.Select(skill => new Button { Text = skill.Name, Url = skill.Link }).ToArray();

            return buttons;
        }
    }
}
