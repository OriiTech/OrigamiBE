using AutoMapper;
using Origami.BusinessTier.Payload.Question;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class QuestionModule : Profile
    {
        public QuestionModule()
        {
            CreateMap<Question, GetQuestionResponse>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));
            CreateMap<QuestionInfo, Question>()
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}