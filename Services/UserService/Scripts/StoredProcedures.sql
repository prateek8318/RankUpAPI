

USE [ankUp_UserDB]
GO



-- User_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_GetById]
GO
CREATE PROCEDURE [dbo].[User_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
        CountryCode,
        Gender,
        DateOfBirth,
        Qualification,
        PreferredLanguage,
        ProfilePhoto,
        PreferredExam,
        StateId,
        LanguageId,
        QualificationId,
        ExamId,
        CategoryId,
        StreamId,
        RefreshToken,
        RefreshTokenExpiryTime,
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Users] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- User_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_GetAll]
GO
CREATE PROCEDURE [dbo].[User_GetAll]
    @Page INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
        CountryCode,
        Gender,
        DateOfBirth,
        Qualification,
        PreferredLanguage,
        ProfilePhoto,
        PreferredExam,
        StateId,
        LanguageId,
        QualificationId,
        ExamId,
        CategoryId,
        StreamId,
        RefreshToken,
        RefreshTokenExpiryTime,
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Users] 
    WHERE IsActive = 1
    ORDER BY CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- User_GetByPhoneNumber
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetByPhoneNumber]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_GetByPhoneNumber]
GO
CREATE PROCEDURE [dbo].[User_GetByPhoneNumber]
    @PhoneNumber NVARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
        CountryCode,
        Gender,
        DateOfBirth,
        Qualification,
        PreferredLanguage,
        ProfilePhoto,
        PreferredExam,
        StateId,
        LanguageId,
        QualificationId,
        ExamId,
        CategoryId,
        StreamId,
        RefreshToken,
        RefreshTokenExpiryTime,
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Users] 
    WHERE PhoneNumber = @PhoneNumber AND IsActive = 1;
END
GO

-- User_GetByEmail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetByEmail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_GetByEmail]
GO
CREATE PROCEDURE [dbo].[User_GetByEmail]
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Email,
        PhoneNumber,
        CountryCode,
        Gender,
        DateOfBirth,
        Qualification,
        PreferredLanguage,
        ProfilePhoto,
        PreferredExam,
        StateId,
        LanguageId,
        QualificationId,
        ExamId,
        CategoryId,
        StreamId,
        RefreshToken,
        RefreshTokenExpiryTime,
        LastLoginAt,
        IsPhoneVerified,
        InterestedInIntlExam,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Users] 
    WHERE Email = @Email AND IsActive = 1;
END
GO

-- User_GetTotalCount
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetTotalCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_GetTotalCount]
GO
CREATE PROCEDURE [dbo].[User_GetTotalCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) as TotalCount
    FROM [dbo].[Users] 
    WHERE IsActive = 1;
END
GO

-- User_GetDailyActiveCount
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_GetDailyActiveCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_GetDailyActiveCount]
GO
CREATE PROCEDURE [dbo].[User_GetDailyActiveCount]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) as DailyActiveCount
    FROM [dbo].[Users] 
    WHERE IsActive = 1 
    AND CAST(LastLoginAt AS DATE) = CAST(GETDATE() AS DATE);
END
GO

-- User_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_Create]
GO
CREATE PROCEDURE [dbo].[User_Create]
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(15),
    @PasswordHash NVARCHAR(255),
    @ProfilePhoto NVARCHAR(255),
    @EmailVerified BIT = 0,
    @IsActive BIT = 1,
    @PreferredLanguage NVARCHAR(5) = 'en',
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Users] (
        Name, Email, PhoneNumber, PasswordHash, ProfilePhoto, 
        EmailVerified, IsActive, PreferredLanguage, CreatedAt, UpdatedAt
    )
    VALUES (
        @Name, @Email, @PhoneNumber, @PasswordHash, @ProfilePhoto, 
        @EmailVerified, @IsActive, @PreferredLanguage, @CreatedAt, @UpdatedAt
    );
    
    SET @UserId = SCOPE_IDENTITY();
END
GO

