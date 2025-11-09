using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.TicketType;

namespace Origami.API.Controllers
{
    [ApiController]
    public class TicketTypeController : BaseController<TicketTypeController>
    {
        private readonly ITicketTypeService _ticketTypeService;

        public TicketTypeController(ILogger<TicketTypeController> logger, ITicketTypeService ticketTypeService)
            : base(logger)
        {
            _ticketTypeService = ticketTypeService;
        }
        [HttpPost(ApiEndPointConstant.TicketType.TicketTypesEndPoint)]
        public async Task<IActionResult> CreateTicketType(TicketTypeInfo request)
        {
            var id = await _ticketTypeService.CreateTicketType(request);
            return Ok(new { TicketTypeId = id });
        }

        [HttpGet(ApiEndPointConstant.TicketType.TicketTypeEndPoint)]
        public async Task<IActionResult> GetTicketType(int id)
        {
            var response = await _ticketTypeService.GetTicketTypeById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.TicketType.TicketTypesEndPoint)]
        public async Task<IActionResult> ViewAllTicketType([FromQuery] PagingModel pagingModel)
        {
            var response = await _ticketTypeService.ViewAllTicketType(pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.TicketType.TicketTypeEndPoint)]
        public async Task<IActionResult> UpdateTicketType(int id, TicketTypeInfo request)
        {
            var success = await _ticketTypeService.UpdateTicketType(id, request);
            return Ok(success ? "UpdateTicketTypeSuccess" : "UpdateTicketTypeFailed");
        }

        [HttpDelete(ApiEndPointConstant.TicketType.TicketTypeEndPoint)]
        public async Task<IActionResult> DeleteTicketType(int id)
        {
            var success = await _ticketTypeService.DeleteTicketType(id);
            return Ok(success ? "DeleteTicketTypeSuccess" : "DeleteTicketTypeFailed");
        }
    }
}
