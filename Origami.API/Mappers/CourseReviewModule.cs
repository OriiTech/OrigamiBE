using AutoMapper;
using Origami.BusinessTier.Payload.CourseReview;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class CourseReviewModule : Profile
    {
        public CourseReviewModule()
        {
            CreateMap<CourseReview, GetCourseReviewResponse>();
            CreateMap<CourseReviewInfo, CourseReview>()
                .ForMember(d => d.ReviewId, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());
        }
    }
}