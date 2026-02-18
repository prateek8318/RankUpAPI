using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterService.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCountryAndStateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubdivisionLabelEn",
                table: "Countries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubdivisionLabelHi",
                table: "Countries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubdivisionLabelEn",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "SubdivisionLabelHi",
                table: "Countries");
        }
    }
}
