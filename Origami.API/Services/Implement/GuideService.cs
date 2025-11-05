using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Guide;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class GuideService : BaseService<GuideService>, IGuideService
    {
        private readonly IConfiguration _configuration;
        public GuideService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<GuideService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewGuide(GuideInfo request)
        {
            var repo = _unitOfWork.GetRepository<Guide>();

            var existingGuide = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.Title.ToLower() == request.Title.ToLower(),
                asNoTracking: true
            );

            if (existingGuide != null)
                throw new BadHttpRequestException("GuideExisted");

            var newGuide = _mapper.Map<Guide>(request);
            newGuide.CreatedAt = DateTime.UtcNow;
            newGuide.UpdatedAt = DateTime.UtcNow;

            //Chua co AuthorId trong token nen chua gan duoc
            await repo.InsertAsync(newGuide);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newGuide.GuideId;
        }

        public async Task<GetGuideResponse> GetGuideById(int id)
        {
            Guide guide = await _unitOfWork.GetRepository<Guide>().GetFirstOrDefaultAsync(
                predicate: x => x.GuideId == id,
                include: q => q.Include(x => x.Author)
                       .Include(x => x.Origami), 
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("GuideNotFound");
            return _mapper.Map<GetGuideResponse>(guide);
        }

        public async Task<bool> UpdateGuideInfo(int id, GuideInfo request)
        {
            var repo = _unitOfWork.GetRepository<DataTier.Models.Guide>();
            var guide = await repo.GetFirstOrDefaultAsync(
            predicate: x => x.GuideId == id,
            asNoTracking: false
            ) ?? throw new BadHttpRequestException("GuideNotFound");


            // Update fields
            if (!string.IsNullOrEmpty(request.Title) && request.Title != guide.Title)
            {
                bool titleExists = await repo.AnyAsync(x => x.Title == request.Title && x.GuideId != id);
                if (titleExists)
                    throw new BadHttpRequestException("GuideTitleAlreadyUsed");

                guide.Title = request.Title;
            }
            if (!string.IsNullOrEmpty(request.Description))
                guide.Description = request.Description;

            if (request.Price.HasValue)
                guide.Price = request.Price.Value;

            if (request.OrigamiId.HasValue)
                guide.OrigamiId = request.OrigamiId;

            guide.UpdatedAt = DateTime.UtcNow;
            // Commit the changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<GetGuideResponse>> ViewAllGuide(GuideFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Guide>();

            Expression<Func<Guide, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.Title) || x.Title.Contains(filter.Title)) &&
                (string.IsNullOrEmpty(filter.Description) || x.Description.Contains(filter.Description)) &&
                (!filter.MinPrice.HasValue || x.Price >= filter.MinPrice.Value) &&
                (!filter.MaxPrice.HasValue || x.Price <= filter.MaxPrice.Value) &&
                (!filter.AuthorId.HasValue || x.AuthorId == filter.AuthorId.Value) &&
                (!filter.OrigamiId.HasValue || x.OrigamiId == filter.OrigamiId.Value) &&
                (!filter.CreatedAt.HasValue || x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date);

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetGuideResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderBy(o => o.Title),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }


    }
}
