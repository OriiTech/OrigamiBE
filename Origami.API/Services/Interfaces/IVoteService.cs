using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Vote;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IVoteService
    {
        Task<int> CreateNewVote(VoteInfo request);
        Task<GetVoteResponse> GetVoteById(int id);
        Task<IPaginate<GetVoteResponse>> ViewAllVotes(VoteFilter filter, PagingModel pagingModel);
        Task<bool> DeleteVote(int id);
    }
}