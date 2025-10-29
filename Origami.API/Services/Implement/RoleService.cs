using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Role;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class RoleService : BaseService<RoleService>, IRoleService
    {
        private readonly IConfiguration _configuration;

        public RoleService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<RoleService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewRole(RoleInfo request)
        {
            var repo = _unitOfWork.GetRepository<Role>();

            var existingRole = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.RoleName.ToLower() == request.RoleName.ToLower(),
                asNoTracking: true
            );

            if (existingRole != null)
                throw new BadHttpRequestException("RoleExisted");

            var newRole = _mapper.Map<Role>(request);

            await repo.InsertAsync(newRole);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newRole.RoleId;
        }

        public async Task<GetRoleResponse> GetRoleById(int id)
        {
            Role role = await _unitOfWork.GetRepository<Role>().GetFirstOrDefaultAsync(
                predicate: x => x.RoleId == id,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("RoleNotFound");

            return _mapper.Map<GetRoleResponse>(role);
        }

        public async Task<IPaginate<GetRoleResponse>> ViewAllRoles(RoleFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetRoleResponse> response = await _unitOfWork.GetRepository<Role>().GetPagingListAsync(
                selector: x => _mapper.Map<GetRoleResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(r => r.RoleName),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateRoleInfo(int id, RoleInfo request)
        {
            var repo = _unitOfWork.GetRepository<Role>();
            var role = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.RoleId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("RoleNotFound");

            if (!string.IsNullOrEmpty(request.RoleName) && request.RoleName != role.RoleName)
            {
                bool roleNameExists = await repo.AnyAsync(x => x.RoleName.ToLower() == request.RoleName.ToLower());
                if (roleNameExists)
                    throw new BadHttpRequestException("RoleNameAlreadyUsed");

                role.RoleName = request.RoleName;
            }

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }
    }
}