using System;
using System.Collections.Generic;
using System.Linq;
using GranSteL.Helpers.Redis;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class SkillsService : ISkillsService
    {
        private const string CacheKey = "SKILLS";

        private readonly IRedisCacheService _cache;

        public SkillsService(IRedisCacheService cache)
        {
            _cache = cache;
        }

        private static readonly Random Rnd = new Random();

        public ICollection<Skill> GetSkills(int? areaId)
        {
            _cache.TryGet(CacheKey, out ICollection<Skill> skills);

            if (areaId == null)
            {
                return skills;
            }

            skills = FilterSkillsOrDefault(skills, s => s.Areas.Contains(areaId.GetValueOrDefault()));

            return skills;
        }

        public ICollection<Skill> GetSkills(ICollection<string> categories)
        {
            _cache.TryGet(CacheKey, out ICollection<Skill> skills);

            if (categories == null)
            {
                return skills;
            }

            categories = categories.Select(c => c.ToUpperInvariant()).ToList();

            skills = FilterSkillsOrDefault(skills, s => s.Categories.Any(c => categories.Contains(c.ToUpper())));

            return skills;
        }

        private ICollection<Skill> FilterSkillsOrDefault(ICollection<Skill> skills, Func<Skill, bool> predicate)
        {
            var filtered = skills
                .Where(predicate)
                .ToList();

            if (filtered.Any())
            {
                return filtered;
            }

            filtered = skills.Where(s => !s.IsLocationBinded).OrderBy(x => Rnd.Next()).Take(3).ToList();

            return filtered;
        }
    }
}