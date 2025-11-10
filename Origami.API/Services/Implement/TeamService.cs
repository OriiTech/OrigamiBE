using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Team;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class TeamService : BaseService<TeamService>, ITeamService
    {
        private readonly IConfiguration _configuration;

        public TeamService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<TeamService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewTeam(TeamInfo request)
        {
            var repo = _unitOfWork.GetRepository<Team>();

            // Kiểm tra ChallengeId có tồn tại không
            var challengeRepo = _unitOfWork.GetRepository<Challenge>();
            var challenge = await challengeRepo.GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == request.ChallengeId,
                asNoTracking: true
            );
            if (challenge == null)
                throw new BadHttpRequestException("ChallengeNotFound");

            // Kiểm tra TeamName có trùng trong cùng Challenge không
            bool teamNameExists = await repo.AnyAsync(
                x => x.ChallengeId == request.ChallengeId && x.TeamName.ToLower() == request.TeamName.ToLower()
            );
            if (teamNameExists)
                throw new BadHttpRequestException("TeamNameAlreadyExistsInChallenge");

            var newTeam = _mapper.Map<Team>(request);
            newTeam.CreatedAt = DateTime.UtcNow;

            await repo.InsertAsync(newTeam);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newTeam.TeamId;
        }

        public async Task<GetTeamResponse> GetTeamById(int id)
        {
            Team team = await _unitOfWork.GetRepository<Team>().GetFirstOrDefaultAsync(
                predicate: x => x.TeamId == id,
                include: q => q.Include(t => t.Challenge).Include(t => t.TeamMembers),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("TeamNotFound");

            var response = _mapper.Map<GetTeamResponse>(team);
            response.MemberCount = team.TeamMembers?.Count ?? 0;
            return response;
        }

        public async Task<IPaginate<GetTeamResponse>> ViewAllTeams(TeamFilter filter, PagingModel pagingModel)
        {
            var response = await _unitOfWork.GetRepository<Team>().GetPagingListAsync(
                selector: x => new GetTeamResponse
                {
                    TeamId = x.TeamId,
                    ChallengeId = x.ChallengeId,
                    ChallengeTitle = x.Challenge != null ? x.Challenge.Title : null,
                    TeamName = x.TeamName,
                    CreatedAt = x.CreatedAt,
                    MemberCount = x.TeamMembers != null ? x.TeamMembers.Count : 0
                },
                filter: filter,
                orderBy: x => x.OrderByDescending(t => t.CreatedAt),
                include: q => q.Include(t => t.Challenge).Include(t => t.TeamMembers),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateTeamInfo(int id, TeamInfo request)
        {
            var repo = _unitOfWork.GetRepository<Team>();
            var team = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TeamId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("TeamNotFound");

            // Kiểm tra ChallengeId có tồn tại không (nếu có thay đổi)
            if (request.ChallengeId != team.ChallengeId)
            {
                var challengeRepo = _unitOfWork.GetRepository<Challenge>();
                var challenge = await challengeRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.ChallengeId == request.ChallengeId,
                    asNoTracking: true
                );
                if (challenge == null)
                    throw new BadHttpRequestException("ChallengeNotFound");

                team.ChallengeId = request.ChallengeId;
            }

            // Kiểm tra TeamName có trùng trong cùng Challenge không (nếu có thay đổi)
            if (!string.IsNullOrEmpty(request.TeamName) && request.TeamName != team.TeamName)
            {
                bool teamNameExists = await repo.AnyAsync(
                    x => x.ChallengeId == team.ChallengeId &&
                         x.TeamName.ToLower() == request.TeamName.ToLower() &&
                         x.TeamId != id
                );
                if (teamNameExists)
                    throw new BadHttpRequestException("TeamNameAlreadyExistsInChallenge");

                team.TeamName = request.TeamName;
            }

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteTeam(int id)
        {
            var repo = _unitOfWork.GetRepository<Team>();
            var team = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TeamId == id,
                include: q => q.Include(t => t.TeamMembers)
                              .Include(t => t.Leaderboards)
                              .Include(t => t.Submissions),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("TeamNotFound");

            // Kiểm tra xem Team có TeamMember, Leaderboard, hoặc Submission nào không
            if (team.TeamMembers != null && team.TeamMembers.Any())
                throw new BadHttpRequestException("TeamHasMembersCannotDelete");
            if (team.Leaderboards != null && team.Leaderboards.Any())
                throw new BadHttpRequestException("TeamHasLeaderboardsCannotDelete");
            if (team.Submissions != null && team.Submissions.Any())
                throw new BadHttpRequestException("TeamHasSubmissionsCannotDelete");

            repo.Delete(team);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}