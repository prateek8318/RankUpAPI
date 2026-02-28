
-- Fix UserSocialLogin Stored Procedures for Social Login


USE [RankUp_UserDB]
GO

-- Drop and recreate UserSocialLogin_Insert with all required fields
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_Insert]
GO

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
GO

-- Drop and recreate UserSocialLogin_GetByProviderAndGoogleId
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

-- Drop and recreate UserSocialLogin_GetByUserIdAndProvider
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

PRINT 'UserSocialLogin stored procedures updated successfully!';
PRINT 'Procedures Updated:';
PRINT '1. UserSocialLogin_Insert';
PRINT '2. UserSocialLogin_GetByProviderAndGoogleId';
PRINT '3. UserSocialLogin_GetByUserIdAndProvider';
PRINT '====================================================';
GO
