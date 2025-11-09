using AutoMapper;
using Origami.BusinessTier.Payload.Comment;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class CommentModule : Profile
    {
        public CommentModule()
        {
            CreateMap<Comment, GetCommentResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.GuideTitle, opt => opt.MapFrom(src => src.Guide.Title));


            CreateMap<CommentInfo, Comment>()
                .ForMember(dest => dest.CommentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<CommentUpdateInfo, Comment>()
                .ForMember(dest => dest.CommentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Guide, opt => opt.Ignore());
        }
    }
}
