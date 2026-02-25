-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get total count of active users
-- =============================================
CREATE PROCEDURE [dbo].[User_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT COUNT(*) AS TotalUsers
        FROM Users
        WHERE IsActive = 1;
    END TRY
    BEGIN CATCH
        SELECT 0 AS TotalUsers, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
