CREATE PROCEDURE [dbo].[UserSocialLogin_Insert]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @AvatarUrl NVARCHAR(500) = NULL,
    @Email NVARCHAR(255),
    @Name NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO UserSocialLogins (
            UserId, Provider, GoogleId, AvatarUrl,
            Email, Name, CreatedAt, UpdatedAt
        )
        VALUES (
            @UserId, @Provider, @GoogleId, @AvatarUrl,
            @Email, @Name, GETUTCDATE(), GETUTCDATE()
        );
        
        SELECT SCOPE_IDENTITY() AS Id;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
