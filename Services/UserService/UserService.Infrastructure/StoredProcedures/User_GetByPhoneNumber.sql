-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get user by phone number
-- =============================================
CREATE PROCEDURE [dbo].[User_GetByPhoneNumber]
    @PhoneNumber NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            Id,
            Name,
            Email,
            PhoneNumber,
            -- Properties that may not exist as real columns in the current DB schema
            CAST(NULL AS NVARCHAR(256))  AS PasswordHash,
            CAST(NULL AS NVARCHAR(10))   AS CountryCode,
            CAST(NULL AS NVARCHAR(20))   AS Gender,
            CAST(NULL AS DATE)           AS DateOfBirth,
            CAST(NULL AS NVARCHAR(100))  AS Qualification,
            PreferredLanguage,
            CAST(NULL AS NVARCHAR(255))  AS ProfilePhoto,
            CAST(NULL AS NVARCHAR(100))  AS PreferredExam,
            CAST(NULL AS INT)            AS StateId,
            CAST(NULL AS INT)            AS LanguageId,
            CAST(NULL AS INT)            AS QualificationId,
            CAST(NULL AS INT)            AS ExamId,
            CAST(NULL AS INT)            AS CategoryId,
            CAST(NULL AS INT)            AS StreamId,
            CAST(NULL AS NVARCHAR(MAX))  AS RefreshToken,
            CAST(NULL AS DATETIME2)      AS RefreshTokenExpiryTime,
            CAST(0 AS BIT)               AS IsPhoneVerified,
            CAST(0 AS BIT)               AS InterestedInIntlExam,
            IsActive,
            CreatedAt,
            UpdatedAt,
            CAST(NULL AS DATETIME2)      AS LastLoginAt,
            CAST(NULL AS NVARCHAR(100))  AS GoogleId
        FROM Users
        WHERE PhoneNumber = @PhoneNumber AND IsActive = 1;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
