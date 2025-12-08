using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Leaderboard;

namespace Origami.API.Controllers
{
    [ApiController]
    public class LeaderboardController : BaseController<LeaderboardController>
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILogger<LeaderboardController> logger, ILeaderboardService leaderboardService) : base(logger)
        {
            _leaderboardService = leaderboardService;
        }

        // Get leaderboard by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.Leaderboard.LeaderboardEndPoint)]
        [ProducesResponseType(typeof(GetLeaderboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLeaderboard(int id)
        {
            var response = await _leaderboardService.GetLeaderboardById(id);
            return Ok(response);
        }

        // View all leaderboards with filter and paging

        [Authorize]
        [HttpGet(ApiEndPointConstant.Leaderboard.LeaderboardsEndPoint)]
        [ProducesResponseType(typeof(GetLeaderboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllLeaderboards([FromQuery] LeaderboardFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _leaderboardService.ViewAllLeaderboards(filter, pagingModel);
            return Ok(response);
        }


        // Create new leaderboard

        [Authorize(Roles = "admin, staff")]
        [HttpPost(ApiEndPointConstant.Leaderboard.LeaderboardsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateLeaderboard(LeaderboardInfo request)
        {
            var response = await _leaderboardService.CreateNewLeaderboard(request);
            return Ok(response);
        }

        // Update leaderboard

        [Authorize(Roles = "admin, staff")]
        [HttpPatch(ApiEndPointConstant.Leaderboard.LeaderboardEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLeaderboard(int id, LeaderboardInfo request)
        {
            var isSuccessful = await _leaderboardService.UpdateLeaderboard(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }


        // Delete leaderboard

        [Authorize(Roles = "admin, staff")]
        [HttpDelete(ApiEndPointConstant.Leaderboard.LeaderboardEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLeaderboard(int id)
        {
            var isSuccessful = await _leaderboardService.DeleteLeaderboard(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}