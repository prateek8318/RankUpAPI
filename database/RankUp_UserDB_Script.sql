USE [master]
GO

-- Drop if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RankUp_UserDB')
BEGIN
    ALTER DATABASE [RankUp_UserDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RankUp_UserDB];
END
GO

-- Create with Linux path
CREATE DATABASE [RankUp_UserDB]
ON PRIMARY 
( NAME = N'RankUp_UserDB', FILENAME = N'/var/opt/mssql/data/RankUp_UserDB.mdf', SIZE = 8192KB, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'RankUp_UserDB_log', FILENAME = N'/var/opt/mssql/data/RankUp_UserDB_log.ldf', SIZE = 8192KB, FILEGROWTH = 65536KB )
GO

USE [RankUp_UserDB]
GO

-- Migrations History Table
CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId] [nvarchar](150) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
)
GO

-- Users Table
CREATE TABLE [dbo].[Users](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NULL,
    [Email] [nvarchar](100) NULL,
    [PasswordHash] [nvarchar](255) NULL,
    [PhoneNumber] [nvarchar](15) NULL,
    [Gender] [nvarchar](20) NULL,
    [DateOfBirth] [date] NULL,
    [Qualification] [nvarchar](100) NULL,
    [ProfilePhoto] [nvarchar](255) NULL,
    [PreferredExam] [nvarchar](100) NULL,
    [StateId] [int] NULL,
    [LanguageId] [int] NULL,
    [QualificationId] [int] NULL,
    [ExamId] [int] NULL,
    [RefreshToken] [nvarchar](max) NULL,
    [RefreshTokenExpiryTime] [datetime2](7) NULL,
    [LastLoginAt] [datetime2](7) NULL,
    [IsPhoneVerified] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CountryCode] [nvarchar](10) NULL,
    [InterestedInIntlExam] [bit] NOT NULL DEFAULT 0,
    [PreferredLanguage] [nvarchar](5) NULL,
    [CategoryId] [int] NULL,
    [StreamId] [int] NULL,
    [DeviceId] [nvarchar](100) NULL,
    [DeviceType] [nvarchar](50) NULL,
    [DeviceName] [nvarchar](100) NULL,
    [FcmToken] [nvarchar](500) NULL,
    [LastDeviceLoginAt] [datetime2](7) NULL,
    [LastDeviceType] [nvarchar](50) NULL,
    [LastDeviceName] [nvarchar](100) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- UserSocialLogins Table
CREATE TABLE [dbo].[UserSocialLogins](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL,
    [Provider] [nvarchar](max) NULL,
    [GoogleId] [nvarchar](max) NULL,
    [Email] [nvarchar](max) NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [AvatarUrl] [nvarchar](max) NULL,
    [AccessToken] [nvarchar](max) NULL,
    [RefreshToken] [nvarchar](max) NULL,
    [ExpiresAt] [datetime2](7) NULL,
    [CreatedAt] [datetime2](7) NOT NULL,
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_UserSocialLogins] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Indexes
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]([Email] ASC) WHERE ([Email] IS NOT NULL)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_PhoneNumber] ON [dbo].[Users]([PhoneNumber] ASC) WHERE ([PhoneNumber] IS NOT NULL)
GO
CREATE NONCLUSTERED INDEX [IX_UserSocialLogins_UserId] ON [dbo].[UserSocialLogins]([UserId] ASC)
GO

