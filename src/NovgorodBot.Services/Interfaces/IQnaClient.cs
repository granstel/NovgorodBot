using System.Threading.Tasks;
using NovgorodBot.Models.Qna;

namespace NovgorodBot.Services
{
    public interface IQnaClient
    {
        Task<Response> GetAnswerAsync(string knowledgeBase, string question);
    }
}