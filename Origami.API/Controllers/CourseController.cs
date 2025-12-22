using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;

namespace Origami.API.Controllers
{
    [ApiController]
    public class CourseController : BaseController<CourseController>
    {
        private readonly ICourseService _courseService;

        public CourseController(ILogger<CourseController> logger, ICourseService courseService) : base(logger)
        {
            _courseService = courseService;
        }

        [HttpGet(ApiEndPointConstant.Course.CourseEndPoint)]
        [ProducesResponseType(typeof(GetCourseResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourse(int id)
        {
            var response = await _courseService.GetCourseById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Course.CoursesEndPoint)]
        [ProducesResponseType(typeof(GetCourseResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllCourses([FromQuery] CourseFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _courseService.ViewAllCourses(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Course.CoursesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCourse(CourseInfo request)
        {
            var response = await _courseService.CreateNewCourse(request);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Course.CourseEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCourseInfo(int id, CourseInfo request)
        {
            var isSuccessful = await _courseService.UpdateCourseInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpDelete(ApiEndPointConstant.Course.CourseEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var isSuccessful = await _courseService.DeleteCourse(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}