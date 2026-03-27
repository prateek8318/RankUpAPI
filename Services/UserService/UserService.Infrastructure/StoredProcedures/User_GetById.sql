-- =============================================
-- Author: RankUpAPI Team
-- Create date: 24/02/2026
-- Description: Stored procedure to get user by ID
-- =============================================
CREATE PROCEDURE [dbo].[User_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            u.Id,
            u.Name,
            u.Email,
            u.PhoneNumber,
            u.CountryCode,
            u.Gender,
            u.DateOfBirth,
            u.Qualification,
            u.PreferredLanguage AS LanguagePreference,
            u.ProfilePhoto,
            u.PreferredExam,
            u.StateId,
            s.Name AS StateName,
            CAST(NULL AS NVARCHAR(100)) AS StateNameHi,
            u.LanguageId,
            l.Name AS LanguageName,
            l.Name AS LanguageNameHi,
            u.QualificationId,
            q.Name AS QualificationName,
            q.NameHi AS QualificationNameHi,
            u.ExamId,
            CAST(NULL AS NVARCHAR(100)) AS ExamName,
            CAST(NULL AS NVARCHAR(100)) AS ExamNameHi,
            u.CategoryId,
            c.NameEn AS CategoryName,
            c.NameHi AS CategoryNameHi,
            u.StreamId,
            st.Name AS StreamName,
            st.NameHi AS StreamNameHi,
            u.LastLoginAt,
            u.IsPhoneVerified,
            u.IsActive,
            u.CreatedAt,
            u.ProfileCompleted,
            u.InterestedInIntlExam,
            u.DeviceId,
            u.DeviceType,
            u.DeviceName,
            u.FcmToken,
            u.LastDeviceLoginAt,
            u.LastDeviceType,
            u.LastDeviceName,
            u.LoginType,
            DATEADD(MINUTE, 330, u.CreatedAt) AS CreatedAtIST,
            CASE
                WHEN u.LastLoginAt IS NULL THEN NULL
                ELSE DATEADD(MINUTE, 330, u.LastLoginAt)
            END AS LastLoginAtIST
        FROM Users u
        LEFT JOIN [RankUp_MasterDB].[dbo].[States] s ON s.Id = u.StateId
        LEFT JOIN [RankUp_MasterDB].[dbo].[Languages] l ON l.Id = u.LanguageId
        LEFT JOIN [RankUp_MasterDB].[dbo].[Qualifications] q ON q.Id = u.QualificationId
        LEFT JOIN [RankUp_MasterDB].[dbo].[Categories] c ON c.Id = u.CategoryId
        LEFT JOIN [RankUp_MasterDB].[dbo].[Streams] st ON st.Id = u.StreamId
        WHERE u.Id = @Id AND u.IsActive = 1;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
