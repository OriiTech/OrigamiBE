using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Category;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Implement
{
    public class CategoryService : BaseService<CategoryService>, ICategoryService
    {
        private readonly IConfiguration _configuration;

        public CategoryService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<CategoryService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }
        public async Task<int> CreateNewCategory(CategoryInfo request)
        {
            var repo = _unitOfWork.GetRepository<Category>();

            var existed = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CategoryName.ToLower() == request.CategoryName.ToLower(),
                asNoTracking: true
            );
            if (existed != null)
                throw new BadHttpRequestException("CategoryExisted");

            var newCategory = _mapper.Map<Category>(request);

            await repo.InsertAsync(newCategory);
            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newCategory.CategoryId;
        }
        public async Task<GetCategoryResponse> GetCategoryById(int id)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetFirstOrDefaultAsync(
                predicate: x => x.CategoryId == id,
                include: q => q.Include(c => c.Guides),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("CategoryNotFound");

            return _mapper.Map<GetCategoryResponse>(category);
        }

        public async Task<IPaginate<GetCategoryResponse>> ViewAllCategories(PagingModel pagingModel)
        {
            IPaginate<GetCategoryResponse> response = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
                selector: x => _mapper.Map<GetCategoryResponse>(x),
                orderBy: x => x.OrderBy(c => c.CategoryName),
                include: q => q.Include(c => c.Guides),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateCategory(int id, CategoryInfo request)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var category = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CategoryId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CategoryNotFound");

            if (!string.IsNullOrEmpty(request.CategoryName))
                category.CategoryName = request.CategoryName;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var category = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CategoryId == id,
                include: q => q.Include(c => c.Guides),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CategoryNotFound");

            if (category.Guides.Any())
                throw new BadHttpRequestException("CategoryHasGuidesCannotDelete");

            repo.Delete(category);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}
