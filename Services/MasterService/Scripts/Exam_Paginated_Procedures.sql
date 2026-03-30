-- Paginated stored procedures for Exam table

-- Exam_GetAllWithLanguagesPaginated
CREATE OR ALTER PROCEDURE [dbo].[Exam_GetAllWithLanguagesPaginated]
    @Skip INT = 0,
    @Take INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Exams] e
    WHERE e.IsDeleted = 0;
    
    -- Get paginated exams
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
    WHERE e.IsDeleted = 0
    ORDER BY e.Id
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;
    
    -- Get languages for paginated exams
    SELECT 
        el.Id,
        el.ExamId,
        el.LanguageId,
        el.Name,
        el.Description,
        l.Code as LanguageCode,
        l.Name as LanguageName
    FROM [dbo].[ExamLanguages] el
    INNER JOIN [dbo].[Languages] l ON el.LanguageId = l.Id
    INNER JOIN [dbo].[Exams] e ON el.ExamId = e.Id
    WHERE e.IsDeleted = 0
    ORDER BY el.ExamId, el.LanguageId;
END

-- Exam_GetActiveWithLanguagesPaginated
CREATE OR ALTER PROCEDURE [dbo].[Exam_GetActiveWithLanguagesPaginated]
    @Skip INT = 0,
    @Take INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Exams] e
    WHERE e.IsDeleted = 0 AND e.IsActive = 1;
    
    -- Get paginated active exams
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
    WHERE e.IsDeleted = 0 AND e.IsActive = 1
    ORDER BY e.Id
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;
    
    -- Get languages for paginated active exams
    SELECT 
        el.Id,
        el.ExamId,
        el.LanguageId,
        el.Name,
        el.Description,
        l.Code as LanguageCode,
        l.Name as LanguageName
    FROM [dbo].[ExamLanguages] el
    INNER JOIN [dbo].[Languages] l ON el.LanguageId = l.Id
    INNER JOIN [dbo].[Exams] e ON el.ExamId = e.Id
    WHERE e.IsDeleted = 0 AND e.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    -- Get qualifications for paginated active exams
    SELECT 
        eq.Id,
        eq.ExamId,
        eq.QualificationId,
        q.Name as QualificationName,
        q.CountryCode
    FROM [dbo].[ExamQualifications] eq
    INNER JOIN [dbo].[Qualifications] q ON eq.QualificationId = q.Id
    INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id
    WHERE e.IsDeleted = 0 AND e.IsActive = 1
    ORDER BY eq.ExamId, eq.QualificationId;
END
