using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Ticket;

namespace Origami.API.Controllers
{
    [ApiController]
    public class TicketController : BaseController<TicketController>
    {
        private readonly ITicketService _ticketService;
        public TicketController(ILogger<TicketController> logger, ITicketService ticketService)
            : base(logger)
        {
            _ticketService = ticketService;
        }

        [HttpPost(ApiEndPointConstant.Ticket.TicketsEndPoint)]
        public async Task<IActionResult> CreateTicket(TicketInfo request)
        {
            var id = await _ticketService.CreateTicket(request);
            return Ok(new { TicketId = id });
        }

        [HttpGet(ApiEndPointConstant.Ticket.TicketEndPoint)]
        public async Task<IActionResult> GetTicket(int id)
        {
            var response = await _ticketService.GetTicketById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Ticket.TicketsEndPoint)]
        public async Task<IActionResult> ViewAllTicket([FromQuery] TicketFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _ticketService.ViewAllTicket(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Ticket.TicketEndPoint)]
        public async Task<IActionResult> UpdateTicket(int id, TicketInfo request)
        {
            var success = await _ticketService.UpdateTicket(id, request);
            return Ok(success ? "UpdateTicketSuccess" : "UpdateTicketFailed");
        }

        [HttpDelete(ApiEndPointConstant.Ticket.TicketEndPoint)]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var success = await _ticketService.DeleteTicket(id);
            return Ok(success ? "DeleteTicketSuccess" : "DeleteTicketFailed");
        }
    }
}
