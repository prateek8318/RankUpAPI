USE [RankUp_MasterDB]
GO

-- =====================================================
-- Stream_GetAllWithData - Comprehensive Stream Data Procedure
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetAllWithData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_GetAllWithData]
GO

CREATE PROCEDURE [dbo].[Stream_GetAllWithData]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Main Stream Data with Qualification and Language Information
    SELECT 
        s.Id as StreamId,
        s.Name as StreamName,
        s.Description as StreamDescription,
        s.QualificationId,
        s.IsActive as StreamIsActive,
        s.CreatedAt as StreamCreatedAt,
        s.UpdatedAt as StreamUpdatedAt,
        
        -- Qualification Information
        q.Id as QualificationId,
        q.Name as QualificationName,
        q.Description as QualificationDescription,
        q.CountryCode,
        q.IsActive as QualificationIsActive,
        q.CreatedAt as QualificationCreatedAt,
        q.UpdatedAt as QualificationUpdatedAt,
        
        -- Country Information
        c.Id as CountryId,
        c.Name as CountryName,
        c.Code as CountryCode,
        c.IsActive as CountryIsActive,
        c.CreatedAt as CountryCreatedAt,
        c.UpdatedAt as CountryUpdatedAt,
        
        -- Stream Languages (JSON format for multiple languages)
        (
            SELECT 
                sl.LanguageId,
                l.Code as LanguageCode,
                l.Name as LanguageName,
                sl.Name as StreamLocalName,
                sl.Description as StreamLocalDescription
            FROM [dbo].[StreamLanguages] sl
            INNER JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
            WHERE sl.StreamId = s.Id AND sl.IsActive = 1 AND l.IsActive = 1
            FOR JSON PATH
        ) as StreamLanguages,
        
        -- Qualification Languages (JSON format for multiple languages)
        (
            SELECT 
                ql.LanguageId,
                l.Code as LanguageCode,
                l.Name as LanguageName,
                ql.Name as QualificationLocalName,
                ql.Description as QualificationLocalDescription
            FROM [dbo].[QualificationLanguages] ql
            INNER JOIN [dbo].[Languages] l ON ql.LanguageId = l.Id
            WHERE ql.QualificationId = q.Id AND ql.IsActive = 1 AND l.IsActive = 1
            FOR JSON PATH
        ) as QualificationLanguages,
        
        -- Related Exams Count
        (
            SELECT COUNT(*)
            FROM [dbo].[ExamQualifications] eq
            INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id
            WHERE eq.QualificationId = q.Id 
            AND (eq.StreamId IS NULL OR eq.StreamId = s.Id)
            AND eq.IsActive = 1 AND e.IsActive = 1
        ) as RelatedExamsCount,
        
        -- Related Exams (JSON format)
        (
            SELECT 
                e.Id as ExamId,
                e.Name as ExamName,
                e.Description as ExamDescription,
                e.CountryCode as ExamCountryCode,
                e.MinAge,
                e.MaxAge,
                e.ImageUrl,
                e.IsInternational,
                e.IsActive as ExamIsActive,
                e.CreatedAt as ExamCreatedAt,
                e.UpdatedAt as ExamUpdatedAt
            FROM [dbo].[ExamQualifications] eq
            INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id
            WHERE eq.QualificationId = q.Id 
            AND (eq.StreamId IS NULL OR eq.StreamId = s.Id)
            AND eq.IsActive = 1 AND e.IsActive = 1
            FOR JSON PATH
        ) as RelatedExams
        
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[Countries] c ON q.CountryCode = c.Code
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND c.IsActive = 1
    ORDER BY c.Name, q.Name, s.Name;
END
GO

-- Alternative version with separate result sets for better readability
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetAllDetailed]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_GetAllDetailed]
GO

CREATE PROCEDURE [dbo].[Stream_GetAllDetailed]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Result Set 1: Main Stream Data
    SELECT 
        s.Id as StreamId,
        s.Name as StreamName,
        s.Description as StreamDescription,
        s.QualificationId,
        s.IsActive as StreamIsActive,
        s.CreatedAt as StreamCreatedAt,
        s.UpdatedAt as StreamUpdatedAt,
        q.Name as QualificationName,
        q.Description as QualificationDescription,
        q.CountryCode,
        c.Name as CountryName
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[Countries] c ON q.CountryCode = c.Code
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND c.IsActive = 1
    ORDER BY c.Name, q.Name, s.Name;
    
    -- Result Set 2: Stream Languages
    SELECT 
        sl.StreamId,
        sl.LanguageId,
        l.Code as LanguageCode,
        l.Name as LanguageName,
        sl.Name as StreamLocalName,
        sl.Description as StreamLocalDescription,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt
    FROM [dbo].[StreamLanguages] sl
    INNER JOIN [dbo].[Streams] s ON sl.StreamId = s.Id
    INNER JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
    WHERE sl.IsActive = 1 AND s.IsActive = 1 AND l.IsActive = 1
    ORDER BY sl.StreamId, l.Name;
    
    -- Result Set 3: Qualification Languages
    SELECT 
        ql.QualificationId,
        ql.LanguageId,
        l.Code as LanguageCode,
        l.Name as LanguageName,
        ql.Name as QualificationLocalName,
        ql.Description as QualificationLocalDescription,
        ql.IsActive,
        ql.CreatedAt,
        ql.UpdatedAt
    FROM [dbo].[QualificationLanguages] ql
    INNER JOIN [dbo].[Qualifications] q ON ql.QualificationId = q.Id
    INNER JOIN [dbo].[Languages] l ON ql.LanguageId = l.Id
    WHERE ql.IsActive = 1 AND q.IsActive = 1 AND l.IsActive = 1
    ORDER BY ql.QualificationId, l.Name;
    
    -- Result Set 4: Related Exams
    SELECT 
        s.Id as StreamId,
        s.Name as StreamName,
        e.Id as ExamId,
        e.Name as ExamName,
        e.Description as ExamDescription,
        e.CountryCode as ExamCountryCode,
        e.MinAge,
        e.MaxAge,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive as ExamIsActive,
        e.CreatedAt as ExamCreatedAt,
        e.UpdatedAt as ExamUpdatedAt
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[ExamQualifications] eq ON q.Id = eq.QualificationId
    INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND e.IsActive = 1 
    AND eq.IsActive = 1 AND (eq.StreamId IS NULL OR eq.StreamId = s.Id)
    ORDER BY s.Name, e.Name;
END
GO

-- Simple version for basic stream data
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = N'[dbo].[Stream_GetAllSimple]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_GetAllSimple]
GO

CREATE PROCEDURE [dbo].[Stream_GetAllSimple]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        s.Name,
        s.Description,
        s.QualificationId,
        q.Name as QualificationName,
        q.CountryCode,
        c.Name as CountryName,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[Countries] c ON q.CountryCode = c.Code
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND c.IsActive = 1
    ORDER BY c.Name, q.Name, s.Name;
END
GO

PRINT 'Stream Comprehensive Data Procedures Created Successfully!';
PRINT 'Procedures Created:';
PRINT '1. Stream_GetAllWithData - Single result set with JSON nested data';
PRINT '2. Stream_GetAllDetailed - Multiple result sets for detailed analysis';
PRINT '3. Stream_GetAllSimple - Basic stream data with qualification and country';
GO
