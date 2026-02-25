using Microsoft.EntityFrameworkCore.Migrations;

namespace MasterService.Infrastructure.Migrations
{
    public partial class AddCategoryStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Category_GetAll
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_GetAll]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[Category_GetAll]
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT 
                            Id, NameEn, NameHi, [Key], Type, IsActive, CreatedAt, UpdatedAt
                        FROM [Categories]
                        ORDER BY NameEn;
                    END;');
                END");

            // Category_GetById
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_GetById]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[Category_GetById]
                        @Id INT
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT 
                            Id, NameEn, NameHi, [Key], Type, IsActive, CreatedAt, UpdatedAt
                        FROM [Categories]
                        WHERE Id = @Id;
                    END;');
                END");

            // Category_GetActive
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_GetActive]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[Category_GetActive]
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT 
                            Id, NameEn, NameHi, [Key], Type, IsActive, CreatedAt, UpdatedAt
                        FROM [Categories]
                        WHERE IsActive = 1
                        ORDER BY NameEn;
                    END;');
                END");

            // Category_GetActiveByType
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_GetActiveByType]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[Category_GetActiveByType]
                        @Type NVARCHAR(50)
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        SELECT 
                            Id, NameEn, NameHi, [Key], Type, IsActive, CreatedAt, UpdatedAt
                        FROM [Categories]
                        WHERE IsActive = 1 AND Type = @Type
                        ORDER BY NameEn;
                    END;');
                END");

            // Category_Create
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Create]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[Category_Create]
                        @NameEn NVARCHAR(100),
                        @NameHi NVARCHAR(100),
                        @Key NVARCHAR(50),
                        @Type NVARCHAR(50),
                        @IsActive BIT,
                        @CreatedAt DATETIME2,
                        @UpdatedAt DATETIME2,
                        @Id INT OUTPUT
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        INSERT INTO [Categories] (NameEn, NameHi, Key, Type, IsActive, CreatedAt, UpdatedAt)
                        VALUES (@NameEn, @NameHi, @Key, @Type, @IsActive, @CreatedAt, @UpdatedAt);
                        
                        SET @Id = SCOPE_IDENTITY();
                    END;');
                END");

            // Category_Update
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Update]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[Category_Update]
                        @Id INT,
                        @NameEn NVARCHAR(100),
                        @NameHi NVARCHAR(100),
                        @Key NVARCHAR(50),
                        @Type NVARCHAR(50),
                        @IsActive BIT,
                        @UpdatedAt DATETIME2
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        UPDATE [Categories]
                        SET 
                            NameEn = ISNULL(@NameEn, NameEn),
                            NameHi = ISNULL(@NameHi, NameHi),
                            Key = ISNULL(@Key, Key),
                            Type = ISNULL(@Type, Type),
                            IsActive = ISNULL(@IsActive, IsActive),
                            UpdatedAt = @UpdatedAt
                        WHERE Id = @Id;
                    END;');
                END");

            // Category_Delete
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Delete]') AND type in (N'P', N'PC'))
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[Category_Delete]
                        @Id INT
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        
                        DELETE FROM [Categories]
                        WHERE Id = @Id;
                    END;');
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[Category_GetAll]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[Category_GetById]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[Category_GetActive]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[Category_GetActiveByType]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[Category_Create]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[Category_Update]");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[Category_Delete]");
        }
    }
}