-- User_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_Update]
GO
CREATE PROCEDURE [dbo].[User_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(15),
    @CountryCode NVARCHAR(10),
    @Gender NVARCHAR(20),
    @DateOfBirth DATE,
    @Qualification NVARCHAR(100),
    @PreferredLanguage NVARCHAR(5),
    @ProfilePhoto NVARCHAR(255),
    @PreferredExam NVARCHAR(100),
    @StateId INT,
    @LanguageId INT,
    @QualificationId INT,
    @ExamId INT,
    @CategoryId INT,
    @StreamId INT,
    @RefreshToken NVARCHAR(500),
    @RefreshTokenExpiryTime DATETIME,
    @IsPhoneVerified BIT,
    @InterestedInIntlExam BIT,
    @IsActive BIT,
    @UpdatedAt DATETIME,
    @LastLoginAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Users] 
    SET Name = @Name,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        CountryCode = @CountryCode,
        Gender = @Gender,
        DateOfBirth = @DateOfBirth,
        Qualification = @Qualification,
        PreferredLanguage = @PreferredLanguage,
        ProfilePhoto = @ProfilePhoto,
        PreferredExam = @PreferredExam,
        StateId = @StateId,
        LanguageId = @LanguageId,
        QualificationId = @QualificationId,
        ExamId = @ExamId,
        CategoryId = @CategoryId,
        StreamId = @StreamId,
        RefreshToken = @RefreshToken,
        RefreshTokenExpiryTime = @RefreshTokenExpiryTime,
        IsPhoneVerified = @IsPhoneVerified,
        InterestedInIntlExam = @InterestedInIntlExam,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt,
        LastLoginAt = @LastLoginAt
    WHERE Id = @Id;
END
GO

-- User_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[User_Delete]
GO
CREATE PROCEDURE [dbo].[User_Delete]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Users] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @UserId;
END
GO


-- UserSocialLogin_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_GetById]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetById]
    @Id INT
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
    WHERE Id = @Id;
END
GO

-- UserSocialLogin_GetByUserId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_GetByUserId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_GetByUserId]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByUserId]
    @UserId INT
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
    WHERE UserId = @UserId;
END
GO

-- UserSocialLogin_GetByProviderAndProviderId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_GetByProviderAndProviderId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndProviderId]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByProviderAndProviderId]
    @Provider NVARCHAR(50),
    @ProviderId NVARCHAR(255)
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
    AND (@Provider = 'Google' AND GoogleId = @ProviderId);
END
GO

-- UserSocialLogin_GetByEmail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_GetByEmail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_GetByEmail]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_GetByEmail]
    @Email NVARCHAR(100)
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
    WHERE Email = @Email;
END
GO

-- UserSocialLogin_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_Create]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_Create]
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100),
    @AvatarUrl NVARCHAR(500),
    @AccessToken NVARCHAR(1000),
    @RefreshToken NVARCHAR(1000),
    @ExpiresAt DATETIME,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[UserSocialLogins] (
        UserId, Provider, GoogleId, Email, Name, AvatarUrl,
        AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt
    )
    VALUES (
        @UserId, @Provider, @GoogleId, @Email, @Name, @AvatarUrl,
        @AccessToken, @RefreshToken, @ExpiresAt, @CreatedAt, @UpdatedAt
    );
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- UserSocialLogin_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_Update]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_Update]
    @Id INT,
    @UserId INT,
    @Provider NVARCHAR(50),
    @GoogleId NVARCHAR(255),
    @Email NVARCHAR(100),
    @Name NVARCHAR(100),
    @AvatarUrl NVARCHAR(500),
    @AccessToken NVARCHAR(1000),
    @RefreshToken NVARCHAR(1000),
    @ExpiresAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[UserSocialLogins] 
    SET UserId = @UserId,
        Provider = @Provider,
        GoogleId = @GoogleId,
        Email = @Email,
        Name = @Name,
        AvatarUrl = @AvatarUrl,
        AccessToken = @AccessToken,
        RefreshToken = @RefreshToken,
        ExpiresAt = @ExpiresAt,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- UserSocialLogin_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSocialLogin_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UserSocialLogin_Delete]
GO
CREATE PROCEDURE [dbo].[UserSocialLogin_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[UserSocialLogins] 
    WHERE Id = @Id;
END
GO

-- PRINT 'UserService Stored Procedures Completed Successfully!';
-- PRINT '====================================================';
-- PRINT 'Entities Covered:';
-- PRINT '1. User (8 procedures)';
-- PRINT '2. UserSocialLogin (7 procedures)';
-- PRINT '====================================================';
-- PRINT 'Total Procedures: 15';
-- PRINT 'Ready for UserService Dapper Repository Implementation!';
-- PRINT '====================================================';
GO