-- Foreign Key
ALTER TABLE [dbo].[UserSocialLogins] ADD CONSTRAINT [FK_UserSocialLogins_Users_UserId] 
FOREIGN KEY([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
GO

-- Migrations Data
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260116134239_CheckSchema', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260116134823_AddCountryCodeToUser', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260122094004_AddInternationalExamFields', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260122095813_RemoveDuplicateInternationalExamColumn', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260209060538_UpdateLanguagePreferenceToPreferredLanguage', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260209083338_AddCategoryAndStreamToUser', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260223102258_InitialCreateBaseline', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260223102447_RemoveObsoleteUserColumns', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260223102542_DropPasswordHashColumn', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260223125150_AddGoogleIdToUsers', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260224170857_AddUserSocialLoginsTable', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260224172444_FixUserSocialLoginsNullableColumns', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] VALUES (N'20260225050306_FixUserIdIdentity', N'8.0.0')
GO

-- Stored Procedures
CREATE PROCEDURE [dbo].[User_Create]
    @Name NVARCHAR(100), @Email NVARCHAR(100), @PhoneNumber NVARCHAR(15),
    @PasswordHash NVARCHAR(255), @ProfilePhoto NVARCHAR(255),
    @IsActive BIT = 1, @PreferredLanguage NVARCHAR(5) = 'en',
    @IsPhoneVerified BIT = 0, @InterestedInIntlExam BIT = 0,
    @CreatedAt DATETIME, @UpdatedAt DATETIME, @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[Users] (Name, Email, PhoneNumber, PasswordHash, ProfilePhoto, IsActive, PreferredLanguage, IsPhoneVerified, InterestedInIntlExam, CreatedAt, UpdatedAt)
    VALUES (@Name, @Email, @PhoneNumber, @PasswordHash, @ProfilePhoto, @IsActive, @PreferredLanguage, @IsPhoneVerified, @InterestedInIntlExam, @CreatedAt, @UpdatedAt);
    SET @UserId = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE [dbo].[User_Update]
    @Id INT, @Name NVARCHAR(100), @Email NVARCHAR(100), @PhoneNumber NVARCHAR(15),
    @CountryCode NVARCHAR(10), @Gender NVARCHAR(20), @DateOfBirth DATE,
    @Qualification NVARCHAR(100), @PreferredLanguage NVARCHAR(5), @ProfilePhoto NVARCHAR(255),
    @PreferredExam NVARCHAR(100), @StateId INT, @LanguageId INT, @QualificationId INT,
    @ExamId INT, @CategoryId INT, @StreamId INT, @RefreshToken NVARCHAR(500),
    @RefreshTokenExpiryTime DATETIME, @IsPhoneVerified BIT, @InterestedInIntlExam BIT,
    @IsActive BIT, @UpdatedAt DATETIME, @LastLoginAt DATETIME,
    @DeviceId NVARCHAR(100), @DeviceType NVARCHAR(50), @DeviceName NVARCHAR(100),
    @FcmToken NVARCHAR(500), @LastDeviceLoginAt DATETIME, @LastDeviceType NVARCHAR(50),
    @LastDeviceName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Users] SET
        Name=@Name, Email=@Email, PhoneNumber=@PhoneNumber, CountryCode=@CountryCode,
        Gender=@Gender, DateOfBirth=@DateOfBirth, Qualification=@Qualification,
        PreferredLanguage=@PreferredLanguage, ProfilePhoto=@ProfilePhoto,
        PreferredExam=@PreferredExam, StateId=@StateId, LanguageId=@LanguageId,
        QualificationId=@QualificationId, ExamId=@ExamId, CategoryId=@CategoryId,
        StreamId=@StreamId, RefreshToken=@RefreshToken, RefreshTokenExpiryTime=@RefreshTokenExpiryTime,
        IsPhoneVerified=@IsPhoneVerified, InterestedInIntlExam=@InterestedInIntlExam,
        IsActive=@IsActive, UpdatedAt=@UpdatedAt, LastLoginAt=@LastLoginAt,
        DeviceId=@DeviceId, DeviceType=@DeviceType, DeviceName=@DeviceName,
        FcmToken=@FcmToken, LastDeviceLoginAt=@LastDeviceLoginAt,
        LastDeviceType=@LastDeviceType, LastDeviceName=@LastDeviceName
    WHERE Id=@Id;
END
GO

CREATE PROCEDURE [dbo].[User_Delete] @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Users] SET IsActive=0, UpdatedAt=GETDATE() WHERE Id=@UserId;
END
GO

