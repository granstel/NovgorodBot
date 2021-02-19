using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf.WellKnownTypes;
using NovgorodBot.Services.Configuration;
using NovgorodBot.Services.Extensions;
using NLog;
using NovgorodBot.Models;

namespace NovgorodBot.Services
{
    public class DialogflowService : IDialogflowService
    {
        private const string StartCommand = "/start";
        private const string ErrorCommand = "/error";

        private const string RelevantToLocationCommand = "RelevantToLocation";
        private const string GeolocationAllowedCommand = "Geolocation.Allowed";

        private const string GeolocationRejectedCommand = "Geolocation.Rejected";
        private const string GoToStartCommand = "Start";

        private const string IsOldUserCommand = "ISOLDUSER";

        private const string SimpleUtteranceCommand = "SimpleUtterance";

        private const string WelcomeEventName = "Welcome";
        private const string ErrorEventName = "Error";
        private const string RelevantToLocationEventName = "RelevantToLocation";
        private const string EasyWelcomeEventName = "EasyWelcomeWithoutGeolocation";
        private const string StartEventName = "Start";

        private readonly Dictionary<string, string> _commandDictionary = new Dictionary<string, string>
        {
            {StartCommand, WelcomeEventName},
            {ErrorCommand, ErrorEventName},

            {RelevantToLocationCommand, RelevantToLocationEventName},
            {GeolocationAllowedCommand, RelevantToLocationEventName},

            {GeolocationRejectedCommand, StartEventName},
            {GoToStartCommand, StartEventName},

            {IsOldUserCommand, EasyWelcomeEventName},
            {SimpleUtteranceCommand, WelcomeEventName}
        };

        private readonly Logger _log = LogManager.GetLogger(nameof(DialogflowService));

        private readonly SessionsClient _dialogflowClient;
        private readonly ContextsClient _contextsClient;
        private readonly DialogflowConfiguration _configuration;
        private readonly IMapper _mapper;

        private readonly Dictionary<Source, Func<Request, EventInput>> _eventResolvers;

        public DialogflowService(SessionsClient dialogflowClient, ContextsClient contextsClient, DialogflowConfiguration configuration, IMapper mapper)
        {
            _dialogflowClient = dialogflowClient;
            _configuration = configuration;
            _mapper = mapper;
            _contextsClient = contextsClient;

            _eventResolvers = new Dictionary<Source, Func<Request, EventInput>>
            {
                {Source.Yandex, YandexEventResolve},
            };
        }

        public async Task<Dialog> GetResponseAsync(Request request, IDictionary<string, string> eventParameters = null)
        {
            var intentRequest = CreateQuery(request, eventParameters);

            if (_configuration.LogQuery)
                _log.Trace($"Request:{System.Environment.NewLine}{intentRequest.Serialize()}");

            var intentResponse = await _dialogflowClient.DetectIntentAsync(intentRequest);

            if (_configuration.LogQuery)
                _log.Trace($"Response:{System.Environment.NewLine}{intentResponse.Serialize()}");

            var queryResult = intentResponse.QueryResult;

            var response = _mapper.Map<Dialog>(queryResult);

            return response;
        }

        public Task DeleteContextAsync(string sessionId, string contextName)
        {
            return _contextsClient.DeleteContextAsync(new ContextName(_configuration.ProjectId, sessionId, contextName));
        }

        private DetectIntentRequest CreateQuery(Request request, IDictionary<string, string> eventParameters = null)
        {
            var session = new SessionName(_configuration.ProjectId, request.SessionId);

            var eventInput = ResolveEvent(request, eventParameters);

            var query = new QueryInput
            {
                Text = new TextInput
                {
                    Text = request.Text,
                    LanguageCode = _configuration.LanguageCode
                },
            };

            if (eventInput != null)
            {
                query.Event = eventInput;
            }

            var intentRequest = new DetectIntentRequest
            {
                SessionAsSessionName = session,
                QueryInput = query
            };

            return intentRequest;
        }

        private EventInput ResolveEvent(Request request, IDictionary<string, string> eventParameters = null)
        {
            var result = default(EventInput);

            var sourceMessenger = request?.Source;

            if (sourceMessenger != null)
            {
                var sourceValue = sourceMessenger.Value;

                if (_eventResolvers.ContainsKey(sourceValue))
                {
                    result = _eventResolvers[sourceValue].Invoke(request);
                }
                else
                {
                    result = EventByCommand(request.Text);
                }
            }

            if (result != null && eventParameters?.Any() == true)
            {
                result.Parameters = new Struct();

                foreach (var eventParameter in eventParameters)
                {
                    result.Parameters.Fields.Add(eventParameter.Key, new Value {StringValue = eventParameter.Value});
                }
            }

            return result;
        }

        private EventInput EventByCommand(string requestText)
        {
            var result = default(EventInput);

            if (_commandDictionary.TryGetValue(requestText, out var eventName))
            {
                result = GetEvent(eventName);
            }

            return result;
        }

        private EventInput GetEvent(string name)
        {
            return new EventInput
            {
                Name = name,
                LanguageCode = _configuration.LanguageCode
            };
        }

        private EventInput YandexEventResolve(Request request)
        {
            EventInput result;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (request.NewSession == true && string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.Text))
            {
                result = GetEvent(WelcomeEventName);
            }
            else
            {
                result = EventByCommand(request.Text);
            }

            return result;
        }
    }
}
