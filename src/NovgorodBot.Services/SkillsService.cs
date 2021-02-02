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
                Name = "\"Cувениры Великого Новгорода\"",
                Link = "https://dialogs.yandex.ru/store/skills/cd309398-suveniry-velikogo-novgoroda/activate?deeplink=true",
                Areas = new [] { 0 },
                Categories = new[] {ActionsCategories.Souvenirs}
            },
            new Skill
            {
                Name = "\"Занимательная история Великого Новгорода\"",
                Link = "https://dialogs.yandex.ru/store/skills/12ef2083-sochinyal/activate?deeplink=true",
                Areas = new [] { 0 },
                Categories = new[] {ActionsCategories.Quest}
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

        public ICollection<Skill> GetSkills(ICollection<ActionsCategories> categories)
        {
            if (categories == null)
            {
                return Skills;
            }

            var names = Skills
                .Where(s => s.Categories.Any(categories.Contains))
                .ToList();

            return names;
        }
    }
}