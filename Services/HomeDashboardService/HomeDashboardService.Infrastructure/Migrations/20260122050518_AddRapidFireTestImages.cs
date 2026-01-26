using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeDashboardService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRapidFireTestImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageUrl",
                table: "RapidFireTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "RapidFireTests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImageUrl",
                table: "RapidFireTests");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "RapidFireTests");
        }
    }
}
