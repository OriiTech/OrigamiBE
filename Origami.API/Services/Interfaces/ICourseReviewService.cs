using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.CourseReview;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ICourseReviewService
    {
        Task<int> CreateNewCourseReview(CourseReviewInfo request);
        Task<GetCourseReviewResponse> GetCourseReviewById(int id);
        Task<IPaginate<GetCourseReviewResponse>> ViewAllCourseReviews(CourseReviewFilter filter, PagingModel pagingModel);
        Task<bool> UpdateCourseReview(int id, CourseReviewInfo request);
        Task<bool> DeleteCourseReview(int id);
    }
}