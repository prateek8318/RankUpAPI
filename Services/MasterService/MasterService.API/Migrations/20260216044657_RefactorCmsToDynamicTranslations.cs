using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterService.API.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCmsToDynamicTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create CmsContentTranslations table first
            migrationBuilder.CreateTable(
                name: "CmsContentTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CmsContentId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CmsContentTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CmsContentTranslations_CmsContents_CmsContentId",
                        column: x => x.CmsContentId,
                        principalTable: "CmsContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CmsContentTranslations_CmsContentId_LanguageCode",
                table: "CmsContentTranslations",
                columns: new[] { "CmsContentId", "LanguageCode" },
                unique: true);

            // 2. Migrate existing data: TitleEn/ContentEn -> en, TitleHi/ContentHi -> hi
            migrationBuilder.Sql(@"
                INSERT INTO CmsContentTranslations (CmsContentId, LanguageCode, Title, Content)
                SELECT Id, 'en', TitleEn, ContentEn FROM CmsContents;
            ");
            migrationBuilder.Sql(@"
                INSERT INTO CmsContentTranslations (CmsContentId, LanguageCode, Title, Content)
                SELECT Id, 'hi', ISNULL(TitleHi, TitleEn), ISNULL(ContentHi, ContentEn)
                FROM CmsContents WHERE TitleHi IS NOT NULL OR ContentHi IS NOT NULL;
            ");
            // If no Hindi data, we still have en. For rows with only en, the above second INSERT won't add hi - that's fine.
            // Actually the second INSERT adds hi only when TitleHi or ContentHi is not null. For rows with both null, we get only en. Good.

            // 3. Drop old columns
            migrationBuilder.DropColumn(name: "ContentEn", table: "CmsContents");
            migrationBuilder.DropColumn(name: "ContentHi", table: "CmsContents");
            migrationBuilder.DropColumn(name: "TitleEn", table: "CmsContents");
            migrationBuilder.DropColumn(name: "TitleHi", table: "CmsContents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add columns before dropping table
            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "CmsContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentHi",
                table: "CmsContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "CmsContents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleHi",
                table: "CmsContents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            // Restore data from translations (en and hi)
            migrationBuilder.Sql(@"
                UPDATE c SET TitleEn = t.Title, ContentEn = t.Content
                FROM CmsContents c
                INNER JOIN CmsContentTranslations t ON t.CmsContentId = c.Id AND t.LanguageCode = 'en';
            ");
            migrationBuilder.Sql(@"
                UPDATE c SET TitleHi = t.Title, ContentHi = t.Content
                FROM CmsContents c
                INNER JOIN CmsContentTranslations t ON t.CmsContentId = c.Id AND t.LanguageCode = 'hi';
            ");

            migrationBuilder.DropTable(name: "CmsContentTranslations");
        }
    }
}
