using System;
using AutoMapper;
using Newtonsoft.Json;
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
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.ThumbnailUrl))
                .ForMember(dest => dest.Subtitle, opt => opt.MapFrom(src => src.Subtitle))
                .ForMember(dest => dest.Objectives, opt => opt.MapFrom(src => src.Objectives != null ? JsonConvert.SerializeObject(src.Objectives) : null))
                .ForMember(dest => dest.PaidOnly, opt => opt.MapFrom(src => src.PaidOnly))
                .ForMember(dest => dest.Trending, opt => opt.MapFrom(src => src.Trending))
                .ForMember(dest => dest.PreviewVideoUrl, opt => opt.MapFrom(src => src.PreviewVideoUrl))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt ?? DateTime.UtcNow));
        }
    }
}