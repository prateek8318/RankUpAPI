using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestedInInternationalExamToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInterestedInInternationalExam",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInterestedInInternationalExam",
                table: "Users");
        }
    }
}
