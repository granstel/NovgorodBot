using System.Collections.Generic;

namespace NovgorodBot.Models.Internal
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
    }
}
