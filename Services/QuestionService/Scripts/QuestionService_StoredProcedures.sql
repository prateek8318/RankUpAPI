PRINT 'Creating QuestionService Flow-3 tables and procedures...';
GO

/* 
   This script is intentionally idempotent and schema-drift tolerant.
   Some dev DBs may already have partial/older versions of these tables.
*/

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

-- Backfill/upgrade columns if the table already existed
IF COL_LENGTH('dbo.Topics', 'ExamId') IS NULL
    ALTER TABLE dbo.Topics ADD ExamId INT NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Topics', 'IsActive') IS NULL
    ALTER TABLE dbo.Topics ADD IsActive BIT NOT NULL DEFAULT(1);
GO

IF COL_LENGTH('dbo.Topics', 'CreatedAt') IS NULL
    ALTER TABLE dbo.Topics ADD CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME());
GO

IF COL_LENGTH('dbo.Topics', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.Topics ADD UpdatedAt DATETIME2 NULL;
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
        Marks DECIMAL(10,2) NOT NULL DEFAULT(1),
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

-- Backfill/upgrade columns if the table already existed
IF COL_LENGTH('dbo.Questions', 'ModuleId') IS NULL
    ALTER TABLE dbo.Questions ADD ModuleId INT NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Questions', 'ExamId') IS NULL
    ALTER TABLE dbo.Questions ADD ExamId INT NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Questions', 'SubjectId') IS NULL
    ALTER TABLE dbo.Questions ADD SubjectId INT NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Questions', 'TopicId') IS NULL
    ALTER TABLE dbo.Questions ADD TopicId INT NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Questions', 'Marks') IS NULL
    ALTER TABLE dbo.Questions ADD Marks DECIMAL(10,2) NOT NULL DEFAULT(1);
GO

IF COL_LENGTH('dbo.Questions', 'NegativeMarks') IS NULL
    ALTER TABLE dbo.Questions ADD NegativeMarks DECIMAL(10,2) NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Questions', 'Difficulty') IS NULL
    ALTER TABLE dbo.Questions ADD Difficulty INT NOT NULL DEFAULT(2);
GO

IF COL_LENGTH('dbo.Questions', 'CorrectAnswer') IS NULL
    ALTER TABLE dbo.Questions ADD CorrectAnswer NVARCHAR(1) NOT NULL DEFAULT('A');
GO

IF COL_LENGTH('dbo.Questions', 'SameExplanationForAllLanguages') IS NULL
    ALTER TABLE dbo.Questions ADD SameExplanationForAllLanguages BIT NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Questions', 'IsPublished') IS NULL
    ALTER TABLE dbo.Questions ADD IsPublished BIT NOT NULL DEFAULT(0);
GO

IF COL_LENGTH('dbo.Questions', 'PublishedAt') IS NULL
    ALTER TABLE dbo.Questions ADD PublishedAt DATETIME2 NULL;
GO

IF COL_LENGTH('dbo.Questions', 'IsActive') IS NULL
    ALTER TABLE dbo.Questions ADD IsActive BIT NOT NULL DEFAULT(1);
GO

IF COL_LENGTH('dbo.Questions', 'CreatedAt') IS NULL
    ALTER TABLE dbo.Questions ADD CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME());
GO

IF COL_LENGTH('dbo.Questions', 'UpdatedAt') IS NULL
    ALTER TABLE dbo.Questions ADD UpdatedAt DATETIME2 NULL;
GO

-- Columns used by quiz/session SPs and image update API
IF COL_LENGTH('dbo.Questions', 'QuestionImageUrl') IS NULL
    ALTER TABLE dbo.Questions ADD QuestionImageUrl NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.Questions', 'OptionAImageUrl') IS NULL
    ALTER TABLE dbo.Questions ADD OptionAImageUrl NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.Questions', 'OptionBImageUrl') IS NULL
    ALTER TABLE dbo.Questions ADD OptionBImageUrl NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.Questions', 'OptionCImageUrl') IS NULL
    ALTER TABLE dbo.Questions ADD OptionCImageUrl NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.Questions', 'OptionDImageUrl') IS NULL
    ALTER TABLE dbo.Questions ADD OptionDImageUrl NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.Questions', 'ExplanationImageUrl') IS NULL
    ALTER TABLE dbo.Questions ADD ExplanationImageUrl NVARCHAR(500) NULL;
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

-- Backfill/upgrade columns if the table already existed
IF COL_LENGTH('dbo.QuestionTranslations', 'QuestionImageUrl') IS NULL
    ALTER TABLE dbo.QuestionTranslations ADD QuestionImageUrl NVARCHAR(500) NULL;
