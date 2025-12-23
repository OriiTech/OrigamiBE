using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ICourseService
    {
        Task<int> CreateNewCourse(CourseInfo request);
        Task<IPaginate<GetCourseResponse>> ViewAllCourses(CourseFilter filter, PagingModel pagingModel);
        Task<GetCourseResponse> GetCourseById(int id);
        Task<IPaginate<CourseListItemResponse>> GetCoursesExtendedAsync(CourseFilter filter, PagingModel pagingModel);
        Task<CourseDetailResponse> GetCourseDetailAsync(int id);
        Task<CourseReviewSummaryResponse> GetCourseReviewsAsync(int courseId, int recentCount = 5);
        Task<CourseQuestionSummaryResponse> GetCourseQuestionsAsync(int courseId, int recentCount = 5);
        Task<List<LessonWithLecturesResponse>> GetLessonsWithLecturesAsync(int courseId);
        Task<LectureDetailResponse> GetLectureDetailAsync(int lectureId);
        Task<bool> UpdateCourseInfo(int id, CourseInfo request);
        Task<bool> DeleteCourse(int id);
    }
}
