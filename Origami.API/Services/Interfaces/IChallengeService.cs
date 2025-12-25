using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Challenge;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IChallengeService
    {
        Task<int> CreateNewChallenge(ChallengeInfo request);
        Task<IPaginate<GetChallengeResponse>> ViewAllChallenges(ChallengeFilter filter, PagingModel pagingModel);
        Task<GetChallengeResponse> GetChallengeById(int id);
        Task<bool> UpdateChallengeInfo(int id, ChallengeInfo request);
        Task<bool> DeleteChallenge(int id);
        Task<IPaginate<ChallengeListItemDto>> GetChallengeListAsync(ChallengeListFilter filter, PagingModel pagingModel);
        Task<ChallengeDetailDto> GetChallengeDetailAsync(int challengeId);
    }
}