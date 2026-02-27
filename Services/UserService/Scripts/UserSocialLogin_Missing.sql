-- =====================================================

USE [RankUp_UserDB]
GO

-- UserSocialLogin_GetByProviderAndGoogleId (Missing procedure)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_GetByProviderAndGoogleId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndGoogleId]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndGoogleId]
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Provider,
        GoogleId,
        Email,
        Name,
        AvatarUrl,
        AccessToken,
        RefreshToken,
        ExpiresAt,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[UserSocialLogins] 
    WHERE Provider = @Provider 
    AND GoogleId = @GoogleId;
END
GO

-- UserSocialLogin_GetByUserIdAndProvider (Missing procedure)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_GetByUserIdAndProvider]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_GetByUserIdAndProvider]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByUserIdAndProvider]
    @UserId INT,
    @Provider NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        UserId,
        Provider,
        GoogleId,
        Email,
        Name,
        AvatarUrl,
        AccessToken,
        RefreshToken,
        ExpiresAt,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[UserSocialLogins] 
    WHERE UserId = @UserId 
    AND Provider = @Provider;
END
GO

-- UserSocialLogin_Insert (Missing procedure)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_Insert]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_Insert]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @AvatarUrl NVARCHAR(500),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[UserSocialLogins] (
        UserId, Provider, GoogleId, AvatarUrl, Email, Name,
        CreatedAt, UpdatedAt
    )
    VALUES (
        @UserId, @Provider, @GoogleId, @AvatarUrl, @Email, @Name,
        GETDATE(), GETDATE()
    );
    
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

PRINT 'Missing UserSocialLogin Stored Procedures Created Successfully!';
PRINT '====================================================';
PRINT 'Procedures Added:';
PRINT '1. UserSocialLogin_GetByProviderAndGoogleId';
PRINT '2. UserSocialLogin_GetByUserIdAndProvider';
PRINT '3. UserSocialLogin_Insert';
PRINT '====================================================';
GO
