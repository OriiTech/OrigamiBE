using Microsoft.EntityFrameworkCore;

namespace Origami.DataTier.Models;

public partial class OrigamiDbContext : DbContext
{
    public OrigamiDbContext(DbContextOptions<OrigamiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<Badge> Badges { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Challenge> Challenges { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<Commission> Commissions { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<CourseAccess> CourseAccesses { get; set; }
    public virtual DbSet<CourseReview> CourseReviews { get; set; }
    public virtual DbSet<CourseTargetLevel> CourseTargetLevels { get; set; }
    public virtual DbSet<Favorite> Favorites { get; set; }
    public virtual DbSet<Guide> Guides { get; set; }
    public virtual DbSet<GuideAccess> GuideAccesses { get; set; }
    public virtual DbSet<GuideCategory> GuideCategories { get; set; }
    public virtual DbSet<GuideCreator> GuideCreators { get; set; }
    public virtual DbSet<GuidePreview> GuidePreviews { get; set; }
    public virtual DbSet<GuidePromoPhoto> GuidePromoPhotos { get; set; }
    public virtual DbSet<GuideRating> GuideRatings { get; set; }
    public virtual DbSet<GuideRequirement> GuideRequirements { get; set; }
    public virtual DbSet<GuideView> GuideViews { get; set; }
    public virtual DbSet<Instructor> Instructors { get; set; }
    public virtual DbSet<Leaderboard> Leaderboards { get; set; }
    public virtual DbSet<Lecture> Lectures { get; set; }
    public virtual DbSet<LectureProgress> LectureProgresses { get; set; }
    public virtual DbSet<Lesson> Lessons { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<Origami> Origamis { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<Resource> Resources { get; set; }
    public virtual DbSet<Revenue> Revenues { get; set; }
    public virtual DbSet<ReviewResponse> ReviewResponses { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Score> Scores { get; set; }
    public virtual DbSet<Step> Steps { get; set; }
    public virtual DbSet<StepTip> StepTips { get; set; }
    public virtual DbSet<Submission> Submissions { get; set; }
    public virtual DbSet<TargetLevel> TargetLevels { get; set; }
    public virtual DbSet<Team> Teams { get; set; }
    public virtual DbSet<TeamMember> TeamMembers { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }
    public virtual DbSet<TicketType> TicketTypes { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserBadge> UserBadges { get; set; }
    public virtual DbSet<UserProfile> UserProfiles { get; set; }
    public virtual DbSet<Vote> Votes { get; set; }
    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId);
            entity.ToTable("Answer");
            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Question).WithMany(p => p.Answers).HasForeignKey(d => d.QuestionId);
            entity.HasOne(d => d.User).WithMany(p => p.Answers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.BadgeId);
            entity.ToTable("Badge");
            entity.Property(e => e.BadgeId).HasColumnName("badge_id");
            entity.Property(e => e.BadgeDescription).HasMaxLength(255).HasColumnName("badge_description");
            entity.Property(e => e.BadgeName).HasMaxLength(100).HasColumnName("badge_name");
            entity.Property(e => e.ConditionType).HasMaxLength(50).HasColumnName("condition_type");
            entity.Property(e => e.ConditionValue).HasMaxLength(50).HasColumnName("condition_value");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity.ToTable("Category");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName).HasMaxLength(100).HasColumnName("category_name");
            entity.Property(e => e.Type).HasMaxLength(20).HasDefaultValue("topic").HasColumnName("type");
        });

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId);
            entity.ToTable("Challenge");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.ChallengeType).HasMaxLength(100).HasColumnName("challenge_type");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnType("datetime").HasColumnName("end_date");
            entity.Property(e => e.IsTeamBased).HasDefaultValue(true).HasColumnName("is_team_based");
            entity.Property(e => e.MaxTeamSize).HasColumnName("max_team_size");
            entity.Property(e => e.StartDate).HasColumnType("datetime").HasColumnName("start_date");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Challenges).HasForeignKey(d => d.CreatedBy).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Challenge_User");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId);
            entity.ToTable("Comment");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.ParentId).HasColumnName("ParentId");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Guide).WithMany(p => p.Comments).HasForeignKey(d => d.GuideId).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasForeignKey(d => d.ParentId).HasConstraintName("FK_Comment_ParentComment");
            entity.HasOne(d => d.User).WithMany(p => p.Comments).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasKey(e => e.CommissionId);
            entity.ToTable("Commission");
            entity.Property(e => e.CommissionId).HasColumnName("commission_id");
            entity.Property(e => e.Percent).HasColumnType("decimal(5, 2)").HasColumnName("percent");
            entity.Property(e => e.RevenueId).HasColumnName("revenue_id");
            entity.HasOne(d => d.Revenue).WithMany(p => p.Commissions).HasForeignKey(d => d.RevenueId);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId);
            entity.ToTable("Course");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Bestseller).HasDefaultValue(false).HasColumnName("bestseller");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Language).HasMaxLength(50).HasColumnName("language");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
            entity.Property(e => e.PublishedAt).HasColumnType("datetime").HasColumnName("published_at");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(255).HasColumnName("thumbnail_url");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses).HasForeignKey(d => d.TeacherId);
        });

        modelBuilder.Entity<CourseAccess>(entity =>
        {
            entity.HasKey(e => e.AccessId);
            entity.ToTable("Course_access");
            entity.Property(e => e.AccessId).HasColumnName("access_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.LearnerId).HasColumnName("learner_id");
            entity.Property(e => e.PurchasedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("purchased_at");
            entity.HasOne(d => d.Course).WithMany(p => p.CourseAccesses).HasForeignKey(d => d.CourseId);
            entity.HasOne(d => d.Learner).WithMany(p => p.CourseAccesses).HasForeignKey(d => d.LearnerId);
        });

        modelBuilder.Entity<CourseReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.ToTable("Course_review");
            entity.HasCheckConstraint("CK_CourseReview_Rating", "[rating] >= 1 AND [rating] <= 5");
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews).HasForeignKey(d => d.CourseId);
            entity.HasOne(d => d.User).WithMany(p => p.CourseReviews).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<CourseTargetLevel>(entity =>
        {
            entity.HasKey(e => new { e.CourseId, e.LevelId }).HasName("PK_Course_Target");
            entity.ToTable("Course_target_level");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.HasOne(d => d.Course).WithMany(p => p.CourseTargetLevels).HasForeignKey(d => d.CourseId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Course_Target_Course");
            entity.HasOne(d => d.Level).WithMany(p => p.CourseTargetLevels).HasForeignKey(d => d.LevelId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Course_Target_Level");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId);
            entity.ToTable("Favorite");
            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Guide).WithMany(p => p.Favorites).HasForeignKey(d => d.GuideId).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.User).WithMany(p => p.Favorites).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Guide>(entity =>
        {
            entity.HasKey(e => e.GuideId);
            entity.ToTable("Guide");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Bestseller).HasColumnName("bestseller");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsNew).HasColumnName("is_new");
            entity.Property(e => e.OrigamiId).HasColumnName("OrigamiID");
            entity.Property(e => e.PaidOnly).HasColumnName("paid_only");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
            entity.Property(e => e.Subtitle).HasMaxLength(255).HasColumnName("subtitle");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.Property(e => e.Trending).HasColumnName("trending");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("updated_at");
            entity.HasOne(d => d.Author).WithMany(p => p.Guides).HasForeignKey(d => d.AuthorId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Guide_User");
            entity.HasOne(d => d.Origami).WithMany(p => p.Guides).HasForeignKey(d => d.OrigamiId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Guide_Origami");
        });

        modelBuilder.Entity<GuideAccess>(entity =>
        {
            entity.HasKey(e => e.AccessId);
            entity.ToTable("GuideAccess");
            entity.HasIndex(e => new { e.UserId, e.GuideId }, "UQ_User_Guide").IsUnique();
            entity.Property(e => e.AccessId).HasColumnName("access_id");
            entity.Property(e => e.GrantedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("granted_at");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Guide).WithMany(p => p.GuideAccesses).HasForeignKey(d => d.GuideId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_GuideAccess_Guide");
            entity.HasOne(d => d.User).WithMany(p => p.GuideAccesses).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_GuideAccess_User");
        });

        modelBuilder.Entity<GuideCategory>(entity =>
        {
            entity.HasKey(e => new { e.GuideId, e.CategoryId });
            entity.ToTable("GuideCategory");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.HasOne(d => d.Category).WithMany().HasForeignKey(d => d.CategoryId);
            entity.HasOne(d => d.Guide).WithMany().HasForeignKey(d => d.GuideId);
        });

        modelBuilder.Entity<GuideCreator>(entity =>
        {
            entity.HasKey(e => e.CreatorId);
            entity.ToTable("GuideCreator");
            entity.Property(e => e.CreatorId).ValueGeneratedNever().HasColumnName("creator_id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2").HasColumnName("created_at");
            entity.HasOne(d => d.Creator).WithOne(p => p.GuideCreator).HasForeignKey<GuideCreator>(d => d.CreatorId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_GuideCreator_User");
        });

        modelBuilder.Entity<GuidePreview>(entity =>
        {
            entity.HasKey(e => e.GuideId);
            entity.ToTable("GuidePreview");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.VideoAvailable).HasColumnName("video_available");
            entity.Property(e => e.VideoUrl).HasMaxLength(500).HasColumnName("video_url");
            entity.HasOne(d => d.Guide).WithOne(p => p.GuidePreview).HasForeignKey<GuidePreview>(d => d.GuideId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_GuidePreview_Guide");
        });

        modelBuilder.Entity<GuidePromoPhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId);
            entity.ToTable("GuidePromoPhoto");
            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.Url).HasMaxLength(500).HasColumnName("url");
            entity.HasOne(d => d.Guide).WithMany().HasForeignKey(d => d.GuideId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_GuidePromoPhoto_Guide");
        });

        modelBuilder.Entity<GuideRating>(entity =>
        {
            entity.HasKey(e => new { e.GuideId, e.UserId });
            entity.ToTable("GuideRating");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.HasOne(d => d.Guide).WithMany().HasForeignKey(d => d.GuideId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_GuideRating_Guide");
            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_GuideRating_User");
        });

        modelBuilder.Entity<GuideRequirement>(entity =>
        {
            entity.HasKey(e => e.GuideId);
            entity.ToTable("GuideRequirement");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.PaperType).HasMaxLength(100).HasColumnName("paper_type");
            entity.Property(e => e.PaperSize).HasMaxLength(50).HasColumnName("paper_size");
            entity.Property(e => e.Colors).HasMaxLength(200).HasColumnName("colors");
            entity.Property(e => e.Tools).HasMaxLength(500).HasColumnName("tools");
            entity.HasOne(d => d.Guide).WithOne().HasForeignKey<GuideRequirement>(d => d.GuideId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_GuideRequirement_Guide");
        });

        modelBuilder.Entity<GuideView>(entity =>
        {
            entity.HasKey(e => e.ViewId);
            entity.ToTable("GuideView");
            entity.Property(e => e.ViewId).HasColumnName("view_id");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ViewedAt).HasColumnType("datetime").HasColumnName("viewed_at");
            entity.HasOne(d => d.Guide).WithMany().HasForeignKey(d => d.GuideId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_GuideView_Guide");
            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_GuideView_User");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId);
            entity.ToTable("Instructor");
            entity.Property(e => e.InstructorId).ValueGeneratedNever().HasColumnName("instructor_id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.RatingAvg).HasColumnType("decimal(3, 2)").HasColumnName("rating_avg");
            entity.Property(e => e.TotalCourses).HasDefaultValue(0).HasColumnName("total_courses");
            entity.Property(e => e.TotalReviews).HasDefaultValue(0).HasColumnName("total_reviews");
            entity.HasOne(d => d.InstructorNavigation).WithOne(p => p.Instructor).HasForeignKey<Instructor>(d => d.InstructorId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Instructor_User");
        });

        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(e => e.LeaderboardId);
            entity.ToTable("Leaderboard");
            entity.HasCheckConstraint("CK_Leaderboard_TeamOrUser", "[TeamID] IS NOT NULL AND [UserID] IS NULL OR [TeamID] IS NULL AND [UserID] IS NOT NULL");
            entity.Property(e => e.LeaderboardId).HasColumnName("LeaderboardID");
            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.Rank).HasColumnName("Rank");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TotalScore).HasDefaultValue(0m).HasColumnType("decimal(10, 2)").HasColumnName("TotalScore");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("UpdatedAt");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.HasOne(d => d.Challenge).WithMany(p => p.Leaderboards).HasForeignKey(d => d.ChallengeId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Leaderboard_Challenge");
            entity.HasOne(d => d.Team).WithMany(p => p.Leaderboards).HasForeignKey(d => d.TeamId).HasConstraintName("FK_Leaderboard_Team");
            entity.HasOne(d => d.User).WithMany(p => p.Leaderboards).HasForeignKey(d => d.UserId).HasConstraintName("FK_Leaderboard_User");
        });

        modelBuilder.Entity<Lecture>(entity =>
        {
            entity.HasKey(e => e.LectureId);
            entity.ToTable("Lecture");
            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Url).HasMaxLength(500).HasColumnName("url");
            entity.Property(e => e.Type).HasMaxLength(50).HasColumnName("type");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.PreviewAvailable).HasDefaultValue(false).HasColumnName("preview_available");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.HasOne(d => d.Lesson).WithMany(p => p.Lectures).HasForeignKey(d => d.LessonId);
        });

        modelBuilder.Entity<LectureProgress>(entity =>
        {
            entity.HasKey(e => new { e.LectureId, e.UserId });
            entity.ToTable("LectureProgress");
            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime").HasColumnName("completed_at");
            entity.HasOne(d => d.Lecture).WithMany().HasForeignKey(d => d.LectureId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_LectureProgress_Lecture");
            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_LectureProgress_User");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId);
            entity.ToTable("Lesson");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.HasOne(d => d.Course).WithMany(p => p.Lessons).HasForeignKey(d => d.CourseId);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId);
            entity.ToTable("Question");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Course).WithMany(p => p.Questions).HasForeignKey(d => d.CourseId);
            entity.HasOne(d => d.User).WithMany(p => p.Questions).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);
            entity.ToTable("Refresh_token");
            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime").HasColumnName("expires_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.RefreshToken1).HasMaxLength(512).HasColumnName("refresh_token");
            entity.Property(e => e.RevokedAt).HasColumnType("datetime").HasColumnName("revoked_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId);
            entity.ToTable("Resource");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.Property(e => e.Url).HasMaxLength(500).HasColumnName("url");
            entity.Property(e => e.Type).HasMaxLength(50).HasColumnName("type");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.HasOne(d => d.Lecture).WithMany(p => p.Resources).HasForeignKey(d => d.LectureId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.ToTable("Role");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasMaxLength(50).HasColumnName("role_name");
        });

        modelBuilder.Entity<TargetLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId);
            entity.ToTable("Target_level");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId);
            entity.ToTable("Ticket");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.Content).HasColumnName("Content");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("Title");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.TicketType).WithMany(p => p.Tickets).HasForeignKey(d => d.TicketTypeId);
            entity.HasOne(d => d.User).WithMany(p => p.Tickets).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeId);
            entity.ToTable("Ticket_type");
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.TicketTypeName).HasMaxLength(50).HasColumnName("ticket_type_name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.ToTable("Transaction");
            entity.HasIndex(e => e.OrderId, "IX_Transaction_OrderId");
            entity.HasCheckConstraint("CK_Transaction_Type", "[transaction_type] IN ('transfer', 'refund', 'purchase', 'deposit')");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)").HasColumnName("amount");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ReceiverWalletId).HasColumnName("receiver_wallet_id");
            entity.Property(e => e.SenderWalletId).HasColumnName("sender_wallet_id");
            entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
            entity.Property(e => e.TransactionType).HasMaxLength(30).HasColumnName("transaction_type");
            entity.HasOne(d => d.Order).WithMany(p => p.Transactions).HasForeignKey(d => d.OrderId);
            entity.HasOne(d => d.ReceiverWallet).WithMany(p => p.TransactionReceiverWallets).HasForeignKey(d => d.ReceiverWalletId).HasConstraintName("FK_Transaction_ReceiverWallet");
            entity.HasOne(d => d.SenderWallet).WithMany(p => p.TransactionSenderWallets).HasForeignKey(d => d.SenderWalletId).HasConstraintName("FK_Transaction_SenderWallet");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.ToTable("User");
            entity.HasIndex(e => e.Email, "UQ_User_Email").IsUnique();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl).HasMaxLength(255).HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.Password).HasMaxLength(255).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("updated_at");
            entity.Property(e => e.Username).HasMaxLength(100).HasColumnName("username");
            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BadgeId });
            entity.ToTable("User_badge");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.BadgeId).HasColumnName("badge_id");
            entity.Property(e => e.EarnedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("earned_at");
            entity.HasOne(d => d.Badge).WithMany(p => p.UserBadges).HasForeignKey(d => d.BadgeId).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.User).WithMany(p => p.UserBadges).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Added UserProfile based on requirements
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.ToTable("UserProfile");
            entity.Property(e => e.UserId).ValueGeneratedNever().HasColumnName("user_id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.HasOne(d => d.User).WithOne(p => p.UserProfile).HasForeignKey<UserProfile>(d => d.UserId);
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(e => e.VoteId);
            entity.ToTable("Vote");
            entity.HasIndex(e => new { e.SubmissionId, e.UserId }, "UQ_Vote").IsUnique();
            entity.Property(e => e.VoteId).HasColumnName("vote_id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VotedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("voted_at");
            entity.HasOne(d => d.Submission).WithMany(p => p.Votes).HasForeignKey(d => d.SubmissionId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Vote_Submission");
            entity.HasOne(d => d.User).WithMany(p => p.Votes).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Vote_User");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId);
            entity.ToTable("Wallet");
            entity.Property(e => e.WalletId).HasColumnName("wallet_id");
            entity.Property(e => e.Balance).HasDefaultValue(0m).HasColumnType("decimal(18, 2)").HasColumnName("balance");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.User).WithOne(p => p.Wallet).HasForeignKey<Wallet>(d => d.UserId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.ToTable("Orders");
            entity.HasIndex(e => e.BuyerUserId, "IX_Orders_Buyer");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.BuyerUserId).HasColumnName("buyer_user_id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("VND").HasColumnName("currency");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("pending").HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("updated_at");
            entity.HasOne(d => d.BuyerUser).WithMany(p => p.Orders).HasForeignKey(d => d.BuyerUserId).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId);
            entity.ToTable("OrderItem");
            entity.HasCheckConstraint("CK_OrderItem_ProductType", "[product_type] IN ('course', 'guide')");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductType).HasMaxLength(20).HasColumnName("product_type");
            entity.Property(e => e.Quantity).HasDefaultValue(1).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)").HasColumnName("unit_price");
            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasForeignKey(d => d.OrderId);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.ScoreId);
            entity.ToTable("Score");
            entity.Property(e => e.ScoreId).HasColumnName("score_id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.Score1).HasColumnType("decimal(10, 2)").HasColumnName("score");
            entity.Property(e => e.ScoreAt).HasColumnType("datetime").HasColumnName("score_at");
            entity.Property(e => e.ScoreBy).HasColumnName("score_by");
            entity.HasOne(d => d.Submission).WithMany(p => p.Scores).HasForeignKey(d => d.SubmissionId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Score_Submission");
            entity.HasOne(d => d.ScoreByNavigation).WithMany().HasForeignKey(d => d.ScoreBy).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Score_User");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId);
            entity.ToTable("Submission");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.SubmittedBy).HasColumnName("submitted_by");
            entity.Property(e => e.FileUrl).HasMaxLength(500).HasColumnName("file_url");
            entity.Property(e => e.SubmittedAt).HasColumnType("datetime").HasColumnName("submitted_at");
            entity.HasOne(d => d.Challenge).WithMany().HasForeignKey(d => d.ChallengeId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Submission_Challenge");
            entity.HasOne(d => d.Team).WithMany().HasForeignKey(d => d.TeamId).HasConstraintName("FK_Submission_Team");
            entity.HasOne(d => d.SubmittedByNavigation).WithMany().HasForeignKey(d => d.SubmittedBy).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Submission_User");
        });

        modelBuilder.Entity<Step>(entity =>
        {
            entity.HasKey(e => e.StepId);
            entity.ToTable("Step");
            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.StepNumber).HasColumnName("step_number");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasMaxLength(500).HasColumnName("image_url");
            entity.Property(e => e.VideoUrl).HasMaxLength(500).HasColumnName("video_url");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasColumnName("updated_at");
            entity.HasOne(d => d.Guide).WithMany().HasForeignKey(d => d.GuideId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Step_Guide");
        });

        modelBuilder.Entity<StepTip>(entity =>
        {
            entity.HasKey(e => e.TipId);
            entity.ToTable("StepTip");
            entity.Property(e => e.TipId).HasColumnName("tip_id");
            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.HasOne(d => d.Step).WithMany(p => p.StepTips).HasForeignKey(d => d.StepId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_StepTip_Step");
        });

        modelBuilder.Entity<Origami>(entity =>
        {
            entity.HasKey(e => e.OrigamiId);
            entity.ToTable("Origami");
            entity.Property(e => e.OrigamiId).HasColumnName("origami_id");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasMaxLength(500).HasColumnName("image_url");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.HasOne(d => d.CreatedByNavigation).WithMany().HasForeignKey(d => d.CreatedBy).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Origami_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.ToTable("Notification");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.IsRead).HasDefaultValue(false).HasColumnName("is_read");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.HasKey(e => e.RevenueId);
            entity.ToTable("Revenue");
            entity.Property(e => e.RevenueId).HasColumnName("revenue_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)").HasColumnName("amount");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("created_at");
            entity.HasOne(d => d.Course).WithMany().HasForeignKey(d => d.CourseId).HasConstraintName("FK_Revenue_Course");
            entity.HasOne(d => d.Guide).WithMany().HasForeignKey(d => d.GuideId).HasConstraintName("FK_Revenue_Guide");
            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).HasConstraintName("FK_Revenue_User");
        });

        modelBuilder.Entity<ReviewResponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId);
            entity.ToTable("ReviewResponse");
            entity.Property(e => e.ResponseId).HasColumnName("response_id");
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.HasOne(d => d.Review).WithMany().HasForeignKey(d => d.ReviewId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_ReviewResponse_CourseReview");
            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_ReviewResponse_User");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId);
            entity.ToTable("Team");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.TeamName).HasMaxLength(255).HasColumnName("team_name");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.HasOne(d => d.Challenge).WithMany().HasForeignKey(d => d.ChallengeId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Team_Challenge");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.TeamMemberId);
            entity.ToTable("TeamMember");
            entity.Property(e => e.TeamMemberId).HasColumnName("team_member_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.JoinedAt).HasColumnType("datetime").HasColumnName("joined_at");
            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers).HasForeignKey(d => d.TeamId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_TeamMember_Team");
            entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_TeamMember_User");
        });

        // Add other entity configurations as needed...

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}