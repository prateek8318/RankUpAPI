-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get daily active users count
-- =============================================
CREATE PROCEDURE [dbo].[User_GetDailyActiveCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT COUNT(*) AS DailyActiveUsers
        FROM Users
        WHERE IsActive = 1
        AND CAST(CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE);
    END TRY
    BEGIN CATCH
        SELECT 0 AS DailyActiveUsers, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
