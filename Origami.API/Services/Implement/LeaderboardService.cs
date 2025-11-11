// Origami.API/Services/Implement/LeaderboardService.cs
using AutoMapper;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Leaderboard;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class LeaderboardService : BaseService<LeaderboardService>, ILeaderboardService
    {
        public LeaderboardService(IUnitOfWork<OrigamiDbContext> uow, ILogger<LeaderboardService> logger, IMapper mapper, IHttpContextAccessor hca)
            : base(uow, logger, mapper, hca) { }

        public async Task<int> CreateNewLeaderboard(LeaderboardInfo request)
        {
            await EnsureChallengeExists(request.ChallengeId);
            if (request.TeamId.HasValue) await EnsureTeamExists(request.TeamId.Value);
            if (request.UserId.HasValue) await EnsureUserExists(request.UserId.Value);
            if (!request.TeamId.HasValue && !request.UserId.HasValue)
                throw new BadHttpRequestException("TeamOrUserRequired");

            var entity = _mapper.Map<Leaderboard>(request);
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Leaderboard>().InsertAsync(entity);
            if (await _unitOfWork.CommitAsync() <= 0) throw new BadHttpRequestException("CreateFailed");
            return entity.LeaderboardId;
        }

        public async Task<GetLeaderboardResponse> GetLeaderboardById(int id)
        {
            var lb = await _unitOfWork.GetRepository<Leaderboard>().GetFirstOrDefaultAsync(
                predicate: x => x.LeaderboardId == id,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("LeaderboardNotFound");

            return _mapper.Map<GetLeaderboardResponse>(lb);
        }

        public Task<IPaginate<GetLeaderboardResponse>> ViewAllLeaderboards(LeaderboardFilter filter, PagingModel paging)
        {
            return _unitOfWork.GetRepository<Leaderboard>().GetPagingListAsync(
                selector: x => _mapper.Map<GetLeaderboardResponse>(x),
                predicate: null,
                orderBy: q => q.OrderByDescending(x => x.TotalScore),
                include: null,
                page: paging.page,
                size: paging.size,
                filter: filter
            );
        }

        public async Task<bool> UpdateLeaderboard(int id, LeaderboardInfo request)
        {
            var repo = _unitOfWork.GetRepository<Leaderboard>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.LeaderboardId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("LeaderboardNotFound");

            if (request.ChallengeId != entity.ChallengeId)
                await EnsureChallengeExists(request.ChallengeId);
            if (request.TeamId != entity.TeamId && request.TeamId.HasValue)
                await EnsureTeamExists(request.TeamId.Value);
            if (request.UserId != entity.UserId && request.UserId.HasValue)
                await EnsureUserExists(request.UserId.Value);
            if (!request.TeamId.HasValue && !request.UserId.HasValue)
                throw new BadHttpRequestException("TeamOrUserRequired");

            entity.ChallengeId = request.ChallengeId;
            entity.TeamId = request.TeamId;
            entity.UserId = request.UserId;
            entity.TotalScore = request.TotalScore;
            entity.Rank = request.Rank;
            entity.UpdatedAt = DateTime.UtcNow;

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteLeaderboard(int id)
        {
            var repo = _unitOfWork.GetRepository<Leaderboard>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.LeaderboardId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("LeaderboardNotFound");

            repo.Delete(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        private async Task EnsureChallengeExists(int challengeId)
        {
            var ok = await _unitOfWork.GetRepository<Challenge>().AnyAsync(x => x.ChallengeId == challengeId);
            if (!ok) throw new BadHttpRequestException("ChallengeNotFound");
        }
        private async Task EnsureTeamExists(int teamId)
        {
            var ok = await _unitOfWork.GetRepository<Team>().AnyAsync(x => x.TeamId == teamId);
            if (!ok) throw new BadHttpRequestException("TeamNotFound");
        }
        private async Task EnsureUserExists(int userId)
        {
            var ok = await _unitOfWork.GetRepository<User>().AnyAsync(x => x.UserId == userId);
            if (!ok) throw new BadHttpRequestException("UserNotFound");
        }
    }
}