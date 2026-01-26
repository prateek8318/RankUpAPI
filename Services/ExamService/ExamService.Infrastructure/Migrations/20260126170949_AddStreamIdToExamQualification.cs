using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamIdToExamQualification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StreamId",
                table: "ExamQualifications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamId",
                table: "ExamQualifications");
        }
    }
}
