using AutoMapper;
using Microsoft.Extensions.Configuration;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.TicketType;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Implement
{
    public class TicketTypeService : BaseService<TicketTypeService>, ITicketTypeService
    {
        public TicketTypeService(
           IUnitOfWork<OrigamiDbContext> unitOfWork,
           ILogger<TicketTypeService> logger,
           IMapper mapper,
           IHttpContextAccessor httpContextAccessor)
           : base(unitOfWork, logger, mapper, httpContextAccessor)
        { }
        public async Task<int> CreateTicketType(TicketTypeInfo request)
        {
            var repo = _unitOfWork.GetRepository<TicketType>();

            bool exists = await repo.AnyAsync(x => x.TicketTypeName == request.TicketTypeName);
            if (exists)
                throw new BadHttpRequestException("TicketTypeAlreadyExists");

            var entity = new TicketType
            {
                TicketTypeName = request.TicketTypeName
            };

            await repo.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity.TicketTypeId;
        }
        public async Task<bool> UpdateTicketType(int id, TicketTypeInfo request)
        {
            var repo = _unitOfWork.GetRepository<TicketType>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TicketTypeId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("TicketTypeNotFound");

            if (!string.IsNullOrEmpty(request.TicketTypeName) && request.TicketTypeName != entity.TicketTypeName)
            {
                bool nameExists = await repo.AnyAsync(x => x.TicketTypeName == request.TicketTypeName && x.TicketTypeId != id);
                if (nameExists)
                    throw new BadHttpRequestException("TicketTypeNameAlreadyUsed");

                entity.TicketTypeName = request.TicketTypeName;
            }

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
        public async Task<bool> DeleteTicketType(int id)
        {
            var repo = _unitOfWork.GetRepository<TicketType>();
            var entity = await repo.GetFirstOrDefaultAsync(predicate: x => x.TicketTypeId == id);

            if (entity == null)
                throw new BadHttpRequestException("TicketTypeNotFound");

            repo.Delete(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }
        public async Task<GetTicketTypeResponse> GetTicketTypeById(int id)
        {
            var repo = _unitOfWork.GetRepository<TicketType>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.TicketTypeId == id,
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("TicketTypeNotFound");

            return _mapper.Map<GetTicketTypeResponse>(entity);
        }
        public async Task<IPaginate<GetTicketTypeResponse>> ViewAllTicketType(PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<TicketType>();

            var result = await repo.GetPagingListAsync(
                selector: x => _mapper.Map<GetTicketTypeResponse>(x),
                orderBy: q => q.OrderBy(o => o.TicketTypeId),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return result;
        }
    }
}
