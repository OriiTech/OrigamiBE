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
            public const string UpdateUserRoleEndPoint = UserEndPoint + "/role";
            public const string MyProfileEndPoint = ApiEndpoint + "/users/me/profile";
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
            public const string GuideCardsEndPoint = GuidesEndPoint + "/cards";
            public const string MyGuideCardsEndPoint = GuideCardsEndPoint + "/my";
            public const string MyFavoriteGuideCardsEndPoint = GuideCardsEndPoint + "/favorites";
            public const string GuideDetailEndPoint = GuideEndPoint + "/detail";
            public const string GuideViewEndPoint = GuideEndPoint + "/view";
            public const string GuidePromoPhotoEndPoint = GuideEndPoint + "/promo-photos";
            public const string GuidePromoPhotoUpdateEndPoint = GuidePromoPhotoEndPoint + "/{photoId}";
            public const string GuidePurchaseEndPoint = GuideEndPoint + "/{id}/purchase";
        }

        //Auth
        public static class Auth
        {
            public const string GoogleLogin = ApiEndpoint + "/auth/google-login";
            public const string GoogleLoginInitiate = ApiEndpoint + "/auth/google-login-initiate";
            public const string GoogleCallback = ApiEndpoint + "/auth/google-callback";
            public const string Register = ApiEndpoint + "/auth/register";
            public const string Login = ApiEndpoint + "/auth/login";
            public const string Refresh = ApiEndpoint + "/auth/refresh";
            public const string Logout = ApiEndpoint + "/auth/logout";
            public const string HashPassword = ApiEndpoint + "/auth/hash";
            public const string ChangePassword = ApiEndpoint + "/auth/change-password";
            public const string ChangeEmail = ApiEndpoint + "/auth/change-email";
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
            public const string ChallengeListEndPoint = ChallengesEndPoint + "/list";
        }
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

        //Team
        public static class Team
        {
            public const string TeamsEndPoint = ApiEndpoint + "/teams";
            public const string TeamEndPoint = TeamsEndPoint + "/{id}";
        }

        //TeamMember
        public static class TeamMember
        {
            public const string TeamMembersEndPoint = ApiEndpoint + "/team-members";
            public const string TeamMemberEndPoint = TeamMembersEndPoint + "/{id}";
        }

        //Vote
        public static class Vote
        {
            public const string VotesEndPoint = ApiEndpoint + "/votes";
            public const string VoteEndPoint = VotesEndPoint + "/{id}";
        }

        //Submission
        public static class Submission
        {
            public const string SubmissionsEndPoint = ApiEndpoint + "/submissions";
            public const string SubmissionEndPoint = SubmissionsEndPoint + "/{id}";
            public const string SubmissionFeed = "challenges/{challengeId}/submissions/feed";
            public const string SaveSubmission = "submissions/save";
            public const string SubmitSubmission = "submissions/submit";
        }

        //Leaderboard
        public static class Leaderboard
        {
            public const string LeaderboardsEndPoint = ApiEndpoint + "/leaderboards";
            public const string LeaderboardEndPoint = LeaderboardsEndPoint + "/{id}";
        }

        //CourseReview
        public static class CourseReview
        {
            public const string CourseReviewsEndPoint = ApiEndpoint + "/course-reviews";
            public const string CourseReviewEndPoint = CourseReviewsEndPoint + "/{id}";
        }

        //Score
        public static class Score
        {
            public const string ScoresEndPoint = ApiEndpoint + "/scores";
            public const string ScoreEndPoint = ScoresEndPoint + "/{id}";
        }


        //category
        public static class Category
        {
            public const string CategoriesEndPoint = ApiEndpoint + "/categories";
            public const string CategoryEndPoint = CategoriesEndPoint + "/{id}";
        }

        //Badge
        public static class Badge
        {
            public const string BadgesEndPoint = ApiEndpoint + "/badges";
            public const string BadgeEndPoint = BadgesEndPoint + "/{id}";
        }
        //UserBadge
        public static class UserBadge
        {
            public const string UserBadgesEndPoint = ApiEndpoint + "/user-badges";
            public const string UserBadgeEndPoint = UserBadgesEndPoint + "/{id}";
            public const string MyBadgesEndPoint = ApiEndpoint + "/users/me/badges";
        }

        //Wallet
        public static class Wallet
        {
            public const string MyWalletEndPoint = ApiEndpoint + "/users/me/wallet";
            public const string MyTransactionsEndPoint = ApiEndpoint + "/users/me/transactions";
            public const string TopUpEndPoint = ApiEndpoint + "/users/me/wallet/top-up";
            public const string VnPayCallbackEndPoint = ApiEndpoint + "/wallet/vnpay-callback";
        }
    }
}