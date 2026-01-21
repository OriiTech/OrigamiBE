using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload.UserProfile;

namespace Origami.API.Controllers
{
    [ApiController]
    public class UserProfileController : BaseController<UserProfileController>
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(
            ILogger<UserProfileController> logger,
            IUserProfileService userProfileService) : base(logger)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Lấy profile của chính user đang đăng nhập
        /// </summary>
        [Authorize(Roles = RoleConstants.User)]
        [HttpGet(ApiEndPointConstant.User.MyProfileEndPoint)]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile()
        {
            var response = await _userProfileService.GetMyProfileAsync();
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật profile (display_name, avatar_url, bio) cho user hiện tại
        /// </summary>
        [Authorize(Roles = RoleConstants.User)]
        [HttpPut(ApiEndPointConstant.User.MyProfileEndPoint)]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileRequest request)
        {
            var response = await _userProfileService.UpdateMyProfileAsync(request);
            return Ok(response);
        }
    }
}

