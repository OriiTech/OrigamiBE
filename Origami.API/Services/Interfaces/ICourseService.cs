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
        Task<bool> UpdateCourseInfo(int id, CourseInfo request);
        Task<bool> DeleteCourse(int id);

        Task<IPaginate<GetCourseCardResponse>> GetCoursesAsync(CourseFilter filter, PagingModel pagingModel);
        Task<GetCourseDetailResponse> GetCourseDetailAsync(int id);
    }
}