CREATE PROCEDURE [dbo].[User_GetById] @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.Id, u.Name, u.Email, u.PhoneNumber, u.CountryCode, u.Gender, u.DateOfBirth,
        u.Qualification, u.PreferredLanguage, u.ProfilePhoto, u.PreferredExam,
        u.StateId, NULL AS StateName, NULL AS StateNameHi,
        u.LanguageId, NULL AS LanguageName, NULL AS LanguageNameHi,
        u.QualificationId, NULL AS QualificationName, NULL AS QualificationNameHi,
        u.ExamId, NULL AS ExamName, NULL AS ExamNameHi,
        u.CategoryId, NULL AS CategoryName, NULL AS CategoryNameHi,
        u.StreamId, NULL AS StreamName, NULL AS StreamNameHi,
        u.RefreshToken, u.RefreshTokenExpiryTime, u.LastLoginAt,
        u.IsPhoneVerified, u.InterestedInIntlExam, u.IsActive, u.CreatedAt, u.UpdatedAt
    FROM [dbo].[Users] u
    WHERE u.Id=@Id AND u.IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[User_GetById_Basic] @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Email, PasswordHash, PhoneNumber, CountryCode, Gender, DateOfBirth,
        Qualification, PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId,
        QualificationId, ExamId, CategoryId, StreamId, RefreshToken, RefreshTokenExpiryTime,
        LastLoginAt, IsPhoneVerified, InterestedInIntlExam, IsActive, CreatedAt, UpdatedAt,
        DeviceId, DeviceType, DeviceName, FcmToken, LastDeviceLoginAt, LastDeviceType, LastDeviceName
    FROM [dbo].[Users] WHERE Id=@Id AND IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[User_GetByEmail] @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Email, PhoneNumber, CountryCode, Gender, DateOfBirth, Qualification,
        PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId, QualificationId,
        ExamId, CategoryId, StreamId, RefreshToken, RefreshTokenExpiryTime, LastLoginAt,
        IsPhoneVerified, InterestedInIntlExam, IsActive, CreatedAt, UpdatedAt
    FROM [dbo].[Users] WHERE Email=@Email AND IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[User_GetByPhoneNumber] @PhoneNumber NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Email, PhoneNumber, CountryCode, Gender, DateOfBirth, Qualification,
        PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId, QualificationId,
        ExamId, CategoryId, StreamId, RefreshToken, RefreshTokenExpiryTime, LastLoginAt,
        IsPhoneVerified, InterestedInIntlExam, IsActive, CreatedAt, UpdatedAt
    FROM [dbo].[Users] WHERE PhoneNumber=@PhoneNumber AND IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[User_GetByGoogleId] @GoogleId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Email, PhoneNumber, CountryCode, Gender, DateOfBirth, Qualification,
        PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId, QualificationId,
        ExamId, CategoryId, StreamId, RefreshToken, RefreshTokenExpiryTime,
        IsPhoneVerified, InterestedInIntlExam, IsActive, CreatedAt, UpdatedAt, LastLoginAt
    FROM [dbo].[Users] WHERE IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[User_GetAll] @Page INT = 1, @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    SELECT Id, Name, Email, PhoneNumber, CountryCode, Gender, DateOfBirth, Qualification,
        PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId, QualificationId,
        ExamId, CategoryId, StreamId, RefreshToken, RefreshTokenExpiryTime, LastLoginAt,
        IsPhoneVerified, InterestedInIntlExam, IsActive, CreatedAt, UpdatedAt
    FROM [dbo].[Users] WHERE IsActive=1
    ORDER BY CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE PROCEDURE [dbo].[User_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) as TotalCount FROM [dbo].[Users] WHERE IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[User_GetDailyActiveCount]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) as DailyActiveCount FROM [dbo].[Users]
    WHERE IsActive=1 AND CAST(LastLoginAt AS DATE) = CAST(GETDATE() AS DATE);
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_Create]
    @UserId INT, @Provider NVARCHAR(50), @GoogleId NVARCHAR(255),
    @Email NVARCHAR(100), @Name NVARCHAR(100), @AvatarUrl NVARCHAR(500),
    @AccessToken NVARCHAR(1000), @RefreshToken NVARCHAR(1000),
    @ExpiresAt DATETIME, @CreatedAt DATETIME, @UpdatedAt DATETIME, @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[UserSocialLogins] (UserId, Provider, GoogleId, Email, Name, AvatarUrl, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt)
    VALUES (@UserId, @Provider, @GoogleId, @Email, @Name, @AvatarUrl, @AccessToken, @RefreshToken, @ExpiresAt, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_Insert]
    @UserId INT, @Provider NVARCHAR(50), @GoogleId NVARCHAR(255),
    @AvatarUrl NVARCHAR(500), @Email NVARCHAR(100), @Name NVARCHAR(100),
    @AccessToken NVARCHAR(500), @RefreshToken NVARCHAR(500), @ExpiresAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[UserSocialLogins] (UserId, Provider, GoogleId, AvatarUrl, Email, Name, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt)
    VALUES (@UserId, @Provider, @GoogleId, @AvatarUrl, @Email, @Name, @AccessToken, @RefreshToken, @ExpiresAt, GETDATE(), GETDATE());
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_Update]
    @Id INT, @UserId INT, @Provider NVARCHAR(50), @GoogleId NVARCHAR(255),
    @Email NVARCHAR(100), @Name NVARCHAR(100), @AvatarUrl NVARCHAR(500),
    @AccessToken NVARCHAR(1000), @RefreshToken NVARCHAR(1000),
    @ExpiresAt DATETIME, @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[UserSocialLogins] SET
        UserId=@UserId, Provider=@Provider, GoogleId=@GoogleId, Email=@Email,
        Name=@Name, AvatarUrl=@AvatarUrl, AccessToken=@AccessToken,
        RefreshToken=@RefreshToken, ExpiresAt=@ExpiresAt, UpdatedAt=@UpdatedAt
    WHERE Id=@Id;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_Delete] @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[UserSocialLogins] WHERE Id=@Id;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetById] @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Provider, GoogleId, Email, Name, AvatarUrl, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    FROM [dbo].[UserSocialLogins] WHERE Id=@Id;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetByEmail] @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Provider, GoogleId, Email, Name, AvatarUrl, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    FROM [dbo].[UserSocialLogins] WHERE Email=@Email;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetByUserId] @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Provider, GoogleId, Email, Name, AvatarUrl, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    FROM [dbo].[UserSocialLogins] WHERE UserId=@UserId;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetByUserIdAndProvider]
    @UserId INT, @Provider NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Provider, GoogleId, Email, Name, AvatarUrl, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    FROM [dbo].[UserSocialLogins] WHERE UserId=@UserId AND Provider=@Provider;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndGoogleId]
    @Provider NVARCHAR(50), @GoogleId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Provider, GoogleId, Email, Name, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    FROM [dbo].[UserSocialLogins] WHERE Provider=@Provider AND GoogleId=@GoogleId;
