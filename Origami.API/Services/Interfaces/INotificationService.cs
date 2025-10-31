using Microsoft.AspNetCore.Identity.Data;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Notification;
using Origami.BusinessTier.Payload.User;
using Origami.DataTier.Paginate;

namespace Origami.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IPaginate<GetNotificationResponse>> ViewAllNotifications(NotificationFilter filter, PagingModel pagingModel);
        Task<int> CreateNewNotification(NotificationInfo request);
    }
}
