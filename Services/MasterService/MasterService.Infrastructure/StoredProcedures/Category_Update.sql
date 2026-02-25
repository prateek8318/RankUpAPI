-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to update category
-- =============================================
CREATE PROCEDURE [dbo].[Category_Update]
    @CategoryId INT,
    @NameEn NVARCHAR(100) = NULL,
    @NameHi NVARCHAR(100) = NULL,
    @Key NVARCHAR(50) = NULL,
    @Type NVARCHAR(50) = NULL,
    @IsActive BIT = NULL,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE Categories
        SET 
            NameEn = ISNULL(@NameEn, NameEn),
            NameHi = ISNULL(@NameHi, NameHi),
            Key = ISNULL(@Key, Key),
            Type = ISNULL(@Type, Type),
            IsActive = ISNULL(@IsActive, IsActive),
            UpdatedAt = @UpdatedAt
        WHERE Id = @CategoryId;
        
        SELECT @@ROWCOUNT AS RowsAffected, 'Category updated successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS RowsAffected, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
