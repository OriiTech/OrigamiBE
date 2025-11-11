// Origami.API/Services/Implement/ScoreService.cs
using AutoMapper;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Score;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class ScoreService : BaseService<ScoreService>, IScoreService
    {
        public ScoreService(IUnitOfWork<OrigamiDbContext> uow, ILogger<ScoreService> logger, IMapper mapper, IHttpContextAccessor hca)
            : base(uow, logger, mapper, hca) { }

        public async Task<int> CreateNewScore(ScoreInfo request)
        {
            await EnsureSubmissionExists(request.SubmissionId);
            await EnsureUserExists(request.ScoreBy);

            var repo = _unitOfWork.GetRepository<Score>();
            bool dup = await repo.AnyAsync(x => x.SubmissionId == request.SubmissionId && x.ScoreBy == request.ScoreBy);
            if (dup) throw new BadHttpRequestException("ScoreAlreadyExists");

            var entity = _mapper.Map<Score>(request);
            entity.ScoreAt = DateTime.UtcNow;

            await repo.InsertAsync(entity);
            if (await _unitOfWork.CommitAsync() <= 0) throw new BadHttpRequestException("CreateFailed");
            return entity.ScoreId;
        }

        public async Task<GetScoreResponse> GetScoreById(int id)
        {
            var s = await _unitOfWork.GetRepository<Score>().GetFirstOrDefaultAsync(
                predicate: x => x.ScoreId == id,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("ScoreNotFound");

            return _mapper.Map<GetScoreResponse>(s);
        }

        public Task<IPaginate<GetScoreResponse>> ViewAllScores(ScoreFilter filter, PagingModel paging)
        {
            return _unitOfWork.GetRepository<Score>().GetPagingListAsync(
                selector: x => _mapper.Map<GetScoreResponse>(x),
                predicate: null,
                orderBy: q => q.OrderByDescending(x => x.ScoreAt),
                include: null,
                page: paging.page,
                size: paging.size,
                filter: filter
            );
        }

        public async Task<bool> UpdateScore(int id, ScoreInfo request)
        {
            var repo = _unitOfWork.GetRepository<Score>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ScoreId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("ScoreNotFound");

            if (request.SubmissionId != entity.SubmissionId)
                await EnsureSubmissionExists(request.SubmissionId);
            if (request.ScoreBy != entity.ScoreBy)
                await EnsureUserExists(request.ScoreBy);

            bool dup = await repo.AnyAsync(x =>
                x.SubmissionId == request.SubmissionId &&
                x.ScoreBy == request.ScoreBy &&
                x.ScoreId != id
            );
            if (dup) throw new BadHttpRequestException("ScoreAlreadyExists");

            entity.SubmissionId = request.SubmissionId;
            entity.ScoreBy = request.ScoreBy;
            entity.Score1 = request.Score;

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteScore(int id)
        {
            var repo = _unitOfWork.GetRepository<Score>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ScoreId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("ScoreNotFound");

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