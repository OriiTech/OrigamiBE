using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Origami.DataTier.Models;

public partial class OrigamiDbContext : DbContext
{
    public OrigamiDbContext()
    {
    }

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

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Guide> Guides { get; set; }

    public virtual DbSet<GuideAccess> GuideAccesses { get; set; }

    public virtual DbSet<Leaderboard> Leaderboards { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Origami> Origamis { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Revenue> Revenues { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Step> Steps { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBadge> UserBadges { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=OrigamiDB;User Id=sa;Password=1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__Answer__33724318F7DDB79F");

            entity.ToTable("Answer");

            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__Answer__question__01142BA1");

            entity.HasOne(d => d.User).WithMany(p => p.Answers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Answer__user_id__02084FDA");
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.BadgeId).HasName("PK__Badge__E7989656B39A165B");

            entity.ToTable("Badge");

            entity.Property(e => e.BadgeId).HasColumnName("badge_id");
            entity.Property(e => e.BadgeDescription)
                .HasMaxLength(255)
                .HasColumnName("badge_description");
            entity.Property(e => e.BadgeName)
                .HasMaxLength(100)
                .HasColumnName("badge_name");
            entity.Property(e => e.ConditionType)
                .HasMaxLength(50)
                .HasColumnName("condition_type");
            entity.Property(e => e.ConditionValue)
                .HasMaxLength(50)
                .HasColumnName("condition_value");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B4A9B474A1");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__Challeng__CF6351910EBBD5CA");

            entity.ToTable("Challenge");

            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.ChallengeType)
                .HasMaxLength(100)
                .HasColumnName("challenge_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsTeamBased)
                .HasDefaultValue(true)
                .HasColumnName("is_team_based");
            entity.Property(e => e.MaxTeamSize).HasColumnName("max_team_size");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Challenges)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Challenge_User");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__E79576871F01EF43");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Guide).WithMany(p => p.Comments)
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__guide_i__02FC7413");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__user_id__03F0984C");
        });

        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasKey(e => e.CommissionId).HasName("PK__Commissi__D19D7CC9D117AA0D");

            entity.ToTable("Commission");

            entity.Property(e => e.CommissionId).HasColumnName("commission_id");
            entity.Property(e => e.Percent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("percent");
            entity.Property(e => e.RevenueId).HasColumnName("revenue_id");

            entity.HasOne(d => d.Revenue).WithMany(p => p.Commissions)
                .HasForeignKey(d => d.RevenueId)
                .HasConstraintName("FK__Commissio__reven__04E4BC85");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__8F1EF7AE3695E66B");

            entity.ToTable("Course");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Course__teacher___05D8E0BE");
        });

        modelBuilder.Entity<CourseAccess>(entity =>
        {
            entity.HasKey(e => e.AccessId).HasName("PK__Course_a__10FA1E20E506A329");

            entity.ToTable("Course_access");

            entity.Property(e => e.AccessId).HasColumnName("access_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.LearnerId).HasColumnName("learner_id");
            entity.Property(e => e.PurchasedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("purchased_at");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseAccesses)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Course_ac__cours__06CD04F7");

            entity.HasOne(d => d.Learner).WithMany(p => p.CourseAccesses)
                .HasForeignKey(d => d.LearnerId)
                .HasConstraintName("FK__Course_ac__learn__07C12930");
        });

        modelBuilder.Entity<CourseReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Course_r__60883D909C6247AD");

            entity.ToTable("Course_review");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Course_re__cours__08B54D69");

            entity.HasOne(d => d.User).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Course_re__user___09A971A2");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__46ACF4CB4B707C8A");

            entity.ToTable("Favorite");

            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Guide).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorite__guide___0D7A0286");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorite__user_i__0E6E26BF");
        });

        modelBuilder.Entity<Guide>(entity =>
        {
            entity.HasKey(e => e.GuideId).HasName("PK__Guide__04A822412670CBFF");

            entity.ToTable("Guide");

            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.OrigamiId).HasColumnName("OrigamiID");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Author).WithMany(p => p.Guides)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Guide__author_id__0F624AF8");

            entity.HasOne(d => d.Origami).WithMany(p => p.Guides)
                .HasForeignKey(d => d.OrigamiId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Guide_Origami");

            entity.HasMany(d => d.Categories).WithMany(p => p.Guides)
                .UsingEntity<Dictionary<string, object>>(
                    "GuideCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Guide_cat__categ__10566F31"),
                    l => l.HasOne<Guide>().WithMany()
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Guide_cat__guide__114A936A"),
                    j =>
                    {
                        j.HasKey("GuideId", "CategoryId").HasName("PK__Guide_ca__59FCCCDAE02EC207");
                        j.ToTable("GuideCategory");
                        j.IndexerProperty<int>("GuideId").HasColumnName("guide_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
        });

        modelBuilder.Entity<GuideAccess>(entity =>
        {
            entity.HasKey(e => e.AccessId).HasName("PK__GuideAcc__10FA1E2087C9A4C9");

            entity.ToTable("GuideAccess");

            entity.HasIndex(e => new { e.UserId, e.GuideId }, "UQ_User_Guide").IsUnique();

            entity.Property(e => e.AccessId).HasColumnName("access_id");
            entity.Property(e => e.GrantedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("granted_at");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Guide).WithMany(p => p.GuideAccesses)
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideAccess_Guide");

            entity.HasOne(d => d.User).WithMany(p => p.GuideAccesses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideAccess_User");
        });

        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(e => e.LeaderboardId).HasName("PK__Leaderbo__B358A1E650493078");

            entity.ToTable("Leaderboard");

            entity.Property(e => e.LeaderboardId).HasColumnName("LeaderboardID");
            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TotalScore).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Challenge).WithMany(p => p.Leaderboards)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK_Leaderboard_Challenge");

            entity.HasOne(d => d.Team).WithMany(p => p.Leaderboards)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_Leaderboard_Team");

            entity.HasOne(d => d.User).WithMany(p => p.Leaderboards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Leaderboard_User");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__Lesson__6421F7BE45199E70");

            entity.ToTable("Lesson");

            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Lesson__course_i__151B244E");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F6BB72656");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__user___160F4887");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__46596229E2DAC65D");

            entity.HasIndex(e => e.BuyerUserId, "IX_Orders_Buyer");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.BuyerUserId).HasColumnName("buyer_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("VND")
                .HasColumnName("currency");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.BuyerUser).WithMany(p => p.Orders)
                .HasForeignKey(d => d.BuyerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__buyer_us__5224328E");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__3764B6BCB975590F");

            entity.ToTable("OrderItem");

            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductType)
                .HasMaxLength(20)
                .HasColumnName("product_type");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__order__56E8E7AB");
        });

        modelBuilder.Entity<Origami>(entity =>
        {
            entity.HasKey(e => e.OrigamiId).HasName("PK__Origami__500FAC08CA4F3CED");

            entity.ToTable("Origami");

            entity.Property(e => e.OrigamiId).HasColumnName("OrigamiID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Origamis)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Origami_User");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__2EC21549D431B879");

            entity.ToTable("Question");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Question__course__17036CC0");

            entity.HasOne(d => d.User).WithMany(p => p.Questions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Question__user_i__17F790F9");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Refresh___CB3C9E17E7EF4440");

            entity.ToTable("Refresh_token");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.RefreshToken1)
                .HasMaxLength(512)
                .HasColumnName("refresh_token");
            entity.Property(e => e.RevokedAt)
                .HasColumnType("datetime")
                .HasColumnName("revoked_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.HasKey(e => e.RevenueId).HasName("PK__Revenue__3DF902E9A2E6EC05");

            entity.ToTable("Revenue");

            entity.Property(e => e.RevenueId).HasColumnName("revenue_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Revenues)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Revenue__course___18EBB532");

            entity.HasOne(d => d.Guide).WithMany(p => p.Revenues)
                .HasForeignKey(d => d.GuideId)
                .HasConstraintName("FK__Revenue__guide_i__19DFD96B");

            entity.HasOne(d => d.User).WithMany(p => p.Revenues)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Revenue__user_id__1AD3FDA4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CC784BE016");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("PK__Score__7DD229F1AA01F393");

            entity.ToTable("Score");

            entity.Property(e => e.ScoreId).HasColumnName("ScoreID");
            entity.Property(e => e.Score1)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("Score");
            entity.Property(e => e.ScoreAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");

            entity.HasOne(d => d.ScoreByNavigation).WithMany(p => p.Scores)
                .HasForeignKey(d => d.ScoreBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Score_User");

            entity.HasOne(d => d.Submission).WithMany(p => p.Scores)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Score_Submission");
        });

        modelBuilder.Entity<Step>(entity =>
        {
            entity.HasKey(e => e.StepId).HasName("PK__Steps__B2E1DE81D499DE8F");

            entity.ToTable("Step");

            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.StepNumber).HasColumnName("step_number");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(255)
                .HasColumnName("video_url");

            entity.HasOne(d => d.Guide).WithMany(p => p.Steps)
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Steps__guide_id__1BC821DD");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__449EE105DAD96CD1");

            entity.ToTable("Submission");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Challenge).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK_Submission_Challenge");

            entity.HasOne(d => d.SubmittedByNavigation).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.SubmittedBy)
                .HasConstraintName("FK_Submission_User");

            entity.HasOne(d => d.Team).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_Submission_Team");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__123AE7B91E7AA665");

            entity.ToTable("Team");

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TeamName).HasMaxLength(255);

            entity.HasOne(d => d.Challenge).WithMany(p => p.Teams)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK_Team_Challenge");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.TeamMemberId).HasName("PK__TeamMemb__C7C09285ED6982A2");

            entity.ToTable("TeamMember");

            entity.HasIndex(e => new { e.TeamId, e.UserId }, "UQ_TeamMember").IsUnique();

            entity.Property(e => e.TeamMemberId).HasColumnName("TeamMemberID");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_TeamMember_Team");

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_TeamMember_User");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__D596F96B5425D56E");

            entity.ToTable("Ticket");

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.TicketType).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TicketTypeId)
                .HasConstraintName("FK__Ticket__ticket_t__1EA48E88");

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Ticket__user_id__1F98B2C1");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeId).HasName("PK__Ticket_t__9531B7D18DCADC25");

            entity.ToTable("Ticket_type");

            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.TicketTypeName)
                .HasMaxLength(50)
                .HasColumnName("ticket_type_name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__85C600AF3D3F1D9F");

            entity.ToTable("Transaction");

            entity.HasIndex(e => e.OrderId, "IX_Transaction_OrderId");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ReceiverWalletId).HasColumnName("receiver_wallet_id");
            entity.Property(e => e.SenderWalletId).HasColumnName("sender_wallet_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(30)
                .HasColumnName("transaction_type");

            entity.HasOne(d => d.Order).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Transacti__order__58D1301D");

            entity.HasOne(d => d.ReceiverWallet).WithMany(p => p.TransactionReceiverWallets)
                .HasForeignKey(d => d.ReceiverWalletId)
                .HasConstraintName("FK__Transacti__recei__208CD6FA");

            entity.HasOne(d => d.SenderWallet).WithMany(p => p.TransactionSenderWallets)
                .HasForeignKey(d => d.SenderWalletId)
                .HasConstraintName("FK__Transacti__sende__2180FB33");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370FA4D9BD59");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E6164B7307E3C").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__role_id__22751F6C");
        });

        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BadgeId }).HasName("PK__User_bad__C7C7BE6A8E451287");

            entity.ToTable("User_badge");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.BadgeId).HasColumnName("badge_id");
            entity.Property(e => e.EarnedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("earned_at");

            entity.HasOne(d => d.Badge).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.BadgeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_badg__badge__236943A5");

            entity.HasOne(d => d.User).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_badg__user___245D67DE");
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("PK__Vote__9F5405AE055254B3");

            entity.ToTable("Vote");

            entity.HasIndex(e => new { e.SubmissionId, e.UserId }, "UQ_Vote").IsUnique();

            entity.Property(e => e.VoteId).HasColumnName("vote_id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VotedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("voted_at");

            entity.HasOne(d => d.Submission).WithMany(p => p.Votes)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK_Vote_Submission");

            entity.HasOne(d => d.User).WithMany(p => p.Votes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_User");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("PK__Wallet__0EE6F041390DDE41");

            entity.ToTable("Wallet");

            entity.Property(e => e.WalletId).HasColumnName("wallet_id");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wallet__user_id__2739D489");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
