using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Origami.DataTier.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Badge",
                columns: table => new
                {
                    badge_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    badge_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    badge_description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    condition_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    condition_value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Badge__E79896562CBF5AF6", x => x.badge_id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "topic")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__D54EE9B4B8FDF50C", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__760965CCB0C0365D", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionSnapshot",
                columns: table => new
                {
                    snapshot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    challenge_id = table.Column<int>(type: "int", nullable: false),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    team_id = table.Column<int>(type: "int", nullable: true),
                    rank = table.Column<int>(type: "int", nullable: false),
                    is_locked = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    total_score = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    judge_score = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    community_score = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    judge_criteria_json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    judge_scores_json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    community_stats_json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    snapshot_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__C27CFBF76EEA5D97", x => x.snapshot_id);
                });

            migrationBuilder.CreateTable(
                name: "Target_level",
                columns: table => new
                {
                    level_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Target_l__03461643B39AC844", x => x.level_id);
                });

            migrationBuilder.CreateTable(
                name: "Ticket_type",
                columns: table => new
                {
                    ticket_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ticket_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ticket_t__9531B7D191378DCB", x => x.ticket_type_id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__B9BE370F96F30EB3", x => x.user_id);
                    table.ForeignKey(
                        name: "FK__User__role_id__3EDC53F0",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Challenge",
                columns: table => new
                {
                    challenge_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    challenge_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    max_team_size = table.Column<int>(type: "int", nullable: true),
                    is_team_based = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    phase = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    level = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    theme = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    promo_photo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    is_free = table.Column<bool>(type: "bit", nullable: true),
                    entry_fee = table.Column<decimal>(type: "decimal(9,2)", nullable: true),
                    prize_pool = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_featured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__CF635191A9AE1D91", x => x.challenge_id);
                    table.ForeignKey(
                        name: "FK_Challenge_User",
                        column: x => x.created_by,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    teacher_id = table.Column<int>(type: "int", nullable: true),
                    language = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    thumbnail_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    published_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    bestseller = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    subtitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    objectives = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paid_only = table.Column<bool>(type: "bit", nullable: true),
                    trending = table.Column<bool>(type: "bit", nullable: true),
                    preview_video_url = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Course__8F1EF7AE56E431FC", x => x.course_id);
                    table.ForeignKey(
                        name: "FK__Course__teacher___7B264821",
                        column: x => x.teacher_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__E059842F92A0DDD4", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__Notificat__user___19AACF41",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buyer_user_id = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "VND"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__46596229347BC86D", x => x.order_id);
                    table.ForeignKey(
                        name: "FK__Orders__buyer_us__1B9317B3",
                        column: x => x.buyer_user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Origami",
                columns: table => new
                {
                    OrigamiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Origami__500FAC081AA4110C", x => x.OrigamiID);
                    table.ForeignKey(
                        name: "FK_Origami_User",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Refresh_token",
                columns: table => new
                {
                    token_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Refresh___CB3C9E17CED13E75", x => x.token_id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    ticket_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    ticket_type_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ticket__D596F96BD015F51C", x => x.ticket_id);
                    table.ForeignKey(
                        name: "FK__Ticket__ticket_t__3A179ED3",
                        column: x => x.ticket_type_id,
                        principalTable: "Ticket_type",
                        principalColumn: "ticket_type_id");
                    table.ForeignKey(
                        name: "FK__Ticket__user_id__3B0BC30C",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "User_badge",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    badge_id = table.Column<int>(type: "int", nullable: false),
                    earned_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_bad__C7C7BE6AF08D0D77", x => new { x.user_id, x.badge_id });
                    table.ForeignKey(
                        name: "FK__User_badg__badge__3FD07829",
                        column: x => x.badge_id,
                        principalTable: "Badge",
                        principalColumn: "badge_id");
                    table.ForeignKey(
                        name: "FK__User_badg__user___40C49C62",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    avatar_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_UserProfile_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    wallet_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Wallet__0EE6F041108BBFE6", x => x.wallet_id);
                    table.ForeignKey(
                        name: "FK__Wallet__user_id__44952D46",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengeCategory",
                columns: table => new
                {
                    challenge_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__9237BF0AD6872640", x => new { x.challenge_id, x.category_id });
                    table.ForeignKey(
                        name: "FK__Challenge__categ__6BE40491",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK__Challenge__chall__6CD828CA",
                        column: x => x.challenge_id,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengeJudge",
                columns: table => new
                {
                    challenge_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__24F8B2E153C1613C", x => new { x.challenge_id, x.user_id });
                    table.ForeignKey(
                        name: "FK__Challenge__chall__6DCC4D03",
                        column: x => x.challenge_id,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id");
                    table.ForeignKey(
                        name: "FK__Challenge__user___6EC0713C",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengeOtherRequirement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    challenge_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__3213E83F75C07E91", x => x.id);
                    table.ForeignKey(
                        name: "FK__Challenge__chall__6FB49575",
                        column: x => x.challenge_id,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengePrize",
                columns: table => new
                {
                    prize_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    challenge_id = table.Column<int>(type: "int", nullable: false),
                    rank = table.Column<int>(type: "int", nullable: false),
                    cash = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__6EC2CFD9C2F98FC3", x => x.prize_id);
                    table.ForeignKey(
                        name: "FK_ChallengePrize_Challenge",
                        column: x => x.challenge_id,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengeRequirement",
                columns: table => new
                {
                    challenge_id = table.Column<int>(type: "int", nullable: false),
                    paper_requirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    folding_constraints = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    photography_requirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    model_requirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    maximum_submissions = table.Column<int>(type: "int", nullable: true),
                    team_size = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__CF63519175A58024", x => x.challenge_id);
                    table.ForeignKey(
                        name: "FK__Challenge__chall__73852659",
                        column: x => x.challenge_id,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengeRule",
                columns: table => new
                {
                    rule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    challenge_id = table.Column<int>(type: "int", nullable: true),
                    section = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__E92A9296CAEF3011", x => x.rule_id);
                    table.ForeignKey(
                        name: "FK__Challenge__chall__74794A92",
                        column: x => x.challenge_id,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengeSchedule",
                columns: table => new
                {
                    challenge_id = table.Column<int>(type: "int", nullable: false),
                    registration_start = table.Column<DateTime>(type: "datetime", nullable: true),
                    submission_start = table.Column<DateTime>(type: "datetime", nullable: true),
                    submission_end = table.Column<DateTime>(type: "datetime", nullable: true),
                    voting_start = table.Column<DateTime>(type: "datetime", nullable: true),
                    voting_end = table.Column<DateTime>(type: "datetime", nullable: true),
                    results_date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__CF63519192F72810", x => x.challenge_id);
                    table.ForeignKey(
                        name: "FK__Challenge__chall__76619304",
                        column: x => x.challenge_id,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id");
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeID = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Team__123AE7B996C8A353", x => x.TeamID);
                    table.ForeignKey(
                        name: "FK_Team_Challenge",
                        column: x => x.ChallengeID,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Course_access",
                columns: table => new
                {
                    access_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    learner_id = table.Column<int>(type: "int", nullable: true),
                    purchased_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Course_a__10FA1E20E14F364C", x => x.access_id);
                    table.ForeignKey(
                        name: "FK__Course_ac__cours__7C1A6C5A",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Course_ac__learn__7D0E9093",
                        column: x => x.learner_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Course_category",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_category", x => new { x.course_id, x.category_id });
                    table.ForeignKey(
                        name: "FK_CC_Category",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK_CC_Course",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "course_id");
                });

            migrationBuilder.CreateTable(
                name: "Course_review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    like_count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Course_r__60883D9043CADFD0", x => x.review_id);
                    table.ForeignKey(
                        name: "FK__Course_re__cours__7FEAFD3E",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Course_re__user___00DF2177",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Course_target_level",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "int", nullable: false),
                    level_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course_Target", x => new { x.course_id, x.level_id });
                    table.ForeignKey(
                        name: "FK_Course_Target_Course",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Course_Target_Level",
                        column: x => x.level_id,
                        principalTable: "Target_level",
                        principalColumn: "level_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lesson",
                columns: table => new
                {
                    lesson_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Lesson__6421F7BE2C587C2D", x => x.lesson_id);
                    table.ForeignKey(
                        name: "FK__Lesson__course_i__18B6AB08",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "course_id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    order_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    product_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderIte__3764B6BCD1900135", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK__OrderItem__order__1A9EF37A",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "Guide",
                columns: table => new
                {
                    guide_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    OrigamiID = table.Column<int>(type: "int", nullable: true),
                    subtitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    paid_only = table.Column<bool>(type: "bit", nullable: false),
                    bestseller = table.Column<bool>(type: "bit", nullable: false),
                    trending = table.Column<bool>(type: "bit", nullable: false),
                    is_new = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Guide__04A822410669B805", x => x.guide_id);
                    table.ForeignKey(
                        name: "FK_Guide_Origami",
                        column: x => x.OrigamiID,
                        principalTable: "Origami",
                        principalColumn: "OrigamiID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__Guide__author_id__05A3D694",
                        column: x => x.author_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_wallet_id = table.Column<int>(type: "int", nullable: true),
                    receiver_wallet_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    transaction_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    order_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transact__85C600AF675022F2", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK__Transacti__order__3BFFE745",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK__Transacti__recei__3CF40B7E",
                        column: x => x.receiver_wallet_id,
                        principalTable: "Wallet",
                        principalColumn: "wallet_id");
                    table.ForeignKey(
                        name: "FK__Transacti__sende__3DE82FB7",
                        column: x => x.sender_wallet_id,
                        principalTable: "Wallet",
                        principalColumn: "wallet_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengePrizeBadge",
                columns: table => new
                {
                    prize_id = table.Column<int>(type: "int", nullable: false),
                    badge_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengePrizeBadge", x => new { x.prize_id, x.badge_id });
                    table.ForeignKey(
                        name: "FK_ChallengePrizeBadge_Badge",
                        column: x => x.badge_id,
                        principalTable: "Badge",
                        principalColumn: "badge_id");
                    table.ForeignKey(
                        name: "FK_ChallengePrizeBadge_Prize",
                        column: x => x.prize_id,
                        principalTable: "ChallengePrize",
                        principalColumn: "prize_id");
                });

            migrationBuilder.CreateTable(
                name: "ChallengeRuleItem",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rule_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Challeng__52020FDD2E6BFCF6", x => x.item_id);
                    table.ForeignKey(
                        name: "FK__Challenge__rule___756D6ECB",
                        column: x => x.rule_id,
                        principalTable: "ChallengeRule",
                        principalColumn: "rule_id");
                });

            migrationBuilder.CreateTable(
                name: "Leaderboard",
                columns: table => new
                {
                    LeaderboardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeID = table.Column<int>(type: "int", nullable: false),
                    TeamID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    TotalScore = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    JudgeScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    VoteScore = table.Column<decimal>(type: "decimal(6,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Leaderbo__B358A1E6522E66FB", x => x.LeaderboardID);
                    table.ForeignKey(
                        name: "FK_Leaderboard_Challenge",
                        column: x => x.ChallengeID,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Leaderboard_Team",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                    table.ForeignKey(
                        name: "FK_Leaderboard_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    SubmissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeID = table.Column<int>(type: "int", nullable: false),
                    TeamID = table.Column<int>(type: "int", nullable: true),
                    SubmittedBy = table.Column<int>(type: "int", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__449EE1058E86028E", x => x.SubmissionID);
                    table.ForeignKey(
                        name: "FK_Submission_Challenge",
                        column: x => x.ChallengeID,
                        principalTable: "Challenge",
                        principalColumn: "challenge_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submission_Team",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID");
                    table.ForeignKey(
                        name: "FK_Submission_User",
                        column: x => x.SubmittedBy,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    TeamMemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TeamMemb__C7C0928526483173", x => x.TeamMemberID);
                    table.ForeignKey(
                        name: "FK_TeamMember_Team",
                        column: x => x.TeamID,
                        principalTable: "Team",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review_response",
                columns: table => new
                {
                    response_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    review_id = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    user_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review_r__EBECD896AE0A90A3", x => x.response_id);
                    table.ForeignKey(
                        name: "FK_RR_Review",
                        column: x => x.review_id,
                        principalTable: "Course_review",
                        principalColumn: "review_id");
                    table.ForeignKey(
                        name: "FK_ReviewResponse_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Lecture",
                columns: table => new
                {
                    lecture_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lesson_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    duration_minutes = table.Column<int>(type: "int", nullable: true),
                    preview_available = table.Column<bool>(type: "bit", nullable: false),
                    content_url = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Lecture__797827F5F7150BD8", x => x.lecture_id);
                    table.ForeignKey(
                        name: "FK_Lecture_Lesson",
                        column: x => x.lesson_id,
                        principalTable: "Lesson",
                        principalColumn: "lesson_id");
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comment__E7957687EF0059BF", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_Comment_Parent",
                        column: x => x.ParentId,
                        principalTable: "Comment",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "FK__Comment__guide_i__7755B73D",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                    table.ForeignKey(
                        name: "FK__Comment__user_id__7849DB76",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Favorite",
                columns: table => new
                {
                    favorite_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Favorite__46ACF4CBC8A8E82F", x => x.favorite_id);
                    table.ForeignKey(
                        name: "FK__Favorite__guide___03BB8E22",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                    table.ForeignKey(
                        name: "FK__Favorite__user_i__04AFB25B",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "GuideAccess",
                columns: table => new
                {
                    access_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    granted_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideAcc__10FA1E20F902B0D2", x => x.access_id);
                    table.ForeignKey(
                        name: "FK_GuideAccess_Guide",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                    table.ForeignKey(
                        name: "FK_GuideAccess_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "GuideCategory",
                columns: table => new
                {
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideCat__59FCCCDAA35C61DF", x => new { x.guide_id, x.category_id });
                    table.ForeignKey(
                        name: "FK__GuideCate__categ__0A688BB1",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK__GuideCate__guide__0B5CAFEA",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                });

            migrationBuilder.CreateTable(
                name: "GuidePreview",
                columns: table => new
                {
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    video_available = table.Column<bool>(type: "bit", nullable: false),
                    video_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuidePre__04A82241C2B2888D", x => x.guide_id);
                    table.ForeignKey(
                        name: "FK_GuidePreview_Guide",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                });

            migrationBuilder.CreateTable(
                name: "GuidePromoPhoto",
                columns: table => new
                {
                    photo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuidePro__CB48C83D6EF2E7E0", x => x.photo_id);
                    table.ForeignKey(
                        name: "FK_GuidePromoPhoto_Guide",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                });

            migrationBuilder.CreateTable(
                name: "GuideRating",
                columns: table => new
                {
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideRating", x => new { x.guide_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_GuideRating_Guide",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                    table.ForeignKey(
                        name: "FK_GuideRating_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "GuideRequirement",
                columns: table => new
                {
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    paper_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    paper_size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    colors = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    tools = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideReq__04A8224125551C7D", x => x.guide_id);
                    table.ForeignKey(
                        name: "FK_GuideRequirement_Guide",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                });

            migrationBuilder.CreateTable(
                name: "GuideView",
                columns: table => new
                {
                    view_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    viewed_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GuideVie__B5A34EE2EF77198C", x => x.view_id);
                    table.ForeignKey(
                        name: "FK_GuideView_Guide",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                    table.ForeignKey(
                        name: "FK_GuideView_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Revenue",
                columns: table => new
                {
                    revenue_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    guide_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Revenue__3DF902E9B703666A", x => x.revenue_id);
                    table.ForeignKey(
                        name: "FK__Revenue__course___22401542",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Revenue__guide_i__2334397B",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                    table.ForeignKey(
                        name: "FK__Revenue__user_id__24285DB4",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Step",
                columns: table => new
                {
                    step_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guide_id = table.Column<int>(type: "int", nullable: false),
                    step_number = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    video_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Step__B2E1DE81285B81E5", x => x.step_id);
                    table.ForeignKey(
                        name: "FK__Step__guide_id__29E1370A",
                        column: x => x.guide_id,
                        principalTable: "Guide",
                        principalColumn: "guide_id");
                });

            migrationBuilder.CreateTable(
                name: "Score",
                columns: table => new
                {
                    ScoreID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionID = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ScoreAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ScoreBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Score__7DD229F13E8EE837", x => x.ScoreID);
                    table.ForeignKey(
                        name: "FK_Score_Submission",
                        column: x => x.SubmissionID,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID");
                    table.ForeignKey(
                        name: "FK_Score_User",
                        column: x => x.ScoreBy,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionComment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__E7957687C7B68FC9", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_SubmissionComment_Parent",
                        column: x => x.parent_id,
                        principalTable: "SubmissionComment",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "FK_SubmissionComment_Submission",
                        column: x => x.submission_id,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID");
                    table.ForeignKey(
                        name: "FK_SubmissionComment_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionFoldingDetail",
                columns: table => new
                {
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    paper_size = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    paper_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    complexity = table.Column<int>(type: "int", nullable: true),
                    folding_time_minute = table.Column<int>(type: "int", nullable: true),
                    source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    original_designer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__9B535595100D5A97", x => x.submission_id);
                    table.ForeignKey(
                        name: "FK__Submissio__submi__318258D2",
                        column: x => x.submission_id,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionImage",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    submission_id = table.Column<int>(type: "int", nullable: true),
                    url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    thumbnail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    display_order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__DC9AC95578981537", x => x.image_id);
                    table.ForeignKey(
                        name: "FK__Submissio__submi__32767D0B",
                        column: x => x.submission_id,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionLike",
                columns: table => new
                {
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionLike", x => new { x.submission_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_SubmissionLike_Submission",
                        column: x => x.submission_id,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID");
                    table.ForeignKey(
                        name: "FK_SubmissionLike_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionView",
                columns: table => new
                {
                    view_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    viewed_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Submissi__B5A34EE292D8AF25", x => x.view_id);
                    table.ForeignKey(
                        name: "FK_SubmissionView_Submission",
                        column: x => x.submission_id,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID");
                    table.ForeignKey(
                        name: "FK_SubmissionView_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    vote_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    submission_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    voted_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vote__9F5405AE911B0D52", x => x.vote_id);
                    table.ForeignKey(
                        name: "FK_Vote_Submission",
                        column: x => x.submission_id,
                        principalTable: "Submission",
                        principalColumn: "SubmissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vote_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Lecture_progress",
                columns: table => new
                {
                    lecture_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    is_completed = table.Column<bool>(type: "bit", nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecture_progress", x => new { x.lecture_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_LP_Lecture",
                        column: x => x.lecture_id,
                        principalTable: "Lecture",
                        principalColumn: "lecture_id");
                    table.ForeignKey(
                        name: "FK_LP_User",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    question_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    target_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Course"),
                    lecture_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__2EC215497CA82362", x => x.question_id);
                    table.ForeignKey(
                        name: "FK_Question_Lecture",
                        column: x => x.lecture_id,
                        principalTable: "Lecture",
                        principalColumn: "lecture_id");
                    table.ForeignKey(
                        name: "FK__Question__course__1D7B6025",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__Question__user_i__1E6F845E",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    resource_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lecture_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    url = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Resource__4985FC73F5A81035", x => x.resource_id);
                    table.ForeignKey(
                        name: "FK_Resource_Lecture",
                        column: x => x.lecture_id,
                        principalTable: "Lecture",
                        principalColumn: "lecture_id");
                });

            migrationBuilder.CreateTable(
                name: "Commission",
                columns: table => new
                {
                    commission_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    revenue_id = table.Column<int>(type: "int", nullable: true),
                    percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Commissi__D19D7CC92309F437", x => x.commission_id);
                    table.ForeignKey(
                        name: "FK__Commissio__reven__7A3223E8",
                        column: x => x.revenue_id,
                        principalTable: "Revenue",
                        principalColumn: "revenue_id");
                });

            migrationBuilder.CreateTable(
                name: "StepTip",
                columns: table => new
                {
                    tip_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    step_id = table.Column<int>(type: "int", nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StepTip__377877B2931E3198", x => x.tip_id);
                    table.ForeignKey(
                        name: "FK_StepTip_Step",
                        column: x => x.step_id,
                        principalTable: "Step",
                        principalColumn: "step_id");
                });

            migrationBuilder.CreateTable(
                name: "ScoreCriteria",
                columns: table => new
                {
                    score_id = table.Column<int>(type: "int", nullable: false),
                    creativity = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    execution = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    theme = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    difficulty = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    presentation = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ScoreCri__8CA1905016540416", x => x.score_id);
                    table.ForeignKey(
                        name: "FK_ScoreCriteria_Score",
                        column: x => x.score_id,
                        principalTable: "Score",
                        principalColumn: "ScoreID");
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    answer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Answer__33724318AE61C471", x => x.answer_id);
                    table.ForeignKey(
                        name: "FK__Answer__question__690797E6",
                        column: x => x.question_id,
                        principalTable: "Question",
                        principalColumn: "question_id");
                    table.ForeignKey(
                        name: "FK__Answer__user_id__69FBBC1F",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_question_id",
                table: "Answer",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_user_id",
                table: "Answer",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Challenge_created_by",
                table: "Challenge",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeCategory_category_id",
                table: "ChallengeCategory",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeJudge_user_id",
                table: "ChallengeJudge",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeOtherRequirement_challenge_id",
                table: "ChallengeOtherRequirement",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengePrize_challenge_id",
                table: "ChallengePrize",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengePrizeBadge_badge_id",
                table: "ChallengePrizeBadge",
                column: "badge_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRule_challenge_id",
                table: "ChallengeRule",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRuleItem_rule_id",
                table: "ChallengeRuleItem",
                column: "rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_guide_id",
                table: "Comment",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ParentId",
                table: "Comment",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_user_id",
                table: "Comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Commission_revenue_id",
                table: "Commission",
                column: "revenue_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_teacher_id",
                table: "Course",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_access_course_id",
                table: "Course_access",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_access_learner_id",
                table: "Course_access",
                column: "learner_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_category_category_id",
                table: "Course_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_review_course_id",
                table: "Course_review",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_review_user_id",
                table: "Course_review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_target_level_level_id",
                table: "Course_target_level",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_guide_id",
                table: "Favorite",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "UX_Favorite_User_Guide",
                table: "Favorite",
                columns: new[] { "user_id", "guide_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guide_author_id",
                table: "Guide",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Guide_OrigamiID",
                table: "Guide",
                column: "OrigamiID");

            migrationBuilder.CreateIndex(
                name: "IX_GuideAccess_guide_id",
                table: "GuideAccess",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Guide",
                table: "GuideAccess",
                columns: new[] { "user_id", "guide_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuideCategory_category_id",
                table: "GuideCategory",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuidePromoPhoto_guide_id",
                table: "GuidePromoPhoto",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRating_user_id",
                table: "GuideRating",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_GuideRating",
                table: "GuideRating",
                columns: new[] { "guide_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuideView_guide_id",
                table: "GuideView",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuideView_user_id",
                table: "GuideView",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_ChallengeID",
                table: "Leaderboard",
                column: "ChallengeID");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_TeamID",
                table: "Leaderboard",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_UserID",
                table: "Leaderboard",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_lesson_id",
                table: "Lecture",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_progress_user_id",
                table: "Lecture_progress",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_course_id",
                table: "Lesson",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_user_id",
                table: "Notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_order_id",
                table: "OrderItem",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Buyer",
                table: "Orders",
                column: "buyer_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Origami_CreatedBy",
                table: "Origami",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Question_course_id",
                table: "Question",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Question_lecture_id",
                table: "Question",
                column: "lecture_id");

            migrationBuilder.CreateIndex(
                name: "IX_Question_user_id",
                table: "Question",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Refresh_token_user_id",
                table: "Refresh_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_lecture_id",
                table: "Resource",
                column: "lecture_id");

            migrationBuilder.CreateIndex(
                name: "IX_Revenue_course_id",
                table: "Revenue",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Revenue_guide_id",
                table: "Revenue",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_Revenue_user_id",
                table: "Revenue",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_response_review_id",
                table: "Review_response",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_response_user_id",
                table: "Review_response",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Score_ScoreBy",
                table: "Score",
                column: "ScoreBy");

            migrationBuilder.CreateIndex(
                name: "IX_Score_SubmissionID",
                table: "Score",
                column: "SubmissionID");

            migrationBuilder.CreateIndex(
                name: "IX_Step_guide_id",
                table: "Step",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_StepTip_step_id",
                table: "StepTip",
                column: "step_id");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ChallengeID",
                table: "Submission",
                column: "ChallengeID");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_SubmittedBy",
                table: "Submission",
                column: "SubmittedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_TeamID",
                table: "Submission",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionComment_parent_id",
                table: "SubmissionComment",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionComment_submission_id",
                table: "SubmissionComment",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionComment_user_id",
                table: "SubmissionComment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionImage_submission_id",
                table: "SubmissionImage",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionLike_user_id",
                table: "SubmissionLike",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSnapshot_Challenge_Rank",
                table: "SubmissionSnapshot",
                columns: new[] { "challenge_id", "rank" });

            migrationBuilder.CreateIndex(
                name: "UX_SubmissionSnapshot_Challenge_Submission",
                table: "SubmissionSnapshot",
                columns: new[] { "challenge_id", "submission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionView_submission_id",
                table: "SubmissionView",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionView_user_id",
                table: "SubmissionView",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Team_ChallengeID",
                table: "Team",
                column: "ChallengeID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_UserID",
                table: "TeamMember",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ_TeamMember",
                table: "TeamMember",
                columns: new[] { "TeamID", "UserID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ticket_type_id",
                table: "Ticket",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_user_id",
                table: "Ticket",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_OrderId",
                table: "Transaction",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_receiver_wallet_id",
                table: "Transaction",
                column: "receiver_wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_sender_wallet_id",
                table: "Transaction",
                column: "sender_wallet_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_role_id",
                table: "User",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ__User__AB6E6164DF88A60C",
                table: "User",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_badge_badge_id",
                table: "User_badge",
                column: "badge_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Vote",
                table: "Vote",
                columns: new[] { "submission_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Vote_User_Submission",
                table: "Vote",
                columns: new[] { "user_id", "submission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_user_id",
                table: "Wallet",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "ChallengeCategory");

            migrationBuilder.DropTable(
                name: "ChallengeJudge");

            migrationBuilder.DropTable(
                name: "ChallengeOtherRequirement");

            migrationBuilder.DropTable(
                name: "ChallengePrizeBadge");

            migrationBuilder.DropTable(
                name: "ChallengeRequirement");

            migrationBuilder.DropTable(
                name: "ChallengeRuleItem");

            migrationBuilder.DropTable(
                name: "ChallengeSchedule");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Commission");

            migrationBuilder.DropTable(
                name: "Course_access");

            migrationBuilder.DropTable(
                name: "Course_category");

            migrationBuilder.DropTable(
                name: "Course_target_level");

            migrationBuilder.DropTable(
                name: "Favorite");

            migrationBuilder.DropTable(
                name: "GuideAccess");

            migrationBuilder.DropTable(
                name: "GuideCategory");

            migrationBuilder.DropTable(
                name: "GuidePreview");

            migrationBuilder.DropTable(
                name: "GuidePromoPhoto");

            migrationBuilder.DropTable(
                name: "GuideRating");

            migrationBuilder.DropTable(
                name: "GuideRequirement");

            migrationBuilder.DropTable(
                name: "GuideView");

            migrationBuilder.DropTable(
                name: "Leaderboard");

            migrationBuilder.DropTable(
                name: "Lecture_progress");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Refresh_token");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "Review_response");

            migrationBuilder.DropTable(
                name: "ScoreCriteria");

            migrationBuilder.DropTable(
                name: "StepTip");

            migrationBuilder.DropTable(
                name: "SubmissionComment");

            migrationBuilder.DropTable(
                name: "SubmissionFoldingDetail");

            migrationBuilder.DropTable(
                name: "SubmissionImage");

            migrationBuilder.DropTable(
                name: "SubmissionLike");

            migrationBuilder.DropTable(
                name: "SubmissionSnapshot");

            migrationBuilder.DropTable(
                name: "SubmissionView");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "User_badge");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "Vote");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "ChallengePrize");

            migrationBuilder.DropTable(
                name: "ChallengeRule");

            migrationBuilder.DropTable(
                name: "Revenue");

            migrationBuilder.DropTable(
                name: "Target_level");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Course_review");

            migrationBuilder.DropTable(
                name: "Score");

            migrationBuilder.DropTable(
                name: "Step");

            migrationBuilder.DropTable(
                name: "Ticket_type");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "Badge");

            migrationBuilder.DropTable(
                name: "Lecture");

            migrationBuilder.DropTable(
                name: "Submission");

            migrationBuilder.DropTable(
                name: "Guide");

            migrationBuilder.DropTable(
                name: "Lesson");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Origami");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "Challenge");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
