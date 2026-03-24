USE [RankUp_MasterDB];
GO

-- =====================================================
-- FIX EXAM STORED PROCEDURES TO INCLUDE EXAMQUALIFICATIONS
-- =====================================================

-- Drop and recreate Exam_GetActiveWithLanguages to include ExamQualifications
IF OBJECT_ID('dbo.Exam_GetActiveWithLanguages', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetActiveWithLanguages;
GO
CREATE PROCEDURE dbo.Exam_GetActiveWithLanguages
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all active exams
    SELECT e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    WHERE e.IsActive = 1
    ORDER BY e.Name;
    
    -- Get all exam languages for active exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    WHERE el.IsActive = 1 AND e.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    -- Get all exam qualifications for active exams
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId,
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    INNER JOIN dbo.Exams e ON eq.ExamId = e.Id
    WHERE eq.IsActive = 1 AND e.IsActive = 1
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO

-- Drop and recreate Exam_GetActiveWithLanguagesLocalized to include ExamQualifications
IF OBJECT_ID('dbo.Exam_GetActiveWithLanguagesLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetActiveWithLanguagesLocalized;
GO
CREATE PROCEDURE dbo.Exam_GetActiveWithLanguagesLocalized
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all active exams with localized names
    SELECT e.Id, 
           COALESCE(el.Name, e.Name) as Name,
           COALESCE(el.Description, e.Description) as Description,
           e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamLanguages el ON e.Id = el.ExamId 
        AND el.LanguageId = CASE @LanguageCode 
            WHEN 'en' THEN 50 
            WHEN 'hi' THEN 49 
            ELSE 50 
        END
        AND el.IsActive = 1
    WHERE e.IsActive = 1
    ORDER BY COALESCE(el.Name, e.Name);
    
    -- Get all exam languages for active exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    WHERE el.IsActive = 1 AND e.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    -- Get all exam qualifications for active exams
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId,
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    INNER JOIN dbo.Exams e ON eq.ExamId = e.Id
    WHERE eq.IsActive = 1 AND e.IsActive = 1
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO

PRINT 'Fixed Exam stored procedures to include ExamQualifications successfully!';
