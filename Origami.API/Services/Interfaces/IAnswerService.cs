using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Answer;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IAnswerService
    {
        Task<int> CreateNewAnswer(AnswerInfo request);
        Task<IPaginate<GetAnswerResponse>> ViewAllAnswers(AnswerFilter filter, PagingModel pagingModel);
        Task<GetAnswerResponse> GetAnswerById(int id);
        Task<bool> UpdateAnswerInfo(int id, AnswerInfo request);
        Task<bool> DeleteAnswer(int id);
    }
}