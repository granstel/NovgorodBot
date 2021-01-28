using System;
using AutoMapper;
using NovgorodBot.Models.Internal;
using Yandex.Dialogs.Models;
using Yandex.Dialogs.Models.Input;
using NovgorodBot.Models;
using InternalModels = NovgorodBot.Models.Internal;
using YandexModels = Yandex.Dialogs.Models;

namespace NovgorodBot.Messengers.Yandex
{
    /// <summary>
    /// Probably, registered at MappingModule of "Services" project
    /// </summary>
    public class YandexProfile : Profile
    {
        public YandexProfile()
        {
            CreateMap<InputModel, InternalModels.Request>()
                .ForMember(d => d.ChatHash, m => m.MapFrom((s, d) => s.Session?.SkillId))
                .ForMember(d => d.UserHash, m => m.MapFrom((s, d) => s.Session?.UserId))
                .ForMember(d => d.Text, m => m.MapFrom((s, d) => s.Request?.OriginalUtterance))
                .ForMember(d => d.SessionId, m => m.MapFrom((s, d) => s.Session?.SessionId))
                .ForMember(d => d.NewSession, m => m.MapFrom((s, d) => s.Session?.New))
                .ForMember(d => d.Language, m => m.MapFrom((s, d) => s.Meta?.Locale))
                .ForMember(d => d.Geolocation, m => m.MapFrom((s, d) => s.Session?.Location))
                .ForMember(d => d.Source, m => m.MapFrom(s => Source.Yandex));

            CreateMap<Location, Geolocation>()
                .ForMember(d => d.Lat, m => m.MapFrom(s => s.Lat))
                .ForMember(d => d.Lon, m => m.MapFrom(s => s.Lon));

            CreateMap<InternalModels.Response, OutputModel>()
                .ForMember(d => d.Response, m => m.MapFrom(s => s))
                .ForMember(d => d.Session, m => m.MapFrom(s => s))
                .ForMember(d => d.Version, m => m.Ignore())
                .ForMember(d => d.StartAccountLinking, m => m.Ignore())
                .ForMember(d => d.UserStateUpdate, m => m.Ignore())
                .ForMember(d => d.SessionState, m => m.Ignore())
                .ForMember(d => d.ApplicationState, m => m.Ignore());

            CreateMap<InternalModels.Response, YandexModels.Response>()
                .ForMember(d => d.Text, m => m.MapFrom(s => s.Text.Replace(Environment.NewLine, "\n")))
                .ForMember(d => d.Tts, m => m.MapFrom(s => s.AlternativeText.Replace(Environment.NewLine, "\n")))
                .ForMember(d => d.EndSession, m => m.MapFrom(s => s.Finished))
                .ForMember(d => d.Card, m => m.Ignore())
                .ForMember(d => d.Buttons, m => m.Ignore())
                .ForMember(d => d.Directives, m => m.Ignore());

            CreateMap<InternalModels.Response, Session>()
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserHash))
                .ForMember(d => d.MessageId, m => m.Ignore())
                .ForMember(d => d.SessionId, m => m.Ignore())
                .ForMember(d => d.Application, m => m.Ignore())
                .ForMember(d => d.User, m => m.Ignore());

            CreateMap<InputModel, OutputModel>()
                .ForMember(d => d.Session, m => m.MapFrom(s => s.Session))
                .ForMember(d => d.Version, m => m.MapFrom(s => s.Version))
                .ForMember(d => d.Response, m => m.Ignore())
                .ForMember(d => d.StartAccountLinking, m => m.Ignore())
                .ForMember(d => d.UserStateUpdate, m => m.Ignore())
                .ForMember(d => d.SessionState, m => m.Ignore())
                .ForMember(d => d.ApplicationState, m => m.Ignore());
        }
    }
}
