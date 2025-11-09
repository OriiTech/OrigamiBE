using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Ticket;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Implement
{
    public class TicketService : BaseService<TicketService>, ITicketService
    {
        private readonly IConfiguration _configuration;
        public TicketService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<TicketService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }
        public async Task<int> CreateTicket(TicketInfo request)
        {
            var repo = _unitOfWork.GetRepository<Ticket>();

            var ticket = new Ticket
            {
                UserId = request.UserId,
                TicketTypeId = request.TicketTypeId,
                Title = request.Title,
                Content = request.Content,
                Status = request.Status ?? "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await repo.InsertAsync(ticket);
            await _unitOfWork.CommitAsync();

            return ticket.TicketId;
        }
        public async Task<GetTicketResponse> GetTicketById(int id)
        {
            var repo = _unitOfWork.GetRepository<Ticket>();
            var ticket = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TicketId == id,
                include: x => x.Include(t => t.TicketType).Include(t => t.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("TicketNotFound");

            return _mapper.Map<GetTicketResponse>(ticket);
        }
        public async Task<IPaginate<GetTicketResponse>> ViewAllTicket(TicketFilter filter, PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Ticket>();

            Expression<Func<Ticket, bool>> predicate = x =>
                (!filter.UserId.HasValue || x.UserId == filter.UserId.Value) &&
                (!filter.TicketTypeId.HasValue || x.TicketTypeId == filter.TicketTypeId.Value) &&
                (string.IsNullOrEmpty(filter.Status) || x.Status == filter.Status) &&
                (!filter.CreatedAt.HasValue || x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date);

            var response = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetTicketResponse>(x),
                predicate: predicate,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
        public async Task<bool> UpdateTicket(int id, TicketInfo request)
        {
            var repo = _unitOfWork.GetRepository<Ticket>();
            var ticket = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TicketId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("TicketNotFound");

            if (!string.IsNullOrEmpty(request.Title))
                ticket.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Content))
                ticket.Content = request.Content;

            if (!string.IsNullOrEmpty(request.Status))
                ticket.Status = request.Status;

            if (request.TicketTypeId != 0)
                ticket.TicketTypeId = request.TicketTypeId;

            bool success = await _unitOfWork.CommitAsync() > 0;
            return success;
        }
        public async Task<bool> DeleteTicket(int id)
        {
            var repo = _unitOfWork.GetRepository<Ticket>();
            var ticket = await repo.GetFirstOrDefaultAsync(predicate: x => x.TicketId == id);

            if (ticket == null)
                throw new BadHttpRequestException("TicketNotFound");

            repo.Delete(ticket);
            return await _unitOfWork.CommitAsync() > 0;
        }
    }
}
