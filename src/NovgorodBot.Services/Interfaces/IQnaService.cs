using System.Threading.Tasks;

namespace NovgorodBot.Services
{
    public interface IQnaService
    {
        Task<string> GetAnswerAsync(string question);
    }
}