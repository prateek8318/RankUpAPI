-- =====================================================
-- Fix QUOTED_IDENTIFIER Issue for Stored Procedures
-- =====================================================

USE [RankUp_UserDB]
GO

-- Drop and recreate User_Create with proper settings
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_Create]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[User_Create]
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @PasswordHash NVARCHAR(256),
    @ProfilePhoto NVARCHAR(500),
    @IsActive BIT = 1,
    @PreferredLanguage NVARCHAR(5) = 'en',
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO Users (
            Name, Email, PhoneNumber, PasswordHash, ProfilePhoto,
            IsActive, PreferredLanguage, CreatedAt, UpdatedAt
        )
        VALUES (
            @Name, @Email, @PhoneNumber, @PasswordHash, @ProfilePhoto,
            @IsActive, @PreferredLanguage, @CreatedAt, @UpdatedAt
        );
        
        SET @UserId = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        SET @UserId = 0;
    END CATCH
END
GO

-- Drop and recreate UserSocialLogin_Insert with proper settings
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_Insert]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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

PRINT 'Stored procedures recreated with proper QUOTED_IDENTIFIER settings!';
GO
