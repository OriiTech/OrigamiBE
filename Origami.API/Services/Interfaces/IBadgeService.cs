using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Badge;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IBadgeService
    {
        Task<int> CreateBadge(BadgeInfo request);
        Task<bool> DeleteBadge(int id);
        Task<GetBadgeResponse> GetBadgeById(int id);
        Task<bool> UpdateBadge(int id, BadgeInfo request);
        Task<IPaginate<GetBadgeResponse>> ViewAllBadge(BadgeFilter filter, PagingModel pagingModel);
    }
}
