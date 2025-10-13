using Microsoft.AspNetCore.Identity.Data;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.User;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateNewUser(UserInfo request);
        Task<IPaginate<GetUserResponse>> ViewAllUser(UserFilter filter, PagingModel pagingModel);
        Task<bool> UpdateUserInfo(int id, UserInfo request);
        Task<GetUserResponse> GetUserById(int id);
    }
}
