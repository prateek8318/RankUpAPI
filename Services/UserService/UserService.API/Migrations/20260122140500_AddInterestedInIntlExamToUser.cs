using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestedInIntlExamToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InterestedInIntlExam",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestedInIntlExam",
                table: "Users");
        }
    }
}
