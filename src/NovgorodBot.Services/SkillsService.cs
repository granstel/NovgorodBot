using System.Collections.Generic;
using System.Linq;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class SkillsService : ISkillsService
    {
        private static readonly List<Skill> Skills = new List<Skill>
        {
            new Skill
            {
                Name = "Купить сувениры",
                Link = "https://dialogs.yandex.ru/store/skills/cd309398-suveniry-velikogo-novgoroda/activate?deeplink=true",
                Areas = new [] { 0 }
            },
            new Skill
            {
                Name = "Послушать занимательную историю",
                Link = "https://dialogs.yandex.ru/store/skills/12ef2083-sochinyal/activate?deeplink=true",
                Areas = new [] { 0 }
            }
        };

        public ICollection<Skill> GetSkills(int? areaId)
        {
            if (areaId == null)
            {
                return Skills;
            }

            var names = Skills
                .Where(s => s.Areas.Contains(areaId.GetValueOrDefault()))
                .ToList();

            return names;
        }
    }
}