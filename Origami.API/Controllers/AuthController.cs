using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload.Auth;
using Origami.BusinessTier.Payload.User;

namespace Origami.API.Controllers
{
    [ApiController]
    public class AuthController : BaseController<AuthController>
    {
        private readonly IAuthService _authService;
        private readonly IPasswordHashService _authServiceHash;

        public AuthController(ILogger<AuthController> logger, IAuthService authService, IPasswordHashService authServiceHash) : base(logger)
        {
            _authService = authService;
            _authServiceHash = authServiceHash;
        }

        [HttpPost(ApiEndPointConstant.Auth.Register)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var res = await _authService.Register(request);
            return CreatedAtAction(nameof(Login), new { }, res);
        }

        [HttpPost(ApiEndPointConstant.Auth.Login)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var res = await _authService.Login(request);
            return Ok(res);
        }

        [HttpPost(ApiEndPointConstant.Auth.Refresh)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var res = await _authService.Refresh(request);
            return Ok(res);
        }

        [HttpPost(ApiEndPointConstant.Auth.Logout)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            var ok = await _authService.Logout(request.RefreshToken);
            return Ok(ok ? "LogoutSuccess" : "LogoutFailed");
        }

        [HttpPost(ApiEndPointConstant.Auth.HashPassword)]
        [ProducesResponseType(typeof(HashPasswordResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> HashPassword([FromBody] HashPasswordRequest request)
        {
            var res = await _authServiceHash.HashAsync(request);
            return Ok(res);
        }
    }
}