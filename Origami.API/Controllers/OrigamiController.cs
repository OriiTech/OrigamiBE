using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Origami;

namespace Origami.API.Controllers
{
    [ApiController]
    public class OrigamiController : BaseController<OrigamiController>
    {
        private readonly IOrigamiService _origamiService;

        public OrigamiController(ILogger<OrigamiController> logger, IOrigamiService OrigamiService) : base(logger)
        {
            _origamiService = OrigamiService;
        }


        //Get origami by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.Origami.OrigamiEndPoint)]
        [ProducesResponseType(typeof(GetOrigamiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrigami(int id)
        {
            var response = await _origamiService.GetOrigamiById(id);
            return Ok(response);
        }

        //View all origamis with filter and paging

        [Authorize]
        [HttpGet(ApiEndPointConstant.Origami.OrigamisEndPoint)]
        [ProducesResponseType(typeof(GetOrigamiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllOrigami([FromQuery] OrigamiFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _origamiService.ViewAllOrigami(filter, pagingModel);
            return Ok(response);
        }

        //Create new origami

        [Authorize]
        [HttpPost(ApiEndPointConstant.Origami.OrigamisEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrigami([FromBody] OrigamiInfo request)
        {
            var id = await _origamiService.CreateNewOrigami(request);
            request.CreatedBy = CurrentUserId;
            return CreatedAtAction(nameof(GetOrigami), new { id }, new { id });
        }

        // Update origami info

        [Authorize]
        [HttpPatch(ApiEndPointConstant.Origami.OrigamiEndPoint)]
        [ProducesResponseType(typeof(GetOrigamiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrigamiInfo(int id, OrigamiInfo request)
        {
            var isSuccessful = await _origamiService.UpdateOrigamiInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
