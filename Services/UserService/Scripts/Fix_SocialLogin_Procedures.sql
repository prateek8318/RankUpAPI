-- Check UserSocialLogin_Update stored procedure
SELECT 
    name,
    type_desc,
    OBJECTPROPERTY(OBJECT_ID(name), 'ExecIsQuotedIdentOn') AS IsQuotedIdentifierOn
FROM sys.objects 
WHERE name LIKE '%UserSocialLogin%' AND type = 'P';
GO

-- Show the current procedure definition
IF OBJECT_ID('[dbo].[UserSocialLogin_Update]', 'P') IS NOT NULL
BEGIN
    PRINT 'Current UserSocialLogin_Update procedure:'
    SELECT definition FROM sys.sql_modules WHERE object_id = OBJECT_ID('[dbo].[UserSocialLogin_Update]');
END
GO

-- Drop and recreate UserSocialLogin_Update with correct parameters
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('[dbo].[UserSocialLogin_Update]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[UserSocialLogin_Update];
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_Update]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @AvatarUrl NVARCHAR(500),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100),
    @AccessToken NVARCHAR(500),
    @RefreshToken NVARCHAR(500),
    @ExpiresAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[UserSocialLogins]
    SET 
        AvatarUrl = @AvatarUrl,
        Email = @Email,
        Name = @Name,
        AccessToken = @AccessToken,
        RefreshToken = @RefreshToken,
        ExpiresAt = @ExpiresAt,
        UpdatedAt = GETDATE()
    WHERE UserId = @UserId AND Provider = @Provider AND GoogleId = @GoogleId;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Also fix UserSocialLogin_Insert if it has similar issues
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('[dbo].[UserSocialLogin_Insert]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[UserSocialLogin_Insert];
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_Insert]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @AvatarUrl NVARCHAR(500),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100),
    @AccessToken NVARCHAR(500),
    @RefreshToken NVARCHAR(500),
    @ExpiresAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[UserSocialLogins] (
        UserId, Provider, GoogleId, AvatarUrl, Email, Name, 
        AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    )
    VALUES (
        @UserId, @Provider, @GoogleId, @AvatarUrl, @Email, @Name,
        @AccessToken, @RefreshToken, @ExpiresAt, GETDATE(), GETDATE()
    );
    
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- Verify the procedures were created with correct QUOTED_IDENTIFIER setting
SELECT 
    name,
    type_desc,
    OBJECTPROPERTY(OBJECT_ID(name), 'ExecIsQuotedIdentOn') AS IsQuotedIdentifierOn
FROM sys.objects 
WHERE name LIKE '%UserSocialLogin%' AND type = 'P'
ORDER BY name;
GO
