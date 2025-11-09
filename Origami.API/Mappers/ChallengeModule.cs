using AutoMapper;
using Origami.BusinessTier.Payload.Challenge;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class ChallengeModule : Profile
    {
        public ChallengeModule()
        {
            CreateMap<Challenge, GetChallengeResponse>()
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByNavigation != null ? src.CreatedByNavigation.Username : null));
            CreateMap<ChallengeInfo, Challenge>()
                .ForMember(dest => dest.ChallengeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}