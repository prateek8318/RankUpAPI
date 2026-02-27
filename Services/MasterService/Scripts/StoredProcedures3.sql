

USE [RankUp_MasterDB]
GO

-- 8. CMS CONTENT STORED PROCEDURES


-- CmsContent_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_GetById]
GO
CREATE PROCEDURE [dbo].[CmsContent_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        ContentType,
        Title,
        Content,
        Status,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[CmsContents] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- CmsContent_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_GetAll]
GO
CREATE PROCEDURE [dbo].[CmsContent_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        ContentType,
        Title,
        Content,
        Status,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[CmsContents] 
    WHERE IsActive = 1
    ORDER BY ContentType, Title;
END
GO

-- CmsContent_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_GetActive]
GO
CREATE PROCEDURE [dbo].[CmsContent_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        ContentType,
        Title,
        Content,
        Status,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[CmsContents] 
    WHERE IsActive = 1
    ORDER BY ContentType, Title;
END
GO

-- CmsContent_GetByType
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetByType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_GetByType]
GO
CREATE PROCEDURE [dbo].[CmsContent_GetByType]
    @ContentType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        ContentType,
        Title,
        Content,
        Status,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[CmsContents] 
    WHERE ContentType = @ContentType AND IsActive = 1
    ORDER BY Title;
END
GO

-- CmsContent_GetByStatus
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_GetByStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_GetByStatus]
GO
CREATE PROCEDURE [dbo].[CmsContent_GetByStatus]
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        ContentType,
        Title,
        Content,
        Status,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[CmsContents] 
    WHERE Status = @Status AND IsActive = 1
    ORDER BY ContentType, Title;
END
GO

-- CmsContent_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_Create]
GO
CREATE PROCEDURE [dbo].[CmsContent_Create]
    @ContentType NVARCHAR(50),
    @Title NVARCHAR(200),
    @Content NVARCHAR(MAX),
    @Status NVARCHAR(20),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[CmsContents] (ContentType, Title, Content, Status, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ContentType, @Title, @Content, @Status, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- CmsContent_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_Update]
GO
CREATE PROCEDURE [dbo].[CmsContent_Update]
    @Id INT,
    @ContentType NVARCHAR(50),
    @Title NVARCHAR(200),
    @Content NVARCHAR(MAX),
    @Status NVARCHAR(20),
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[CmsContents] 
    SET ContentType = @ContentType,
        Title = @Title,
        Content = @Content,
        Status = @Status,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- CmsContent_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContent_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CmsContent_Delete]
GO
CREATE PROCEDURE [dbo].[CmsContent_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[CmsContents] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO



-- StateLanguage_GetByStateId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StateLanguage_GetByStateId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StateLanguage_GetByStateId]
GO
CREATE PROCEDURE [dbo].[StateLanguage_GetByStateId]
    @StateId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        sl.Id,
        sl.StateId,
        sl.LanguageId,
        sl.Name,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt,
        l.Code as LanguageCode,
        l.Name as LanguageName
    FROM [dbo].[StateLanguages] sl
    INNER JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
    WHERE sl.StateId = @StateId AND sl.IsActive = 1 AND l.IsActive = 1
    ORDER BY l.Name;
END
GO

-- StateLanguage_GetByLanguageId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StateLanguage_GetByLanguageId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StateLanguage_GetByLanguageId]
GO
CREATE PROCEDURE [dbo].[StateLanguage_GetByLanguageId]
    @LanguageId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        sl.Id,
        sl.StateId,
        sl.LanguageId,
        sl.Name,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt,
        s.Name as StateName,
        s.Code as StateCode
    FROM [dbo].[StateLanguages] sl
    INNER JOIN [dbo].[States] s ON sl.StateId = s.Id
    WHERE sl.LanguageId = @LanguageId AND sl.IsActive = 1 AND s.IsActive = 1
    ORDER BY s.Name;
END
GO

-- QualificationLanguage_GetByQualificationId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QualificationLanguage_GetByQualificationId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QualificationLanguage_GetByQualificationId]
GO
CREATE PROCEDURE [dbo].[QualificationLanguage_GetByQualificationId]
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ql.Id,
        ql.QualificationId,
        ql.LanguageId,
        ql.Name,
        ql.Description,
        ql.IsActive,
        ql.CreatedAt,
        ql.UpdatedAt,
        l.Code as LanguageCode,
        l.Name as LanguageName
    FROM [dbo].[QualificationLanguages] ql
    INNER JOIN [dbo].[Languages] l ON ql.LanguageId = l.Id
    WHERE ql.QualificationId = @QualificationId AND ql.IsActive = 1 AND l.IsActive = 1
    ORDER BY l.Name;
END
GO

-- StreamLanguage_GetByStreamId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StreamLanguage_GetByStreamId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StreamLanguage_GetByStreamId]
GO
CREATE PROCEDURE [dbo].[StreamLanguage_GetByStreamId]
    @StreamId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        sl.Id,
        sl.StreamId,
        sl.LanguageId,
        sl.Name,
        sl.Description,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt,
        l.Code as LanguageCode,
        l.Name as LanguageName
    FROM [dbo].[StreamLanguages] sl
    INNER JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
    WHERE sl.StreamId = @StreamId AND sl.IsActive = 1 AND l.IsActive = 1
    ORDER BY l.Name;
