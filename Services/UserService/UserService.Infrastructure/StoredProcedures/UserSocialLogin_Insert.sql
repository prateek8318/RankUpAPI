CREATE PROCEDURE [dbo].[UserSocialLogin_Insert]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @AvatarUrl NVARCHAR(500) = NULL,
    @Email NVARCHAR(255),
    @Name NVARCHAR(255),
    @AccessToken NVARCHAR(1000) = NULL,
    @RefreshToken NVARCHAR(1000) = NULL,
    @ExpiresAt DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO UserSocialLogins (
            UserId, Provider, GoogleId, AvatarUrl,
            Email, Name, AccessToken, RefreshToken, ExpiresAt,
            CreatedAt, UpdatedAt
        )
        VALUES (
            @UserId, @Provider, @GoogleId, @AvatarUrl,
            @Email, @Name, @AccessToken, @RefreshToken, @ExpiresAt,
            GETUTCDATE(), GETUTCDATE()
        );
        
        SELECT SCOPE_IDENTITY() AS Id;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
