using AutoMapper;
using Origami.BusinessTier.Payload.Badge;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class BadgeModule : Profile
    {
        public BadgeModule()
        {
            CreateMap<Badge, GetBadgeResponse>();
            CreateMap<BadgeInfo, Badge>();
        }
    }
}