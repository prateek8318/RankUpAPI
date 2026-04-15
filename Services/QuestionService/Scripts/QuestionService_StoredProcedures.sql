PRINT 'Creating QuestionService Flow-3 tables and procedures...';
GO

IF OBJECT_ID('dbo.Topics', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Topics
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        SubjectId INT NOT NULL,
        ExamId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.Questions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Questions
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ModuleId INT NOT NULL,
        ExamId INT NOT NULL,
        SubjectId INT NOT NULL,
        TopicId INT NOT NULL,
        Marks INT NOT NULL DEFAULT(1),
        NegativeMarks DECIMAL(10,2) NOT NULL DEFAULT(0),
        Difficulty INT NOT NULL,
        CorrectAnswer NVARCHAR(1) NOT NULL,
        SameExplanationForAllLanguages BIT NOT NULL DEFAULT(0),
        IsPublished BIT NOT NULL DEFAULT(0),
        PublishedAt DATETIME2 NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF OBJECT_ID('dbo.QuestionTranslations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuestionTranslations
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        QuestionId INT NOT NULL,
        LanguageCode NVARCHAR(10) NOT NULL,
        QuestionText NVARCHAR(MAX) NOT NULL,
        OptionA NVARCHAR(2000) NOT NULL,
        OptionB NVARCHAR(2000) NOT NULL,
        OptionC NVARCHAR(2000) NOT NULL,
        OptionD NVARCHAR(2000) NOT NULL,
        Explanation NVARCHAR(MAX) NULL,
        QuestionImageUrl NVARCHAR(500) NULL,
        OptionAImageUrl NVARCHAR(500) NULL,
        OptionBImageUrl NVARCHAR(500) NULL,
        OptionCImageUrl NVARCHAR(500) NULL,
        OptionDImageUrl NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_QuestionTranslations_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_QuestionTranslations_QuestionId_LanguageCode
        ON dbo.QuestionTranslations(QuestionId, LanguageCode);
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Topic_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Topic_Create]
GO
CREATE PROCEDURE [dbo].[Topic_Create]
    @Name NVARCHAR(200),
    @SubjectId INT,
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Topics (Name, SubjectId, ExamId, IsActive, CreatedAt)
    VALUES (@Name, @SubjectId, @ExamId, 1, SYSUTCDATETIME());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Topic_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Topic_GetAll]
GO
CREATE PROCEDURE [dbo].[Topic_GetAll]
    @SubjectId INT = NULL,
    @ExamId INT = NULL,
    @IncludeInactive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, SubjectId, ExamId, IsActive
    FROM dbo.Topics
    WHERE (@IncludeInactive = 1 OR IsActive = 1)
      AND (@SubjectId IS NULL OR SubjectId = @SubjectId)
      AND (@ExamId IS NULL OR ExamId = @ExamId)
    ORDER BY Name;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminCreate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminCreate]
GO
CREATE PROCEDURE [dbo].[Question_AdminCreate]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT,
    @Marks INT,
    @NegativeMarks DECIMAL(10,2),
    @Difficulty INT,
    @CorrectAnswer NVARCHAR(1),
    @SameExplanationForAllLanguages BIT,
    @IsPublished BIT,
    @TranslationsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.Questions
        (
            ModuleId, ExamId, SubjectId, TopicId, Marks, NegativeMarks, Difficulty,
            CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt
        )
        VALUES
        (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @Marks, @NegativeMarks, @Difficulty,
            @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME()
        );

        DECLARE @QuestionId INT = CAST(SCOPE_IDENTITY() AS INT);

        INSERT INTO dbo.QuestionTranslations
        (
            QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD,
            Explanation, QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl, CreatedAt
        )
        SELECT
            @QuestionId,
            j.LanguageCode,
            j.QuestionText,
            j.OptionA,
            j.OptionB,
            j.OptionC,
            j.OptionD,
            j.Explanation,
            j.QuestionImageUrl,
            j.OptionAImageUrl,
            j.OptionBImageUrl,
            j.OptionCImageUrl,
            j.OptionDImageUrl,
            SYSUTCDATETIME()
        FROM OPENJSON(@TranslationsJson)
        WITH
        (
            LanguageCode NVARCHAR(10) '$.LanguageCode',
            QuestionText NVARCHAR(MAX) '$.QuestionText',
            OptionA NVARCHAR(2000) '$.OptionA',
            OptionB NVARCHAR(2000) '$.OptionB',
            OptionC NVARCHAR(2000) '$.OptionC',
            OptionD NVARCHAR(2000) '$.OptionD',
            Explanation NVARCHAR(MAX) '$.Explanation',
            QuestionImageUrl NVARCHAR(500) '$.QuestionImageUrl',
            OptionAImageUrl NVARCHAR(500) '$.OptionAImageUrl',
            OptionBImageUrl NVARCHAR(500) '$.OptionBImageUrl',
            OptionCImageUrl NVARCHAR(500) '$.OptionCImageUrl',
            OptionDImageUrl NVARCHAR(500) '$.OptionDImageUrl'
        ) j;

        COMMIT TRANSACTION;
        SELECT @QuestionId AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminUpdate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminUpdate]
