using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Guide;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IGuideService
    {
        //Task<int> CreateNewGuide(GuideInfo request);
        Task<GetGuideResponse> GetGuideById(int id);
        Task<GetGuideDetailResponse> GetGuideDetailById(int id);
        Task<bool> UpdateGuideInfo(int id, GuideInfo request);
        Task<IPaginate<GetGuideResponse>> ViewAllGuide(GuideFilter filter, PagingModel pagingModel);
        Task<IPaginate<GetGuideCardResponse>> ViewAllGuideCard(GuideCardFilter filter, PagingModel pagingModel);
        Task<IPaginate<GetGuideCardResponse>> ViewMyGuideCards(PagingModel pagingModel);
        Task<IPaginate<GetGuideCardResponse>> ViewMyFavoriteGuideCards(PagingModel pagingModel);
        Task<IPaginate<GetGuideCardResponse>> ViewMyPurchasedGuideCards(PagingModel pagingModel);
        Task IncreaseView(int id);
        Task<int> CreateGuideAsync(GuideSaveRequest request);
        Task<int> AddPromoPhotoAsync(int guideId, AddPromoPhotoRequest request);
        Task<bool> UpdatePromoPhotoAsync(int guideId, int photoId, UpdatePromoPhotoRequest request);
        Task<PaymentGuideResponse> PurchaseGuideAsync(PaymentGuideRequest request);
    }
}
