using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Constants;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Notification;
using Origami.BusinessTier.Payload.User;
namespace Origami.API.Controllers
{
    [ApiController]
    public class NotificationController : BaseController<NotificationController>
    {
        private readonly INotificationService _notificationService;
        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService) : base(logger)
        {
            _notificationService = notificationService;
        }

        //Get all notifications with filter and paging

        [Authorize]
        [HttpGet(ApiEndPointConstant.Notification.NotificationsEndPoint)]
        [ProducesResponseType(typeof(GetNotificationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllNotifications([FromQuery] NotificationFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _notificationService.ViewAllNotifications(filter, pagingModel);
            return Ok(response);
        }

        //Create new notification

        [Authorize(Roles ="admin, staff")]
        [HttpPost(ApiEndPointConstant.Notification.NotificationsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateNotification(NotificationInfo request)
        {
            var response = await _notificationService.CreateNewNotification(request);
            return Ok(response);
        }

        //Delete notification by id

        [Authorize(Roles ="admin, staff")]
        [HttpDelete(ApiEndPointConstant.Notification.NotificationEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var isSuccessful = await _notificationService.DeleteNotificationById(id);
            if (!isSuccessful) return Ok("DeleteStatusFailed");
            return Ok("DeleteStatusSuccess");
        }
    }
}
