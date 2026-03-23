USE [RankUp_UserDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_Create]
GO

CREATE PROCEDURE [dbo].[User_Create]
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @PasswordHash NVARCHAR(256),
    @ProfilePhoto NVARCHAR(500),
    @IsActive BIT = 1,
    @PreferredLanguage NVARCHAR(5) = 'en',
    @IsPhoneVerified BIT = 0,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Users (
        Name, Email, PhoneNumber, PasswordHash, ProfilePhoto, 
        IsActive, PreferredLanguage, IsPhoneVerified, CreatedAt, UpdatedAt
    )
    VALUES (
        @Name, @Email, @PhoneNumber, @PasswordHash, @ProfilePhoto, 
        @IsActive, @PreferredLanguage, @IsPhoneVerified, @CreatedAt, @UpdatedAt
    );
    
    SET @UserId = SCOPE_IDENTITY();
END
GO

PRINT 'User_Create procedure recreated successfully!';
GO
