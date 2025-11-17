using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Category;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<int> CreateNewCategory(CategoryInfo request);
        Task<bool> DeleteCategory(int id);
        Task<GetCategoryResponse> GetCategoryById(int id);
        Task<bool> UpdateCategory(int id, CategoryInfo request);
        Task<IPaginate<GetCategoryResponse>> ViewAllCategories(PagingModel pagingModel);
    }
}