GO
IF COL_LENGTH('dbo.QuestionTranslations', 'OptionAImageUrl') IS NULL
    ALTER TABLE dbo.QuestionTranslations ADD OptionAImageUrl NVARCHAR(500) NULL;
GO
IF COL_LENGTH('dbo.QuestionTranslations', 'OptionBImageUrl') IS NULL
    ALTER TABLE dbo.QuestionTranslations ADD OptionBImageUrl NVARCHAR(500) NULL;
GO
IF COL_LENGTH('dbo.QuestionTranslations', 'OptionCImageUrl') IS NULL
    ALTER TABLE dbo.QuestionTranslations ADD OptionCImageUrl NVARCHAR(500) NULL;
GO
IF COL_LENGTH('dbo.QuestionTranslations', 'OptionDImageUrl') IS NULL
    ALTER TABLE dbo.QuestionTranslations ADD OptionDImageUrl NVARCHAR(500) NULL;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_QuestionTranslations_QuestionId_LanguageCode'
      AND object_id = OBJECT_ID('dbo.QuestionTranslations')
)
BEGIN
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
    DECLARE @ResolvedExamId INT = NULL;

    IF COL_LENGTH('dbo.Subjects', 'ExamId') IS NOT NULL
    BEGIN
        DECLARE @TopicCreateSql NVARCHAR(MAX) = N'
            SELECT TOP 1 @ResolvedExamIdOut = s.ExamId
            FROM dbo.Subjects s
            WHERE s.Id = @SubjectIdIn;';

        EXEC sp_executesql
            @TopicCreateSql,
            N'@SubjectIdIn INT, @ResolvedExamIdOut INT OUTPUT',
            @SubjectIdIn = @SubjectId,
            @ResolvedExamIdOut = @ResolvedExamId OUTPUT;
    END
    ELSE IF OBJECT_ID('dbo.ExamSubjects', 'U') IS NOT NULL
    BEGIN
        IF @ExamId IS NOT NULL AND @ExamId > 0
        BEGIN
            IF EXISTS
            (
                SELECT 1
                FROM dbo.ExamSubjects es
                WHERE es.SubjectId = @SubjectId
                  AND es.ExamId = @ExamId
                  AND ISNULL(es.IsActive, 1) = 1
            )
            BEGIN
                SET @ResolvedExamId = @ExamId;
            END
        END
        ELSE
        BEGIN
            SELECT TOP 1 @ResolvedExamId = es.ExamId
            FROM dbo.ExamSubjects es
            WHERE es.SubjectId = @SubjectId
              AND ISNULL(es.IsActive, 1) = 1
            ORDER BY es.ExamId;
        END
    END
    ELSE IF EXISTS (SELECT 1 FROM dbo.Subjects s WHERE s.Id = @SubjectId)
    BEGIN
        -- Fallback for environments without explicit exam-subject mapping table.
        SET @ResolvedExamId = @ExamId;
    END

    IF @ResolvedExamId IS NULL
    BEGIN
        THROW 50010, 'Invalid SubjectId: subject not found for topic creation.', 1;
    END

    IF @ExamId IS NOT NULL AND @ExamId > 0 AND @ExamId <> @ResolvedExamId
    BEGIN
        THROW 50011, 'Subject does not belong to provided ExamId.', 1;
    END

    INSERT INTO dbo.Topics (Name, SubjectId, ExamId, IsActive, CreatedAt)
    VALUES (@Name, @SubjectId, @ResolvedExamId, 1, SYSUTCDATETIME());
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
    @TopicId INT = NULL,
    @Marks INT,
    @NegativeMarks DECIMAL(10,2),
    @Difficulty INT,
    @CorrectAnswer NVARCHAR(1),
    @SameExplanationForAllLanguages BIT,
    @IsPublished BIT,
    @TranslationsJson NVARCHAR(MAX),
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF COL_LENGTH('dbo.Subjects', 'ExamId') IS NOT NULL
        BEGIN
            DECLARE @SubjectMappedToExam BIT = 0;
            DECLARE @SubjectExamCheckSql NVARCHAR(MAX) = N'
                IF EXISTS (
                    SELECT 1
                    FROM dbo.Subjects s
                    WHERE s.Id = @SubjectIdIn
                      AND s.ExamId = @ExamIdIn
                )
                    SELECT @IsMappedOut = 1;
                ELSE
                    SELECT @IsMappedOut = 0;';

            EXEC sp_executesql
                @SubjectExamCheckSql,
                N'@SubjectIdIn INT, @ExamIdIn INT, @IsMappedOut BIT OUTPUT',
                @SubjectIdIn = @SubjectId,
                @ExamIdIn = @ExamId,
                @IsMappedOut = @SubjectMappedToExam OUTPUT;

            IF @SubjectMappedToExam = 0
            BEGIN
                THROW 50020, 'Invalid mapping: SubjectId is not mapped to ExamId.', 1;
            END
        END
        ELSE IF OBJECT_ID('dbo.ExamSubjects', 'U') IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM dbo.ExamSubjects es
                WHERE es.SubjectId = @SubjectId
                  AND es.ExamId = @ExamId
                  AND ISNULL(es.IsActive, 1) = 1
            )
            BEGIN
                THROW 50020, 'Invalid mapping: SubjectId is not mapped to ExamId.', 1;
            END
        END
        ELSE IF NOT EXISTS (
            SELECT 1
            FROM dbo.Subjects s
            WHERE s.Id = @SubjectId
        )
        BEGIN
            THROW 50020, 'Invalid mapping: SubjectId is not mapped to ExamId.', 1;
        END

        -- Topic is optional for some modules. Treat NULL/0 as "not provided".
        IF @TopicId IS NOT NULL AND @TopicId <= 0
            SET @TopicId = NULL;

        IF @TopicId IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM dbo.Topics t
                WHERE t.Id = @TopicId
                  AND t.SubjectId = @SubjectId
                  AND t.ExamId = @ExamId
                  AND t.IsActive = 1
            )
            BEGIN
                THROW 50021, 'Invalid mapping: TopicId is not mapped to the given ExamId and SubjectId.', 1;
            END
        END

        DECLARE @QuestionText NVARCHAR(MAX) = NULL;
        SELECT @QuestionText = j.QuestionText
        FROM OPENJSON(@TranslationsJson)
        WITH (QuestionText NVARCHAR(MAX) '$.QuestionText') j;

        -- Insert into dbo.Questions with schema-drift tolerance:
        -- some DBs have QuestionText on Questions, some keep it only in translations.
        DECLARE @CreatedByResolved INT = ISNULL(@CreatedBy, 0);

        IF COL_LENGTH('dbo.Questions', 'QuestionText') IS NOT NULL AND COL_LENGTH('dbo.Questions', 'CreatedBy') IS NOT NULL
        BEGIN
            INSERT INTO dbo.Questions
            (
                ModuleId, ExamId, SubjectId, TopicId, QuestionText, Marks, NegativeMarks, Difficulty,
                CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt, CreatedBy
            )
            VALUES
            (
                @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), ISNULL(@QuestionText, N''), @Marks, @NegativeMarks, @Difficulty,
                @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME(), @CreatedByResolved
            );
        END
        ELSE IF COL_LENGTH('dbo.Questions', 'QuestionText') IS NOT NULL AND COL_LENGTH('dbo.Questions', 'CreatedBy') IS NULL
        BEGIN
            INSERT INTO dbo.Questions
            (
                ModuleId, ExamId, SubjectId, TopicId, QuestionText, Marks, NegativeMarks, Difficulty,
                CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt
            )
            VALUES
            (
                @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), ISNULL(@QuestionText, N''), @Marks, @NegativeMarks, @Difficulty,
                @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME()
            );
        END
        ELSE
        BEGIN
            -- If QuestionText column doesn't exist, still try to set CreatedBy when present
            IF COL_LENGTH('dbo.Questions', 'CreatedBy') IS NOT NULL
            BEGIN
                INSERT INTO dbo.Questions
                (
                    ModuleId, ExamId, SubjectId, TopicId, Marks, NegativeMarks, Difficulty,
                    CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt, CreatedBy
                )
                VALUES
                (
                    @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), @Marks, @NegativeMarks, @Difficulty,
                    @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME(), @CreatedByResolved
                );
            END
            ELSE
            BEGIN
                INSERT INTO dbo.Questions
                (
                    ModuleId, ExamId, SubjectId, TopicId, Marks, NegativeMarks, Difficulty,
                    CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt
                )
                VALUES
                (
                    @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), @Marks, @NegativeMarks, @Difficulty,
                    @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME()
                );
            END
        END

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
        IF COL_LENGTH('dbo.Subjects', 'ExamId') IS NOT NULL
        BEGIN
            DECLARE @SubjectMappedToExam BIT = 0;
            DECLARE @SubjectExamCheckSql NVARCHAR(MAX) = N'
                IF EXISTS (
                    SELECT 1
                    FROM dbo.Subjects s
                    WHERE s.Id = @SubjectIdIn
                      AND s.ExamId = @ExamIdIn
                )
                    SELECT @IsMappedOut = 1;
                ELSE
                    SELECT @IsMappedOut = 0;';

            EXEC sp_executesql
                @SubjectExamCheckSql,
                N'@SubjectIdIn INT, @ExamIdIn INT, @IsMappedOut BIT OUTPUT',
                @SubjectIdIn = @SubjectId,
                @ExamIdIn = @ExamId,
                @IsMappedOut = @SubjectMappedToExam OUTPUT;

            IF @SubjectMappedToExam = 0
            BEGIN
                THROW 50022, 'Invalid mapping: SubjectId is not mapped to ExamId.', 1;
            END
        END
        ELSE IF OBJECT_ID('dbo.ExamSubjects', 'U') IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM dbo.ExamSubjects es
                WHERE es.SubjectId = @SubjectId
                  AND es.ExamId = @ExamId
                  AND ISNULL(es.IsActive, 1) = 1
            )
            BEGIN
                THROW 50022, 'Invalid mapping: SubjectId is not mapped to ExamId.', 1;
            END
        END
        ELSE IF NOT EXISTS (
            SELECT 1
            FROM dbo.Subjects s
            WHERE s.Id = @SubjectId
        )
        BEGIN
            THROW 50022, 'Invalid mapping: SubjectId is not mapped to ExamId.', 1;
        END

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Topics t
            WHERE t.Id = @TopicId
              AND t.SubjectId = @SubjectId
              AND t.ExamId = @ExamId
              AND t.IsActive = 1
        )
        BEGIN
            THROW 50023, 'Invalid mapping: TopicId is not mapped to the given ExamId and SubjectId.', 1;
        END

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

