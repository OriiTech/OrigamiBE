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
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore());
        }
    }
}
