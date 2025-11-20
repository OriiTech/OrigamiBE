using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.TeamMember;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class TeamMemberService : BaseService<TeamMemberService>, ITeamMemberService
    {
        private readonly IConfiguration _configuration;

        public TeamMemberService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<TeamMemberService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewTeamMember(BulkAddTeamMemberRequest request)
        {
            var teamRepo = _unitOfWork.GetRepository<Team>();
            var team = await teamRepo.GetFirstOrDefaultAsync(
                predicate: x => x.TeamId == request.TeamId,
                include: q => q.Include(t => t.Challenge).Include(t => t.TeamMembers),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("TeamNotFound");

            var challenge = team.Challenge;
            if (!challenge.IsTeamBased)
                throw new BadHttpRequestException("SoloChallengeCannotAddMembersManually");

            if (request.Members == null || request.Members.Count == 0)
                throw new BadHttpRequestException("MembersRequired");

            var userRepo = _unitOfWork.GetRepository<User>();
            var teamMemberRepo = _unitOfWork.GetRepository<TeamMember>();

            var existingCount = team.TeamMembers?.Count ?? 0;
            var currentCount = existingCount;

            foreach (var member in request.Members)
            {
                // lấy user theo email hoặc username
                if (string.IsNullOrWhiteSpace(member.Email) && string.IsNullOrWhiteSpace(member.Username))
                    throw new BadHttpRequestException("MemberIdentityRequired");

                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x =>
                        (!string.IsNullOrWhiteSpace(member.Email) && x.Email == member.Email) ||
                        (!string.IsNullOrWhiteSpace(member.Username) && x.Username == member.Username),
                    asNoTracking: true
                ) ?? throw new BadHttpRequestException($"UserNotFound: {member.Email ?? member.Username}");

                // check trùng
                bool alreadyExists = team.TeamMembers.Any(tm => tm.UserId == user.UserId);
                if (alreadyExists)
                    continue; // hoặc throw nếu muốn

                // check MaxTeamSize
                if (challenge.MaxTeamSize.HasValue && currentCount >= challenge.MaxTeamSize.Value)
                    throw new BadHttpRequestException("TeamIsFull");

                var newMember = new TeamMember
                {
                    TeamId = team.TeamId,
                    UserId = user.UserId,
                    JoinedAt = DateTime.UtcNow
                };

                await teamMemberRepo.InsertAsync(newMember);
                currentCount++;
            }

            if (currentCount == existingCount)
                throw new BadHttpRequestException("NoMemberAdded");

            await _unitOfWork.CommitAsync();
            return currentCount - existingCount;
        }

        public async Task<GetTeamMemberResponse> GetTeamMemberById(int id)
        {
            TeamMember teamMember = await _unitOfWork.GetRepository<TeamMember>().GetFirstOrDefaultAsync(
                predicate: x => x.TeamMemberId == id,
                include: q => q.Include(tm => tm.Team).Include(tm => tm.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("TeamMemberNotFound");

            return _mapper.Map<GetTeamMemberResponse>(teamMember);
        }

        public async Task<IPaginate<GetTeamMemberResponse>> ViewAllTeamMembers(TeamMemberFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetTeamMemberResponse> response = await _unitOfWork.GetRepository<TeamMember>().GetPagingListAsync(
                selector: x => _mapper.Map<GetTeamMemberResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(tm => tm.JoinedAt),
                include: q => q.Include(tm => tm.Team).Include(tm => tm.User),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateTeamMember(int id, TeamMemberInfo request)
        {
            var repo = _unitOfWork.GetRepository<TeamMember>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TeamMemberId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("TeamMemberNotFound");

            // Nếu đổi TeamId → verify team tồn tại + còn slot
            if (request.TeamId != 0 && request.TeamId != entity.TeamId)
            {
                var teamRepo = _unitOfWork.GetRepository<Team>();
                var newTeam = await teamRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.TeamId == request.TeamId,
                    include: q => q.Include(t => t.Challenge).Include(t => t.TeamMembers),
                    asNoTracking: true
                ) ?? throw new BadHttpRequestException("TeamNotFound");

                // MaxTeamSize check
                if (newTeam.Challenge != null && newTeam.Challenge.MaxTeamSize.HasValue)
                {
                    var count = newTeam.TeamMembers?.Count ?? 0;
                    if (count >= newTeam.Challenge.MaxTeamSize.Value)
                        throw new BadHttpRequestException("TeamIsFull");
                }

                entity.TeamId = request.TeamId;
            }

            // Nếu đổi UserId → verify user tồn tại
            if (request.UserId != 0 && request.UserId != entity.UserId)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.UserId,
                    asNoTracking: true
                ) ?? throw new BadHttpRequestException("UserNotFound");

                entity.UserId = request.UserId;
            }

            // Unique constraint: (TeamId, UserId) phải duy nhất
            bool duplicate = await repo.AnyAsync(x =>
                x.TeamId == entity.TeamId &&
                x.UserId == entity.UserId &&
                x.TeamMemberId != entity.TeamMemberId
            );
            if (duplicate)
                throw new BadHttpRequestException("UserAlreadyInTeam");

            // Không tự đổi JoinedAt khi update
            var ok = await _unitOfWork.CommitAsync() > 0;
            return ok;
        }

        public async Task<bool> DeleteTeamMember(int id)
        {
            var repo = _unitOfWork.GetRepository<TeamMember>();
            var teamMember = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TeamMemberId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("TeamMemberNotFound");

            repo.Delete(teamMember);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}