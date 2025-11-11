using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Leaderboard;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ILeaderboardService
    {
        Task<int> CreateNewLeaderboard(LeaderboardInfo request);
        Task<GetLeaderboardResponse> GetLeaderboardById(int id);
        Task<IPaginate<GetLeaderboardResponse>> ViewAllLeaderboards(LeaderboardFilter filter, PagingModel pagingModel);
        Task<bool> UpdateLeaderboard(int id, LeaderboardInfo request);
        Task<bool> DeleteLeaderboard(int id);
    }
}