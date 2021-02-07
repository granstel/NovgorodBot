using AutoMapper;
using NovgorodBot.Models.Internal;

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
        }
    }
}
