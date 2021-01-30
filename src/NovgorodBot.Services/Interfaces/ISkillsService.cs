using System.Collections.Generic;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public interface ISkillsService
    {
        ICollection<Skill> GetSkills(int areaId);
    }
}