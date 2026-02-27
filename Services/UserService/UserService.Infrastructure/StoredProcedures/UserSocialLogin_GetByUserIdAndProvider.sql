CREATE PROCEDURE [dbo].[UserSocialLogin_GetByUserIdAndProvider]
    @UserId INT,
    @Provider NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id, UserId, Provider, GoogleId, ProviderUserId,
            Email, Name, PictureUrl, CreatedAt, UpdatedAt
        FROM UserSocialLogins
        WHERE UserId = @UserId AND Provider = @Provider;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
