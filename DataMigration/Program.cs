using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Origami.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DataMigration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Origami Data Migration Tool ===");
            Console.WriteLine("Migrating data from Local DB to MonsterASP.NET...\n");

            // Setup configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Connection strings
            var localConnectionString = "Server=.;Database=OrigamiDB;User Id=sa;Password=1;TrustServerCertificate=True;";
            var remoteConnectionString = configuration.GetConnectionString("DefaultConnection");

            // Setup DbContexts
            var localOptions = new DbContextOptionsBuilder<OrigamiDbContext>()
                .UseSqlServer(localConnectionString)
                .Options;

            var remoteOptions = new DbContextOptionsBuilder<OrigamiDbContext>()
                .UseSqlServer(remoteConnectionString)
                .Options;

            using var localContext = new OrigamiDbContext(localOptions);
            using var remoteContext = new OrigamiDbContext(remoteOptions);

            try
            {
                // Test connections
                Console.WriteLine("Testing connections...");
                await localContext.Database.CanConnectAsync();
                Console.WriteLine("✓ Local DB connected");
                await remoteContext.Database.CanConnectAsync();
                Console.WriteLine("✓ Remote DB connected\n");

                // Disable foreign key checks temporarily (if needed)
                // Note: SQL Server doesn't support disabling FK checks easily, so we'll migrate in order

                // Migration order: Tables without dependencies first, then dependent tables
                Console.WriteLine("Starting data migration...\n");

                // 1. Roles (no dependencies)
                await MigrateTable<Role>(localContext, remoteContext, "Role");

                // 2. Users (depends on Role)
                await MigrateTable<User>(localContext, remoteContext, "User");

                // 3. UserProfiles (depends on User)
                await MigrateTable<UserProfile>(localContext, remoteContext, "UserProfile");

                // 4. Categories (no dependencies)
                await MigrateTable<Category>(localContext, remoteContext, "Category");

                // 5. Badges (no dependencies)
                await MigrateTable<Badge>(localContext, remoteContext, "Badge");

                // 6. TargetLevels (no dependencies)
                await MigrateTable<TargetLevel>(localContext, remoteContext, "TargetLevel");

                // 7. TicketTypes (no dependencies)
                await MigrateTable<TicketType>(localContext, remoteContext, "TicketType");

                // 8. Origamis (depends on User)
                await MigrateTable<Origami.DataTier.Models.Origami>(localContext, remoteContext, "Origami");

                // 9. Guides (depends on User, Origami)
                await MigrateTable<Guide>(localContext, remoteContext, "Guide");

                // 9a. GuideCategory (many-to-many: Guide-Category)
                await MigrateJunctionTable(localContext, remoteContext, "GuideCategory", "guide_id", "category_id");

                // 10. GuidePreviews (depends on Guide)
                await MigrateTable<GuidePreview>(localContext, remoteContext, "GuidePreview");

                // 11. GuidePromoPhotos (depends on Guide)
                await MigrateTable<GuidePromoPhoto>(localContext, remoteContext, "GuidePromoPhoto");

                // 12. GuideRequirements (depends on Guide)
                await MigrateTable<GuideRequirement>(localContext, remoteContext, "GuideRequirement");

                // 13. Steps (depends on Guide)
                await MigrateTable<Step>(localContext, remoteContext, "Step");

                // 14. StepTips (depends on Step)
                await MigrateTable<StepTip>(localContext, remoteContext, "StepTip");

                // 15. Courses (depends on User)
                await MigrateTable<Course>(localContext, remoteContext, "Course");

                // 15a. CourseCategory (many-to-many: Course-Category)
                await MigrateJunctionTable(localContext, remoteContext, "Course_category", "course_id", "category_id");

                // 15b. CourseLevel (many-to-many: Course-TargetLevel)
                await MigrateJunctionTable(localContext, remoteContext, "CourseLevel", "course_id", "level_id");

                // 16. Lessons (depends on Course)
                await MigrateTable<Lesson>(localContext, remoteContext, "Lesson");

                // 17. Lectures (depends on Lesson)
                await MigrateTable<Lecture>(localContext, remoteContext, "Lecture");

                // 18. Resources (depends on Lecture)
                await MigrateTable<Resource>(localContext, remoteContext, "Resource");

                // 19. Challenges (depends on User, Category)
                await MigrateTable<Challenge>(localContext, remoteContext, "Challenge");

                // 20. ChallengeSchedules (depends on Challenge)
                await MigrateTable<ChallengeSchedule>(localContext, remoteContext, "ChallengeSchedule");

                // 21. ChallengeRules (depends on Challenge)
                await MigrateTable<ChallengeRule>(localContext, remoteContext, "ChallengeRule");

                // 22. ChallengeRuleItems (depends on ChallengeRule)
                await MigrateTable<ChallengeRuleItem>(localContext, remoteContext, "ChallengeRuleItem");

                // 23. ChallengeRequirements (depends on Challenge)
                await MigrateTable<ChallengeRequirement>(localContext, remoteContext, "ChallengeRequirement");

                // 24. ChallengePrizes (depends on Challenge)
                await MigrateTable<ChallengePrize>(localContext, remoteContext, "ChallengePrize");

                // 25. ChallengeOtherRequirements (depends on Challenge)
                await MigrateTable<ChallengeOtherRequirement>(localContext, remoteContext, "ChallengeOtherRequirement");

                // 26. Teams (depends on Challenge, User)
                await MigrateTable<Team>(localContext, remoteContext, "Team");

                // 27. TeamMembers (depends on Team, User)
                await MigrateTable<TeamMember>(localContext, remoteContext, "TeamMember");

                // 28. Submissions (depends on Challenge, User, Team)
                await MigrateTable<Submission>(localContext, remoteContext, "Submission");

                // 29. SubmissionImages (depends on Submission)
                await MigrateTable<SubmissionImage>(localContext, remoteContext, "SubmissionImage");

                // 30. SubmissionFoldingDetails (depends on Submission)
                await MigrateTable<SubmissionFoldingDetail>(localContext, remoteContext, "SubmissionFoldingDetail");

                // 31. SubmissionSnapshots (depends on Submission)
                await MigrateTable<SubmissionSnapshot>(localContext, remoteContext, "SubmissionSnapshot");

                // 32. SubmissionComments (depends on Submission, User)
                await MigrateTable<SubmissionComment>(localContext, remoteContext, "SubmissionComment");

                // 33. SubmissionViews (depends on Submission, User)
                await MigrateTable<SubmissionView>(localContext, remoteContext, "SubmissionView");

                // 34. SubmissionLikes (depends on Submission, User)
                await MigrateTable<SubmissionLike>(localContext, remoteContext, "SubmissionLike");

                // 35. Scores (depends on Submission, User)
                await MigrateTable<Score>(localContext, remoteContext, "Score");

                // 36. ScoreCriteria (depends on Score)
                await MigrateTable<ScoreCriterion>(localContext, remoteContext, "ScoreCriterion");

                // 37. Votes (depends on Submission, User)
                await MigrateTable<Vote>(localContext, remoteContext, "Vote");

                // 38. Leaderboards (depends on Challenge, User, Team)
                await MigrateTable<Leaderboard>(localContext, remoteContext, "Leaderboard");

                // 39. Comments (depends on Guide, User)
                await MigrateTable<Comment>(localContext, remoteContext, "Comment");

                // 40. Favorites (depends on User, Guide)
                await MigrateTable<Favorite>(localContext, remoteContext, "Favorite");

                // 41. GuideAccesses (depends on User, Guide)
                await MigrateTable<GuideAccess>(localContext, remoteContext, "GuideAccess");

                // 42. GuideRatings (depends on User, Guide)
                await MigrateTable<GuideRating>(localContext, remoteContext, "GuideRating");

                // 43. GuideViews (depends on User, Guide)
                await MigrateTable<GuideView>(localContext, remoteContext, "GuideView");

                // 44. CourseAccesses (depends on User, Course)
                await MigrateTable<CourseAccess>(localContext, remoteContext, "CourseAccess");

                // 45. CourseReviews (depends on User, Course)
                await MigrateTable<CourseReview>(localContext, remoteContext, "CourseReview");

                // 46. ReviewResponses (depends on CourseReview, User)
                await MigrateTable<ReviewResponse>(localContext, remoteContext, "ReviewResponse");

                // 47. LectureProgresses (depends on User, Lecture)
                await MigrateTable<LectureProgress>(localContext, remoteContext, "LectureProgress");

                // 48. Questions (depends on User, Course, Lecture)
                await MigrateTable<Question>(localContext, remoteContext, "Question");

                // 49. Answers (depends on User, Question)
                await MigrateTable<Answer>(localContext, remoteContext, "Answer");

                // 50. Orders (depends on User)
                await MigrateTable<Order>(localContext, remoteContext, "Order");

                // 51. OrderItems (depends on Order)
                await MigrateTable<OrderItem>(localContext, remoteContext, "OrderItem");

                // 52. Transactions (depends on Order)
                await MigrateTable<Transaction>(localContext, remoteContext, "Transaction");

                // 53. Revenues (depends on Guide, User)
                await MigrateTable<Revenue>(localContext, remoteContext, "Revenue");

                // 54. Commissions (depends on User)
                await MigrateTable<Commission>(localContext, remoteContext, "Commission");

                // 55. Wallets (depends on User)
                await MigrateTable<Wallet>(localContext, remoteContext, "Wallet");

                // 56. Tickets (depends on User, TicketType)
                await MigrateTable<Ticket>(localContext, remoteContext, "Ticket");

                // 57. UserBadges (depends on User, Badge)
                await MigrateTable<UserBadge>(localContext, remoteContext, "UserBadge");

                // 58. Notifications (depends on User)
                await MigrateTable<Notification>(localContext, remoteContext, "Notification");

                // 59. RefreshTokens (depends on User)
                await MigrateTable<RefreshToken>(localContext, remoteContext, "RefreshToken");

                Console.WriteLine("\n=== Migration Completed Successfully! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n!!! ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        static async Task MigrateJunctionTable(OrigamiDbContext sourceContext, OrigamiDbContext targetContext, string tableName, string key1Column, string key2Column)
        {
            try
            {
                // Check if table has data in target
                var checkSql = $"SELECT COUNT(*) FROM {tableName}";
                var existingCount = 0;
                try
                {
                    existingCount = await targetContext.Database.ExecuteSqlRawAsync(checkSql);
                }
                catch
                {
                    // Table might not exist, that's ok
                }

                if (existingCount > 0)
                {
                    Console.WriteLine($"  {tableName}: {existingCount} records already exist, skipping...");
                    return;
                }

                // Get data from source using raw SQL
                var sourceCount = 0;
                try
                {
                    sourceCount = await sourceContext.Database.ExecuteSqlRawAsync($"SELECT COUNT(*) FROM {tableName}");
                }
                catch
                {
                    Console.WriteLine($"  {tableName}: Table not found in source, skipping...");
                    return;
                }

                if (sourceCount == 0)
                {
                    Console.WriteLine($"  {tableName}: No data to migrate");
                    return;
                }

                // Use INSERT INTO ... SELECT to copy data
                var insertSql = $@"
                    INSERT INTO {tableName} ({key1Column}, {key2Column})
                    SELECT {key1Column}, {key2Column}
                    FROM {tableName}";

                // Since we can't directly query from another database, we'll use a workaround
                // Read from source and insert into target
                var connection = sourceContext.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT {key1Column}, {key2Column} FROM {tableName}";
                using var reader = await command.ExecuteReaderAsync();

                var records = new List<(int key1, int key2)>();
                while (await reader.ReadAsync())
                {
                    records.Add((reader.GetInt32(0), reader.GetInt32(1)));
                }
                await connection.CloseAsync();

                // Insert into target
                foreach (var (key1, key2) in records)
                {
                    try
                    {
                        await targetContext.Database.ExecuteSqlRawAsync(
                            $"INSERT INTO {tableName} ({key1Column}, {key2Column}) VALUES ({key1}, {key2})");
                    }
                    catch (Exception ex)
                    {
                        // Skip duplicates
                        if (!ex.Message.Contains("PRIMARY KEY") && !ex.Message.Contains("UNIQUE"))
                            throw;
                    }
                }

                await targetContext.SaveChangesAsync();
                Console.WriteLine($"  ✓ {tableName}: {records.Count} records migrated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ {tableName}: ERROR - {ex.Message}");
                // Don't throw for junction tables
            }
        }

        static async Task MigrateTable<T>(OrigamiDbContext sourceContext, OrigamiDbContext targetContext, string tableName) where T : class
        {
            try
            {
                var targetSet = targetContext.Set<T>();

                // Check if data already exists in target
                var existingCount = await targetSet.CountAsync();
                if (existingCount > 0)
                {
                    Console.WriteLine($"  {tableName}: {existingCount} records already exist, skipping...");
                    return;
                }

                // Use raw SQL to copy data
                var sourceConnection = sourceContext.Database.GetDbConnection();
                await sourceConnection.OpenAsync();

                try
                {
                    // Get column information - use brackets for table name
                    var tableNameBracketed = $"[{tableName}]";
                    using var schemaCmd = sourceConnection.CreateCommand();
                    schemaCmd.CommandText = $@"
                        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') as IS_IDENTITY
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = '{tableName}'
                        ORDER BY ORDINAL_POSITION";

                    var columns = new List<(string name, string dataType, bool isNullable, bool isIdentity)>();
                    using var schemaReader = await schemaCmd.ExecuteReaderAsync();
                    while (await schemaReader.ReadAsync())
                    {
                        columns.Add((
                            schemaReader.GetString(0),
                            schemaReader.GetString(1),
                            schemaReader.GetString(2) == "YES",
                            Convert.ToBoolean(schemaReader.GetValue(3) ?? false)
                        ));
                    }

                    if (columns.Count == 0)
                    {
                        Console.WriteLine($"  {tableName}: Could not get table schema, skipping...");
                        return;
                    }

                    // Check source data count - close schema reader first
                    await schemaReader.CloseAsync();
                    
                    using var countCmd = sourceConnection.CreateCommand();
                    countCmd.CommandText = $"SELECT COUNT(*) FROM {tableNameBracketed}";
                    var sourceCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                    
                    if (sourceCount == 0)
                    {
                        Console.WriteLine($"  {tableName}: No data to migrate");
                        return;
                    }

                    // Build column list
                    var columnNames = string.Join(", ", columns.Select(c => $"[{c.name}]"));
                    
                    // Enable IDENTITY_INSERT if there are identity columns
                    var hasIdentity = columns.Any(c => c.isIdentity);
                    
                    var targetConnection = targetContext.Database.GetDbConnection();
                    await targetConnection.OpenAsync();

                    try
                    {
                        if (hasIdentity)
                        {
                            using var identityCmd = targetConnection.CreateCommand();
                            identityCmd.CommandText = $"SET IDENTITY_INSERT {tableNameBracketed} ON";
                            await identityCmd.ExecuteNonQueryAsync();
                        }

                        // Read from source and insert into target using raw SQL
                        using var selectCmd = sourceConnection.CreateCommand();
                        selectCmd.CommandText = $"SELECT {columnNames} FROM {tableNameBracketed}";
                        using var reader = await selectCmd.ExecuteReaderAsync();

                        var insertedCount = 0;
                        while (await reader.ReadAsync())
                        {
                            var values = new List<string>();
                            for (int i = 0; i < columns.Count; i++)
                            {
                                var col = columns[i];
                                if (reader.IsDBNull(i))
                                {
                                    values.Add("NULL");
                                }
                                else
                                {
                                    var value = reader.GetValue(i);
                                    if (value == null || value == DBNull.Value)
                                    {
                                        values.Add("NULL");
                                    }
                                    else if (col.dataType.Contains("bit") || col.dataType == "bit")
                                    {
                                        // Boolean/bit values: convert True/False to 1/0
                                        var boolValue = Convert.ToBoolean(value);
                                        values.Add(boolValue ? "1" : "0");
                                    }
                                    else if (col.dataType.Contains("char") || col.dataType.Contains("text") || col.dataType.Contains("date") || col.dataType.Contains("time") || col.dataType.Contains("nvarchar") || col.dataType.Contains("varchar"))
                                    {
                                        values.Add($"'{value.ToString()?.Replace("'", "''") ?? ""}'");
                                    }
                                    else if (col.dataType.Contains("decimal") || col.dataType.Contains("numeric") || col.dataType.Contains("float") || col.dataType.Contains("money") || col.dataType.Contains("real"))
                                    {
                                        values.Add(value.ToString() ?? "0");
                                    }
                                    else if (col.dataType.Contains("int") || col.dataType.Contains("bigint") || col.dataType.Contains("smallint") || col.dataType.Contains("tinyint"))
                                    {
                                        values.Add(value.ToString() ?? "0");
                                    }
                                    else
                                    {
                                        values.Add(value.ToString() ?? "NULL");
                                    }
                                }
                            }

                            var valuesStr = string.Join(", ", values);
                            using var insertCmd = targetConnection.CreateCommand();
                            insertCmd.CommandText = $"INSERT INTO {tableNameBracketed} ({columnNames}) VALUES ({valuesStr})";
                            
                            try
                            {
                                await insertCmd.ExecuteNonQueryAsync();
                                insertedCount++;
                            }
                            catch (Exception ex)
                            {
                                // Skip duplicates or other errors for individual rows
                                if (!ex.Message.Contains("PRIMARY KEY") && !ex.Message.Contains("UNIQUE"))
                                {
                                    Console.WriteLine($"    Warning: Failed to insert row {insertedCount + 1}: {ex.Message}");
                                }
                            }
                        }

                        if (hasIdentity)
                        {
                            using var identityOffCmd = targetConnection.CreateCommand();
                            identityOffCmd.CommandText = $"SET IDENTITY_INSERT {tableNameBracketed} OFF";
                            await identityOffCmd.ExecuteNonQueryAsync();
                        }

                        Console.WriteLine($"  ✓ {tableName}: {insertedCount} records migrated");
                    }
                    finally
                    {
                        if (hasIdentity)
                        {
                            try
                            {
                                using var identityOffCmd = targetConnection.CreateCommand();
                                identityOffCmd.CommandText = $"SET IDENTITY_INSERT {tableNameBracketed} OFF";
                                await identityOffCmd.ExecuteNonQueryAsync();
                            }
                            catch { }
                        }
                        await targetConnection.CloseAsync();
                    }
                }
                finally
                {
                    await sourceConnection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                // Make sure to turn off IDENTITY_INSERT even on error
                try
                {
                    await targetContext.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {tableName} OFF");
                }
                catch { }

                Console.WriteLine($"  ✗ {tableName}: ERROR - {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"    Inner: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}
