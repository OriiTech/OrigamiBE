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

        [HttpGet(ApiEndPointConstant.Guide.GuideEndPoint)]
        [ProducesResponseType(typeof(GetGuideResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGuide(int id)
        {
            var response = await _guideService.GetGuideById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Guide.GuideDetailEndPoint)]
        [ProducesResponseType(typeof(GetGuideDetailResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGuideDetail(int id)
        {
            var response = await _guideService.GetGuideDetailById(id);
            return Ok(response);
        }
        [HttpPost(ApiEndPointConstant.Guide.GuideViewEndPoint)]
        [Authorize]
        public async Task<IActionResult> IncreaseGuideView(int id)
        {
            await _guideService.IncreaseView(id);
            return NoContent();
        }

        [HttpGet(ApiEndPointConstant.Guide.GuidesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllGuide([FromQuery] GuideFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewAllGuide(filter, pagingModel);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Guide.GuideCardsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetGuideCardResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllGuideCard([FromQuery] GuideCardFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _guideService.ViewAllGuideCard(filter, pagingModel);
            return Ok(response);
        }
        [HttpPost(ApiEndPointConstant.Guide.GuidesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGuide([FromBody] GuideSaveRequest request)
        {
            var id = await _guideService.CreateGuideAsync(request);     
            return Ok(new { guideId = id });
        }

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
