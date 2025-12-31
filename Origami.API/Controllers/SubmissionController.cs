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
        [HttpGet(ApiEndPointConstant.Submission.SubmissionFeed)]
        [ProducesResponseType(typeof(SubmissionFeedDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoadSubmissionFeed([FromRoute] int challengeId,[FromQuery] PagingModel pagingModel)
        {
            var result = await _submissionService.LoadSubmissionFeedAsync(
                challengeId,
                pagingModel
            );

            return Ok(result);
        }

        [HttpPost(ApiEndPointConstant.Submission.SubmissionsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateSubmission(SubmissionInfo request)
        {
            var response = await _submissionService.CreateNewSubmission(request);
            return Ok(response);
        }
        [HttpPost(ApiEndPointConstant.Submission.SaveSubmission)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveSubmission([FromBody] SubmissionSaveDto dto)
        {
            var submissionId = await _submissionService
                .SaveSubmissionAsync(dto, isSubmit: false);

            return Ok(new
            {
                submissionId
            });
        }
        [HttpPost(ApiEndPointConstant.Submission.SubmitSubmission)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitSubmission([FromBody] SubmissionSaveDto dto)
        {
            var submissionId = await _submissionService
                .SaveSubmissionAsync(dto, isSubmit: true);

            return Ok(new
            {
                submissionId
            });
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