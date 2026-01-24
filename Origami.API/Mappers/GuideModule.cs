using AutoMapper;
using Origami.BusinessTier.Payload.Guide;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class GuideModule : Profile
    {
        public GuideModule()
        {

            CreateMap<Guide, GetGuideResponse>();
            CreateMap<GuideInfo, Guide>()
                .ForMember(dest => dest.GuideId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<Guide, GetGuideResponse>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Username))
                .ForMember(dest => dest.OrigamiName, opt => opt.MapFrom(src => src.Origami.Name))
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.Categories.Select(c => c.CategoryId)))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps));
            CreateMap<Guide, GetGuideCardResponse>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.GuideId))
            .ForMember(d => d.CreatorName, o => o.MapFrom(s => s.Author.Username))
            .ForMember(d => d.CreatorImage, o => o.MapFrom(s => s.Author.UserProfile.AvatarUrl))
            .ForMember(d => d.TotalViews, o => o.MapFrom(s => s.GuideViews.Count))
            .ForMember(d => d.Rating,
                o => o.MapFrom(s =>
                    s.GuideRatings.Any()
                        ? s.GuideRatings.Average(r => r.Rating)
                        : 0))
            .ForMember(d => d.PromoPhotos,
                o => o.MapFrom(s => s.GuidePromoPhotos
                    .OrderBy(p => p.DisplayOrder)
                    .Select(p => p.Url)))
            .ForMember(d => d.New, o => o.MapFrom(s => s.IsNew))
            .ForMember(d => d.Category,
                o => o.MapFrom(s => s.Categories)); 

            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.CategoryId))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.CategoryName));

        }
    }
}
