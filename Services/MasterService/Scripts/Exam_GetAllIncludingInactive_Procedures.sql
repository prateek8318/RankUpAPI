-- Create stored procedure for getting all exams including inactive with languages
IF EXISTS (SELECT * FROM sys.objects WHERE name='Exam_GetAllWithLanguagesIncludingInactive' AND xtype='P')
BEGIN
    DROP PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactive];
END
GO

CREATE PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Description,
        e.CountryCode,
        e.MinAge,
        e.MaxAge,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive,
        e.CreatedAt,
        e.UpdatedAt
    FROM [dbo].[Exams] e
    ORDER BY e.Name;
    
    SELECT 
        el.Id,
        el.ExamId,
        el.LanguageId,
        el.Name,
        el.Description,
        el.IsActive,
        el.CreatedAt,
        el.UpdatedAt
    FROM [dbo].[ExamLanguages] el
    WHERE el.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    SELECT 
        eq.Id,
        eq.ExamId,
        eq.QualificationId,
        eq.StreamId,
        eq.IsActive,
        eq.CreatedAt,
        eq.UpdatedAt
    FROM [dbo].[ExamQualifications] eq
    WHERE eq.IsActive = 1
    ORDER BY eq.ExamId;
END
GO

-- Create stored procedure for getting all exams including inactive with languages localized
IF EXISTS (SELECT * FROM sys.objects WHERE name='Exam_GetAllWithLanguagesIncludingInactiveLocalized' AND xtype='P')
BEGIN
    DROP PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactiveLocalized];
END
GO

CREATE PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactiveLocalized]
    @LanguageCode NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        ISNULL(el.Name, e.Name) AS Name,
        e.Description,
        e.CountryCode,
        e.MinAge,
        e.MaxAge,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive,
        e.CreatedAt,
        e.UpdatedAt
    FROM [dbo].[Exams] e
    LEFT JOIN [dbo].[ExamLanguages] el ON e.Id = el.ExamId 
        AND el.LanguageId = (SELECT Id FROM Languages WHERE LanguageCode = @LanguageCode AND IsActive = 1)
        AND el.IsActive = 1
    ORDER BY ISNULL(el.Name, e.Name);
    
    SELECT 
        el.Id,
        el.ExamId,
        el.LanguageId,
        el.Name,
        el.Description,
        el.IsActive,
        el.CreatedAt,
        el.UpdatedAt
    FROM [dbo].[ExamLanguages] el
    WHERE el.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    SELECT 
        eq.Id,
        eq.ExamId,
        eq.QualificationId,
        eq.StreamId,
        eq.IsActive,
        eq.CreatedAt,
        eq.UpdatedAt
    FROM [dbo].[ExamQualifications] eq
    WHERE eq.IsActive = 1
    ORDER BY eq.ExamId;
END
GO
