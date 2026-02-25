-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to delete category
-- =============================================
CREATE PROCEDURE [dbo].[Category_Delete]
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DELETE FROM Categories
        WHERE Id = @CategoryId;
        
        SELECT @@ROWCOUNT AS RowsAffected, 'Category deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS RowsAffected, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
