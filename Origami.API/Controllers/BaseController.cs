using Microsoft.AspNetCore.Mvc;
using Origami.BusinessTier.Constants;
using System.Security.Claims;

namespace Origami.API.Controllers
{
    [Route(ApiEndPointConstant.ApiEndpoint)]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        protected ILogger<T> _logger;
        protected int CurrentUserId
        {
            get
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(claim))
                    throw new UnauthorizedAccessException("User is not logged in.");
                return int.Parse(claim);
            }
        }
        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
