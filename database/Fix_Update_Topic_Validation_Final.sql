-- Fix Topic Validation for Question_AdminUpdate Procedure - FINAL VERSION
-- Skip topic validation for ModuleId != 3 (Deep Practice)

USE [RankUp_QuestionDB]
GO

-- Drop existing procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminUpdate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminUpdate]
GO

PRINT 'Creating updated Question_AdminUpdate procedure with proper topic validation...'
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

        -- FIXED: Topic validation only for ModuleId 3 (Deep Practice)
        -- For ModuleId 1 (Mock Test), 2, 4 - skip topic validation entirely
        IF @ModuleId = 3 AND (@TopicId IS NULL OR @TopicId <= 0)
        BEGIN
            THROW 50023, 'Topic is required for ModuleId 3 (Deep Practice).', 1;
        END

        -- Only validate topic mapping if TopicId is provided AND ModuleId is 3 (Deep Practice)
        IF @TopicId IS NOT NULL AND @TopicId > 0 AND @ModuleId = 3
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
                THROW 50023, 'Invalid mapping: TopicId is not mapped to the given ExamId and SubjectId.', 1;
            END
        END

        -- For ModuleId != 3, set TopicId to NULL to avoid any issues
        DECLARE @FinalTopicId INT = NULL;
        IF @ModuleId = 3 AND @TopicId IS NOT NULL AND @TopicId > 0
            SET @FinalTopicId = @TopicId;

        UPDATE dbo.Questions
        SET ModuleId = @ModuleId,
            ExamId = @ExamId,
            SubjectId = @SubjectId,
            TopicId = @FinalTopicId,
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

PRINT 'Question_AdminUpdate procedure updated with proper topic validation!'
GO
