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
        protected int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
