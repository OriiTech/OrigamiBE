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

        }
    }
}
