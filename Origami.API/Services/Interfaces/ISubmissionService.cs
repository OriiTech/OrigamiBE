using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Submission;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<int> CreateNewSubmission(SubmissionInfo request);
        Task<GetSubmissionResponse> GetSubmissionById(int id);
        Task<IPaginate<GetSubmissionResponse>> ViewAllSubmissions(SubmissionFilter filter, PagingModel pagingModel);
        Task<bool> UpdateSubmission(int id, SubmissionInfo request);
        Task<bool> DeleteSubmission(int id);
        Task<SubmissionFeedDto> LoadSubmissionFeedAsync(int challengeId, PagingModel paging);
    }
}