PRINT 'Applying MockTests schema compatibility patch...';
GO

IF OBJECT_ID('dbo.MockTests', 'U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.MockTests', 'MockTestType') IS NULL
        ALTER TABLE dbo.MockTests ADD MockTestType INT NOT NULL CONSTRAINT DF_MockTests_MockTestType DEFAULT(1);

    IF COL_LENGTH('dbo.MockTests', 'SubjectId') IS NULL
        ALTER TABLE dbo.MockTests ADD SubjectId INT NULL;

    IF COL_LENGTH('dbo.MockTests', 'TopicId') IS NULL
        ALTER TABLE dbo.MockTests ADD TopicId INT NULL;

    IF COL_LENGTH('dbo.MockTests', 'MarksPerQuestion') IS NULL
        ALTER TABLE dbo.MockTests ADD MarksPerQuestion DECIMAL(10,2) NOT NULL CONSTRAINT DF_MockTests_MarksPerQuestion DEFAULT(0);

    IF COL_LENGTH('dbo.MockTests', 'HasNegativeMarking') IS NULL
        ALTER TABLE dbo.MockTests ADD HasNegativeMarking BIT NOT NULL CONSTRAINT DF_MockTests_HasNegativeMarking DEFAULT(0);

    IF COL_LENGTH('dbo.MockTests', 'NegativeMarkingValue') IS NULL
        ALTER TABLE dbo.MockTests ADD NegativeMarkingValue DECIMAL(10,2) NULL;

    IF COL_LENGTH('dbo.MockTests', 'Status') IS NULL
        ALTER TABLE dbo.MockTests ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_MockTests_Status DEFAULT('Active');

    IF COL_LENGTH('dbo.MockTests', 'Year') IS NULL
        ALTER TABLE dbo.MockTests ADD [Year] INT NULL;

    IF COL_LENGTH('dbo.MockTests', 'Difficulty') IS NULL
        ALTER TABLE dbo.MockTests ADD Difficulty NVARCHAR(50) NULL;

    IF COL_LENGTH('dbo.MockTests', 'PaperCode') IS NULL
        ALTER TABLE dbo.MockTests ADD PaperCode NVARCHAR(100) NULL;

    IF COL_LENGTH('dbo.MockTests', 'ExamDate') IS NULL
        ALTER TABLE dbo.MockTests ADD ExamDate DATETIME2 NULL;

    IF COL_LENGTH('dbo.MockTests', 'PublishDateTime') IS NULL
        ALTER TABLE dbo.MockTests ADD PublishDateTime DATETIME2 NULL;

    IF COL_LENGTH('dbo.MockTests', 'ValidTill') IS NULL
        ALTER TABLE dbo.MockTests ADD ValidTill DATETIME2 NULL;

    IF COL_LENGTH('dbo.MockTests', 'ShowResultType') IS NULL
        ALTER TABLE dbo.MockTests ADD ShowResultType NVARCHAR(20) NOT NULL CONSTRAINT DF_MockTests_ShowResultType DEFAULT('1');

    IF COL_LENGTH('dbo.MockTests', 'ImageUrl') IS NULL
        ALTER TABLE dbo.MockTests ADD ImageUrl NVARCHAR(500) NULL;
END
GO

PRINT 'MockTests schema compatibility patch completed.';
GO
