-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get all active categories
-- =============================================
CREATE PROCEDURE [dbo].[Category_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, NameEn, NameHi, Key, Type, IsActive, CreatedAt, UpdatedAt
        FROM Categories
        WHERE IsActive = 1
        ORDER BY Type, NameEn;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
