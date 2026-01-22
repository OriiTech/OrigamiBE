using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.UserBadge;
using Origami.BusinessTier.Utils.EnumConvert;

namespace Origami.API.Controllers
{
    [ApiController]
    public class UserBadgeController : BaseController<UserBadgeController>
    {
        private readonly IUserBadgeService _userBadgeService;

        public UserBadgeController(
            ILogger<UserBadgeController> logger,
            IUserBadgeService userBadgeService)
            : base(logger)
        {
            _userBadgeService = userBadgeService;
        }

        [Authorize(Roles = RoleConstants.Staff)]
        [HttpPost(ApiEndPointConstant.UserBadge.UserBadgesEndPoint)]
        public async Task<IActionResult> GrantBadge(UserBadgeInfo request)
        {
            var ok = await _userBadgeService.GrantBadge(request);
            return Ok(ok ? "GrantBadgeSuccess" : "GrantBadgeFailed");
        }

        [Authorize(Roles = RoleConstants.Staff)]
        [HttpDelete(ApiEndPointConstant.UserBadge.UserBadgeEndPoint)]
        public async Task<IActionResult> RevokeBadge(int userId, int badgeId)
        {
            var ok = await _userBadgeService.RevokeBadge(userId, badgeId);
            return Ok(ok ? "RevokeBadgeSuccess" : "RevokeBadgeFailed");
        }

        [HttpGet(ApiEndPointConstant.UserBadge.UserBadgeEndPoint)]
        public async Task<IActionResult> GetUserBadge(int userId, int badgeId)
        {
            var response = await _userBadgeService.GetUserBadge(userId, badgeId);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.UserBadge.UserBadgesEndPoint)]
        public async Task<IActionResult> ViewAll([FromQuery] UserBadgeFilter filter, [FromQuery] PagingModel paging)
        {
            var result = await _userBadgeService.ViewAllUserBadges(filter, paging);
            return Ok(result);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet(ApiEndPointConstant.UserBadge.MyBadgesEndPoint)]
        public async Task<IActionResult> GetMyBadges()
        {
            var result = await _userBadgeService.GetMyBadgesAsync();
            return Ok(result);
        }
    }
}
