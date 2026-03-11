-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get daily active users count
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetDailyActiveCount]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[User_GetDailyActiveCount]
GO

CREATE PROCEDURE [dbo].[User_GetDailyActiveCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT COUNT(*) AS DailyActiveUsers
        FROM Users
        WHERE LastLoginAt >= DATEADD(DAY, -1, GETDATE()) AND IsActive = 1;
    END TRY
    BEGIN CATCH
        SELECT 0 AS DailyActiveUsers, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
