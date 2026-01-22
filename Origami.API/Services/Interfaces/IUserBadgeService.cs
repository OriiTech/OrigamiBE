using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.UserBadge;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IUserBadgeService
    {
        Task<GetUserBadgeResponse> GetUserBadge(int userId, int badgeId);
        Task<bool> GrantBadge(UserBadgeInfo request);
        Task<bool> RevokeBadge(int userId, int badgeId);
        Task<IPaginate<GetUserBadgeResponse>> ViewAllUserBadges(UserBadgeFilter filter, PagingModel pagingModel);
        Task<List<GetUserBadgeResponse>> GetMyBadgesAsync();
    }
}
