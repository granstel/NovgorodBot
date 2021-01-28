using System.Threading.Tasks;
using NovgorodBot.Models.Internal;

namespace NovgorodBot.Services
{
    public interface IDialogflowService
    {
        Task<Dialog> GetResponseAsync(Request request);
    }
}