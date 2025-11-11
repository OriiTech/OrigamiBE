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

        [HttpGet(ApiEndPointConstant.Leaderboard.LeaderboardEndPoint)]
        [ProducesResponseType(typeof(GetLeaderboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLeaderboard(int id)
        {
            var response = await _leaderboardService.GetLeaderboardById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Leaderboard.LeaderboardsEndPoint)]
        [ProducesResponseType(typeof(GetLeaderboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllLeaderboards([FromQuery] LeaderboardFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _leaderboardService.ViewAllLeaderboards(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Leaderboard.LeaderboardsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateLeaderboard(LeaderboardInfo request)
        {
            var response = await _leaderboardService.CreateNewLeaderboard(request);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Leaderboard.LeaderboardEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLeaderboard(int id, LeaderboardInfo request)
        {
            var isSuccessful = await _leaderboardService.UpdateLeaderboard(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

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