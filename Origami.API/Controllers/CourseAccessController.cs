using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.CourseAccess;

namespace Origami.API.Controllers
{
    [ApiController]
    public class CourseAccessController : BaseController<CourseAccessController>
    {
        private readonly ICourseAccessService _courseAccessService;

        public CourseAccessController(ILogger<CourseAccessController> logger, ICourseAccessService courseAccessService) : base(logger)
        {
            _courseAccessService = courseAccessService;
        }


        // Get course access by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.CourseAccess.CourseAccessEndPoint)]
        [ProducesResponseType(typeof(GetCourseAccessResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseAccess(int id)
        {
            var response = await _courseAccessService.GetCourseAccessById(id);
            return Ok(response);
        }

        // View all course accesses with filter and paging
        [Authorize]
        [HttpGet(ApiEndPointConstant.CourseAccess.CourseAccessesEndPoint)]
        [ProducesResponseType(typeof(GetCourseAccessResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllCourseAccesses([FromQuery] CourseAccessFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _courseAccessService.ViewAllCourseAccesses(filter, pagingModel);
            return Ok(response);
        }

        // Create new course access

        [Authorize(Roles ="admin, staff")]
        [HttpPost(ApiEndPointConstant.CourseAccess.CourseAccessesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCourseAccess(CourseAccessInfo request)
        {
            var response = await _courseAccessService.CreateNewCourseAccess(request);
            return Ok(response);
        }

        //Delete course access
        [Authorize(Roles ="admin, staff")]
        [HttpDelete(ApiEndPointConstant.CourseAccess.CourseAccessEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCourseAccess(int id)
        {
            var isSuccessful = await _courseAccessService.DeleteCourseAccess(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}