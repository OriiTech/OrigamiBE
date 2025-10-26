using AutoMapper;
using Origami.BusinessTier.Payload.Role;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class RoleModule : Profile
    {
        public RoleModule()
        {
            CreateMap<Role, GetRoleResponse>();
            CreateMap<RoleInfo, Role>();
        }
    }
}