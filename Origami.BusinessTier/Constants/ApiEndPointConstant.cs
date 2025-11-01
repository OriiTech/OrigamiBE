using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Constants
{
    public static class ApiEndPointConstant
    {

        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        //User
        public static class User
        {
            public const string UsersEndPoint = ApiEndpoint + "/users";
            public const string UserEndPoint = UsersEndPoint + "/{id}";
        }

        //Role
        public static class Role
        {
            public const string RolesEndPoint = ApiEndpoint + "/roles";
            public const string RoleEndPoint = RolesEndPoint + "/{id}";
        }

        //Notification
        public static class Notification
        {
            public const string NotificationsEndPoint = ApiEndpoint + "/notifications";
            public const string NotificationEndPoint = NotificationsEndPoint + "/{id}";
        }
    }
    
}
