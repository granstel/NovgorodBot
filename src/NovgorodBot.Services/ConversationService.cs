using System;
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

            if(request.Geolocation != null)
            {
                response = await GetResponseByLocationAsync(request);
            }

            if (response != null)
            {
                return response;
            }

            var dialog = await _dialogflowService.GetResponseAsync(request);

            if(dialog?.Action?.Equals("REQUESTLOCATION", System.StringComparison.InvariantCultureIgnoreCase) == true)
            {
                response.RequestGeolocation = true;
            }

            response.Text = dialog.Response;
            response.Finished = dialog.EndConversation;

            return response;
        }

        private async Task<Response> GetResponseByLocationAsync(Request request)
        {
            return await Task.FromResult(default(Response));
        }
    }
}
