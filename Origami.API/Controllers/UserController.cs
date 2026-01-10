using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.User;
using Origami.BusinessTier.Utils.EnumConvert;

namespace Origami.API.Controllers
{
    [ApiController]
    public class UserController : BaseController<UserController>
    {
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }

        [HttpGet(ApiEndPointConstant.User.UserEndPoint)]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(int id)
        {
            var response = await _userService.GetUserById(id);
            return Ok(response);
        }
        [HttpGet(ApiEndPointConstant.User.UsersEndPoint)]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllUser([FromQuery] UserFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _userService.ViewAllUser(filter, pagingModel);
            return Ok(response);
        }

        [Authorize(Roles = RoleConstants.User)]
        [HttpPatch(ApiEndPointConstant.User.UserEndPoint)]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserInfo(int id, UserInfo request)
        {
            var isSuccessful = await _userService.UpdateUserInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
        [Authorize(Roles = RoleConstants.Staff)]
        [HttpPatch(ApiEndPointConstant.User.UpdateUserRoleEndPoint)]
        [ProducesResponseType(typeof(UpdateUserRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request, [FromHeader(Name = "X-Admin-Email")] string? adminEmail)
        {
            if (string.IsNullOrEmpty(adminEmail))
                return BadRequest("Admin email is required in X-Admin-Email header");
                request.UserId = id;
                var response = await _userService.UpdateUserRole(request, adminEmail);
            return Ok(response);
        }
    }
}
