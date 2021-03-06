﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf.WellKnownTypes;
using NovgorodBot.Models;
using NovgorodBot.Services.Extensions;

namespace NovgorodBot.Services.Mapping
{
    public class DialogflowProfile : Profile
    {
        public DialogflowProfile()
        {
            CreateMap<QueryResult, Dialog>()
                .ForMember(d => d.Parameters, m => m.MapFrom(s => GetParameters(s)))
                .ForMember(d => d.Templates, m => m.MapFrom(s => ParseTemplates(s)))
                .ForMember(d => d.Response, m => m.MapFrom(s => s.FulfillmentText))
                .ForMember(d => d.Buttons, m => m.MapFrom(s => GetButtons(s)))
                .ForMember(d => d.ParametersIncomplete, m => m.MapFrom(s => !s.AllRequiredParamsPresent))
                .ForMember(d => d.Action, m => m.MapFrom(s => s.Action))
                .ForMember(d => d.EndConversation, m => m.Ignore())
                .AfterMap((s, d) =>
                {
                    d.EndConversation = s.DiagnosticInfo?.Fields?.Where(f => string.Equals(f.Key, "end_conversation"))
                                            .Select(f => f.Value.BoolValue).FirstOrDefault() ?? false;
                });
        }

        private IDictionary<string, string[]> GetParameters(QueryResult queryResult)
        {
            var dictionary = new Dictionary<string, string[]>();

            var fields = queryResult?.Parameters.Fields;

            if (fields?.Any() != true)
            {
                return dictionary;
            }

            foreach (var field in fields)
            {
                if (field.Value.KindCase == Value.KindOneofCase.StringValue)
                {
                    dictionary.Add(field.Key, new[] { field.Value.StringValue });
                }
                else if (field.Value.KindCase == Value.KindOneofCase.ListValue)
                {
                    dictionary.Add(field.Key, field.Value.ListValue.Values.Select(v => v.StringValue).Distinct().ToArray());
                }
                else if (field.Value.KindCase == Value.KindOneofCase.StructValue)
                {
                    var stringValues = new List<string>();

                    foreach (var valueField in field.Value.StructValue.Fields)
                    {
                        if (valueField.Value.KindCase == Value.KindOneofCase.StringValue)
                        {
                            stringValues.Add(valueField.Value.StringValue);
                        }
                    }

                    dictionary.Add(field.Key, stringValues.ToArray());
                }
            }

            return dictionary;
        }

        private Button[] GetButtons(QueryResult queryResult)
        {
            var quickReplies = queryResult?.FulfillmentMessages
                ?.Where(m => m.MessageCase == Intent.Types.Message.MessageOneofCase.QuickReplies)
                .SelectMany(m => m.QuickReplies.QuickReplies_.Select(r => new Button
                {
                    Text = r,
                    QuickReply = true
                })).Where(r => r != null).ToList();

            var cards = queryResult?.FulfillmentMessages
                ?.Where(m => m.MessageCase == Intent.Types.Message.MessageOneofCase.Card)
                .SelectMany(m => m.Card.Buttons.Select(b => new Button
                {
                    Text = b.Text,
                    Url = b.Postback
                })).Where(b => b != null).ToList();

            if (quickReplies != null)
            {
                quickReplies.AddRange(cards);

                return quickReplies.ToArray();
            }

            return null;
        }

        private ICollection<Template> ParseTemplates(QueryResult queryResult)
        {
            var result = new List<Template>();

            var sourcePayloads = queryResult?.FulfillmentMessages?
                .Where(m => m.MessageCase == Intent.Types.Message.MessageOneofCase.Payload)
                .Select(m => m.Payload.ToString().Deserialize<Template>()).ToList();

            if (sourcePayloads?.Any() == true)
            {
                result.AddRange(sourcePayloads);
            }

            return result;
        }
    }
}