GO
CREATE PROCEDURE [dbo].[Question_AdminUpdate]
    @Id INT,
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT,
    @Marks INT,
    @NegativeMarks DECIMAL(10,2),
    @Difficulty INT,
    @CorrectAnswer NVARCHAR(1),
    @SameExplanationForAllLanguages BIT,
    @IsPublished BIT,
    @IsActive BIT,
    @TranslationsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.Questions
        SET ModuleId = @ModuleId,
            ExamId = @ExamId,
            SubjectId = @SubjectId,
            TopicId = @TopicId,
            Marks = @Marks,
            NegativeMarks = @NegativeMarks,
            Difficulty = @Difficulty,
            CorrectAnswer = @CorrectAnswer,
            SameExplanationForAllLanguages = @SameExplanationForAllLanguages,
            IsPublished = @IsPublished,
            PublishedAt = CASE WHEN @IsPublished = 1 THEN ISNULL(PublishedAt, SYSUTCDATETIME()) ELSE NULL END,
            IsActive = @IsActive,
            UpdatedAt = SYSUTCDATETIME()
        WHERE Id = @Id;

        DELETE FROM dbo.QuestionTranslations WHERE QuestionId = @Id;

        INSERT INTO dbo.QuestionTranslations
        (
            QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD,
            Explanation, QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl, CreatedAt
        )
        SELECT
            @Id,
            j.LanguageCode,
            j.QuestionText,
            j.OptionA,
            j.OptionB,
            j.OptionC,
            j.OptionD,
            j.Explanation,
            j.QuestionImageUrl,
            j.OptionAImageUrl,
            j.OptionBImageUrl,
            j.OptionCImageUrl,
            j.OptionDImageUrl,
            SYSUTCDATETIME()
        FROM OPENJSON(@TranslationsJson)
        WITH
        (
            LanguageCode NVARCHAR(10) '$.LanguageCode',
            QuestionText NVARCHAR(MAX) '$.QuestionText',
            OptionA NVARCHAR(2000) '$.OptionA',
            OptionB NVARCHAR(2000) '$.OptionB',
            OptionC NVARCHAR(2000) '$.OptionC',
            OptionD NVARCHAR(2000) '$.OptionD',
            Explanation NVARCHAR(MAX) '$.Explanation',
            QuestionImageUrl NVARCHAR(500) '$.QuestionImageUrl',
            OptionAImageUrl NVARCHAR(500) '$.OptionAImageUrl',
            OptionBImageUrl NVARCHAR(500) '$.OptionBImageUrl',
            OptionCImageUrl NVARCHAR(500) '$.OptionCImageUrl',
            OptionDImageUrl NVARCHAR(500) '$.OptionDImageUrl'
        ) j;

        COMMIT TRANSACTION;
        SELECT @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminGetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminGetById]
