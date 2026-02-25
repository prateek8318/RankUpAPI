-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get user by ID
-- =============================================
CREATE PROCEDURE [dbo].[User_GetById]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, Name, Email, PhoneNumber, ProfileImageUrl,
            EmailVerified, IsActive, PreferredLanguage, CreatedAt, UpdatedAt
        FROM Users
        WHERE Id = @UserId AND IsActive = 1;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
