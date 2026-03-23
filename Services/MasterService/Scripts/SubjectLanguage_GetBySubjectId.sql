USE [RankUp_MasterDB]
GO

PRINT 'Creating SubjectLanguage_GetBySubjectId Procedure...';
PRINT '====================================================';

-- SubjectLanguage_GetBySubjectId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubjectLanguage_GetBySubjectId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId]
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId]
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        sl.Id,
        sl.SubjectId,
        sl.LanguageId,
        sl.Name,
        sl.Description,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt,
        l.Id as Language_Id,
        l.Name as Language_Name,
        l.Code as Language_Code,
        l.IsActive as Language_IsActive
    FROM [dbo].[SubjectLanguages] sl
    LEFT JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
    WHERE sl.SubjectId = @SubjectId
    ORDER BY l.Code;
END
GO

PRINT '====================================================';
PRINT 'SUBJECTLANGUAGE GETBY SUBJECTID PROCEDURE CREATED SUCCESSFULLY!';
PRINT '====================================================';
GO
