using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload.UserBadge;
using Origami.DataTier.Models;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Implement
{
    public class BadgeEvaluator : IBadgeEvaluator
    {
        private readonly IUnitOfWork<OrigamiDbContext> _unitOfWork;
        private readonly IUserBadgeService _userBadgeService;

        public BadgeEvaluator(
            IUnitOfWork<OrigamiDbContext> unitOfWork,
            IUserBadgeService userBadgeService)
        {
            _unitOfWork = unitOfWork;
            _userBadgeService = userBadgeService;
        }

        public async Task EvaluateBadgesForUser(int userId)
        {
            var badgeRepo = _unitOfWork.GetRepository<Badge>();
            var userBadgeRepo = _unitOfWork.GetRepository<UserBadge>();

            // check user badge list
            var userBadges = await userBadgeRepo.GetListAsync(
                predicate: x => x.UserId == userId
            );
            var ownedBadgeIds = userBadges.Select(x => x.BadgeId).ToHashSet();

            // check all badge conditions
            var allBadges = await badgeRepo.GetListAsync();

            foreach (var badge in allBadges)
            {
                if (ownedBadgeIds.Contains(badge.BadgeId))
                    continue;

                if (await CheckCondition(userId, badge))
                {
                    await _userBadgeService.GrantBadge(new UserBadgeInfo
                    {
                        UserId = userId,
                        BadgeId = badge.BadgeId,
                        EarnedAt = DateTime.UtcNow
                    });
                }
            }
        }

        private async Task<bool> CheckCondition(int userId, Badge badge)
        {
            switch (badge.ConditionType)
            {
                case "GuideCreatedCount":
                    return await CheckGuideCreated(userId, badge);

                case "CommentCount":
                    return await CheckCommentCount(userId, badge);

                default:
                    return false;
            }
        }

        private async Task<bool> CheckGuideCreated(int userId, Badge badge)
        {
            var repo = _unitOfWork.GetRepository<Guide>();
            int count = await repo.CountAsync(x => x.AuthorId == userId);
            return count >= int.Parse(badge.ConditionValue);
        }

        private async Task<bool> CheckCommentCount(int userId, Badge badge)
        {
            var repo = _unitOfWork.GetRepository<Comment>();
            int count = await repo.CountAsync(x => x.UserId == userId);
            return count >= int.Parse(badge.ConditionValue);
        }
    }
}
