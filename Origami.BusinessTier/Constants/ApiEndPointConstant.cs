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

        //Course
        public static class Course
        {
            public const string CoursesEndPoint = ApiEndpoint + "/courses";
            public const string CourseEndPoint = CoursesEndPoint + "/{id}";
        }

        //Lesson
        public static class Lesson
        {
            public const string LessonsEndPoint = ApiEndpoint + "/lessons";
            public const string LessonEndPoint = LessonsEndPoint + "/{id}";
        }

        //Question
        public static class Question
        {
            public const string QuestionsEndPoint = ApiEndpoint + "/question";
            public const string QuestionEndPoint = QuestionsEndPoint + "/{id}";
        }

        //Answer
        public static class Answer
        {
            public const string AnswersEndPoint = ApiEndpoint + "/answer";
            public const string AnswerEndPoint = AnswersEndPoint + "/{id}";
        }

        //CourseAccess
        public static class CourseAccess
        {
            public const string CourseAccessesEndPoint = ApiEndpoint + "/course-accesses";
            public const string CourseAccessEndPoint = CourseAccessesEndPoint + "/{id}";
        }
    }
    
}
