using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Question;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<int> CreateNewQuestion(QuestionInfo request);
        Task<IPaginate<GetQuestionResponse>> ViewAllQuestions(QuestionFilter filter, PagingModel pagingModel);
        Task<GetQuestionResponse> GetQuestionById(int id);
        Task<bool> UpdateQuestionInfo(int id, QuestionInfo request);
        Task<bool> DeleteQuestion(int id);
    }
}