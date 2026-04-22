USE [RankUp_MasterDB];
GO

-- =====================================================
-- EXAM LIST STORED PROCEDURES WITH LANGUAGE SUPPORT
-- =====================================================

-- Exam_GetAllWithLanguages
IF OBJECT_ID('dbo.Exam_GetAllWithLanguages', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetAllWithLanguages;
GO
CREATE PROCEDURE dbo.Exam_GetAllWithLanguages
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all exams
    SELECT e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    ORDER BY e.Name;
    
    -- Get all exam languages
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
END
GO

-- Exam_GetActiveWithLanguages
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
    WHERE e.IsActive = 1 AND eq.IsActive = 1
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO

-- Exam_GetActiveWithLanguagesLocalized
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
    WHERE e.IsActive = 1 AND eq.IsActive = 1
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO

-- Exam_GetByFilterWithLanguages
IF OBJECT_ID('dbo.Exam_GetByFilterWithLanguages', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetByFilterWithLanguages;
GO
CREATE PROCEDURE dbo.Exam_GetByFilterWithLanguages
    @CountryCode     NVARCHAR(10) = NULL,
    @QualificationId INT = NULL,
    @StreamId        INT = NULL,
    @MinAge          INT = NULL,
    @MaxAge          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get filtered exams
    SELECT DISTINCT
        e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
        e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY e.Name;
    
    -- Get exam languages for filtered exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE el.IsActive = 1 AND e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY el.ExamId, el.LanguageId;
    
    -- Get exam qualifications for filtered exams
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    INNER JOIN dbo.Exams e ON eq.ExamId = e.Id
    WHERE e.IsActive = 1 AND eq.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR eq.StreamId = @StreamId)
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO

-- Exam_GetByFilterWithLanguagesLocalized
IF OBJECT_ID('dbo.Exam_GetByFilterWithLanguagesLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetByFilterWithLanguagesLocalized;
GO
CREATE PROCEDURE dbo.Exam_GetByFilterWithLanguagesLocalized
    @LanguageCode     NVARCHAR(10) = NULL,
    @CountryCode      NVARCHAR(10) = NULL,
    @QualificationId  INT = NULL,
    @StreamId         INT = NULL,
    @MinAge           INT = NULL,
    @MaxAge           INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get filtered exams with localized names
    SELECT DISTINCT
        e.Id, 
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
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY COALESCE(el.Name, e.Name);
    
    -- Get exam languages for filtered exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE el.IsActive = 1 AND e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY el.ExamId, el.LanguageId;
    
    -- Get exam qualifications for filtered exams
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    INNER JOIN dbo.Exams e ON eq.ExamId = e.Id
    WHERE e.IsActive = 1 AND eq.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR eq.StreamId = @StreamId)
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO

PRINT 'Exam List procedures with language support created successfully!';
