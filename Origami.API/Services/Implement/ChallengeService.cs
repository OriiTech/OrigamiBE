using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Challenge;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class ChallengeService : BaseService<ChallengeService>, IChallengeService
    {
        private readonly IConfiguration _configuration;

        public ChallengeService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<ChallengeService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewChallenge(ChallengeInfo request)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();

            if (request.CreatedBy.HasValue)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.CreatedBy.Value,
                    asNoTracking: true
                );
                if (user == null)
                    throw new BadHttpRequestException("UserNotFound");
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.EndDate.Value <= request.StartDate.Value)
                    throw new BadHttpRequestException("EndDateMustBeAfterStartDate");
            }

            var newChallenge = _mapper.Map<Challenge>(request);
            newChallenge.CreatedAt = DateTime.UtcNow;

            await repo.InsertAsync(newChallenge);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newChallenge.ChallengeId;
        }

        public async Task<GetChallengeResponse> GetChallengeById(int id)
        {
            Challenge challenge = await _unitOfWork.GetRepository<Challenge>().GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == id,
                include: q => q.Include(c => c.CreatedByNavigation),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            return _mapper.Map<GetChallengeResponse>(challenge);
        }

        public async Task<IPaginate<GetChallengeResponse>> ViewAllChallenges(ChallengeFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetChallengeResponse> response = await _unitOfWork.GetRepository<Challenge>().GetPagingListAsync(
                selector: x => _mapper.Map<GetChallengeResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(c => c.CreatedAt),
                include: q => q.Include(c => c.CreatedByNavigation),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateChallengeInfo(int id, ChallengeInfo request)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();
            var challenge = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            if (request.CreatedBy.HasValue && request.CreatedBy != challenge.CreatedBy)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.CreatedBy.Value,
                    asNoTracking: true
                );
                if (user == null)
                    throw new BadHttpRequestException("UserNotFound");

                challenge.CreatedBy = request.CreatedBy;
            }

            var startDate = request.StartDate ?? challenge.StartDate;
            var endDate = request.EndDate ?? challenge.EndDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                if (endDate.Value <= startDate.Value)
                    throw new BadHttpRequestException("EndDateMustBeAfterStartDate");
            }

            if (!string.IsNullOrEmpty(request.Title))
                challenge.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Description))
                challenge.Description = request.Description;
            if (!string.IsNullOrEmpty(request.ChallengeType))
                challenge.ChallengeType = request.ChallengeType;
            if (request.StartDate.HasValue)
                challenge.StartDate = request.StartDate;
            if (request.EndDate.HasValue)
                challenge.EndDate = request.EndDate;
            if (request.MaxTeamSize.HasValue)
                challenge.MaxTeamSize = request.MaxTeamSize;
            challenge.IsTeamBased = request.IsTeamBased;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteChallenge(int id)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();
            var challenge = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == id,
                include: q => q.Include(c => c.Leaderboards)
                              .Include(c => c.Submissions)
                              .Include(c => c.Teams),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            if (challenge.Leaderboards != null && challenge.Leaderboards.Any())
                throw new BadHttpRequestException("ChallengeHasLeaderboardsCannotDelete");
            if (challenge.Submissions != null && challenge.Submissions.Any())
                throw new BadHttpRequestException("ChallengeHasSubmissionsCannotDelete");
            if (challenge.Teams != null && challenge.Teams.Any())
                throw new BadHttpRequestException("ChallengeHasTeamsCannotDelete");

            repo.Delete(challenge);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}