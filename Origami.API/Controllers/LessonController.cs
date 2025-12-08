using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Lesson;

namespace Origami.API.Controllers
{
    [ApiController]
    public class LessonController : BaseController<LessonController>
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILogger<LessonController> logger, ILessonService lessonService) : base(logger)
        {
            _lessonService = lessonService;
        }

        // Get lesson by id
        [Authorize]
        [HttpGet(ApiEndPointConstant.Lesson.LessonEndPoint)]
        [ProducesResponseType(typeof(GetLessonResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLesson(int id)
        {
            var response = await _lessonService.GetLessonById(id);
            return Ok(response);
        }

        // View all lessons with filter and paging

        [Authorize]
        [HttpGet(ApiEndPointConstant.Lesson.LessonsEndPoint)]
        [ProducesResponseType(typeof(GetLessonResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllLessons([FromQuery] LessonFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _lessonService.ViewAllLessons(filter, pagingModel);
            return Ok(response);
        }

        // Create new lesson

        [Authorize(Roles = "admin, staff, sensei")]
        [HttpPost(ApiEndPointConstant.Lesson.LessonsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateLesson(LessonInfo request)
        {
            var response = await _lessonService.CreateNewLesson(request);
            return Ok(response);
        }

        // Update lesson info

        [Authorize(Roles = "admin, staff, sensei")]
        [HttpPatch(ApiEndPointConstant.Lesson.LessonEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLessonInfo(int id, LessonInfo request)
        {
            var isSuccessful = await _lessonService.UpdateLessonInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        // Delete lesson

        [Authorize(Roles = "admin, staff, sensei")]
        [HttpDelete(ApiEndPointConstant.Lesson.LessonEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var isSuccessful = await _lessonService.DeleteLesson(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}