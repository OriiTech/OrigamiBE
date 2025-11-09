using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Step;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class StepService: BaseService<StepService>, IStepService
    {
        private readonly IConfiguration _configuration;
        public StepService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<StepService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }
        public async Task<int> CreateStep(StepInfo request)
        {
            var repo = _unitOfWork.GetRepository<Step>();

            var newStep = _mapper.Map<Step>(request);
            newStep.CreatedAt = DateTime.UtcNow;
            newStep.UpdatedAt = DateTime.UtcNow;
            
            await repo.InsertAsync(newStep);
            var result = await _unitOfWork.CommitAsync() > 0;

            if (!result)
                throw new BadHttpRequestException("CreateStepFailed");

            return newStep.StepId;
        }
        public async Task<GetStepResponse> GetStepById(int id)
        {
            var repo = _unitOfWork.GetRepository<Step>();
            var step = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.StepId == id,
                include: q => q.Include(g => g.Guide),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("StepNotFound");

            return _mapper.Map<GetStepResponse>(step);
        }
        public async Task<bool> UpdateStepInfo(int id, StepInfo request)
        {
            var repo = _unitOfWork.GetRepository<Step>();
            var step = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.StepId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("StepNotFound");
            //update fields
            if (!string.IsNullOrEmpty(request.Title))
                step.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                step.Description = request.Description;

            if (!string.IsNullOrEmpty(request.ImageUrl))
                step.ImageUrl = request.ImageUrl;

            if (!string.IsNullOrEmpty(request.VideoUrl))
                step.VideoUrl = request.VideoUrl;

            step.UpdatedAt = DateTime.UtcNow;
            //commit
            return await _unitOfWork.CommitAsync() > 0;
        }
        public async Task<bool> DeleteStep(int id)
        {
            var repo = _unitOfWork.GetRepository<Step>();
            var step = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.StepId == id
            ) ?? throw new BadHttpRequestException("StepNotFound");

            repo.Delete(step);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<IPaginate<GetStepResponse>> ViewAllSteps(StepFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Step>();

            Expression<Func<Step, bool>> predicate = x =>
                (!filter.GuideId.HasValue || x.GuideId == filter.GuideId.Value) &&
                (string.IsNullOrEmpty(filter.Title) || x.Title.Contains(filter.Title)) &&
                (!filter.CreatedAt.HasValue || x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date);

            return await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetStepResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderBy(x => x.StepNumber),
                page: pagingModel.page,
                size: pagingModel.size
            );
        }
    }
}
