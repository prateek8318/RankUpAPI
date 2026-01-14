using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionTextEnglish = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    QuestionTextHindi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    QuestionImageUrlEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionImageUrlHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionVideoUrlEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionVideoUrlHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OptionAEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionBEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionCEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionDEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionAHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionBHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionCHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionDHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OptionImageAUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OptionImageBUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OptionImageCUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OptionImageDUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ExplanationEnglish = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExplanationHindi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SolutionImageUrlEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SolutionImageUrlHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SolutionVideoUrlEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SolutionVideoUrlHindi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    Marks = table.Column<int>(type: "int", nullable: false),
                    NegativeMarks = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedTimeInSeconds = table.Column<int>(type: "int", nullable: false),
                    IsMcq = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
