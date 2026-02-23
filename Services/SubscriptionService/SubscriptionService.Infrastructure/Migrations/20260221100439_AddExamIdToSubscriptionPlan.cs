using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExamIdToSubscriptionPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "SubscriptionPlans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_ExamId",
                table: "SubscriptionPlans",
                column: "ExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPlans_ExamId",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "SubscriptionPlans");
        }
    }
}
