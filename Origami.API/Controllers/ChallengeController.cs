using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Challenge;
using Origami.BusinessTier.Utils.EnumConvert;

namespace Origami.API.Controllers
{
    [ApiController]
    public class ChallengeController : BaseController<ChallengeController>
    {
        private readonly IChallengeService _challengeService;

        public ChallengeController(ILogger<ChallengeController> logger, IChallengeService challengeService) : base(logger)
        {
            _challengeService = challengeService;
        }

        [HttpGet(ApiEndPointConstant.Challenge.ChallengeEndPoint)]
        [ProducesResponseType(typeof(GetChallengeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChallenge(int id)
        {
            var response = await _challengeService.GetChallengeById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Challenge.ChallengesEndPoint)]
        [ProducesResponseType(typeof(GetChallengeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllChallenges([FromQuery] ChallengeFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _challengeService.ViewAllChallenges(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.Staff)]
        [HttpPost(ApiEndPointConstant.Challenge.ChallengesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateChallenge(ChallengeInfo request)
        {
            var response = await _challengeService.CreateNewChallenge(request);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.Staff)]
        [HttpPatch(ApiEndPointConstant.Challenge.ChallengeEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateChallengeInfo(int id, ChallengeInfo request)
        {
            var isSuccessful = await _challengeService.UpdateChallengeInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [Authorize(Roles = RoleConstants.Staff)]
        [HttpDelete(ApiEndPointConstant.Challenge.ChallengeEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteChallenge(int id)
        {
            var isSuccessful = await _challengeService.DeleteChallenge(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}