-- Fix User_Update stored procedure with QUOTED_IDENTIFIER ON
SET QUOTED_IDENTIFIER ON;
GO

-- Drop the existing procedure if it exists
IF OBJECT_ID('[dbo].[User_Update]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[User_Update];
GO

-- Recreate the procedure with QUOTED_IDENTIFIER ON
SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE [dbo].[User_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @CountryCode NVARCHAR(10),
    @Gender NVARCHAR(10),
    @DateOfBirth DATE,
    @Qualification NVARCHAR(100),
    @PreferredLanguage NVARCHAR(10),
    @ProfilePhoto NVARCHAR(500),
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
    SET 
        Name = @Name,
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
    
    SELECT @Id AS Id;
END
GO

-- Verify the procedure was created with correct QUOTED_IDENTIFIER setting
SELECT 
    name,
    type_desc,
    OBJECTPROPERTY(OBJECT_ID(name), 'ExecIsQuotedIdentOn') AS IsQuotedIdentifierOn
FROM sys.objects 
WHERE name = 'User_Update' AND type = 'P';
GO
