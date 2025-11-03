using AutoMapper;
using Origami.BusinessTier.Payload.Notification;
using Origami.DataTier.Models;

namespace Origami.API.Mappers
{
    public class NotificationModule : Profile
    {
        public NotificationModule()
        {
            CreateMap<Notification, GetNotificationResponse>();
            CreateMap<NotificationInfo, Notification>();
        }

    }
}
