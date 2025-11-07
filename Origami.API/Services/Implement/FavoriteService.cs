using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Favorite;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class FavoriteService: BaseService<FavoriteService>, IFavoriteService
    {
        private readonly IConfiguration _configuration;
        public FavoriteService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<FavoriteService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }
        public async Task<int> CreateFavorite(FavoriteInfo request)
        {
            var repo = _unitOfWork.GetRepository<Favorite>();

            bool alreadyExists = await repo.AnyAsync(x => x.UserId == request.UserId && x.GuideId == request.GuideId);
            if (alreadyExists)
                throw new BadHttpRequestException("FavoriteAlreadyExists");

            var favorite = new Favorite
            {
                UserId = request.UserId,
                GuideId = request.GuideId,
                CreatedAt = DateTime.UtcNow
            };

            await repo.InsertAsync(favorite);
            await _unitOfWork.CommitAsync();

            return favorite.FavoriteId;
        }
        public async Task<bool> DeleteFavorite(int id)
        {
            var repo = _unitOfWork.GetRepository<Favorite>();
            var favorite = await repo.GetFirstOrDefaultAsync(predicate: x => x.FavoriteId == id);

            if (favorite == null)
                throw new BadHttpRequestException("FavoriteNotFound");

            repo.Delete(favorite);
            return await _unitOfWork.CommitAsync() > 0;
        }
        public async Task<GetFavoriteResponse> GetFavoriteById(int id)
        {
            var repo = _unitOfWork.GetRepository<Favorite>();
            var favorite = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.FavoriteId == id,
                include: x => x.Include(f => f.Guide).Include(f => f.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("FavoriteNotFound");

            return _mapper.Map<GetFavoriteResponse>(favorite);
        }
        public async Task<IPaginate<GetFavoriteResponse>> ViewAllFavorite(FavoriteFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Favorite>();

            Expression<Func<Favorite, bool>> predicate = x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId.Value) &&
                (!filter.GuideId.HasValue || x.GuideId == filter.GuideId.Value) &&
                (!filter.CreatedAt.HasValue || x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date);

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetFavoriteResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}
