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

        //Origami
        public static class Origami
        {
            public const string OrigamisEndPoint = ApiEndpoint + "/origamis";
            public const string OrigamiEndPoint = OrigamisEndPoint + "/{id}";
        }
        //Guide
        public static class Guide
        {
            public const string GuidesEndPoint = ApiEndpoint + "/guides";
            public const string GuideEndPoint = GuidesEndPoint + "/{id}";
        }

        //Auth
        public static class Auth
        {
            public const string Login = ApiEndpoint + "/auth/login";
            public const string Refresh = ApiEndpoint + "/auth/refresh";
            public const string Logout = ApiEndpoint + "/auth/logout";
            public const string HashPassword = ApiEndpoint + "/auth/hash";
        }
        //Step
        public static class Step
        {
            public const string StepsEndPoint = ApiEndpoint + "/steps";
            public const string StepEndPoint = StepsEndPoint + "/{id}";
        }
        //Challenge
        public static class Challenge
        {
            public const string ChallengesEndPoint = ApiEndpoint + "/challenges";
            public const string ChallengeEndPoint = ChallengesEndPoint + "/{id}";
        //Favorite
        public static class Favorite
        {
            public const string FavoritesEndPoint = ApiEndpoint + "/favorites";
            public const string FavoriteEndPoint = FavoritesEndPoint + "/{id}";
        }
        //Comment
        public static class Comment
        {
            public const string CommentsEndPoint = ApiEndpoint + "/comments";
            public const string CommentEndPoint = CommentsEndPoint + "/{id}";
        }
    }
}
