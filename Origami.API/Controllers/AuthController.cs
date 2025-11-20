using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload.Auth;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
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

        [HttpGet(ApiEndPointConstant.Auth.GoogleLogin)]
        public IActionResult GoogleLogin([FromQuery] string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet(ApiEndPointConstant.Auth.GoogleCallback)]
        public async Task<IActionResult> GoogleCallback([FromQuery] string returnUrl = "/")
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                await HttpContext.SignOutAsync("External");
                return Redirect($"{returnUrl}?error=ExternalLoginFailed");
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            await HttpContext.SignOutAsync("External");

            if (string.IsNullOrEmpty(email))
                return Redirect($"{returnUrl}?error=EmailRequired");

            var authResponse = await _authService.LoginWithGoogle(email, name ?? email);

            var query = new Dictionary<string, string?>
            {
                ["accessToken"] = authResponse.AccessToken,
                ["refreshToken"] = authResponse.RefreshToken,
                ["expiresIn"] = authResponse.AccessTokenExpiresAt?.ToString("o")
            };

            var redirectUrl = QueryHelpers.AddQueryString(returnUrl, query);
            return Redirect(redirectUrl);
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