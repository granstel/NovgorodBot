using AutoMapper;
using NovgorodBot.Models;

namespace NovgorodBot.Services.Mapping
{
    public class InternalProfile : Profile
    {
        public InternalProfile()
        {
            CreateMap<Request, Response>()
                .ForMember(d => d.ChatHash, m => m.MapFrom(s => s.ChatHash))
                .ForMember(d => d.UserHash, m => m.MapFrom(s => s.UserHash))
                .ForMember(d => d.Text, m => m.Ignore())
                .ForMember(d => d.AlternativeText, m => m.Ignore())
                .ForMember(d => d.Finished, m => m.Ignore())
                .ForMember(d => d.RequestGeolocation, m => m.Ignore())
                .ForMember(d => d.Buttons, m => m.Ignore());

            CreateMap<Dialog, Response>()
                .ForMember(d => d.Text, m => m.MapFrom(s => s.Response))
                .ForMember(d => d.Finished, m => m.MapFrom((s, d) => (s?.EndConversation).GetValueOrDefault()))
                .ForMember(d => d.RequestGeolocation, m => m.MapFrom(s => s.IsLocationRequested()))
                .ForMember(d => d.Buttons, m => m.MapFrom(s => s.Buttons))
                .ForMember(d => d.ChatHash, m => m.Ignore())
                .ForMember(d => d.UserHash, m => m.Ignore())
                .ForMember(d => d.AlternativeText, m => m.Ignore());
        }
    }
}
