using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Role;

namespace Origami.API.Controllers
{
    [ApiController]
    public class RoleController : BaseController<RoleController>
    {
        private readonly IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService) : base(logger)
        {
            _roleService = roleService;
        }

        [HttpGet(ApiEndPointConstant.Role.RoleEndPoint)]
        [ProducesResponseType(typeof(GetRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRole(int id)
        {
            var response = await _roleService.GetRoleById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Role.RolesEndPoint)]
        [ProducesResponseType(typeof(GetRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllRoles([FromQuery] RoleFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _roleService.ViewAllRoles(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Role.RolesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRole(RoleInfo request)
        {
            var response = await _roleService.CreateNewRole(request);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Role.RoleEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRoleInfo(int id, RoleInfo request)
        {
            var isSuccessful = await _roleService.UpdateRoleInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}