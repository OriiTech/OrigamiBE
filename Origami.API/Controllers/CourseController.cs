using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;
using Origami.DataTier.Paginate;

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

        // --- Legacy/basic endpoints ---
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

        // --- Extended endpoints aligning with course.json (fields thiếu sẽ null/comment trong service) ---

        [HttpGet(ApiEndPointConstant.Course.CoursesEndPoint + "/extended")]
        [ProducesResponseType(typeof(IPaginate<CourseListItemResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCoursesExtended([FromQuery] CourseFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _courseService.GetCoursesExtendedAsync(filter, pagingModel);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Course.CourseEndPoint + "/detail")]
        [ProducesResponseType(typeof(CourseDetailResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseDetail(int id)
        {
            var response = await _courseService.GetCourseDetailAsync(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Course.CourseEndPoint + "/reviews")]
        [ProducesResponseType(typeof(CourseReviewSummaryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseReviews(int id, [FromQuery] int recent = 5)
        {
            var response = await _courseService.GetCourseReviewsAsync(id, recent);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Course.CourseEndPoint + "/questions")]
        [ProducesResponseType(typeof(CourseQuestionSummaryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseQuestions(int id, [FromQuery] int recent = 5)
        {
            var response = await _courseService.GetCourseQuestionsAsync(id, recent);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Course.CourseEndPoint + "/lessons")]
        [ProducesResponseType(typeof(List<LessonWithLecturesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourseLessons(int id)
        {
            var response = await _courseService.GetLessonsWithLecturesAsync(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Course.CoursesEndPoint + "/lectures/{lectureId}")]
        [ProducesResponseType(typeof(LectureDetailResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLectureDetail(int lectureId)
        {
            var response = await _courseService.GetLectureDetailAsync(lectureId);
            return Ok(response);
        }
    }
}