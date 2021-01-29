using System.Collections.Generic;

namespace NovgorodBot.Services
{
    public interface ISkillsService
    {
        ICollection<string> GetSkillsNames(int areaId);
    }
}