using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StreamId",
                table: "Qualifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Streams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_StreamId",
                table: "Qualifications",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_Name",
                table: "Streams",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Qualifications_Streams_StreamId",
                table: "Qualifications",
                column: "StreamId",
                principalTable: "Streams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Qualifications_Streams_StreamId",
                table: "Qualifications");

            migrationBuilder.DropTable(
                name: "Streams");

            migrationBuilder.DropIndex(
                name: "IX_Qualifications_StreamId",
                table: "Qualifications");

            migrationBuilder.DropColumn(
                name: "StreamId",
                table: "Qualifications");
        }
    }
}
