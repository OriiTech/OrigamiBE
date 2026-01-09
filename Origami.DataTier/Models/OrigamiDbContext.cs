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

    public virtual DbSet<ChallengeOtherRequirement> ChallengeOtherRequirements { get; set; }

    public virtual DbSet<ChallengePrize> ChallengePrizes { get; set; }

    public virtual DbSet<ChallengeRequirement> ChallengeRequirements { get; set; }

    public virtual DbSet<ChallengeRule> ChallengeRules { get; set; }

    public virtual DbSet<ChallengeRuleItem> ChallengeRuleItems { get; set; }

    public virtual DbSet<ChallengeSchedule> ChallengeSchedules { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Commission> Commissions { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseAccess> CourseAccesses { get; set; }

    public virtual DbSet<CourseReview> CourseReviews { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Guide> Guides { get; set; }

    public virtual DbSet<GuideAccess> GuideAccesses { get; set; }

    public virtual DbSet<GuidePreview> GuidePreviews { get; set; }

    public virtual DbSet<GuidePromoPhoto> GuidePromoPhotos { get; set; }

    public virtual DbSet<GuideRating> GuideRatings { get; set; }

    public virtual DbSet<GuideRequirement> GuideRequirements { get; set; }

    public virtual DbSet<GuideView> GuideViews { get; set; }

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

    public virtual DbSet<ScoreCriterion> ScoreCriteria { get; set; }

    public virtual DbSet<Step> Steps { get; set; }

    public virtual DbSet<StepTip> StepTips { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<SubmissionComment> SubmissionComments { get; set; }

    public virtual DbSet<SubmissionFoldingDetail> SubmissionFoldingDetails { get; set; }

    public virtual DbSet<SubmissionImage> SubmissionImages { get; set; }

    public virtual DbSet<SubmissionLike> SubmissionLikes { get; set; }

    public virtual DbSet<SubmissionSnapshot> SubmissionSnapshots { get; set; }

    public virtual DbSet<SubmissionView> SubmissionViews { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=OrigamiDB;User Id=sa;Password=1;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__Answer__33724318AE61C471");

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
                .HasConstraintName("FK__Answer__question__690797E6");

            entity.HasOne(d => d.User).WithMany(p => p.Answers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Answer__user_id__69FBBC1F");
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.BadgeId).HasName("PK__Badge__E79896562CBF5AF6");

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
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B4B8FDF50C");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasDefaultValue("topic")
                .HasColumnName("type");
        });

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__Challeng__CF635191A9AE1D91");

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
            entity.Property(e => e.EntryFee)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("entry_fee");
            entity.Property(e => e.IsFeatured).HasColumnName("is_featured");
            entity.Property(e => e.IsFree).HasColumnName("is_free");
            entity.Property(e => e.IsTeamBased)
                .HasDefaultValue(true)
                .HasColumnName("is_team_based");
            entity.Property(e => e.Level)
                .HasMaxLength(40)
                .HasColumnName("level");
            entity.Property(e => e.MaxTeamSize).HasColumnName("max_team_size");
            entity.Property(e => e.Phase)
                .HasMaxLength(40)
                .HasColumnName("phase");
            entity.Property(e => e.PrizePool)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("prize_pool");
            entity.Property(e => e.PromoPhoto)
                .HasMaxLength(1000)
                .HasColumnName("promo_photo");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(40)
                .HasColumnName("status");
            entity.Property(e => e.Theme)
                .HasMaxLength(255)
                .HasColumnName("theme");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Challenges)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Challenge_User");

            entity.HasMany(d => d.Categories).WithMany(p => p.Challenges)
                .UsingEntity<Dictionary<string, object>>(
                    "ChallengeCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Challenge__categ__6BE40491"),
                    l => l.HasOne<Challenge>().WithMany()
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Challenge__chall__6CD828CA"),
                    j =>
                    {
                        j.HasKey("ChallengeId", "CategoryId").HasName("PK__Challeng__9237BF0AD6872640");
                        j.ToTable("ChallengeCategory");
                        j.IndexerProperty<int>("ChallengeId").HasColumnName("challenge_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });

            entity.HasMany(d => d.Users).WithMany(p => p.ChallengesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ChallengeJudge",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Challenge__user___6EC0713C"),
                    l => l.HasOne<Challenge>().WithMany()
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Challenge__chall__6DCC4D03"),
                    j =>
                    {
                        j.HasKey("ChallengeId", "UserId").HasName("PK__Challeng__24F8B2E153C1613C");
                        j.ToTable("ChallengeJudge");
                        j.IndexerProperty<int>("ChallengeId").HasColumnName("challenge_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<ChallengeOtherRequirement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Challeng__3213E83F75C07E91");

            entity.ToTable("ChallengeOtherRequirement");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");

            entity.HasOne(d => d.Challenge).WithMany(p => p.ChallengeOtherRequirements)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK__Challenge__chall__6FB49575");
        });

        modelBuilder.Entity<ChallengePrize>(entity =>
        {
            entity.HasKey(e => e.PrizeId).HasName("PK__Challeng__6EC2CFD9C2F98FC3");

            entity.ToTable("ChallengePrize");

            entity.Property(e => e.PrizeId).HasColumnName("prize_id");
            entity.Property(e => e.Cash)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("cash");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Rank).HasColumnName("rank");

            entity.HasOne(d => d.Challenge).WithMany(p => p.ChallengePrizes)
                .HasForeignKey(d => d.ChallengeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChallengePrize_Challenge");

            entity.HasMany(d => d.Badges).WithMany(p => p.Prizes)
                .UsingEntity<Dictionary<string, object>>(
                    "ChallengePrizeBadge",
                    r => r.HasOne<Badge>().WithMany()
                        .HasForeignKey("BadgeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ChallengePrizeBadge_Badge"),
                    l => l.HasOne<ChallengePrize>().WithMany()
                        .HasForeignKey("PrizeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ChallengePrizeBadge_Prize"),
                    j =>
                    {
                        j.HasKey("PrizeId", "BadgeId");
                        j.ToTable("ChallengePrizeBadge");
                        j.IndexerProperty<int>("PrizeId").HasColumnName("prize_id");
                        j.IndexerProperty<int>("BadgeId").HasColumnName("badge_id");
                    });
        });

        modelBuilder.Entity<ChallengeRequirement>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__Challeng__CF63519175A58024");

            entity.ToTable("ChallengeRequirement");

            entity.Property(e => e.ChallengeId)
                .ValueGeneratedNever()
                .HasColumnName("challenge_id");
            entity.Property(e => e.FoldingConstraints)
                .HasMaxLength(500)
                .HasColumnName("folding_constraints");
            entity.Property(e => e.MaximumSubmissions).HasColumnName("maximum_submissions");
            entity.Property(e => e.ModelRequirements)
                .HasMaxLength(500)
                .HasColumnName("model_requirements");
            entity.Property(e => e.PaperRequirements)
                .HasMaxLength(500)
                .HasColumnName("paper_requirements");
            entity.Property(e => e.PhotographyRequirements)
                .HasMaxLength(500)
                .HasColumnName("photography_requirements");
            entity.Property(e => e.TeamSize).HasColumnName("team_size");

            entity.HasOne(d => d.Challenge).WithOne(p => p.ChallengeRequirement)
                .HasForeignKey<ChallengeRequirement>(d => d.ChallengeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Challenge__chall__73852659");
        });

        modelBuilder.Entity<ChallengeRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__Challeng__E92A9296CAEF3011");

            entity.ToTable("ChallengeRule");

            entity.Property(e => e.RuleId).HasColumnName("rule_id");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.Section)
                .HasMaxLength(255)
                .HasColumnName("section");

            entity.HasOne(d => d.Challenge).WithMany(p => p.ChallengeRules)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK__Challenge__chall__74794A92");
        });

        modelBuilder.Entity<ChallengeRuleItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Challeng__52020FDD2E6BFCF6");

            entity.ToTable("ChallengeRuleItem");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasColumnName("content");
            entity.Property(e => e.RuleId).HasColumnName("rule_id");

            entity.HasOne(d => d.Rule).WithMany(p => p.ChallengeRuleItems)
                .HasForeignKey(d => d.RuleId)
                .HasConstraintName("FK__Challenge__rule___756D6ECB");
        });

        modelBuilder.Entity<ChallengeSchedule>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__Challeng__CF63519192F72810");

            entity.ToTable("ChallengeSchedule");

            entity.Property(e => e.ChallengeId)
                .ValueGeneratedNever()
                .HasColumnName("challenge_id");
            entity.Property(e => e.RegistrationStart)
                .HasColumnType("datetime")
                .HasColumnName("registration_start");
            entity.Property(e => e.ResultsDate)
                .HasColumnType("datetime")
                .HasColumnName("results_date");
            entity.Property(e => e.SubmissionEnd)
                .HasColumnType("datetime")
                .HasColumnName("submission_end");
            entity.Property(e => e.SubmissionStart)
                .HasColumnType("datetime")
                .HasColumnName("submission_start");
            entity.Property(e => e.VotingEnd)
                .HasColumnType("datetime")
                .HasColumnName("voting_end");
            entity.Property(e => e.VotingStart)
                .HasColumnType("datetime")
                .HasColumnName("voting_start");

            entity.HasOne(d => d.Challenge).WithOne(p => p.ChallengeSchedule)
                .HasForeignKey<ChallengeSchedule>(d => d.ChallengeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Challenge__chall__76619304");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__E7957687EF0059BF");

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
                .HasConstraintName("FK__Comment__guide_i__7755B73D");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Comment_Parent");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__user_id__7849DB76");
        });

        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasKey(e => e.CommissionId).HasName("PK__Commissi__D19D7CC92309F437");

            entity.ToTable("Commission");

            entity.Property(e => e.CommissionId).HasColumnName("commission_id");
            entity.Property(e => e.Percent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("percent");
            entity.Property(e => e.RevenueId).HasColumnName("revenue_id");

            entity.HasOne(d => d.Revenue).WithMany(p => p.Commissions)
                .HasForeignKey(d => d.RevenueId)
                .HasConstraintName("FK__Commissio__reven__7A3223E8");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__8F1EF7AE56E431FC");

            entity.ToTable("Course");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.Bestseller)
                .HasDefaultValue(false)
                .HasColumnName("bestseller");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Language)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("language");
            entity.Property(e => e.Objectives).HasColumnName("objectives");
            entity.Property(e => e.PaidOnly).HasColumnName("paid_only");
            entity.Property(e => e.PreviewVideoUrl)
                .HasMaxLength(512)
                .HasColumnName("preview_video_url");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("datetime")
                .HasColumnName("published_at");
            entity.Property(e => e.Subtitle)
                .HasMaxLength(255)
                .HasColumnName("subtitle");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("thumbnail_url");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Trending).HasColumnName("trending");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Course__teacher___7B264821");

            entity.HasMany(d => d.Categories).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CC_Category"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CC_Course"),
                    j =>
                    {
                        j.HasKey("CourseId", "CategoryId");
                        j.ToTable("Course_category");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });

            entity.HasMany(d => d.Levels).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseTargetLevel",
                    r => r.HasOne<TargetLevel>().WithMany()
                        .HasForeignKey("LevelId")
                        .HasConstraintName("FK_Course_Target_Level"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK_Course_Target_Course"),
                    j =>
                    {
                        j.HasKey("CourseId", "LevelId").HasName("PK_Course_Target");
                        j.ToTable("Course_target_level");
                        j.IndexerProperty<int>("CourseId").HasColumnName("course_id");
                        j.IndexerProperty<int>("LevelId").HasColumnName("level_id");
                    });
        });

        modelBuilder.Entity<CourseAccess>(entity =>
        {
            entity.HasKey(e => e.AccessId).HasName("PK__Course_a__10FA1E20E14F364C");

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
                .HasConstraintName("FK__Course_ac__cours__7C1A6C5A");

            entity.HasOne(d => d.Learner).WithMany(p => p.CourseAccesses)
                .HasForeignKey(d => d.LearnerId)
                .HasConstraintName("FK__Course_ac__learn__7D0E9093");
        });

        modelBuilder.Entity<CourseReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Course_r__60883D9043CADFD0");

            entity.ToTable("Course_review");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LikeCount).HasColumnName("like_count");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Course_re__cours__7FEAFD3E");

            entity.HasOne(d => d.User).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Course_re__user___00DF2177");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__46ACF4CBC8A8E82F");

            entity.ToTable("Favorite");

            entity.HasIndex(e => new { e.UserId, e.GuideId }, "UX_Favorite_User_Guide").IsUnique();

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
                .HasConstraintName("FK__Favorite__guide___03BB8E22");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorite__user_i__04AFB25B");
        });

        modelBuilder.Entity<Guide>(entity =>
        {
            entity.HasKey(e => e.GuideId).HasName("PK__Guide__04A822410669B805");

            entity.ToTable("Guide");

            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Bestseller).HasColumnName("bestseller");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsNew)
                .HasDefaultValue(true)
                .HasColumnName("is_new");
            entity.Property(e => e.OrigamiId).HasColumnName("OrigamiID");
            entity.Property(e => e.PaidOnly).HasColumnName("paid_only");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Subtitle)
                .HasMaxLength(255)
                .HasColumnName("subtitle");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Trending).HasColumnName("trending");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Author).WithMany(p => p.Guides)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Guide__author_id__05A3D694");

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
                        .HasConstraintName("FK__GuideCate__categ__0A688BB1"),
                    l => l.HasOne<Guide>().WithMany()
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GuideCate__guide__0B5CAFEA"),
                    j =>
                    {
                        j.HasKey("GuideId", "CategoryId").HasName("PK__GuideCat__59FCCCDAA35C61DF");
                        j.ToTable("GuideCategory");
                        j.IndexerProperty<int>("GuideId").HasColumnName("guide_id");
                        j.IndexerProperty<int>("CategoryId").HasColumnName("category_id");
                    });
        });

        modelBuilder.Entity<GuideAccess>(entity =>
        {
            entity.HasKey(e => e.AccessId).HasName("PK__GuideAcc__10FA1E20F902B0D2");

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

        modelBuilder.Entity<GuidePreview>(entity =>
        {
            entity.HasKey(e => e.GuideId).HasName("PK__GuidePre__04A82241C2B2888D");

            entity.ToTable("GuidePreview");

            entity.Property(e => e.GuideId)
                .ValueGeneratedNever()
                .HasColumnName("guide_id");
            entity.Property(e => e.VideoAvailable).HasColumnName("video_available");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("video_url");

            entity.HasOne(d => d.Guide).WithOne(p => p.GuidePreview)
                .HasForeignKey<GuidePreview>(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuidePreview_Guide");
        });

        modelBuilder.Entity<GuidePromoPhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__GuidePro__CB48C83D6EF2E7E0");

            entity.ToTable("GuidePromoPhoto");

            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .HasColumnName("url");

            entity.HasOne(d => d.Guide).WithMany(p => p.GuidePromoPhotos)
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuidePromoPhoto_Guide");
        });

        modelBuilder.Entity<GuideRating>(entity =>
        {
            entity.HasKey(e => new { e.GuideId, e.UserId });

            entity.ToTable("GuideRating");

            entity.HasIndex(e => new { e.GuideId, e.UserId }, "UQ_GuideRating").IsUnique();

            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.Guide).WithMany(p => p.GuideRatings)
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideRating_Guide");

            entity.HasOne(d => d.User).WithMany(p => p.GuideRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideRating_User");
        });

        modelBuilder.Entity<GuideRequirement>(entity =>
        {
            entity.HasKey(e => e.GuideId).HasName("PK__GuideReq__04A8224125551C7D");

            entity.ToTable("GuideRequirement");

            entity.Property(e => e.GuideId)
                .ValueGeneratedNever()
                .HasColumnName("guide_id");
            entity.Property(e => e.Colors)
                .HasMaxLength(255)
                .HasColumnName("colors");
            entity.Property(e => e.PaperSize)
                .HasMaxLength(50)
                .HasColumnName("paper_size");
            entity.Property(e => e.PaperType)
                .HasMaxLength(100)
                .HasColumnName("paper_type");
            entity.Property(e => e.Tools)
                .HasMaxLength(255)
                .HasColumnName("tools");

            entity.HasOne(d => d.Guide).WithOne(p => p.GuideRequirement)
                .HasForeignKey<GuideRequirement>(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideRequirement_Guide");
        });

        modelBuilder.Entity<GuideView>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PK__GuideVie__B5A34EE2EF77198C");

            entity.ToTable("GuideView");

            entity.Property(e => e.ViewId).HasColumnName("view_id");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("viewed_at");

            entity.HasOne(d => d.Guide).WithMany(p => p.GuideViews)
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuideView_Guide");

            entity.HasOne(d => d.User).WithMany(p => p.GuideViews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_GuideView_User");
        });

        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(e => e.LeaderboardId).HasName("PK__Leaderbo__B358A1E6522E66FB");

            entity.ToTable("Leaderboard");

            entity.Property(e => e.LeaderboardId).HasColumnName("LeaderboardID");
            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.JudgeScore).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TotalScore).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VoteScore).HasColumnType("decimal(6, 2)");

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

        modelBuilder.Entity<Lecture>(entity =>
        {
            entity.HasKey(e => e.LectureId).HasName("PK__Lecture__797827F5F7150BD8");

            entity.ToTable("Lecture");

            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.ContentUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("content_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PreviewAvailable).HasColumnName("preview_available");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Lectures)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lecture_Lesson");
        });

        modelBuilder.Entity<LectureProgress>(entity =>
        {
            entity.HasKey(e => new { e.LectureId, e.UserId });

            entity.ToTable("Lecture_progress");

            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("completed_at");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed");

            entity.HasOne(d => d.Lecture).WithMany(p => p.LectureProgresses)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LP_Lecture");

            entity.HasOne(d => d.User).WithMany(p => p.LectureProgresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LP_User");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__Lesson__6421F7BE2C587C2D");

            entity.ToTable("Lesson");

            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Lesson__course_i__18B6AB08");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F92A0DDD4");

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
                .HasConstraintName("FK__Notificat__user___19AACF41");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__46596229347BC86D");

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
                .HasConstraintName("FK__Orders__buyer_us__1B9317B3");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__3764B6BCD1900135");

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
                .HasConstraintName("FK__OrderItem__order__1A9EF37A");
        });

        modelBuilder.Entity<Origami>(entity =>
        {
            entity.HasKey(e => e.OrigamiId).HasName("PK__Origami__500FAC081AA4110C");

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
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__2EC215497CA82362");

            entity.ToTable("Question");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.TargetType)
                .HasMaxLength(20)
                .HasDefaultValue("Course")
                .HasColumnName("target_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Question__course__1D7B6025");

            entity.HasOne(d => d.Lecture).WithMany(p => p.Questions)
                .HasForeignKey(d => d.LectureId)
                .HasConstraintName("FK_Question_Lecture");

            entity.HasOne(d => d.User).WithMany(p => p.Questions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Question__user_i__1E6F845E");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Refresh___CB3C9E17CED13E75");

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

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__Resource__4985FC73F5A81035");

            entity.ToTable("Resource");

            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LectureId).HasColumnName("lecture_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("url");

            entity.HasOne(d => d.Lecture).WithMany(p => p.Resources)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resource_Lecture");
        });

        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.HasKey(e => e.RevenueId).HasName("PK__Revenue__3DF902E9B703666A");

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
                .HasConstraintName("FK__Revenue__course___22401542");

            entity.HasOne(d => d.Guide).WithMany(p => p.Revenues)
                .HasForeignKey(d => d.GuideId)
                .HasConstraintName("FK__Revenue__guide_i__2334397B");

            entity.HasOne(d => d.User).WithMany(p => p.Revenues)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Revenue__user_id__24285DB4");
        });

        modelBuilder.Entity<ReviewResponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId).HasName("PK__Review_r__EBECD896AE0A90A3");

            entity.ToTable("Review_response");

            entity.Property(e => e.ResponseId).HasColumnName("response_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewResponses)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RR_Review");

            entity.HasOne(d => d.User).WithMany(p => p.ReviewResponses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ReviewResponse_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CCB0C0365D");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("PK__Score__7DD229F13E8EE837");

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

        modelBuilder.Entity<ScoreCriterion>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("PK__ScoreCri__8CA1905016540416");

            entity.Property(e => e.ScoreId)
                .ValueGeneratedNever()
                .HasColumnName("score_id");
            entity.Property(e => e.Creativity)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("creativity");
            entity.Property(e => e.Difficulty)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("difficulty");
            entity.Property(e => e.Execution)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("execution");
            entity.Property(e => e.Presentation)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("presentation");
            entity.Property(e => e.Theme)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("theme");

            entity.HasOne(d => d.Score).WithOne(p => p.ScoreCriterion)
                .HasForeignKey<ScoreCriterion>(d => d.ScoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScoreCriteria_Score");
        });

        modelBuilder.Entity<Step>(entity =>
        {
            entity.HasKey(e => e.StepId).HasName("PK__Step__B2E1DE81285B81E5");

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
                .HasConstraintName("FK__Step__guide_id__29E1370A");
        });

        modelBuilder.Entity<StepTip>(entity =>
        {
            entity.HasKey(e => e.TipId).HasName("PK__StepTip__377877B2931E3198");

            entity.ToTable("StepTip");

            entity.Property(e => e.TipId).HasColumnName("tip_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.StepId).HasColumnName("step_id");

            entity.HasOne(d => d.Step).WithMany(p => p.StepTips)
                .HasForeignKey(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StepTip_Step");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__449EE1058E86028E");

            entity.ToTable("Submission");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.ChallengeId).HasColumnName("ChallengeID");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(40)
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

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

        modelBuilder.Entity<SubmissionComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Submissi__E7957687C7B68FC9");

            entity.ToTable("SubmissionComment");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_SubmissionComment_Parent");

            entity.HasOne(d => d.Submission).WithMany(p => p.SubmissionComments)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubmissionComment_Submission");

            entity.HasOne(d => d.User).WithMany(p => p.SubmissionComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubmissionComment_User");
        });

        modelBuilder.Entity<SubmissionFoldingDetail>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__9B535595100D5A97");

            entity.ToTable("SubmissionFoldingDetail");

            entity.Property(e => e.SubmissionId)
                .ValueGeneratedNever()
                .HasColumnName("submission_id");
            entity.Property(e => e.Complexity).HasColumnName("complexity");
            entity.Property(e => e.FoldingTimeMinute).HasColumnName("folding_time_minute");
            entity.Property(e => e.OriginalDesigner)
                .HasMaxLength(255)
                .HasColumnName("original_designer");
            entity.Property(e => e.PaperSize)
                .HasMaxLength(100)
                .HasColumnName("paper_size");
            entity.Property(e => e.PaperType)
                .HasMaxLength(100)
                .HasColumnName("paper_type");
            entity.Property(e => e.Source)
                .HasMaxLength(50)
                .HasColumnName("source");

            entity.HasOne(d => d.Submission).WithOne(p => p.SubmissionFoldingDetail)
                .HasForeignKey<SubmissionFoldingDetail>(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__submi__318258D2");
        });

        modelBuilder.Entity<SubmissionImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Submissi__DC9AC95578981537");

            entity.ToTable("SubmissionImage");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(1000)
                .HasColumnName("thumbnail");
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .HasColumnName("url");

            entity.HasOne(d => d.Submission).WithMany(p => p.SubmissionImages)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK__Submissio__submi__32767D0B");
        });

        modelBuilder.Entity<SubmissionLike>(entity =>
        {
            entity.HasKey(e => new { e.SubmissionId, e.UserId });

            entity.ToTable("SubmissionLike");

            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Submission).WithMany(p => p.SubmissionLikes)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubmissionLike_Submission");

            entity.HasOne(d => d.User).WithMany(p => p.SubmissionLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubmissionLike_User");
        });

        modelBuilder.Entity<SubmissionSnapshot>(entity =>
        {
            entity.HasKey(e => e.SnapshotId).HasName("PK__Submissi__C27CFBF76EEA5D97");

            entity.ToTable("SubmissionSnapshot");

            entity.HasIndex(e => new { e.ChallengeId, e.Rank }, "IX_SubmissionSnapshot_Challenge_Rank");

            entity.HasIndex(e => new { e.ChallengeId, e.SubmissionId }, "UX_SubmissionSnapshot_Challenge_Submission").IsUnique();

            entity.Property(e => e.SnapshotId).HasColumnName("snapshot_id");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.CommunityScore)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("community_score");
            entity.Property(e => e.CommunityStatsJson).HasColumnName("community_stats_json");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsLocked)
                .HasDefaultValue(true)
                .HasColumnName("is_locked");
            entity.Property(e => e.JudgeCriteriaJson).HasColumnName("judge_criteria_json");
            entity.Property(e => e.JudgeScore)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("judge_score");
            entity.Property(e => e.JudgeScoresJson).HasColumnName("judge_scores_json");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.SnapshotAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("snapshot_at");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.TotalScore)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("total_score");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<SubmissionView>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PK__Submissi__B5A34EE21FD18240");
        modelBuilder.Entity<SubmissionView>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PK__Submissi__B5A34EE292D8AF25");

            entity.ToTable("SubmissionView");

            entity.Property(e => e.ViewId).HasColumnName("view_id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ViewedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("viewed_at");

            entity.HasOne(d => d.Submission).WithMany(p => p.SubmissionViews)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubmissionView_Submission");

            entity.HasOne(d => d.User).WithMany(p => p.SubmissionViews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_SubmissionView_User");
        });

        modelBuilder.Entity<TargetLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Target_l__03461643B39AC844");

            entity.ToTable("Target_level");

            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__123AE7B996C8A353");

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
            entity.HasKey(e => e.TeamMemberId).HasName("PK__TeamMemb__C7C0928526483173");

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
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__D596F96BD015F51C");

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
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.TicketType).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TicketTypeId)
                .HasConstraintName("FK__Ticket__ticket_t__3A179ED3");

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Ticket__user_id__3B0BC30C");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeId).HasName("PK__Ticket_t__9531B7D191378DCB");

            entity.ToTable("Ticket_type");

            entity.Property(e => e.TicketTypeId).HasColumnName("ticket_type_id");
            entity.Property(e => e.TicketTypeName)
                .HasMaxLength(50)
                .HasColumnName("ticket_type_name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__85C600AF675022F2");

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
                .HasConstraintName("FK__Transacti__order__3BFFE745");

            entity.HasOne(d => d.ReceiverWallet).WithMany(p => p.TransactionReceiverWallets)
                .HasForeignKey(d => d.ReceiverWalletId)
                .HasConstraintName("FK__Transacti__recei__3CF40B7E");

            entity.HasOne(d => d.SenderWallet).WithMany(p => p.TransactionSenderWallets)
                .HasForeignKey(d => d.SenderWalletId)
                .HasConstraintName("FK__Transacti__sende__3DE82FB7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__B9BE370F96F30EB3");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__AB6E6164DF88A60C").IsUnique();

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
                .HasConstraintName("FK__User__role_id__3EDC53F0");
        });

        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BadgeId }).HasName("PK__User_bad__C7C7BE6AF08D0D77");

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
                .HasConstraintName("FK__User_badg__badge__3FD07829");

            entity.HasOne(d => d.User).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_badg__user___40C49C62");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserProfile");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .HasColumnName("display_name");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_User");
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("PK__Vote__9F5405AE911B0D52");

            entity.ToTable("Vote");

            entity.HasIndex(e => new { e.SubmissionId, e.UserId }, "UQ_Vote").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.SubmissionId }, "UX_Vote_User_Submission").IsUnique();

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
            entity.HasKey(e => e.WalletId).HasName("PK__Wallet__0EE6F041108BBFE6");

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
                .HasConstraintName("FK__Wallet__user_id__44952D46");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
