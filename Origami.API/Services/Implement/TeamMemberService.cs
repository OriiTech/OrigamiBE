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

        public async Task<int> CreateNewTeamMember(TeamMemberInfo request)
        {
            var repo = _unitOfWork.GetRepository<TeamMember>();

            // Kiểm tra TeamId có tồn tại không
            var teamRepo = _unitOfWork.GetRepository<Team>();
            var team = await teamRepo.GetFirstOrDefaultAsync(
                predicate: x => x.TeamId == request.TeamId,
                include: q => q.Include(t => t.Challenge).Include(t => t.TeamMembers),
                asNoTracking: true
            );
            if (team == null)
                throw new BadHttpRequestException("TeamNotFound");

            // Kiểm tra UserId có tồn tại không
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: x => x.UserId == request.UserId,
                asNoTracking: true
            );
            if (user == null)
                throw new BadHttpRequestException("UserNotFound");

            // Kiểm tra User đã có trong Team này chưa (Unique constraint)
            bool alreadyExists = await repo.AnyAsync(
                x => x.TeamId == request.TeamId && x.UserId == request.UserId
            );
            if (alreadyExists)
                throw new BadHttpRequestException("UserAlreadyInTeam");

            // Kiểm tra MaxTeamSize của Challenge (nếu có)
            if (team.Challenge != null && team.Challenge.MaxTeamSize.HasValue)
            {
                int currentMemberCount = team.TeamMembers?.Count ?? 0;
                if (currentMemberCount >= team.Challenge.MaxTeamSize.Value)
                    throw new BadHttpRequestException("TeamIsFull");
            }

            var newTeamMember = _mapper.Map<TeamMember>(request);
            newTeamMember.JoinedAt = DateTime.UtcNow;

            await repo.InsertAsync(newTeamMember);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newTeamMember.TeamMemberId;
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