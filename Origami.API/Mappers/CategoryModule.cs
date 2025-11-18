using AutoMapper;
using Origami.BusinessTier.Payload.Category;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class CategoryModule : Profile
    {
        public CategoryModule()
        {
            CreateMap<Category, GetCategoryResponse>().ForMember(dest => dest.Guides, opt => opt.MapFrom(src => src.Guides.Select(g => g.Title)));

            CreateMap<CategoryInfo, Category>();

        }
    }
}