using AutoMapper;
using Origami.BusinessTier.Payload.CourseAccess;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class CourseAccessModule : Profile
    {
        public CourseAccessModule()
        {
            CreateMap<CourseAccess, GetCourseAccessResponse>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
                .ForMember(dest => dest.LearnerName, opt => opt.MapFrom(src => src.Learner != null ? src.Learner.Username : null));
            CreateMap<CourseAccessInfo, CourseAccess>()
                .ForMember(dest => dest.AccessId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchasedAt, opt => opt.Ignore());
        }
    }
}