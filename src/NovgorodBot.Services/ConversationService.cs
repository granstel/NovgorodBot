using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NovgorodBot.Models;
using NovgorodBot.Models.Internal;
using NovgorodBot.Services.Extensions;

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

            if (request.RequestType.Equals("Geolocation.Allowed", StringComparison.InvariantCultureIgnoreCase) && request.Geolocation != null)
            {
                _dialogflowService.DeleteContextAsync(request.SessionId, "REQUESTLOCATION").Forget();

                response = await GetResponseByLocationAsync(request);

                return response;
            }

            if (request.RequestType.Equals("Geolocation.Rejected", StringComparison.InvariantCultureIgnoreCase))
            {
                request.Text = request.RequestType;
            }

            if (string.IsNullOrEmpty(request.Text))
            {
                request.Text = request.RequestType;
            }

            var dialog = await _dialogflowService.GetResponseAsync(request);

            response.Text = dialog.Response;

            if (dialog.Parameters.TryGetValue("IsGeolocationRejected", out string[] isGeolocationRejected))
            {
                var isRejected = isGeolocationRejected.Take(1).Select(s =>
                {
                    bool.TryParse(s, out bool result);

                    return result;
                }).FirstOrDefault();

                if (isRejected)
                {
                    var template = dialog.Templates.FirstOrDefault();

                    response.Text = $"{template?.Rejected}{response.Text}";
                }
            }

            if (dialog.Action?.Equals("REQUESTLOCATION", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                response.RequestGeolocation = true;
            }

            if (dialog.Action?.Equals("SHOWRELEVANTSKILLS", StringComparison.InvariantCultureIgnoreCase) == true)
            {
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
            }

            if (dialog.Buttons?.Any() == true && response.Buttons?.Any() != true)
            {
                response.Buttons = dialog.Buttons;
            }

            response.Finished = (dialog?.EndConversation).GetValueOrDefault();

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

            var skills = GetSkillsByArea(area);

            var buttons = GetButtons(skills);

            response = new Response
            {
                Text = dialog.Response,
                Buttons = buttons
            };

            if (request.NewSession == true && request.IsOldUser)
            {
                response.Text = $"С возвращением! {response.Text}";
            }

            return await Task.FromResult(response);
        }

        private async Task<Response> GetNotInAnyAreaResponseAsync(Request request)
        {
            request.Text = "Start";

            var dialog = await _dialogflowService.GetResponseAsync(request);

            var template = dialog.Templates.FirstOrDefault();

            var text = $"{template?.NotInAnyArea}{dialog.Response}";

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
