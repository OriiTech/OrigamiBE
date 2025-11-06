using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Origami;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface IOrigamiService
    {
        Task<int> CreateNewOrigami(OrigamiInfo request);
        Task<GetOrigamiResponse> GetOrigamiById(int id);
        Task<bool> UpdateOrigamiInfo(int id, OrigamiInfo request);
        Task<IPaginate<GetOrigamiResponse>> ViewAllOrigami(OrigamiFilter filter, PagingModel pagingModel);
    }
}
