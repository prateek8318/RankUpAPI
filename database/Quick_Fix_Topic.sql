-- Quick fix for topic validation - just change the critical line
USE [RankUp_QuestionDB]
GO

-- Drop and recreate with minimal change
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminCreate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminCreate]
GO

-- Get the existing procedure content and modify just the topic validation part
EXEC('
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
        -- Subject validation (existing code)
        IF COL_LENGTH(''dbo.Subjects'', ''ExamId'') IS NOT NULL
        BEGIN
            DECLARE @SubjectMappedToExam BIT = 0;
            IF NOT EXISTS (SELECT 1 FROM dbo.Subjects s WHERE s.Id = @SubjectId AND s.ExamId = @ExamId)
                SET @SubjectMappedToExam = 0;
            ELSE
                SET @SubjectMappedToExam = 1;
            
            IF @SubjectMappedToExam = 0
            BEGIN
                THROW 50020, ''Invalid mapping: SubjectId is not mapped to ExamId.'', 1;
            END
        END

        -- FIXED: Topic validation only for ModuleId 3
        IF @TopicId IS NOT NULL AND @TopicId <= 0
            SET @TopicId = NULL;

        -- CRITICAL FIX: Only validate topic for ModuleId 3 (Deep Practice)
        IF @TopicId IS NOT NULL AND @ModuleId = 3
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
                THROW 50021, ''Invalid mapping: TopicId is not mapped to the given ExamId and SubjectId.'', 1;
            END
        END

        -- Simple question insertion
        DECLARE @QuestionText NVARCHAR(MAX) = NULL;
        SELECT @QuestionText = j.QuestionText
        FROM OPENJSON(@TranslationsJson)
        WITH (QuestionText NVARCHAR(MAX) ''$.QuestionText'') j;

        INSERT INTO dbo.Questions
        (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, Marks, NegativeMarks, Difficulty,
            CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt, CreatedBy
        )
        VALUES
        (
            @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), ISNULL(@QuestionText, N''''), @Marks, @NegativeMarks, @Difficulty,       
            @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME(), ISNULL(@CreatedBy, 0)
        );

        DECLARE @QuestionId INT = CAST(SCOPE_IDENTITY() AS INT);

        -- Insert translations
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
            LanguageCode NVARCHAR(10) ''$.LanguageCode'',
            QuestionText NVARCHAR(MAX) ''$.QuestionText'',
            OptionA NVARCHAR(2000) ''$.OptionA'',
            OptionB NVARCHAR(2000) ''$.OptionB'',
            OptionC NVARCHAR(2000) ''$.OptionC'',
            OptionD NVARCHAR(2000) ''$.OptionD'',
            Explanation NVARCHAR(MAX) ''$.Explanation'',
            QuestionImageUrl NVARCHAR(500) ''$.QuestionImageUrl'',
            OptionAImageUrl NVARCHAR(500) ''$.OptionAImageUrl'',
            OptionBImageUrl NVARCHAR(500) ''$.OptionBImageUrl'',
            OptionCImageUrl NVARCHAR(500) ''$.OptionCImageUrl'',
            OptionDImageUrl NVARCHAR(500) ''$.OptionDImageUrl''
        ) j;

        COMMIT TRANSACTION;
        SELECT @QuestionId AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
')
GO

PRINT 'Quick fix applied - topic validation now skipped for ModuleId != 3'
GO
