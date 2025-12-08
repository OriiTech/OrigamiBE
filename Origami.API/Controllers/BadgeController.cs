using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Badge;
using Origami.DataTier.Paginate;

namespace Origami.API.Controllers
{
    [ApiController]
    public class BadgeController : BaseController<BadgeController>
    {
        private readonly IBadgeService _badgeService;

        public BadgeController(
            ILogger<BadgeController> logger,
            IBadgeService badgeService)
            : base(logger)
        {
            _badgeService = badgeService;
        }

        //Create new badge

        [Authorize(Roles = "admin,staff")]
        [HttpPost(ApiEndPointConstant.Badge.BadgesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateBadge(BadgeInfo request)
        {
            var id = await _badgeService.CreateBadge(request);
            return Ok(new { BadgeId = id });
        }

        // Get badge by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.Badge.BadgeEndPoint)]
        [ProducesResponseType(typeof(GetBadgeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBadge(int id)
        {
            var result = await _badgeService.GetBadgeById(id);
            return Ok(result);
        }

        //View all badges

        [Authorize]
        [HttpGet(ApiEndPointConstant.Badge.BadgesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetBadgeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllBadge(
            [FromQuery] BadgeFilter filter,
            [FromQuery] PagingModel paging)
        {
            var result = await _badgeService.ViewAllBadge(filter, paging);
            return Ok(result);
        }

        //Update Badge

        [Authorize(Roles ="admin, staff")]
        [HttpPatch(ApiEndPointConstant.Badge.BadgeEndPoint)]
        public async Task<IActionResult> UpdateBadge(int id, BadgeInfo request)
        {
            var success = await _badgeService.UpdateBadge(id, request);
            return Ok(success ? "UpdateBadgeSuccess" : "UpdateBadgeFailed");
        }

        //Delete Badge

        [Authorize(Roles = "admin, staff")]
        [HttpDelete(ApiEndPointConstant.Badge.BadgeEndPoint)]
        public async Task<IActionResult> DeleteBadge(int id)
        {
            var success = await _badgeService.DeleteBadge(id);
            return Ok(success ? "DeleteBadgeSuccess" : "DeleteBadgeFailed");
        }
    }
}
