CREATE PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndGoogleId]
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, UserId, Provider, GoogleId, ProviderUserId,
            Email, Name, PictureUrl, CreatedAt, UpdatedAt
        FROM UserSocialLogins
        WHERE Provider = @Provider AND GoogleId = @GoogleId;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
