using AutoMapper;
using Origami.BusinessTier.Payload.Lesson;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class LessonModule : Profile
    {
        public LessonModule()
        {
            CreateMap<Lesson, GetLessonResponse>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null));
            CreateMap<LessonInfo, Lesson>()
                .ForMember(dest => dest.LessonId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}