using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Guide;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IGuideService
    {
        Task<int> CreateNewGuide(GuideInfo request);
        Task<GetGuideResponse> GetGuideById(int id);
        Task<bool> UpdateGuideInfo(int id, GuideInfo request);
        Task<IPaginate<GetGuideResponse>> ViewAllGuide(GuideFilter filter, PagingModel pagingModel);
    }
}
