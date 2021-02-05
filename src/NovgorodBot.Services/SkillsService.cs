using System;
using System.Collections.Generic;
using System.Linq;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class SkillsService : ISkillsService
    {
        private static readonly Random Rnd = new Random();

        private static readonly List<Skill> Skills = new List<Skill>
        {
            new Skill
            {
                Name = "\"Cувениры Великого Новгорода\"",
                Url = "https://dialogs.yandex.ru/store/skills/cd309398-suveniry-velikogo-novgoroda/activate?deeplink=true",
                Areas = new [] { 0 },
                Categories = new[] { "SOUVENIR" }
            },
            new Skill
            {
                Name = "\"Занимательная история Великого Новгорода\"",
                Url = "https://dialogs.yandex.ru/store/skills/12ef2083-sochinyal/activate?deeplink=true",
                Areas = new [] { 0 },
                Categories = new[] { "QUEST" }
            }
        };

        public ICollection<Skill> GetSkills(int? areaId)
        {
            if (areaId == null)
            {
                return Skills;
            }

            var skills = GetSkills(s => s.Areas.Contains(areaId.GetValueOrDefault()));

            return skills;
        }

        public ICollection<Skill> GetSkills(ICollection<string> categories)
        {
            if (categories == null)
            {
                return Skills;
            }

            categories = categories.Select(c => c.ToUpperInvariant()).ToList();

            var skills = GetSkills(s => s.Categories.Any(c => categories.Contains(c.ToUpper())));

            return skills;
        }

        private ICollection<Skill> GetSkills(Func<Skill, bool> predicate)
        {
            var skills = Skills
                .Where(predicate)
                .ToList();

            if (skills.Any())
            {
                return skills;
            }

            skills = Skills.Where(s => !s.IsLocationBinded).OrderBy(x => Rnd.Next()).Take(3).ToList();

            return skills;
        }
    }
}