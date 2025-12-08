using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Step;
using Origami.DataTier.Paginate;

namespace Origami.API.Controllers
{
    public class StepController : BaseController<StepController>
    {
        private readonly IStepService _stepService;
        public StepController(ILogger<StepController> logger, IStepService stepService) : base(logger)
        {
            _stepService = stepService;
        }

        // Get step by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.Step.StepEndPoint)]
        [ProducesResponseType(typeof(GetStepResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStep(int id)
        {
            var response = await _stepService.GetStepById(id);
            return Ok(response);
        }


        //Get all steps with filter and paging

        [Authorize(Roles = "admin, staff")]
        [HttpGet(ApiEndPointConstant.Step.StepsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetStepResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllStep([FromQuery] StepFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _stepService.ViewAllSteps(filter, pagingModel);
            return Ok(response);
        }

        // Create new step

        [Authorize]
        [HttpPost(ApiEndPointConstant.Step.StepsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateStep([FromBody] StepInfo request)
        {
            var id = await _stepService.CreateStep(request);
            return CreatedAtAction(nameof(GetStep), new { id }, new { id });
        }

        //Update step info

        [Authorize]
        [HttpPatch(ApiEndPointConstant.Step.StepEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStepInfo(int id, [FromBody] StepInfo request)
        {
            var isSuccessful = await _stepService.UpdateStepInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
