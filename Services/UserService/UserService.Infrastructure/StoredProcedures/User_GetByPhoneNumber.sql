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
        FROM Users
        WHERE PhoneNumber = @PhoneNumber AND IsActive = 1;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
