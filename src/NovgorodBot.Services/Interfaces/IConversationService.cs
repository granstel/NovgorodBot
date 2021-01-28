using System.Threading.Tasks;
using NovgorodBot.Models.Internal;

namespace NovgorodBot.Services
{
    public interface IConversationService
    {
        Task<Response> GetResponseAsync(Request request);
    }
}