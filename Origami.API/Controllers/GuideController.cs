using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Guide;
using Origami.DataTier.Paginate;

namespace Origami.API.Controllers
{
    [ApiController]
    public class GuideController : BaseController<GuideController>
    {
        private readonly IGuideService _guideService;

        public GuideController(ILogger<GuideController> logger, IGuideService guideService) : base(logger)
        {
            _guideService = guideService;
        }


        // Get guide by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.Guide.GuideEndPoint)]
        [ProducesResponseType(typeof(GetGuideResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGuide(int id)
        {
            var response = await _guideService.GetGuideById(id);
            return Ok(response);
        }


        // View all guides with filter and paging

        [Authorize]
        [HttpGet(ApiEndPointConstant.Guide.GuidesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllGuide([FromQuery] GuideFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewAllGuide(filter, pagingModel);
            return Ok(response);
        }


        // Create new guide

        [Authorize(Roles ="admin, staff, sensei")]
        [HttpPost(ApiEndPointConstant.Guide.GuidesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGuide([FromBody] GuideInfo request)
        {
            var id = await _guideService.CreateNewGuide(request);
            //request.AuthorId = CurrentUserId;         
            return CreatedAtAction(nameof(GetGuide), new { id }, new { id });
        }

        // Update guide info
        [Authorize(Roles ="admin, staff, sensei")]
        [HttpPatch(ApiEndPointConstant.Guide.GuideEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGuideInfo(int id, [FromBody] GuideInfo request)
        {
            var isSuccessful = await _guideService.UpdateGuideInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
