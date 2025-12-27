using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Challenge;
using Origami.DataTier.Paginate;

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
            var response = await _challengeService.GetChallengeDetailAsync(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Challenge.ChallengesEndPoint)]
        [ProducesResponseType(typeof(GetChallengeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllChallenges([FromQuery] ChallengeFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _challengeService.ViewAllChallenges(filter, pagingModel);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.Challenge.ChallengeListEndPoint)]
        [ProducesResponseType(typeof(IPaginate<ChallengeListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChallengeList([FromQuery] ChallengeListFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _challengeService.GetChallengeListAsync(filter, pagingModel);
            return Ok(response);
        }
        [HttpPost(ApiEndPointConstant.Challenge.ChallengesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateChallenge(ChallengeCreateDto request)
        {
            var response = await _challengeService.CreateChallengeAsync(request);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Challenge.ChallengeEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateChallengeInfo(int id, ChallengeInfo request)
        {
            var isSuccessful = await _challengeService.UpdateChallengeInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpDelete(ApiEndPointConstant.Challenge.ChallengeEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteChallenge(int id)
        {
            var isSuccessful = await _challengeService.DeleteChallenge(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
        [HttpPost("challenges/judges")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddJudge([FromBody] ChallengeJudgeCommandDto dto)
        {
            await _challengeService.AddJudgeToChallengeAsync(dto);
            return Ok("AddSucess");
        }

        [HttpDelete("challenges/judges")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveJudge([FromBody] ChallengeJudgeCommandDto dto)
        {
            await _challengeService.RemoveJudgeFromChallengeAsync(dto);
            return Ok("DeleteSuccess");
        }

    }
}