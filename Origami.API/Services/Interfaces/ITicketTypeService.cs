using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.TicketType;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ITicketTypeService
    {
        Task<int> CreateTicketType(TicketTypeInfo request);
        Task<bool> UpdateTicketType(int id, TicketTypeInfo request);
        Task<bool> DeleteTicketType(int id);
        Task<GetTicketTypeResponse> GetTicketTypeById(int id);
        Task<IPaginate<GetTicketTypeResponse>> ViewAllTicketType(PagingModel pagingModel);
    }
}
