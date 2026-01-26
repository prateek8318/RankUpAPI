using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestService.Infrastructure.Migrations
{
    public partial class CreateTestServiceTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PracticeModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectMasters_ExamMasters_ExamId",
                        column: x => x.ExamId,
                        principalTable: "ExamMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 60),
                    TotalMarks = table.Column<int>(type: "int", nullable: false, defaultValue: 100),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PassingMarks = table.Column<int>(type: "int", nullable: false, defaultValue: 35),
                    InstructionsEnglish = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InstructionsHindi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSeries_ExamMasters_ExamId",
                        column: x => x.ExamId,
                        principalTable: "ExamMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Difficulty = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    Marks = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    PracticeModeId = table.Column<int>(type: "int", nullable: false),
                    SeriesId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 60),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalMarks = table.Column<int>(type: "int", nullable: false, defaultValue: 100),
                    PassingMarks = table.Column<int>(type: "int", nullable: false, defaultValue: 35),
                    InstructionsEnglish = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InstructionsHindi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_ExamMasters_ExamId",
                        column: x => x.ExamId,
                        principalTable: "ExamMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_PracticeModes_PracticeModeId",
                        column: x => x.PracticeModeId,
                        principalTable: "PracticeModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_SubjectMasters_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SubjectMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tests_TestSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "TestSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Marks = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestQuestions_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTestAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentQuestionIndex = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Score = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalMarks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Accuracy = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AnswersJson = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTestAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTestAttempts_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PracticeModes",
                columns: new[] { "Id", "Name", "Description", "DisplayOrder", "IsFeatured", "IsActive" },
                values: new object[,]
                {
                    { 3, "Mock Test", "Full-length mock tests", 1, true, true },
                    { 4, "Test Series", "Series of practice tests", 2, true, true },
                    { 5, "Deep Practice", "Subject-wise focused practice", 3, true, true },
                    { 6, "Previous Year", "Previous year question papers", 4, true, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamMasters_IsActive",
                table: "ExamMasters",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeModes_IsActive",
                table: "PracticeModes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_IsActive",
                table: "Questions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMasters_ExamId",
                table: "SubjectMasters",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectMasters_IsActive",
                table: "SubjectMasters",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestions_IsActive",
                table: "TestQuestions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestions_QuestionId",
                table: "TestQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestions_TestId",
                table: "TestQuestions",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSeries_ExamId",
                table: "TestSeries",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSeries_IsActive",
                table: "TestSeries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_ExamId",
                table: "Tests",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_IsActive",
                table: "Tests",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_PracticeModeId",
                table: "Tests",
                column: "PracticeModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_SeriesId",
                table: "Tests",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_SubjectId",
                table: "Tests",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_Year",
                table: "Tests",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestAttempts_IsActive",
                table: "UserTestAttempts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestAttempts_Status",
                table: "UserTestAttempts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestAttempts_TestId",
                table: "UserTestAttempts",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTestAttempts_UserId",
                table: "UserTestAttempts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTestAttempts");

            migrationBuilder.DropTable(
                name: "TestQuestions");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "TestSeries");

            migrationBuilder.DropTable(
                name: "SubjectMasters");

            migrationBuilder.DropTable(
                name: "PracticeModes");

            migrationBuilder.DropTable(
                name: "ExamMasters");
        }
    }
}
