using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Vote;
using Origami.BusinessTier.Utils.EnumConvert;

namespace Origami.API.Controllers
{
    [ApiController]
    public class VoteController : BaseController<VoteController>
    {
        private readonly IVoteService _voteService;

        public VoteController(ILogger<VoteController> logger, IVoteService voteService) : base(logger)
        {
            _voteService = voteService;
        }

        [HttpGet(ApiEndPointConstant.Vote.VoteEndPoint)]
        [ProducesResponseType(typeof(GetVoteResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVote(int id)
        {
            var response = await _voteService.GetVoteById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Vote.VotesEndPoint)]
        [ProducesResponseType(typeof(GetVoteResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllVotes([FromQuery] VoteFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _voteService.ViewAllVotes(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPost(ApiEndPointConstant.Vote.VotesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateVote(VoteInfo request)
        {
            var response = await _voteService.CreateNewVote(request);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpDelete(ApiEndPointConstant.Vote.VoteEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVote(int id)
        {
            var isSuccessful = await _voteService.DeleteVote(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}