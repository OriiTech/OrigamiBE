using AutoMapper;
using Origami.BusinessTier.Payload.Course;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class CourseModule : Profile
    {
        public CourseModule()
        {
            CreateMap<Course, GetCourseResponse>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Username : null));
            CreateMap<CourseInfo, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore());
        }
    }
}