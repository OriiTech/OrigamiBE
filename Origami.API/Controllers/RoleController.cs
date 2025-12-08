using Microsoft.AspNetCore.Authorization;
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


        //Get role by id

        [Authorize(Roles ="admin, staff")]
        [HttpGet(ApiEndPointConstant.Role.RoleEndPoint)]
        [ProducesResponseType(typeof(GetRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRole(int id)
        {
            var response = await _roleService.GetRoleById(id);
            return Ok(response);
        }


        //get all roles with filter and paging

        [Authorize(Roles ="admin, staff")]
        [HttpGet(ApiEndPointConstant.Role.RolesEndPoint)]
        [ProducesResponseType(typeof(GetRoleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllRoles([FromQuery] RoleFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _roleService.ViewAllRoles(filter, pagingModel);
            return Ok(response);
        }

        //create new role

        [Authorize(Roles ="admin")]
        [HttpPost(ApiEndPointConstant.Role.RolesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRole(RoleInfo request)
        {
            var response = await _roleService.CreateNewRole(request);
            return Ok(response);
        }


        //update role info

        [Authorize(Roles ="admin")]
        [HttpPatch(ApiEndPointConstant.Role.RoleEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRoleInfo(int id, RoleInfo request)
        {
            var isSuccessful = await _roleService.UpdateRoleInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        //delete role

        [Authorize(Roles ="admin")]
        [HttpDelete(ApiEndPointConstant.Role.RoleEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var isSuccessful = await _roleService.DeleteRoleById(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}