using AutoMapper;
using Origami.BusinessTier.Payload.UserBadge;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class UserBadgeModule : Profile
    {
        public UserBadgeModule()
        {
            CreateMap<UserBadge, GetUserBadgeResponse>()
                .ForMember(dest => dest.BadgeName, opt => opt.MapFrom(src => src.Badge.BadgeName))
                .ForMember(dest => dest.BadgeDescription, opt => opt.MapFrom(src => src.Badge.BadgeDescription));

            CreateMap<UserBadgeInfo, UserBadge>();
        }
    }
}
