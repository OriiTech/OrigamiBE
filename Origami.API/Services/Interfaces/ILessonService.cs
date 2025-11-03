using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Lesson;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ILessonService
    {
        Task<int> CreateNewLesson(LessonInfo request);
        Task<IPaginate<GetLessonResponse>> ViewAllLessons(LessonFilter filter, PagingModel pagingModel);
        Task<GetLessonResponse> GetLessonById(int id);
        Task<bool> UpdateLessonInfo(int id, LessonInfo request);
        Task<bool> DeleteLesson(int id);
    }
}