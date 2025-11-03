using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Origami.BusinessTier.Payload.User;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class UserModule : Profile
    {
        public UserModule()
        {
            CreateMap<User, GetUserResponse>()
                 .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.RoleId));
            CreateMap<RegisterRequest, User>()
          .ForMember(dest => dest.Password, opt => opt.Ignore())
          .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
