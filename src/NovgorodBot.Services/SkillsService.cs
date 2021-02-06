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

        private static readonly Random Rnd = new Random();

        private readonly IRedisCacheService _cache;

        public SkillsService(IRedisCacheService cache)
        {
            _cache = cache;
        }


        public ICollection<Skill> GetSkills(int? areaId)
        {
            _cache.TryGet(CacheKey, out ICollection<Skill> skills);

            skills = FilterSkillsOrDefault(skills, s => areaId.HasValue && s.Areas.Contains(areaId.GetValueOrDefault()));

            return skills;
        }

        public ICollection<Skill> GetSkills(ICollection<string> categories)
        {
            _cache.TryGet(CacheKey, out ICollection<Skill> skills);

            categories = categories.Select(c => c.ToUpperInvariant()).ToList();

            skills = FilterSkillsOrDefault(skills, s => s.Categories.Any(c => categories?.Any() == true && categories.Contains(c.ToUpper())));

            return skills;
        }

        public ICollection<Skill> GetSkills(ICollection<int> areasId)
        {
            _cache.TryGet(CacheKey, out ICollection<Skill> skills);

            skills = FilterSkillsOrDefault(skills, s => areasId?.Any(s.Areas.Contains) == true);

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

            filtered = skills
                .Where(s => !s.IsLocationBinded)
                .OrderBy(x => Rnd.Next())
                .Take(5)
                .Select(s =>
                {
                    s.IsNotRelevant = true;

                    return s;
                })
                .ToList();

            return filtered;
        }
    }
}