using System.Collections.Generic;
using System.Linq;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class SkillsService : ISkillsService
    {
        private static readonly List<Skill> Skills = new List<Skill>
        {
            {
                new Skill
                {
                    Name = "Сувениры Великого Новгорода",
                    Link = "https://dialogs.yandex.ru/store/skills/cd309398-suveniry-velikogo-novgoroda/activate?deeplink=true",
                    Areas = new [] { 0 }
                }
            }
        };

        public ICollection<string> GetSkillsNames(int areaId)
        {
            var names = Skills
                .Where(s => s.Areas.Contains(areaId))
                .Select(s => s.Name)
                .ToList();

            return names;
        }
    }
}