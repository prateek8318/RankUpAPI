using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStateLanguageName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StateLanguages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StateId1",
                table: "StateLanguages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StateLanguages_StateId1",
                table: "StateLanguages",
                column: "StateId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StateLanguages_States_StateId1",
                table: "StateLanguages",
                column: "StateId1",
                principalTable: "States",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StateLanguages_States_StateId1",
                table: "StateLanguages");

            migrationBuilder.DropIndex(
                name: "IX_StateLanguages_StateId1",
                table: "StateLanguages");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StateLanguages");

            migrationBuilder.DropColumn(
                name: "StateId1",
                table: "StateLanguages");
        }
    }
}
