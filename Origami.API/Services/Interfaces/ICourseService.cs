using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ICourseService
    {
        // Legacy APIs
        Task<int> CreateNewCourse(CourseInfo request);
        Task<IPaginate<GetCourseResponse>> ViewAllCourses(CourseFilter filter, PagingModel pagingModel);
        Task<GetCourseResponse> GetCourseById(int id);
        Task<bool> UpdateCourseInfo(int id, CourseInfo request);
        Task<bool> DeleteCourse(int id);

        // Extended APIs
        Task<IPaginate<CourseListItemResponse>> GetCoursesExtendedAsync(CourseFilter filter, PagingModel pagingModel);
        Task<CourseDetailResponse> GetCourseDetailAsync(int id);
        Task<CourseReviewSummaryResponse> GetCourseReviewsAsync(int courseId, int recentCount = 5);
        Task<CourseQuestionSummaryResponse> GetCourseQuestionsAsync(int courseId, int recentCount = 5);
        Task<List<LessonWithLecturesResponse>> GetLessonsWithLecturesAsync(int courseId, int? userId = null);
        Task<LectureDetailResponse> GetLectureDetailAsync(int lectureId);

        // New APIs for Course Save (theo courseSave trong JSON)
        Task<int> CreateOrUpdateCourse(CourseSaveRequest request);
        
        // New APIs for Lesson/Lecture Management (theo lessonSave trong JSON)
        Task<int> CreateOrUpdateLesson(LessonSaveRequest request);
        Task<bool> DeleteLesson(int lessonId);
        Task<int> CreateOrUpdateLecture(LectureSaveRequest request);
        Task<bool> DeleteLecture(int lectureId);
        Task<bool> DeleteResource(int resourceId);

        // New APIs for Course Flags
        Task<bool> MarkCourseTrending(int id, bool trending);
        Task<bool> MarkCourseBestseller(int id, bool bestseller);
    }
}
