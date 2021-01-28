using System.Threading.Tasks;
using NovgorodBot.Models.Internal;

namespace NovgorodBot.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IDialogflowService _dialogflowService;

        public ConversationService(IDialogflowService dialogflowService)
        {
            _dialogflowService = dialogflowService;
        }

        public async Task<Response> GetResponseAsync(Request request)
        {
            //TODO: processing commands, invoking external services, and other cool asynchronous staff to generate response

            Response response = new Response();

            var dialog = await _dialogflowService.GetResponseAsync(request);

            if(dialog?.Action?.Equals("REQUESTLOCATION", System.StringComparison.InvariantCultureIgnoreCase) == true)
            {
                response.RequestGeolocation = true;
            }

            response.Text = dialog.Response;
            response.Finished = dialog.EndConversation;

            return response;
        }
    }
}
