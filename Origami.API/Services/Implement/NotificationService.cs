using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Notification;
using Origami.BusinessTier.Payload.User;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class NotificationService : BaseService<NotificationService>, INotificationService
    {

        private readonly IConfiguration _configuration;
        public NotificationService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<NotificationService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<IPaginate<GetNotificationResponse>> ViewAllNotifications(NotificationFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetNotificationResponse> response = await _unitOfWork.GetRepository<Notification>().GetPagingListAsync(
                selector: x => _mapper.Map<GetNotificationResponse>(x),
                filter: filter,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<int> CreateNewNotification(NotificationInfo request)
        {
            var repo = _unitOfWork.GetRepository<Notification>();
            var newNotification = _mapper.Map<Notification>(request);
            await repo.InsertAsync(newNotification);
            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");
            return newNotification.NotificationId;
        }
    }
}
