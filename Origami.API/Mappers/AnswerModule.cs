using AutoMapper;
using Origami.BusinessTier.Payload.Answer;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class AnswerModule : Profile
    {
        public AnswerModule()
        {
            CreateMap<Answer, GetAnswerResponse>()
                .ForMember(dest => dest.QuestionContent, opt => opt.MapFrom(src => src.Question != null ? src.Question.Content : null))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));
            CreateMap<AnswerInfo, Answer>()
                .ForMember(dest => dest.AnswerId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}