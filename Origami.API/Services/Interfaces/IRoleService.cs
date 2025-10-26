using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Role;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IRoleService
    {
        Task<int> CreateNewRole(RoleInfo request);
        Task<IPaginate<GetRoleResponse>> ViewAllRoles(RoleFilter filter, PagingModel pagingModel);
        Task<GetRoleResponse> GetRoleById(int id);
    }
}