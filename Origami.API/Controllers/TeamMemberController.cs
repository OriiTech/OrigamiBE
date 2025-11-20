using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.TeamMember;

namespace Origami.API.Controllers
{
    [ApiController]
    public class TeamMemberController : BaseController<TeamMemberController>
    {
        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberController(ILogger<TeamMemberController> logger, ITeamMemberService teamMemberService) : base(logger)
        {
            _teamMemberService = teamMemberService;
        }

        [HttpGet(ApiEndPointConstant.TeamMember.TeamMemberEndPoint)]
        [ProducesResponseType(typeof(GetTeamMemberResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamMember(int id)
        {
            var response = await _teamMemberService.GetTeamMemberById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.TeamMember.TeamMembersEndPoint)]
        [ProducesResponseType(typeof(GetTeamMemberResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllTeamMembers([FromQuery] TeamMemberFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _teamMemberService.ViewAllTeamMembers(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = "1")]
        [HttpPost(ApiEndPointConstant.TeamMember.TeamMembersEndPoint + "/bulk")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkAddMembers([FromBody] BulkAddTeamMemberRequest request)
        {
            var added = await _teamMemberService.CreateNewTeamMember(request);
            return Ok($"{added} members added");
        }

        [Authorize(Roles = "1")]
        [HttpPatch(ApiEndPointConstant.TeamMember.TeamMemberEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTeamMember(int id, TeamMemberInfo request)
        {
            var ok = await _teamMemberService.UpdateTeamMember(id, request);
            if (!ok) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [Authorize(Roles = "1")]
        [HttpDelete(ApiEndPointConstant.TeamMember.TeamMemberEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTeamMember(int id)
        {
            var isSuccessful = await _teamMemberService.DeleteTeamMember(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}