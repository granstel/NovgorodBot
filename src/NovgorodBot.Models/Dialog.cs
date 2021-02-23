using System;
using System.Collections.Generic;
using System.Linq;

namespace NovgorodBot.Models
{
    public class Dialog
    {
        public IDictionary<string, string[]> Parameters { get; set; }

        public bool EndConversation { get; set; }

        public bool ParametersIncomplete { get; set; }

        public string Response { get; set; }

        public string Action { get; set; }

        public ICollection<Button> Buttons { get; set; }

        public ICollection<Template> Templates { get; set; }

        public bool IsLocationRejected()
        {
            Parameters.TryGetValue("IsGeolocationRejected", out string[] isGeolocationRejected);

            var isRejected = isGeolocationRejected?.Take(1).Select(s =>
            {
                bool.TryParse(s, out var result);

                return result;
            }).FirstOrDefault();

            return isRejected == true;
        }

        public bool IsLocationRequested()
        {
            var result = Action?.Equals("REQUESTLOCATION", StringComparison.InvariantCultureIgnoreCase) == true;

            return result;
        }

        public bool IsRelevantSkillsRequested()
        {
            var result = Action?.Equals("SHOWRELEVANTSKILLS", StringComparison.InvariantCultureIgnoreCase) == true;

            return result;
        }
    }
}
