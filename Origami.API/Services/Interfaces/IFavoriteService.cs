using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Favorite;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<int> CreateFavorite(FavoriteInfo request);
        Task<bool> DeleteFavorite(int id);
        Task<GetFavoriteResponse> GetFavoriteById(int id);
        Task<IPaginate<GetFavoriteResponse>> ViewAllFavorite(FavoriteFilter filter, PagingModel pagingModel);
    }
}
