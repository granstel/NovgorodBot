using System.Collections.Generic;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public interface ISkillsService
    {
        ICollection<Skill> GetSkills(int? areaId);
        
        ICollection<Skill> GetSkills(ICollection<string> categories);

        ICollection<Skill> GetSkills(ICollection<int> areasId);
    }
}