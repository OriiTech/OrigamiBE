using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Comment;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class CommentService:BaseService<CommentService>, ICommentService
    {
        private readonly IConfiguration _configuration;
        public CommentService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<CommentService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }
        public async Task<int> CreateComment(CommentInfo request)
        {
            var repo = _unitOfWork.GetRepository<Comment>();

            var comment = new Comment
            {
                GuideId = request.GuideId,
                UserId = request.UserId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                ParentId = request.ParentId
            };

            await repo.InsertAsync(comment);
            await _unitOfWork.CommitAsync();

            return comment.CommentId;
        }
        public async Task<bool> DeleteComment(int id)
        {
            var repo = _unitOfWork.GetRepository<Comment>();
            var comment = await repo.GetFirstOrDefaultAsync(predicate: x => x.CommentId == id);

            if (comment == null)
                throw new BadHttpRequestException("CommentNotFound");

            repo.Delete(comment);
            return await _unitOfWork.CommitAsync() > 0;
        }
        public async Task<bool> UpdateComment(int id, CommentUpdateInfo request)
        {
            var repo = _unitOfWork.GetRepository<Comment>();
            var comment = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CommentId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CommentNotFound");

            // Update fields
            if (!string.IsNullOrEmpty(request.Content) && request.Content != comment.Content)
            {
                comment.Content = request.Content;
            }
                
            // Commit the changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }
        public async Task<GetCommentResponse> GetCommentById(int id)
        {
            var repo = _unitOfWork.GetRepository<Comment>();
            var comment = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CommentId == id,
                include: x => x
                    .Include(c => c.User)
                    .Include(c => c.Guide)
                    .Include(c => c.Replies)
                        .ThenInclude(r => r.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("CommentNotFound");

            return _mapper.Map<GetCommentResponse>(comment);
        }
        public async Task<IPaginate<GetCommentResponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Comment>();

            Expression<Func<Comment, bool>> predicate = x =>
                (!filter.GuideId.HasValue || x.GuideId == filter.GuideId.Value) &&
                (!filter.UserId.HasValue || x.UserId == filter.UserId.Value) &&
                (!filter.CreatedAt.HasValue || x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date) &&
                (!filter.ParentId.HasValue || x.ParentId == filter.ParentId.Value);

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetCommentResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                include: x => x.Include(c => c.User).Include(c => c.Replies).ThenInclude(r => r.User),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}
