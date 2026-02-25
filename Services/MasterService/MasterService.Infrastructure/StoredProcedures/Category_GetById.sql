-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get category by ID
-- =============================================
CREATE PROCEDURE [dbo].[Category_GetById]
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, NameEn, NameHi, Key, Type, IsActive, CreatedAt, UpdatedAt
        FROM Categories
        WHERE Id = @CategoryId AND IsActive = 1;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