GO
CREATE PROCEDURE [dbo].[Question_AdminGetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id, ModuleId, ExamId, SubjectId, TopicId, Marks, NegativeMarks, Difficulty,
        CorrectAnswer, SameExplanationForAllLanguages, IsPublished, IsActive
    FROM dbo.Questions
    WHERE Id = @Id;

    SELECT
        QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD,
        Explanation, QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl
    FROM dbo.QuestionTranslations
    WHERE QuestionId = @Id
    ORDER BY LanguageCode;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminGetPaged]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminGetPaged]
GO
CREATE PROCEDURE [dbo].[Question_AdminGetPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ModuleId INT = NULL,
    @SubjectId INT = NULL,
    @ExamId INT = NULL,
    @TopicId INT = NULL,
    @Difficulty INT = NULL,
    @LanguageCode NVARCHAR(10) = NULL,
    @IsPublished BIT = NULL,
    @IncludeInactive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 20;

    ;WITH Filtered AS
    (
        SELECT q.Id, q.ModuleId, q.ExamId, q.SubjectId, q.TopicId, q.Difficulty, q.Marks, q.NegativeMarks, q.IsPublished, q.IsActive, q.CreatedAt
        FROM dbo.Questions q
        WHERE (@IncludeInactive = 1 OR q.IsActive = 1)
          AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
          AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
          AND (@ExamId IS NULL OR q.ExamId = @ExamId)
          AND (@TopicId IS NULL OR q.TopicId = @TopicId)
          AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
          AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished)
    )
    SELECT
        f.Id, f.ModuleId, f.ExamId, f.SubjectId, f.TopicId, f.Difficulty, f.Marks, f.NegativeMarks, f.IsPublished, f.IsActive, f.CreatedAt,
        t.QuestionText AS DisplayQuestionText,
        t.LanguageCode
    FROM Filtered f
    OUTER APPLY
    (
        SELECT TOP 1 qt.QuestionText, qt.LanguageCode
        FROM dbo.QuestionTranslations qt
        WHERE qt.QuestionId = f.Id
          AND (@LanguageCode IS NULL OR qt.LanguageCode = @LanguageCode)
        ORDER BY CASE WHEN qt.LanguageCode = ISNULL(@LanguageCode, 'en') THEN 0 ELSE 1 END
    ) t
    ORDER BY f.CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.Questions q
    WHERE (@IncludeInactive = 1 OR q.IsActive = 1)
      AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
      AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
      AND (@ExamId IS NULL OR q.ExamId = @ExamId)
      AND (@TopicId IS NULL OR q.TopicId = @TopicId)
      AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
      AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished);
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminDashboardStats]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminDashboardStats]
GO
CREATE PROCEDURE [dbo].[Question_AdminDashboardStats]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        COUNT(1) AS TotalQuestions,
        SUM(CASE WHEN CAST(CreatedAt AS DATE) = CAST(SYSUTCDATETIME() AS DATE) THEN 1 ELSE 0 END) AS AddedToday,
        SUM(CASE WHEN NegativeMarks > 0 THEN 1 ELSE 0 END) AS NegativeMarksCount,
        SUM(CASE WHEN IsPublished = 0 THEN 1 ELSE 0 END) AS UnpublishedCount
    FROM dbo.Questions
    WHERE IsActive = 1;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_SetPublishStatus]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_SetPublishStatus]
GO
CREATE PROCEDURE [dbo].[Question_SetPublishStatus]
    @Id INT,
    @IsPublished BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Questions
    SET IsPublished = @IsPublished,
        PublishedAt = CASE WHEN @IsPublished = 1 THEN ISNULL(PublishedAt, SYSUTCDATETIME()) ELSE NULL END,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @Id;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_BulkCreate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_BulkCreate]
GO
CREATE PROCEDURE [dbo].[Question_BulkCreate]
    @QuestionsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    -- JSON array should follow CreateQuestionAdminDto shape.
    -- For controlled rollout, this SP currently returns payload size as inserted count placeholder.
    SELECT COUNT(1) AS InsertedCount
    FROM OPENJSON(@QuestionsJson);
END
GO

PRINT 'QuestionService Flow-3 procedures created successfully.';
GO
