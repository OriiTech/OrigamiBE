using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.CourseAccess;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ICourseAccessService
    {
        Task<int> CreateNewCourseAccess(CourseAccessInfo request);
        Task<IPaginate<GetCourseAccessResponse>> ViewAllCourseAccesses(CourseAccessFilter filter, PagingModel pagingModel);
        Task<GetCourseAccessResponse> GetCourseAccessById(int id);
        Task<bool> DeleteCourseAccess(int id);
    }
}