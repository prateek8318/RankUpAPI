-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get all active users
-- =============================================
CREATE PROCEDURE [dbo].[User_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, Name, Email, PhoneNumber, ProfileImageUrl,
            EmailVerified, IsActive, PreferredLanguage, CreatedAt, UpdatedAt
        FROM Users
        WHERE IsActive = 1
        ORDER BY CreatedAt DESC;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
