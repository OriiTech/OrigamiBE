using AutoMapper;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Badge;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class BadgeService : BaseService<BadgeService>, IBadgeService
    {
        public BadgeService(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            ILogger<BadgeService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateBadge(BadgeInfo request)
        {
            var repo = _unitOfWork.GetRepository<Badge>();

            var exists = await repo.AnyAsync(x => x.BadgeName == request.BadgeName);
            if (exists)
                throw new BadHttpRequestException("BadgeAlreadyExists");

            var badge = _mapper.Map<Badge>(request);
            await repo.InsertAsync(badge);
            await _unitOfWork.CommitAsync();

            return badge.BadgeId;
        }

        public async Task<GetBadgeResponse> GetBadgeById(int id)
        {
            var repo = _unitOfWork.GetRepository<Badge>();
            var badge = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.BadgeId == id,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("BadgeNotFound");

            return _mapper.Map<GetBadgeResponse>(badge);
        }

        public async Task<IPaginate<GetBadgeResponse>> ViewAllBadge(BadgeFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Badge>();

            Expression<Func<Badge, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.BadgeName) || x.BadgeName.Contains(filter.BadgeName)) &&
                (string.IsNullOrEmpty(filter.ConditionType) || x.ConditionType == filter.ConditionType);

            var result = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetBadgeResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderBy(x => x.BadgeName),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return result;
        }

        public async Task<bool> UpdateBadge(int id, BadgeInfo request)
        {
            var repo = _unitOfWork.GetRepository<Badge>();
            var badge = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.BadgeId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("BadgeNotFound");

            if (!string.IsNullOrEmpty(request.BadgeName))
                badge.BadgeName = request.BadgeName;

            if (!string.IsNullOrEmpty(request.BadgeDescription))
                badge.BadgeDescription = request.BadgeDescription;

            if (!string.IsNullOrEmpty(request.ConditionType))
                badge.ConditionType = request.ConditionType;

            if (!string.IsNullOrEmpty(request.ConditionValue))
                badge.ConditionValue = request.ConditionValue;

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteBadge(int id)
        {
            var repo = _unitOfWork.GetRepository<Badge>();
            var badge = await repo.GetFirstOrDefaultAsync(predicate: x => x.BadgeId == id);

            if (badge == null)
                throw new BadHttpRequestException("BadgeNotFound");

            repo.Delete(badge);
            return await _unitOfWork.CommitAsync() > 0;
        }
    }
}
