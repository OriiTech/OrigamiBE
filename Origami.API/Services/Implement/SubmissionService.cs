// Origami.API/Services/Implement/SubmissionService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Submission;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class SubmissionService : BaseService<SubmissionService>, ISubmissionService
    {
        public SubmissionService(IUnitOfWork<OrigamiDbContext> uow, ILogger<SubmissionService> logger, IMapper mapper, IHttpContextAccessor hca)
            : base(uow, logger, mapper, hca) { }

        public async Task<int> CreateNewSubmission(SubmissionInfo request)
        {
            await EnsureChallengeExists(request.ChallengeId);
            if (request.TeamId.HasValue) await EnsureTeamExists(request.TeamId.Value);
            await EnsureUserExists(request.SubmittedBy);

            var entity = _mapper.Map<Submission>(request);
            entity.SubmittedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Submission>().InsertAsync(entity);
            if (await _unitOfWork.CommitAsync() <= 0) throw new BadHttpRequestException("CreateFailed");
            return entity.SubmissionId;
        }

        public async Task<GetSubmissionResponse> GetSubmissionById(int id)
        {
            var s = await _unitOfWork.GetRepository<Submission>().GetFirstOrDefaultAsync(
                predicate: x => x.SubmissionId == id,
                include: q => q.Include(x => x.Challenge)
                               .Include(x => x.Team)
                               .Include(x => x.SubmittedByNavigation),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("SubmissionNotFound");

            return _mapper.Map<GetSubmissionResponse>(s);
        }

        public Task<IPaginate<GetSubmissionResponse>> ViewAllSubmissions(SubmissionFilter filter, PagingModel paging)
        {
            return _unitOfWork.GetRepository<Submission>().GetPagingListAsync(
                selector: x => _mapper.Map<GetSubmissionResponse>(x),
                predicate: null,
                orderBy: q => q.OrderByDescending(x => x.SubmittedAt),
                include: q => q.Include(x => x.Challenge)
                               .Include(x => x.Team)
                               .Include(x => x.SubmittedByNavigation),
                page: paging.page,
                size: paging.size,
                filter: filter
            );
        }

        public async Task<bool> UpdateSubmission(int id, SubmissionInfo request)
        {
            var repo = _unitOfWork.GetRepository<Submission>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.SubmissionId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("SubmissionNotFound");

            if (request.ChallengeId != entity.ChallengeId) await EnsureChallengeExists(request.ChallengeId);
            if (request.TeamId != entity.TeamId && request.TeamId.HasValue) await EnsureTeamExists(request.TeamId.Value);
            if (request.SubmittedBy != entity.SubmittedBy) await EnsureUserExists(request.SubmittedBy);

            entity.ChallengeId = request.ChallengeId;
            entity.TeamId = request.TeamId;
            entity.SubmittedBy = request.SubmittedBy;
            entity.FileUrl = request.FileUrl;

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteSubmission(int id)
        {
            var repo = _unitOfWork.GetRepository<Submission>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.SubmissionId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("SubmissionNotFound");

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