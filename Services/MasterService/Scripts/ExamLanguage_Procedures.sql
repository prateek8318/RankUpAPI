USE [RankUp_MasterDB];
GO

-- =====================================================
-- EXAM LANGUAGE STORED PROCEDURES
-- =====================================================

-- ExamLanguage_GetByExamId
IF OBJECT_ID('dbo.ExamLanguage_GetByExamId', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamLanguage_GetByExamId;
GO
CREATE PROCEDURE dbo.ExamLanguage_GetByExamId
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.ExamId = @ExamId AND el.IsActive = 1;
END
GO

-- ExamLanguage_GetByLanguageId
IF OBJECT_ID('dbo.ExamLanguage_GetByLanguageId', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamLanguage_GetByLanguageId;
GO
CREATE PROCEDURE dbo.ExamLanguage_GetByLanguageId
    @LanguageId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.LanguageId = @LanguageId AND el.IsActive = 1;
END
GO

-- ExamLanguage_Create
IF OBJECT_ID('dbo.ExamLanguage_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamLanguage_Create;
GO
CREATE PROCEDURE dbo.ExamLanguage_Create
    @ExamId     INT,
    @LanguageId INT,
    @Name       NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @IsActive   BIT = 1,
    @CreatedAt  DATETIME2,
    @UpdatedAt  DATETIME2,
    @Id         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ExamId, @LanguageId, @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- ExamLanguage_Update
IF OBJECT_ID('dbo.ExamLanguage_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamLanguage_Update;
GO
CREATE PROCEDURE dbo.ExamLanguage_Update
    @Id         INT,
    @Name       NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @IsActive   BIT,
    @UpdatedAt  DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.ExamLanguages
    SET Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- ExamLanguage_Delete
IF OBJECT_ID('dbo.ExamLanguage_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamLanguage_Delete;
GO
CREATE PROCEDURE dbo.ExamLanguage_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ExamLanguages WHERE Id = @Id;
END
GO

-- =====================================================
-- EXAM QUALIFICATION STORED PROCEDURES
-- =====================================================

-- ExamQualification_GetByExamId
IF OBJECT_ID('dbo.ExamQualification_GetByExamId', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamQualification_GetByExamId;
GO
CREATE PROCEDURE dbo.ExamQualification_GetByExamId
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt,
           q.Name as QualificationName,
           s.Name as StreamName
    FROM dbo.ExamQualifications eq
    LEFT JOIN dbo.Qualifications q ON eq.QualificationId = q.Id
    LEFT JOIN dbo.Streams s ON eq.StreamId = s.Id
    WHERE eq.ExamId = @ExamId AND eq.IsActive = 1;
END
GO

-- ExamQualification_Create
IF OBJECT_ID('dbo.ExamQualification_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamQualification_Create;
GO
CREATE PROCEDURE dbo.ExamQualification_Create
    @ExamId         INT,
    @QualificationId INT,
    @StreamId       INT = NULL,
    @IsActive       BIT = 1,
    @CreatedAt      DATETIME2,
    @UpdatedAt      DATETIME2,
    @Id             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ExamId, @QualificationId, @StreamId, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- ExamQualification_Delete
IF OBJECT_ID('dbo.ExamQualification_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamQualification_Delete;
GO
CREATE PROCEDURE dbo.ExamQualification_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ExamQualifications WHERE Id = @Id;
END
GO

-- =====================================================
-- MISSING EXAM STORED PROCEDURES
-- =====================================================

-- Exam_GetByIdWithRelations
IF OBJECT_ID('dbo.Exam_GetByIdWithRelations', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetByIdWithRelations;
GO
CREATE PROCEDURE dbo.Exam_GetByIdWithRelations
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get exam details
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    WHERE Id = @Id;
    
    -- Get exam languages
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.ExamId = @Id AND el.IsActive = 1;
    
    -- Get exam qualifications
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    WHERE eq.ExamId = @Id AND eq.IsActive = 1;
END
GO

-- Exam_GetByIdWithRelationsLocalized
IF OBJECT_ID('dbo.Exam_GetByIdWithRelationsLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetByIdWithRelationsLocalized;
GO
CREATE PROCEDURE dbo.Exam_GetByIdWithRelationsLocalized
    @Id INT,
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get exam details with localized names
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
    WHERE e.Id = @Id;
    
    -- Get all exam languages for this exam
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.ExamId = @Id AND el.IsActive = 1;
    
    -- Get exam qualifications
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    WHERE eq.ExamId = @Id AND eq.IsActive = 1;
END
GO

-- Exam_GetActiveLocalized
IF OBJECT_ID('dbo.Exam_GetActiveLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetActiveLocalized;
GO
CREATE PROCEDURE dbo.Exam_GetActiveLocalized
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
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
END
GO

-- Exam_GetByFilterLocalized
IF OBJECT_ID('dbo.Exam_GetByFilterLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetByFilterLocalized;
GO
CREATE PROCEDURE dbo.Exam_GetByFilterLocalized
    @LanguageCode     NVARCHAR(10) = NULL,
    @CountryCode      NVARCHAR(10) = NULL,
    @QualificationId  INT = NULL,
    @StreamId         INT = NULL,
    @MinAge           INT = NULL,
    @MaxAge           INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
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
END
GO

-- Exam_Create (with language support)
IF OBJECT_ID('dbo.Exam_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_Create;
GO
CREATE PROCEDURE dbo.Exam_Create
    @Name        NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10) = NULL,
    @MinAge      INT = NULL,
    @MaxAge      INT = NULL,
    @ImageUrl    NVARCHAR(500) = NULL,
    @IsInternational BIT = 0,
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @RelationsJson NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Insert exam
        INSERT INTO dbo.Exams (Name, Description, CountryCode, MinAge, MaxAge, ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt)
        VALUES (@Name, @Description, @CountryCode, @MinAge, @MaxAge, @ImageUrl, @IsInternational, @IsActive, @CreatedAt, @UpdatedAt);
        SET @Id = SCOPE_IDENTITY();
        
        -- Insert exam languages if NamesJson is provided
        IF @NamesJson IS NOT NULL AND @Id > 0
        BEGIN
            INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                LanguageId,
                Name,
                Description,
                1 as IsActive,
                @CreatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@NamesJson) 
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(150) '$.Name',
                Description NVARCHAR(1000) '$.Description'
            );
        END
        
        -- Insert exam qualifications if RelationsJson is provided
        IF @RelationsJson IS NOT NULL AND @Id > 0
        BEGIN
            INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                QualificationId,
                StreamId,
                1 as IsActive,
                @CreatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@RelationsJson) 
            WITH (
                QualificationId INT '$.QualificationId',
                StreamId INT '$.StreamId'
            );
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Exam_Update (with language support)
IF OBJECT_ID('dbo.Exam_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_Update;
GO
CREATE PROCEDURE dbo.Exam_Update
    @Id          INT,
    @Name        NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10) = NULL,
    @MinAge      INT = NULL,
    @MaxAge      INT = NULL,
    @ImageUrl    NVARCHAR(500) = NULL,
    @IsInternational BIT = 0,
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @RelationsJson NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Update exam
        UPDATE dbo.Exams
        SET Name = @Name,
            Description = @Description,
            CountryCode = @CountryCode,
            MinAge = @MinAge,
            MaxAge = @MaxAge,
            ImageUrl = @ImageUrl,
            IsInternational = @IsInternational,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id;
        
        -- Delete existing exam languages for this exam
        DELETE FROM dbo.ExamLanguages WHERE ExamId = @Id;
        
        -- Insert new exam languages if NamesJson is provided
        IF @NamesJson IS NOT NULL
        BEGIN
            INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                LanguageId,
                Name,
                Description,
                1 as IsActive,
                @UpdatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@NamesJson) 
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(150) '$.Name',
                Description NVARCHAR(1000) '$.Description'
            );
        END
        
        -- Delete existing exam qualifications for this exam
        DELETE FROM dbo.ExamQualifications WHERE ExamId = @Id;
        
        -- Insert new exam qualifications if RelationsJson is provided
        IF @RelationsJson IS NOT NULL
        BEGIN
            INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                QualificationId,
                StreamId,
                1 as IsActive,
                @UpdatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@RelationsJson) 
            WITH (
                QualificationId INT '$.QualificationId',
                StreamId INT '$.StreamId'
            );
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Exam_SoftDelete
IF OBJECT_ID('dbo.Exam_SoftDelete', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_SoftDelete;
GO
CREATE PROCEDURE dbo.Exam_SoftDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Exams
    SET IsActive = 0, UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO

-- Exam_SetActive
IF OBJECT_ID('dbo.Exam_SetActive', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_SetActive;
GO
CREATE PROCEDURE dbo.Exam_SetActive
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Exams
    SET IsActive = @IsActive, UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO

-- =====================================================
-- SAMPLE DATA INSERTION
-- =====================================================

-- Insert sample exam languages for existing exams
IF NOT EXISTS (SELECT 1 FROM dbo.ExamLanguages WHERE ExamId = 11)
BEGIN
    -- NIMCET – MCA (English)
    INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (11, 50, N'NIMCET – MCA', N'NIMCET – MCA masters in Computer Applications', 1, GETUTCDATE(), GETUTCDATE());
    
    -- NIMCET – MCA (Hindi)
    INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (11, 49, N'NIMCET – एमसीए', N'NIMCET – कंप्यूटर एप्लिकेशन्स में मास्टर्स', 1, GETUTCDATE(), GETUTCDATE());
    
    -- Test Exam 1538849947 (English)
    INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (15, 50, N'Test Exam 1538849947', N'Test Exam for demonstration purposes', 1, GETUTCDATE(), GETUTCDATE());
    
    -- Test Exam 1538849947 (Hindi)
    INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (15, 49, N'टेस्ट परीक्षा 1538849947', N'प्रदर्शन उद्देश्यों के लिए टेस्ट परीक्षा', 1, GETUTCDATE(), GETUTCDATE());
END
GO

PRINT 'Exam Language and related procedures created successfully!';
