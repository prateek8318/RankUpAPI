using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeDashboardService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomePageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContinuePracticeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuizAttemptId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    QuizTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProgressPercentage = table.Column<int>(type: "int", nullable: false),
                    LastAccessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeRemainingSeconds = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContinuePracticeItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TargetQuizzes = table.Column<int>(type: "int", nullable: false),
                    CompletedQuizzes = table.Column<int>(type: "int", nullable: false),
                    TargetMinutes = table.Column<int>(type: "int", nullable: false),
                    CompletedMinutes = table.Column<int>(type: "int", nullable: false),
                    TargetScore = table.Column<int>(type: "int", nullable: false),
                    AchievedScore = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FreeTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    TotalMarks = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreeTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MotivationMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsGreeting = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivationMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PracticeModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RapidFireTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RapidFireTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionBanners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CtaText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShowToSubscribedUsers = table.Column<bool>(type: "bit", nullable: false),
                    ShowToNonSubscribedUsers = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionBanners", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContinuePracticeItems_IsActive",
                table: "ContinuePracticeItems",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ContinuePracticeItems_QuizAttemptId",
                table: "ContinuePracticeItems",
                column: "QuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_ContinuePracticeItems_QuizId",
                table: "ContinuePracticeItems",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_ContinuePracticeItems_UserId",
                table: "ContinuePracticeItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTargets_IsActive",
                table: "DailyTargets",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTargets_TargetDate",
                table: "DailyTargets",
                column: "TargetDate");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTargets_UserId",
                table: "DailyTargets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FreeTests_DisplayOrder",
                table: "FreeTests",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_FreeTests_ExamId",
                table: "FreeTests",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_FreeTests_IsActive",
                table: "FreeTests",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FreeTests_QuizId",
                table: "FreeTests",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_MotivationMessages_DisplayOrder",
                table: "MotivationMessages",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MotivationMessages_IsActive",
                table: "MotivationMessages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MotivationMessages_IsGreeting",
                table: "MotivationMessages",
                column: "IsGreeting");

            migrationBuilder.CreateIndex(
                name: "IX_MotivationMessages_Type",
                table: "MotivationMessages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeModes_DisplayOrder",
                table: "PracticeModes",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeModes_IsActive",
                table: "PracticeModes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeModes_Type",
                table: "PracticeModes",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_RapidFireTests_DisplayOrder",
                table: "RapidFireTests",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_RapidFireTests_IsActive",
                table: "RapidFireTests",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RapidFireTests_QuizId",
                table: "RapidFireTests",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionBanners_DisplayOrder",
                table: "SubscriptionBanners",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionBanners_IsActive",
                table: "SubscriptionBanners",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContinuePracticeItems");

            migrationBuilder.DropTable(
                name: "DailyTargets");

            migrationBuilder.DropTable(
                name: "FreeTests");

            migrationBuilder.DropTable(
                name: "MotivationMessages");

            migrationBuilder.DropTable(
                name: "PracticeModes");

            migrationBuilder.DropTable(
                name: "RapidFireTests");

            migrationBuilder.DropTable(
                name: "SubscriptionBanners");
        }
    }
}
