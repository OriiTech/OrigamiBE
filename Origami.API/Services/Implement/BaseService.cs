using AutoMapper;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;
using System.Security.Claims;
using static Origami.BusinessTier.Constants.ApiEndPointConstant;

namespace Origami.API.Services.Implement
{
    public abstract class BaseService<T> where T : class
    {
        protected IUnitOfWork<OrigamiDbContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        protected BaseService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<T> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string? GetEmailFromJwt()
        {
            return _httpContextAccessor?
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.Email);
        }

        protected string? GetRoleFromJwt()
        {
            return _httpContextAccessor?
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.Role);
        }
        protected int? GetCurrentUserId()
        {
            var idStr = _httpContextAccessor?.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(idStr, out var id) ? id : null;
        }
    }
}