END
GO

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
        l.Code as LanguageCode,
        l.Name as LanguageName
    FROM [dbo].[SubjectLanguages] sl
    INNER JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
    WHERE sl.SubjectId = @SubjectId AND sl.IsActive = 1 AND l.IsActive = 1
    ORDER BY l.Name;
END
GO

-- ExamLanguage_GetByExamId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamLanguage_GetByExamId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ExamLanguage_GetByExamId]
GO
CREATE PROCEDURE [dbo].[ExamLanguage_GetByExamId]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        el.Id,
        el.ExamId,
        el.LanguageId,
        el.Name,
        el.Description,
        el.IsActive,
        el.CreatedAt,
        el.UpdatedAt,
        l.Code as LanguageCode,
        l.Name as LanguageName
    FROM [dbo].[ExamLanguages] el
    INNER JOIN [dbo].[Languages] l ON el.LanguageId = l.Id
    WHERE el.ExamId = @ExamId AND el.IsActive = 1 AND l.IsActive = 1
    ORDER BY l.Name;
END
GO

-- ExamQualification_GetByExamId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamQualification_GetByExamId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ExamQualification_GetByExamId]
GO
CREATE PROCEDURE [dbo].[ExamQualification_GetByExamId]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        eq.Id,
        eq.ExamId,
        eq.QualificationId,
        eq.StreamId,
        eq.IsActive,
        eq.CreatedAt,
        eq.UpdatedAt,
        q.Name as QualificationName,
        s.Name as StreamName
    FROM [dbo].[ExamQualifications] eq
    LEFT JOIN [dbo].[Qualifications] q ON eq.QualificationId = q.Id AND q.IsActive = 1
    LEFT JOIN [dbo].[Streams] s ON eq.StreamId = s.Id AND s.IsActive = 1
    WHERE eq.ExamId = @ExamId AND eq.IsActive = 1
    ORDER BY q.Name, s.Name;
END
GO

-- ExamQualification_GetByQualificationId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamQualification_GetByQualificationId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ExamQualification_GetByQualificationId]
GO
CREATE PROCEDURE [dbo].[ExamQualification_GetByQualificationId]
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        eq.Id,
        eq.ExamId,
        eq.QualificationId,
        eq.StreamId,
        eq.IsActive,
        eq.CreatedAt,
        eq.UpdatedAt,
        e.Name as ExamName,
        s.Name as StreamName
    FROM [dbo].[ExamQualifications] eq
    INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id AND e.IsActive = 1
    LEFT JOIN [dbo].[Streams] s ON eq.StreamId = s.Id AND s.IsActive = 1
    WHERE eq.QualificationId = @QualificationId AND eq.IsActive = 1
    ORDER BY e.Name, s.Name;
END
GO

-- =====================================================
-- LANGUAGE MAPPING CREATE/UPDATE/DELETE PROCEDURES
-- =====================================================

-- StateLanguage_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StateLanguage_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StateLanguage_Create]
GO
CREATE PROCEDURE [dbo].[StateLanguage_Create]
    @StateId INT,
    @LanguageId INT,
    @Name NVARCHAR(100),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[StateLanguages] (StateId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
    VALUES (@StateId, @LanguageId, @Name, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- QualificationLanguage_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QualificationLanguage_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QualificationLanguage_Create]
GO
CREATE PROCEDURE [dbo].[QualificationLanguage_Create]
    @QualificationId INT,
    @LanguageId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[QualificationLanguages] (QualificationId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@QualificationId, @LanguageId, @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- StreamLanguage_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StreamLanguage_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StreamLanguage_Create]
GO
CREATE PROCEDURE [dbo].[StreamLanguage_Create]
    @StreamId INT,
    @LanguageId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[StreamLanguages] (StreamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@StreamId, @LanguageId, @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- SubjectLanguage_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubjectLanguage_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SubjectLanguage_Create]
GO
CREATE PROCEDURE [dbo].[SubjectLanguage_Create]
    @SubjectId INT,
    @LanguageId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[SubjectLanguages] (SubjectId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@SubjectId, @LanguageId, @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- ExamLanguage_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamLanguage_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ExamLanguage_Create]
GO
CREATE PROCEDURE [dbo].[ExamLanguage_Create]
    @ExamId INT,
    @LanguageId INT,
    @Name NVARCHAR(150),
    @Description NVARCHAR(1000),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[ExamLanguages] (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ExamId, @LanguageId, @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- ExamQualification_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamQualification_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ExamQualification_Create]
GO
CREATE PROCEDURE [dbo].[ExamQualification_Create]
    @ExamId INT,
    @QualificationId INT,
    @StreamId INT,
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[ExamQualifications] (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ExamId, @QualificationId, @StreamId, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

PRINT 'MasterService Stored Procedures - Part 3 (CMS Content & Language Mappings) Completed Successfully!';
GO


PRINT '====================================================';
PRINT 'MasterService All Stored Procedures Generated Successfully!';
PRINT '====================================================';
PRINT 'Entities Covered:';
PRINT '1. Language (6 procedures)';
PRINT '2. State (7 procedures)';
PRINT '3. Country (6 procedures)';
PRINT '4. Qualification (7 procedures)';
PRINT '5. Stream (7 procedures)';
PRINT '6. Subject (6 procedures)';
PRINT '7. Exam (8 procedures)';
PRINT '8. CmsContent (7 procedures)';
PRINT '9. Language Mappings (15+ procedures)';
PRINT '====================================================';
PRINT 'Total Procedures: 69+';
PRINT 'Ready for Dapper Repository Implementation!';
PRINT '====================================================';
GO
