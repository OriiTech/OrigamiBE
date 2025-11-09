using AutoMapper;
using Origami.BusinessTier.Payload.Favorite;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class FavoriteModule : Profile
    {
        public FavoriteModule()
        {
            CreateMap<Favorite, GetFavoriteResponse>()
                .ForMember(dest => dest.GuideTitle, opt => opt.MapFrom(src => src.Guide.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<FavoriteInfo, Favorite>();
        }
    }
}
