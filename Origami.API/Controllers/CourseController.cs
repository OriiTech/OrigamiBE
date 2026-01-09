using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;
using Origami.BusinessTier.Utils.EnumConvert;
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

        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPost(ApiEndPointConstant.Course.CoursesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCourse(CourseInfo request)
        {
            var response = await _courseService.CreateNewCourse(request);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPatch(ApiEndPointConstant.Course.CourseEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCourseInfo(int id, CourseInfo request)
        {
            var isSuccessful = await _courseService.UpdateCourseInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [Authorize(Roles = RoleConstants.Sensei)]
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
        public async Task<IActionResult> GetCourseLessons(int id, [FromQuery] int? userId = null)
        {
            var response = await _courseService.GetLessonsWithLecturesAsync(id, userId);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Course.CoursesEndPoint + "/lectures/{lectureId}")]
        [ProducesResponseType(typeof(LectureDetailResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLectureDetail(int lectureId)
        {
            var response = await _courseService.GetLectureDetailAsync(lectureId);
            return Ok(response);
        }

        // --- New APIs for Course Save (theo courseSave trong JSON) ---
        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPost(ApiEndPointConstant.Course.CoursesEndPoint + "/save")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateCourse(CourseSaveRequest request)
        {
            var response = await _courseService.CreateOrUpdateCourse(request);
            return Ok(response);
        }

        // --- New APIs for Lesson Management (theo lessonSave trong JSON) ---
        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPost(ApiEndPointConstant.Course.CoursesEndPoint + "/lessons/save")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateLesson(LessonSaveRequest request)
        {
            var response = await _courseService.CreateOrUpdateLesson(request);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpDelete(ApiEndPointConstant.Course.CoursesEndPoint + "/lessons/{lessonId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var isSuccessful = await _courseService.DeleteLesson(lessonId);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }

        // --- New APIs for Lecture Management ---
        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPost(ApiEndPointConstant.Course.CoursesEndPoint + "/lectures/save")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrUpdateLecture(LectureSaveRequest request)
        {
            var response = await _courseService.CreateOrUpdateLecture(request);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpDelete(ApiEndPointConstant.Course.CoursesEndPoint + "/lectures/{lectureId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLecture(int lectureId)
        {
            var isSuccessful = await _courseService.DeleteLecture(lectureId);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }

        // --- New APIs for Resource Management ---
        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpDelete(ApiEndPointConstant.Course.CoursesEndPoint + "/resources/{resourceId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteResource(int resourceId)
        {
            var isSuccessful = await _courseService.DeleteResource(resourceId);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }

        // --- New APIs for Course Flags ---
        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPatch(ApiEndPointConstant.Course.CourseEndPoint + "/trending")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkCourseTrending(int id, [FromBody] bool trending)
        {
            var isSuccessful = await _courseService.MarkCourseTrending(id, trending);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPatch(ApiEndPointConstant.Course.CourseEndPoint + "/bestseller")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkCourseBestseller(int id, [FromBody] bool bestseller)
        {
            var isSuccessful = await _courseService.MarkCourseBestseller(id, bestseller);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}