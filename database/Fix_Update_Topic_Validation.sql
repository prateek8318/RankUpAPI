-- Fix Topic Validation for Question_AdminUpdate Procedure
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
    @Marks DECIMAL(5,2),
    @NegativeMarks DECIMAL(5,2),
    @Difficulty INT,
    @CorrectAnswer CHAR(1),
    @SameExplanationForAllLanguages BIT,
    @IsPublished BIT,
    @IsActive BIT,
    @TranslationsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionCount INT = @@TRANCOUNT;
    DECLARE @DifficultyLevel NVARCHAR(20);
    DECLARE @UpdatedBy INT = 1; -- Default admin user
    
    -- Map difficulty enum to string
    SET @DifficultyLevel = CASE @Difficulty 
        WHEN 1 THEN 'Easy'
        WHEN 2 THEN 'Medium'
        WHEN 3 THEN 'Hard'
        ELSE 'Medium'
    END
    
    -- Only start transaction if not already in one
    IF @TransactionCount = 0
        BEGIN TRANSACTION;
    ELSE
        SAVE TRANSACTION QuestionAdminUpdateProc;
    
    BEGIN TRY
        -- Validate question exists
        IF NOT EXISTS (SELECT 1 FROM Questions WHERE Id = @Id)
        BEGIN
            RAISERROR('Question with ID %d does not exist', 16, 1, @Id);
            RETURN -1;
        END
        
        -- Validate Subject exists
        IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = @SubjectId AND IsActive = 1)
        BEGIN
            RAISERROR('Subject with ID %d does not exist or is not active', 16, 1, @SubjectId);
            RETURN -1;
        END
        
        -- Validate Exam exists (if MasterDB is accessible)
        IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'RankUp_MasterDB')
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM [RankUp_MasterDB].[dbo].[Exams] WHERE Id = @ExamId AND IsActive = 1)
            BEGIN
                RAISERROR('Exam with ID %d does not exist or is not active', 16, 1, @ExamId);
                RETURN -1;
            END
        END
        
        -- FIXED: Add topic validation logic similar to Question_AdminCreate
        -- Topic is optional for some modules. Treat NULL/0 as "not provided".
        IF @TopicId IS NOT NULL AND @TopicId <= 0
            SET @TopicId = NULL;
        
        -- FIXED: Only validate topic mapping for ModuleId 3 (Deep Practice)
        -- For ModuleId 1 (Mock Test), 2, 4 - skip topic validation entirely
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
                -- Use the same error number as the create procedure for consistency
                THROW 50021, 'Invalid mapping: TopicId is not mapped to the given ExamId and SubjectId.', 1;
            END
        END
        
        -- Parse translations JSON if provided
        IF @TranslationsJson IS NOT NULL AND LEN(@TranslationsJson) > 0
        BEGIN
            DECLARE @Translations TABLE (
                LanguageCode NVARCHAR(10),
                QuestionText NVARCHAR(MAX),
                OptionA NVARCHAR(500),
                OptionB NVARCHAR(500),
                OptionC NVARCHAR(500),
                OptionD NVARCHAR(500),
                Explanation NVARCHAR(MAX)
            );
            
            INSERT INTO @Translations (LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation)
            SELECT 
                JSON_VALUE(value, '$.LanguageCode'),
                JSON_VALUE(value, '$.QuestionText'),
                JSON_VALUE(value, '$.OptionA'),
                JSON_VALUE(value, '$.OptionB'),
                JSON_VALUE(value, '$.OptionC'),
                JSON_VALUE(value, '$.OptionD'),
                JSON_VALUE(value, '$.Explanation')
            FROM OPENJSON(@TranslationsJson);
            
            -- Update main question with primary language data
            DECLARE @PrimaryQuestionText NVARCHAR(MAX);
            DECLARE @PrimaryOptionA NVARCHAR(500);
            DECLARE @PrimaryOptionB NVARCHAR(500);
            DECLARE @PrimaryOptionC NVARCHAR(500);
            DECLARE @PrimaryOptionD NVARCHAR(500);
            DECLARE @PrimaryExplanation NVARCHAR(MAX);
            
            SELECT TOP 1 
                @PrimaryQuestionText = QuestionText,
                @PrimaryOptionA = OptionA,
                @PrimaryOptionB = OptionB,
                @PrimaryOptionC = OptionC,
                @PrimaryOptionD = OptionD,
                @PrimaryExplanation = Explanation
            FROM @Translations
            WHERE LanguageCode = 'en' OR LanguageCode = 'hi'
            ORDER BY CASE WHEN LanguageCode = 'en' THEN 1 ELSE 2 END;
            
            -- Update main question
            UPDATE Questions SET
                ModuleId = @ModuleId,
                ExamId = @ExamId,
                SubjectId = @SubjectId,
                TopicId = @TopicId,
                QuestionText = ISNULL(@PrimaryQuestionText, QuestionText),
                OptionA = ISNULL(@PrimaryOptionA, OptionA),
                OptionB = ISNULL(@PrimaryOptionB, OptionB),
                OptionC = ISNULL(@PrimaryOptionC, OptionC),
                OptionD = ISNULL(@PrimaryOptionD, OptionD),
                CorrectAnswer = @CorrectAnswer,
                Explanation = ISNULL(@PrimaryExplanation, Explanation),
                Marks = @Marks,
                NegativeMarks = @NegativeMarks,
                DifficultyLevel = @DifficultyLevel,
                IsPublished = @IsPublished,
                IsActive = @IsActive,
                SameExplanationForAllLanguages = @SameExplanationForAllLanguages,
                UpdatedAt = GETUTCDATE()
            WHERE Id = @Id;
            
            -- Update/delete translations
            DELETE FROM QuestionTranslations WHERE QuestionId = @Id;
            
            INSERT INTO QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation)
            SELECT 
                @Id,
                LanguageCode,
                QuestionText,
                OptionA,
                OptionB,
                OptionC,
                OptionD,
                Explanation
            FROM @Translations
            WHERE LanguageCode != 'en'; -- Don't duplicate English if it's the primary
        END
        ELSE
        BEGIN
            -- Update only basic fields without changing content
            UPDATE Questions SET
                ModuleId = @ModuleId,
                ExamId = @ExamId,
                SubjectId = @SubjectId,
                TopicId = @TopicId,
                CorrectAnswer = @CorrectAnswer,
                Marks = @Marks,
                NegativeMarks = @NegativeMarks,
                DifficultyLevel = @DifficultyLevel,
                IsPublished = @IsPublished,
                IsActive = @IsActive,
                SameExplanationForAllLanguages = @SameExplanationForAllLanguages,
                UpdatedAt = GETUTCDATE()
            WHERE Id = @Id;
        END
        
        -- Commit only if we started the transaction
        IF @TransactionCount = 0
            COMMIT TRANSACTION;
        
        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        -- Rollback only if we started the transaction
        IF @TransactionCount = 0
            ROLLBACK TRANSACTION;
        ELSE
            ROLLBACK TRANSACTION QuestionAdminUpdateProc;
        
        -- Re-throw the error
        THROW;
    END CATCH
END
GO

PRINT 'Question_AdminUpdate procedure updated with proper topic validation!'
GO