END
GO

CREATE PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndProviderId]
    @Provider NVARCHAR(50), @ProviderId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Provider, GoogleId, Email, Name, AvatarUrl, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    FROM [dbo].[UserSocialLogins] WHERE Provider=@Provider AND (@Provider='Google' AND GoogleId=@ProviderId);
END
GO

CREATE PROCEDURE [dbo].[Qualification_Create]
    @Name NVARCHAR(200), @Description NVARCHAR(1000) = NULL, @CountryCode NVARCHAR(10),
    @IsActive BIT, @NamesJson NVARCHAR(MAX) = NULL, @CreatedAt DATETIME2, @UpdatedAt DATETIME2, @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Qualifications (Name, Description, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE [dbo].[Qualification_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications WHERE IsActive=1;
END
GO

CREATE PROCEDURE [dbo].[Qualification_GetById] @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications WHERE Id=@Id;
END
GO

CREATE PROCEDURE [dbo].[Qualification_Update]
    @Id INT, @Name NVARCHAR(200), @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10), @IsActive BIT, @NamesJson NVARCHAR(MAX) = NULL, @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Qualifications
    SET Name=@Name, Description=@Description, CountryCode=@CountryCode, IsActive=@IsActive, UpdatedAt=@UpdatedAt
    WHERE Id=@Id;
END
GO