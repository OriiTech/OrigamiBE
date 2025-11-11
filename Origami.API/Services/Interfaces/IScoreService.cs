using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Score;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IScoreService
    {
        Task<int> CreateNewScore(ScoreInfo request);
        Task<GetScoreResponse> GetScoreById(int id);
        Task<IPaginate<GetScoreResponse>> ViewAllScores(ScoreFilter filter, PagingModel pagingModel);
        Task<bool> UpdateScore(int id, ScoreInfo request);
        Task<bool> DeleteScore(int id);
    }
}