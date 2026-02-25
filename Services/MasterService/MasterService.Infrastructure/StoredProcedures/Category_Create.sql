-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to create new category
-- =============================================
CREATE PROCEDURE [dbo].[Category_Create]
    @NameEn NVARCHAR(100),
    @NameHi NVARCHAR(100),
    @Key NVARCHAR(50),
    @Type NVARCHAR(50),
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO Categories (
            NameEn, NameHi, Key, Type, IsActive, CreatedAt, UpdatedAt
        )
        VALUES (
            @NameEn, @NameHi, @Key, @Type, @IsActive, @CreatedAt, @UpdatedAt
        );
        
        SELECT SCOPE_IDENTITY() AS CategoryId, 'Category created successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS CategoryId, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
