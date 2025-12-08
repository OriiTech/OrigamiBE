using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Team;

namespace Origami.API.Controllers
{
    [ApiController]
    public class TeamController : BaseController<TeamController>
    {
        private readonly ITeamService _teamService;

        public TeamController(ILogger<TeamController> logger, ITeamService teamService) : base(logger)
        {
            _teamService = teamService;
        }

        //Get team by id

        [Authorize]
        [HttpGet(ApiEndPointConstant.Team.TeamEndPoint)]
        [ProducesResponseType(typeof(GetTeamResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeam(int id)
        {
            var response = await _teamService.GetTeamById(id);
            return Ok(response);
        }


        //Get all teams with filter and paging

        [Authorize(Roles = "admin, staff")]
        [HttpGet(ApiEndPointConstant.Team.TeamsEndPoint)]
        [ProducesResponseType(typeof(GetTeamResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllTeams([FromQuery] TeamFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _teamService.ViewAllTeams(filter, pagingModel);
            return Ok(response);
        }

        // Create new team

        [Authorize(Roles ="admin, staff,sensei")]
        [HttpPost(ApiEndPointConstant.Team.TeamsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTeam(TeamInfo request)
        {
            var response = await _teamService.CreateNewTeam(request);
            return Ok(response);
        }

        // Update team info

        [Authorize(Roles = "admin, staff,sensei")]
        [HttpPatch(ApiEndPointConstant.Team.TeamEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTeamInfo(int id, TeamInfo request)
        {
            var isSuccessful = await _teamService.UpdateTeamInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        // Delete team

        [Authorize(Roles = "admin, staff,sensei")]
        [HttpDelete(ApiEndPointConstant.Team.TeamEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var isSuccessful = await _teamService.DeleteTeam(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}