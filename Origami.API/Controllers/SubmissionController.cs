using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Submission;

namespace Origami.API.Controllers
{
    [ApiController]
    public class SubmissionController : BaseController<SubmissionController>
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionController(ILogger<SubmissionController> logger, ISubmissionService submissionService) : base(logger)
        {
            _submissionService = submissionService;
        }

        [HttpGet(ApiEndPointConstant.Submission.SubmissionEndPoint)]
        [ProducesResponseType(typeof(GetSubmissionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubmission(int id)
        {
            var response = await _submissionService.GetSubmissionById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Submission.SubmissionsEndPoint)]
        [ProducesResponseType(typeof(GetSubmissionResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllSubmissions([FromQuery] SubmissionFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _submissionService.ViewAllSubmissions(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Submission.SubmissionsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateSubmission(SubmissionInfo request)
        {
            var response = await _submissionService.CreateNewSubmission(request);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Submission.SubmissionEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSubmission(int id, SubmissionInfo request)
        {
            var isSuccessful = await _submissionService.UpdateSubmission(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpDelete(ApiEndPointConstant.Submission.SubmissionEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSubmission(int id)
        {
            var isSuccessful = await _submissionService.DeleteSubmission(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}