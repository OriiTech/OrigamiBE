using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Answer;

namespace Origami.API.Controllers
{
    [ApiController]
    public class AnswerController : BaseController<AnswerController>
    {
        private readonly IAnswerService _answerService;

        public AnswerController(ILogger<AnswerController> logger, IAnswerService answerService) : base(logger)
        {
            _answerService = answerService;
        }

        [HttpGet(ApiEndPointConstant.Answer.AnswerEndPoint)]
        [ProducesResponseType(typeof(GetAnswerResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAnswer(int id)
        {
            var response = await _answerService.GetAnswerById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Answer.AnswersEndPoint)]
        [ProducesResponseType(typeof(GetAnswerResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllAnswers([FromQuery] AnswerFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _answerService.ViewAllAnswers(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Answer.AnswersEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAnswer(AnswerInfo request)
        {
            var response = await _answerService.CreateNewAnswer(request);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Answer.AnswerEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAnswerInfo(int id, AnswerInfo request)
        {
            var isSuccessful = await _answerService.UpdateAnswerInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpDelete(ApiEndPointConstant.Answer.AnswerEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var isSuccessful = await _answerService.DeleteAnswer(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}