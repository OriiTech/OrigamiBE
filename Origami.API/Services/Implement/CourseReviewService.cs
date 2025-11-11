// Origami.API/Services/Implement/CourseReviewService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.CourseReview;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class CourseReviewService : BaseService<CourseReviewService>, ICourseReviewService
    {
        public CourseReviewService(IUnitOfWork<OrigamiDbContext> uow, ILogger<CourseReviewService> logger, IMapper mapper, IHttpContextAccessor hca)
            : base(uow, logger, mapper, hca) { }

        public async Task<int> CreateNewCourseReview(CourseReviewInfo request)
        {
            if (request.CourseId.HasValue) await EnsureCourseExists(request.CourseId.Value);
            if (request.UserId.HasValue) await EnsureUserExists(request.UserId.Value);
            if (request.Rating.HasValue && (request.Rating < 1 || request.Rating > 5))
                throw new BadHttpRequestException("RatingOutOfRange");

            bool dup = await _unitOfWork.GetRepository<CourseReview>().AnyAsync(
                x => x.CourseId == request.CourseId && x.UserId == request.UserId
            );
            if (dup) throw new BadHttpRequestException("AlreadyReviewed");

            var entity = _mapper.Map<CourseReview>(request);
            entity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<CourseReview>().InsertAsync(entity);
            if (await _unitOfWork.CommitAsync() <= 0) throw new BadHttpRequestException("CreateFailed");
            return entity.ReviewId;
        }

        public async Task<GetCourseReviewResponse> GetCourseReviewById(int id)
        {
            var r = await _unitOfWork.GetRepository<CourseReview>().GetFirstOrDefaultAsync(
                predicate: x => x.ReviewId == id,
                include: q => q.Include(x => x.Course).Include(x => x.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("CourseReviewNotFound");

            return _mapper.Map<GetCourseReviewResponse>(r);
        }

        public Task<IPaginate<GetCourseReviewResponse>> ViewAllCourseReviews(CourseReviewFilter filter, PagingModel paging)
        {
            return _unitOfWork.GetRepository<CourseReview>().GetPagingListAsync(
                selector: x => _mapper.Map<GetCourseReviewResponse>(x),
                predicate: null,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                include: q => q.Include(x => x.Course).Include(x => x.User),
                page: paging.page,
                size: paging.size,
                filter: filter
            );
        }

        public async Task<bool> UpdateCourseReview(int id, CourseReviewInfo request)
        {
            var repo = _unitOfWork.GetRepository<CourseReview>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ReviewId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CourseReviewNotFound");

            if (request.CourseId != entity.CourseId && request.CourseId.HasValue)
                await EnsureCourseExists(request.CourseId.Value);
            if (request.UserId != entity.UserId && request.UserId.HasValue)
                await EnsureUserExists(request.UserId.Value);

            bool dup = await repo.AnyAsync(x =>
                x.CourseId == (request.CourseId ?? entity.CourseId) &&
                x.UserId == (request.UserId ?? entity.UserId) &&
                x.ReviewId != id
            );
            if (dup) throw new BadHttpRequestException("AlreadyReviewed");

            entity.CourseId = request.CourseId ?? entity.CourseId;
            entity.UserId = request.UserId ?? entity.UserId;

            if (request.Rating.HasValue)
            {
                if (request.Rating < 1 || request.Rating > 5) throw new BadHttpRequestException("RatingOutOfRange");
                entity.Rating = request.Rating;
            }
            if (!string.IsNullOrEmpty(request.Comment))
                entity.Comment = request.Comment;

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteCourseReview(int id)
        {
            var repo = _unitOfWork.GetRepository<CourseReview>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ReviewId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CourseReviewNotFound");

            repo.Delete(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        private async Task EnsureCourseExists(int courseId)
        {
            var ok = await _unitOfWork.GetRepository<Course>().AnyAsync(x => x.CourseId == courseId);
            if (!ok) throw new BadHttpRequestException("CourseNotFound");
        }
        private async Task EnsureUserExists(int userId)
        {
            var ok = await _unitOfWork.GetRepository<User>().AnyAsync(x => x.UserId == userId);
            if (!ok) throw new BadHttpRequestException("UserNotFound");
        }
    }
}