using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsInternationalToExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInternational",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternational",
                table: "Exams");
        }
    }
}
