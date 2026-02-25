-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to delete user and related data
-- =============================================
CREATE PROCEDURE [dbo].[User_Delete]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Delete user's test attempts
        DELETE FROM UserTestAttempts WHERE UserId = @UserId;
        
        -- Delete user's quiz attempts
        DELETE FROM QuizAttempts WHERE UserId = @UserId;
        
        -- Delete user's subscriptions
        DELETE FROM UserSubscriptions WHERE UserId = @UserId;
        
        -- Delete user's profile images
        DELETE FROM UserProfiles WHERE UserId = @UserId;
        
        -- Delete user's preferences
        DELETE FROM UserPreferences WHERE UserId = @UserId;
        
        -- Delete user's social login accounts
        DELETE FROM UserSocialLogins WHERE UserId = @UserId;
        
        -- Delete user's refresh tokens
        DELETE FROM RefreshTokens WHERE UserId = @UserId;
        
        -- Finally delete the user
        DELETE FROM Users WHERE Id = @UserId;
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Result, 'User deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        SELECT 0 AS Result, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
