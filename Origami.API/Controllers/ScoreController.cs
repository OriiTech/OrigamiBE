using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Score;
using Origami.BusinessTier.Utils.EnumConvert;

namespace Origami.API.Controllers
{
    [ApiController]
    public class ScoreController : BaseController<ScoreController>
    {
        private readonly IScoreService _scoreService;

        public ScoreController(ILogger<ScoreController> logger, IScoreService scoreService) : base(logger)
        {
            _scoreService = scoreService;
        }

        [HttpGet(ApiEndPointConstant.Score.ScoreEndPoint)]
        [ProducesResponseType(typeof(GetScoreResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScore(int id)
        {
            var response = await _scoreService.GetScoreById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Score.ScoresEndPoint)]
        [ProducesResponseType(typeof(GetScoreResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllScores([FromQuery] ScoreFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _scoreService.ViewAllScores(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.Sensei)]
        [HttpPost(ApiEndPointConstant.Score.ScoresEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateScore(ScoreInfo request)
        {
            var response = await _scoreService.CreateNewScore(request);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Score.ScoreEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateScore(int id, ScoreInfo request)
        {
            var isSuccessful = await _scoreService.UpdateScore(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpDelete(ApiEndPointConstant.Score.ScoreEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteScore(int id)
        {
            var isSuccessful = await _scoreService.DeleteScore(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}