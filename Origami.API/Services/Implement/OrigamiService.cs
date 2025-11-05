using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Origami;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class OrigamiService : BaseService<OrigamiService>, IOrigamiService
    {
        private readonly IConfiguration _configuration;
        public OrigamiService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<OrigamiService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewOrigami(OrigamiInfo request)
        {
            var repo = _unitOfWork.GetRepository<DataTier.Models.Origami>();

            var existingOrigami = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.Name.ToLower() == request.Name.ToLower(),
                asNoTracking: true
            );

            if (existingOrigami != null)
                throw new BadHttpRequestException("OrigamiExisted");

            var newOrigami = _mapper.Map<DataTier.Models.Origami>(request);
            newOrigami.CreatedAt = DateTime.UtcNow;

            await repo.InsertAsync(newOrigami);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newOrigami.OrigamiId;
        }

        public async Task<GetOrigamiResponse> GetOrigamiById(int id)
        {
            DataTier.Models.Origami Origami = await _unitOfWork.GetRepository<DataTier.Models.Origami>().GetFirstOrDefaultAsync(
                predicate: x => x.OrigamiId == id,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("OrigamiNotFound");
            return _mapper.Map<GetOrigamiResponse>(Origami);
        }

        public async Task<bool> UpdateOrigamiInfo(int id, OrigamiInfo request)
        {
            var repo = _unitOfWork.GetRepository<DataTier.Models.Origami>();
            var origami = await repo.GetFirstOrDefaultAsync(
            predicate: x => x.OrigamiId == id,
            asNoTracking: false
            ) ?? throw new BadHttpRequestException("OrigamiNotFound");


            // Update fields
            if (!string.IsNullOrEmpty(request.Name) && request.Name != origami.Name)
            {
                bool nameExists = await repo.AnyAsync(x => x.Name == request.Name);
                if (nameExists)
                    throw new BadHttpRequestException("OrigamiNameAlreadyUsed");

                origami.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Description))
                origami.Description = request.Description;

            if (!string.IsNullOrEmpty(request.ImageUrl))
                origami.ImageUrl = request.ImageUrl;

            // Commit the changes
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<GetOrigamiResponse>> ViewAllOrigami(OrigamiFilter filter,PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<DataTier.Models.Origami>();

            Expression<Func<DataTier.Models.Origami, bool>> predicate = x =>
                (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name)) &&
                (string.IsNullOrEmpty(filter.Description) || x.Description.Contains(filter.Description)) &&
                (!filter.CreatedAt.HasValue || x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date) &&
                (!filter.CreatedBy.HasValue || x.CreatedBy == filter.CreatedBy.Value);

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetOrigamiResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderBy(o => o.Name),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

    }
}
