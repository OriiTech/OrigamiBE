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
        private readonly IConfiguration _configuration;

        public AuthController(
            ILogger<AuthController> logger,
            IAuthService authService,
            IPasswordHashService authServiceHash,
            IConfiguration configuration) : base(logger)
        {
            _authService = authService;
            _authServiceHash = authServiceHash;
            _configuration = configuration;
        }

        [HttpPost(ApiEndPointConstant.Auth.SendOtp)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpForRegisterRequest request)
        {
            await _authService.SendOtpForRegisterAsync(request.Email);
            return Ok(new { message = "OtpSent" });
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
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
        {
            // Khi gọi từ Swagger hoặc API client, trả về link đến endpoint initiate để middleware quản lý state
            if (Request.Headers["Accept"].Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase)) ||
                !Request.Headers["Accept"].Any())
            {
                var (scheme, host) = ResolveRequestContext();
                
                // Build URL đến endpoint initiate với returnUrl trong query string
                // Endpoint này sẽ sử dụng Challenge() để middleware tự động quản lý state
                var initiateUrl = $"{scheme}://{host}{ApiEndPointConstant.Auth.GoogleLoginInitiate}";
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    initiateUrl += $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
                }

                return Ok(new
                {
                    authorizationUrl = initiateUrl,
                    message = "Open this URL in your browser to continue Google authentication.",
                    redirectUri = $"{scheme}://{host}{ApiEndPointConstant.Auth.GoogleCallback}"
                });
            }

            // Khi truy cập trực tiếp từ browser, thực hiện Challenge
            var finalReturnUrl = returnUrl ?? "/";
            var callbackUrl = BuildCallbackUrl(finalReturnUrl);
            var properties = new AuthenticationProperties
            {
                RedirectUri = callbackUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet(ApiEndPointConstant.Auth.GoogleLoginInitiate)]
        public IActionResult GoogleLoginInitiate([FromQuery] string? returnUrl = null)
        {
            // Endpoint này được gọi từ mobile app để initiate OAuth flow
            // Sử dụng Challenge() để middleware tự động quản lý state trong cookie
            var finalReturnUrl = returnUrl ?? "/";
            var callbackUrl = BuildCallbackUrl(finalReturnUrl);
            var properties = new AuthenticationProperties
            {
                RedirectUri = callbackUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet(ApiEndPointConstant.Auth.GoogleCallback)]
        public async Task<IActionResult> GoogleCallback([FromQuery] string returnUrl = "/")
        {
            try
            {
                _logger.LogInformation("Google callback received. ReturnUrl: {ReturnUrl}", returnUrl);

                var authenticateResult = await HttpContext.AuthenticateAsync("External");
                if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                {
                    _logger.LogWarning("Google authentication failed. Succeeded: {Succeeded}, Principal: {Principal}", 
                        authenticateResult.Succeeded, authenticateResult.Principal == null ? "null" : "not null");
                    await HttpContext.SignOutAsync("External");
                    return Redirect($"{returnUrl}?error=ExternalLoginFailed");
                }

                var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

                _logger.LogInformation("Google authentication succeeded. Email: {Email}, Name: {Name}", email, name);

                await HttpContext.SignOutAsync("External");

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email not found in Google claims");
                    return Redirect($"{returnUrl}?error=EmailRequired");
                }

                _logger.LogInformation("Calling LoginWithGoogle for email: {Email}", email);
                var authResponse = await _authService.LoginWithGoogle(email, name ?? email);
                _logger.LogInformation("LoginWithGoogle succeeded for email: {Email}", email);

                var query = new Dictionary<string, string?>
                {
                    ["accessToken"] = authResponse.AccessToken,
                    ["refreshToken"] = authResponse.RefreshToken,
                    ["expiresIn"] = authResponse.AccessTokenExpiresAt?.ToString("o")
                };

                var redirectUrl = QueryHelpers.AddQueryString(returnUrl, query);
                _logger.LogInformation("Redirecting to: {RedirectUrl}", redirectUrl);
                return Redirect(redirectUrl);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Bad request in Google callback. Message: {Message}", ex.Message);
                return Redirect($"{returnUrl}?error={Uri.EscapeDataString(ex.Message)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error was encountered while handling the remote login.");
                return Redirect($"{returnUrl}?error=InternalServerError");
            }
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

        [HttpPost(ApiEndPointConstant.Auth.ChangePassword)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var res = await _authService.ChangePassword(request);
            return Ok(res);
        }

        [HttpPost(ApiEndPointConstant.Auth.ChangeEmail)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var res = await _authService.ChangeEmail(request);
            return Ok(res);
        }

        private (string Scheme, string Host) ResolveRequestContext()
        {
            var scheme = Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? Request.Scheme;
            var host = Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? Request.Host.Value;
            return (scheme, host);
        }

        private string BuildCallbackUrl(string returnUrl)
        {
            var (scheme, host) = ResolveRequestContext();
            // Build full URL to our callback endpoint
            var callbackUrl = Url.ActionLink(
                action: nameof(GoogleCallback),
                controller: null,
                values: new { returnUrl },
                protocol: scheme,
                host: host);
            
            // Fallback nếu Url.ActionLink trả null
            if (string.IsNullOrEmpty(callbackUrl))
            {
                callbackUrl = $"{scheme}://{host}{ApiEndPointConstant.Auth.GoogleCallback}?returnUrl={Uri.EscapeDataString(returnUrl)}";
            }
            
            return callbackUrl;
        }
    }
}