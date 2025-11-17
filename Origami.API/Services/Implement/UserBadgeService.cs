using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.UserBadge;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class UserBadgeService : BaseService<UserBadgeService>, IUserBadgeService
    {
        public UserBadgeService(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            ILogger<UserBadgeService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<bool> GrantBadge(UserBadgeInfo request)
        {
            var repo = _unitOfWork.GetRepository<UserBadge>();

            var exists = await repo.AnyAsync(x => x.UserId == request.UserId && x.BadgeId == request.BadgeId);
            if (exists)
                throw new BadHttpRequestException("UserAlreadyHasBadge");

            var ub = _mapper.Map<UserBadge>(request);
            ub.EarnedAt ??= DateTime.UtcNow;

            await repo.InsertAsync(ub);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> RevokeBadge(int userId, int badgeId)
        {
            var repo = _unitOfWork.GetRepository<UserBadge>();

            var ub = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == userId && x.BadgeId == badgeId);

            if (ub == null)
                throw new BadHttpRequestException("UserBadgeNotFound");

            repo.Delete(ub);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<GetUserBadgeResponse> GetUserBadge(int userId, int badgeId)
        {
            var repo = _unitOfWork.GetRepository<UserBadge>();

            var ub = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == userId && x.BadgeId == badgeId,
                include: q => q.Include(x => x.Badge),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("UserBadgeNotFound");

            return _mapper.Map<GetUserBadgeResponse>(ub);
        }

        public async Task<IPaginate<GetUserBadgeResponse>> ViewAllUserBadges(UserBadgeFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<UserBadge>();

            Expression<Func<UserBadge, bool>> predicate = x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId) &&
                (!filter.BadgeId.HasValue || x.BadgeId == filter.BadgeId) &&
                (!filter.EarnedAt.HasValue || x.EarnedAt.Value.Date == filter.EarnedAt.Value.Date);

            var result = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetUserBadgeResponse>(x),
                predicate: predicate,
                include: q => q.Include(x => x.Badge),
                orderBy: q => q.OrderByDescending(x => x.EarnedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return result;
        }
    }
}
