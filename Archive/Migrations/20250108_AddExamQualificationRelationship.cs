using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace RankUpAPI.Migrations
{
    public partial class AddExamQualificationRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing foreign key constraint and column
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Qualifications_QualificationId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_QualificationId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "QualificationId",
                table: "Exams");

            // Create the join table
            migrationBuilder.CreateTable(
                name: "ExamQualifications",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    QualificationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQualifications", x => new { x.ExamId, x.QualificationId });
                    table.ForeignKey(
                        name: "FK_ExamQualifications_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQualifications_Qualifications_QualificationId",
                        column: x => x.QualificationId,
                        principalTable: "Qualifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamQualifications_QualificationId",
                table: "ExamQualifications",
                column: "QualificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamQualifications");

            migrationBuilder.AddColumn<int>(
                name: "QualificationId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_QualificationId",
                table: "Exams",
                column: "QualificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Qualifications_QualificationId",
                table: "Exams",
                column: "QualificationId",
                principalTable: "Qualifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
