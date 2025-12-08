using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.CourseReview;

namespace Origami.API.Controllers
{
    [ApiController]
    public class CourseReviewController : BaseController<CourseReviewController>
    {
        private readonly ICourseReviewService _courseReviewService;

        public CourseReviewController(ILogger<CourseReviewController> logger, ICourseReviewService courseReviewService) : base(logger)
        {
            _courseReviewService = courseReviewService;
        }

        // Get course review by id

        [HttpGet(ApiEndPointConstant.CourseReview.CourseReviewEndPoint)]
        [ProducesResponseType(typeof(GetCourseReviewResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseReview(int id)
        {
            var response = await _courseReviewService.GetCourseReviewById(id);
            return Ok(response);
        }

        // View all course reviews with filter and paging

        [HttpGet(ApiEndPointConstant.CourseReview.CourseReviewsEndPoint)]
        [ProducesResponseType(typeof(GetCourseReviewResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllCourseReviews([FromQuery] CourseReviewFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _courseReviewService.ViewAllCourseReviews(filter, pagingModel);
            return Ok(response);
        }

        // Create new course review

        [Authorize]
        [HttpPost(ApiEndPointConstant.CourseReview.CourseReviewsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCourseReview(CourseReviewInfo request)
        {
            var response = await _courseReviewService.CreateNewCourseReview(request);
            return Ok(response);
        }

        // Update course review

        [Authorize]
        [HttpPatch(ApiEndPointConstant.CourseReview.CourseReviewEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCourseReview(int id, CourseReviewInfo request)
        {
            var isSuccessful = await _courseReviewService.UpdateCourseReview(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        // Delete course review

        [Authorize]
        [HttpDelete(ApiEndPointConstant.CourseReview.CourseReviewEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCourseReview(int id)
        {
            var isSuccessful = await _courseReviewService.DeleteCourseReview(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}