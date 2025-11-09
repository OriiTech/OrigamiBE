using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Ticket;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface ITicketService
    {
        Task<int> CreateTicket(TicketInfo request);
        Task<bool> DeleteTicket(int id);
        Task<GetTicketResponse> GetTicketById(int id);
        Task<bool> UpdateTicket(int id, TicketInfo request);
        Task<IPaginate<GetTicketResponse>> ViewAllTicket(TicketFilter filter, PagingModel pagingModel);
    }
}
