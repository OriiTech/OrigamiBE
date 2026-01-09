using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Team;
using Origami.BusinessTier.Utils.EnumConvert;

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

        [HttpGet(ApiEndPointConstant.Team.TeamEndPoint)]
        [ProducesResponseType(typeof(GetTeamResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeam(int id)
        {
            var response = await _teamService.GetTeamById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Team.TeamsEndPoint)]
        [ProducesResponseType(typeof(GetTeamResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllTeams([FromQuery] TeamFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _teamService.ViewAllTeams(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPost(ApiEndPointConstant.Team.TeamsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTeam(TeamInfo request)
        {
            var response = await _teamService.CreateNewTeam(request);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPatch(ApiEndPointConstant.Team.TeamEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTeamInfo(int id, TeamInfo request)
        {
            var isSuccessful = await _teamService.UpdateTeamInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [Authorize(Roles = RoleConstants.User)]
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