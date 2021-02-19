using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NovgorodBot.Models;
using NovgorodBot.Services.Extensions;

namespace NovgorodBot.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IDialogflowService _dialogflowService;
        private readonly IGeolocationService _geolocationService;
        private readonly ISkillsService _skillsService;
        private readonly IMapper _mapper;

        public ConversationService(
            IDialogflowService dialogflowService,
            IGeolocationService geolocationService,
            ISkillsService skillsService,
            IMapper mapper)
        {
            _dialogflowService = dialogflowService;
            _geolocationService = geolocationService;
            _skillsService = skillsService;
            _mapper = mapper;
        }

        public async Task<Response> GetResponseAsync(Request request)
        {
            var response = await TryGetResponseForGeolocationAsync(request);

            if (response == null)
            {
                if (string.IsNullOrEmpty(request.Text))
                {
                    request.Text = request.RequestType;
                }

                var dialog = await _dialogflowService.GetResponseAsync(request);

                response = _mapper.Map<Response>(dialog);

                if (dialog.IsLocationRejected())
                {
                    var template = dialog.Templates.FirstOrDefault();

                    response.Text = $"{template?.Rejected}{response.Text}";
                }

                response = TryGetResponseWithRelevantSkills(request, dialog, response);

                response.Text = TryGetTextForOldUser(request, dialog, response);
            }

            return response;
        }

        private string TryGetTextForOldUser(Request request, Dialog dialog, Response response)
        {
            if (request.NewSession == true && request.IsOldUser)
            {
                var template = dialog.Templates.FirstOrDefault();

                response.Text = $"{template?.WelcomeBack}{response.Text}";
            }

            return response.Text;
        }

        private Response TryGetResponseWithRelevantSkills(Request request, Dialog dialog, Response response)
        {
            if (!dialog.IsRelevantSkillsRequested())
            {
                return response;
            }

            var relevantSkills = GetRelevantSkills(dialog);

            if (relevantSkills?.Any() != true)
            {
                var area = _geolocationService.GetArea(request.Geolocation);
                relevantSkills = GetSkillsByArea(area);
            }

            if (relevantSkills.All(s => s.IsNotRelevant))
            {
                var template = dialog.Templates.FirstOrDefault();

                response.Text = $"{template?.NotAnyRelevantSkill}{response.Text}";
            }

            var buttons = GetButtons(relevantSkills);

            response.Buttons = buttons;

            return response;
        }

        private async Task<Response> TryGetResponseForGeolocationAsync(Request request)
        {
            if (request.Geolocation == null || !request.IsUserAllowGeolocation() || request.NewSession != true)
            {
                return null;
            }

            _dialogflowService.DeleteContextAsync(request.SessionId, "REQUESTLOCATION").Forget();

            var response = await GetResponseByLocationAsync(request);

            return response;
        }

        private ICollection<Skill> GetRelevantSkills(Dialog dialog)
        {
            dialog.Parameters.TryGetValue("LocationId", out string[] locationsIds);
            dialog.Parameters.TryGetValue("ActionsCategories", out string[] categoriesNames);

            var skills = new List<Skill>();

            if (locationsIds?.Any(l => !string.IsNullOrEmpty(l)) == true)
            {
                var skillsByLocations = GetSkillsByLocations(locationsIds);

                skills.AddRange(skillsByLocations);
            }

            if (categoriesNames?.Any(c => !string.IsNullOrEmpty(c)) == true)
            {
                var skillsByCategories = GetSkillsByCategories(categoriesNames);

                skills.AddRange(skillsByCategories);
            }

            skills = skills.DistinctBy(s => s.Name).ToList();

            return skills;
        }

        private async Task<Response> GetResponseByLocationAsync(Request request)
        {
            Response response;

            var area = _geolocationService.GetArea(request.Geolocation);

            if (area == null)
            {
                response = await GetNotInAnyAreaResponseAsync(request);

                return response;
            }

            var parameters = new Dictionary<string, string>
            {
                { "areaName", area.Name}
            };

            request.Text = "RelevantToLocation";

            var dialog = await _dialogflowService.GetResponseAsync(request, parameters);

            string responseText;

            var skills = GetSkillsByArea(area);

            if (skills.All(s => s.IsNotRelevant))
            {
                var template = dialog.Templates.FirstOrDefault();
                responseText = string.Format(dialog.Response, template?.NoAnySkillsForArea ?? string.Empty);
            }
            else
            {
                responseText = string.Format(dialog.Response, string.Empty);
            }

            var buttons = GetButtons(skills);

            response = new Response
            {
                Text = responseText,
                Buttons = buttons
            };

            response.Text = TryGetTextForOldUser(request, dialog, response);

            return await Task.FromResult(response);
        }

        private async Task<Response> GetNotInAnyAreaResponseAsync(Request request)
        {
            request.Text = "Start";

            var dialog = await _dialogflowService.GetResponseAsync(request);

            var template = dialog.Templates.FirstOrDefault();

            var text = $"{template?.NotInAnyArea}{dialog.Response}";

            if (request.NewSession == true && request.IsOldUser)
            {
                text = $"{template?.WelcomeBack}{text}";
            }

            var response = new Response
            {
                Text = text,
                Buttons = dialog.Buttons
            };

            return response;
        }

        private ICollection<Skill> GetSkillsByArea(GeoArea area)
        {
            var skills = _skillsService.GetSkills(area?.Id);

            return skills;
        }

        private ICollection<Skill> GetSkillsByCategories(ICollection<string> categories)
        {
            var skills = _skillsService.GetSkills(categories);

            return skills;
        }

        private ICollection<Skill> GetSkillsByLocations(ICollection<string> locationsIds)
        {
            var areaIds = locationsIds.Where(l => !string.IsNullOrEmpty(l)).Select(l =>
            {
                if (int.TryParse(l, out var id))
                {
                    return id;
                }

                return -1;

            }).Distinct().Where(a => a > -1).ToList();

            var skills = _skillsService.GetSkills(areaIds);

            return skills;
        }

        private Button[] GetButtons(ICollection<Skill> skills)
        {
            var buttons = skills.Select(skill => new Button { Text = skill.Name, Url = skill.Url }).ToArray();

            return buttons;
        }
    }
}
