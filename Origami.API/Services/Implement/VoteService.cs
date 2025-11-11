using AutoMapper;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Vote;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class VoteService : BaseService<VoteService>, IVoteService
    {
        public VoteService(IUnitOfWork<OrigamiDbContext> uow, ILogger<VoteService> logger, IMapper mapper, IHttpContextAccessor hca)
            : base(uow, logger, mapper, hca) { }

        public async Task<int> CreateNewVote(VoteInfo request)
        {
            await EnsureSubmissionExists(request.SubmissionId);
            await EnsureUserExists(request.UserId);

            var repo = _unitOfWork.GetRepository<Vote>();
            bool dup = await repo.AnyAsync(x => x.SubmissionId == request.SubmissionId && x.UserId == request.UserId);
            if (dup) throw new BadHttpRequestException("AlreadyVoted");

            var entity = _mapper.Map<Vote>(request);
            entity.VotedAt = DateTime.UtcNow;

            await repo.InsertAsync(entity);
            if (await _unitOfWork.CommitAsync() <= 0) throw new BadHttpRequestException("CreateFailed");
            return entity.VoteId;
        }

        public async Task<GetVoteResponse> GetVoteById(int id)
        {
            var v = await _unitOfWork.GetRepository<Vote>().GetFirstOrDefaultAsync(
                predicate: x => x.VoteId == id,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("VoteNotFound");

            return _mapper.Map<GetVoteResponse>(v);
        }

        public Task<IPaginate<GetVoteResponse>> ViewAllVotes(VoteFilter filter, PagingModel paging)
        {
            return _unitOfWork.GetRepository<Vote>().GetPagingListAsync(
                selector: x => _mapper.Map<GetVoteResponse>(x),
                predicate: null,
                orderBy: q => q.OrderByDescending(x => x.VotedAt),
                include: null,
                page: paging.page,
                size: paging.size,
                filter: filter
            );
        }

        public async Task<bool> DeleteVote(int id)
        {
            var repo = _unitOfWork.GetRepository<Vote>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.VoteId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("VoteNotFound");

            repo.Delete(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        private async Task EnsureSubmissionExists(int submissionId)
        {
            var ok = await _unitOfWork.GetRepository<Submission>().AnyAsync(x => x.SubmissionId == submissionId);
            if (!ok) throw new BadHttpRequestException("SubmissionNotFound");
        }
        private async Task EnsureUserExists(int userId)
        {
            var ok = await _unitOfWork.GetRepository<User>().AnyAsync(x => x.UserId == userId);
            if (!ok) throw new BadHttpRequestException("UserNotFound");
        }
    }
}