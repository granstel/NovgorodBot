using System.Collections.Generic;

namespace NovgorodBot.Models
{
    public class Skill
    {
        public string Name { get; set; }
        
        public string Link { get; set; }

        public ICollection<int> Areas { get; set; }
    }
}