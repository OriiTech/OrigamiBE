using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Favorite;
using Origami.DataTier.Paginate;

namespace Origami.API.Controllers
{
    [ApiController]
    public class FavoriteController : BaseController<FavoriteController>
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(ILogger<FavoriteController> logger, IFavoriteService favoriteService) : base(logger)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet(ApiEndPointConstant.Favorite.FavoriteEndPoint)]
        [ProducesResponseType(typeof(GetFavoriteResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavorite(int id)
        {
            var response = await _favoriteService.GetFavoriteById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Favorite.FavoritesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetFavoriteResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllFavorite([FromQuery] FavoriteFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _favoriteService.ViewAllFavorite(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Favorite.FavoritesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateFavorite([FromBody] FavoriteInfo request)
        {
            var id = await _favoriteService.CreateFavorite(request);
            return CreatedAtAction(nameof(GetFavorite), new { id }, new { id });
        }

        [HttpDelete(ApiEndPointConstant.Favorite.FavoriteEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var isSuccessful = await _favoriteService.DeleteFavorite(id);
            if (!isSuccessful) return Ok("DeleteFailed");
            return Ok("DeleteSuccess");
        }
    }
}
