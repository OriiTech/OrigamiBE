using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Category;

namespace Origami.API.Controllers
{
    [ApiController]
    public class CategoryController : BaseController<CategoryController>
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService) : base(logger)
        {
            _categoryService = categoryService;
        }

        //Get category by id

        [HttpGet(ApiEndPointConstant.Category.CategoryEndPoint)]
        [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategory(int id)
        {
            var response = await _categoryService.GetCategoryById(id);
            return Ok(response);
        }

        // View all categories with paging

        [HttpGet(ApiEndPointConstant.Category.CategoriesEndPoint)]
        [ProducesResponseType(typeof(GetCategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllCategories([FromQuery] PagingModel pagingModel)
        {
            var response = await _categoryService.ViewAllCategories( pagingModel);
            return Ok(response);
        }

        //Create new category

        [Authorize(Roles ="admin, staff, sensei")]
        [HttpPost(ApiEndPointConstant.Category.CategoriesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCategory(CategoryInfo request)
        {
            var response = await _categoryService.CreateNewCategory(request);
            return Ok(response);
        }

        //Update category info

        [Authorize(Roles = "admin, staff, sensei")]
        [HttpPatch(ApiEndPointConstant.Category.CategoryEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCategoryInfo(int id, CategoryInfo request)
        {
            var isSuccessful = await _categoryService.UpdateCategory(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        //Delete category

        [Authorize(Roles = "admin, staff, sensei")]
        [HttpDelete(ApiEndPointConstant.Category.CategoryEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isSuccessful = await _categoryService.DeleteCategory(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}
