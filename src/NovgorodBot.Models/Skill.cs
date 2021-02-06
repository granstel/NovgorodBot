using System.Collections.Generic;

namespace NovgorodBot.Models
{
    public class Skill
    {
        public string Name { get; set; }
        
        public string Url { get; set; }

        /// <summary>
        /// Имеет отношение к конкретной локации
        /// </summary>
        public bool IsLocationBinded { get; set; }

        public ICollection<int> Areas { get; set; }

        public ICollection<string> Categories { get; set; }

        public bool IsNotRelevant { get; set; }
    }
}