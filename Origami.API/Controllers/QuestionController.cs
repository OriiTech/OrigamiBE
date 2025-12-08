using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Question;

namespace Origami.API.Controllers
{
    [ApiController]
    public class QuestionController : BaseController<QuestionController>
    {
        private readonly IQuestionService _questionService;

        public QuestionController(ILogger<QuestionController> logger, IQuestionService questionService) : base(logger)
        {
            _questionService = questionService;
        }


        // Get question by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.Question.QuestionEndPoint)]
        [ProducesResponseType(typeof(GetQuestionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var response = await _questionService.GetQuestionById(id);
            return Ok(response);
        }


        // View all questions with filter and paging

        [Authorize]
        [HttpGet(ApiEndPointConstant.Question.QuestionsEndPoint)]
        [ProducesResponseType(typeof(GetQuestionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllQuestions([FromQuery] QuestionFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _questionService.ViewAllQuestions(filter, pagingModel);
            return Ok(response);
        }

        // Create new question

        [Authorize]
        [HttpPost(ApiEndPointConstant.Question.QuestionsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateQuestion(QuestionInfo request)
        {
            var response = await _questionService.CreateNewQuestion(request);
            return Ok(response);
        }


        // Update question info

        [Authorize]
        [HttpPatch(ApiEndPointConstant.Question.QuestionEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateQuestionInfo(int id, QuestionInfo request)
        {
            var isSuccessful = await _questionService.UpdateQuestionInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }


        // Delete question

        [Authorize]
        [HttpDelete(ApiEndPointConstant.Question.QuestionEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var isSuccessful = await _questionService.DeleteQuestion(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}