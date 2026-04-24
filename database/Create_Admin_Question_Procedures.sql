-- Create Admin Question Stored Procedures
-- These procedures are called by QuestionService for admin operations

USE [RankUp_QuestionDB]
GO

-- Drop existing procedures if they exist
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminCreate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminCreate]
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminUpdate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminUpdate]
GO

PRINT 'Creating Question_AdminCreate procedure...'
GO

CREATE PROCEDURE [dbo].[Question_AdminCreate]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT,
    @Marks DECIMAL(5,2),
    @NegativeMarks DECIMAL(5,2),
    @Difficulty INT, -- 1=Easy, 2=Medium, 3=Hard
    @CorrectAnswer CHAR(1),
    @SameExplanationForAllLanguages BIT,
    @IsPublished BIT,
    @TranslationsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionCount INT = @@TRANCOUNT;
    DECLARE @QuestionId INT;
    DECLARE @QuestionText NVARCHAR(MAX);
    DECLARE @OptionA NVARCHAR(500);
    DECLARE @OptionB NVARCHAR(500);
    DECLARE @OptionC NVARCHAR(500);
    DECLARE @OptionD NVARCHAR(500);
    DECLARE @Explanation NVARCHAR(MAX);
    DECLARE @DifficultyLevel NVARCHAR(20);
    DECLARE @CreatedBy INT = 1; -- Default admin user, should be passed if available
    
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
        SAVE TRANSACTION QuestionAdminCreateProc;
    
    BEGIN TRY
        -- Parse translations JSON to get primary language data (English or first language)
        -- For simplicity, we'll extract the first translation as primary question data
        DECLARE @Translations TABLE (
            LanguageCode NVARCHAR(10),
            QuestionText NVARCHAR(MAX),
            OptionA NVARCHAR(500),
            OptionB NVARCHAR(500),
            OptionC NVARCHAR(500),
            OptionD NVARCHAR(500),
            Explanation NVARCHAR(MAX),
            QuestionImageUrl NVARCHAR(500),
            OptionAImageUrl NVARCHAR(500),
            OptionBImageUrl NVARCHAR(500),
            OptionCImageUrl NVARCHAR(500),
            OptionDImageUrl NVARCHAR(500)
        );
        
        -- Parse JSON translations (simplified approach)
        IF @TranslationsJson IS NOT NULL AND LEN(@TranslationsJson) > 0
        BEGIN
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
            
            -- Get primary language data (prefer English, else first record)
            SELECT TOP 1 
                @QuestionText = QuestionText,
                @OptionA = OptionA,
                @OptionB = OptionB,
                @OptionC = OptionC,
                @OptionD = OptionD,
                @Explanation = Explanation
            FROM @Translations
            WHERE LanguageCode = 'en' OR LanguageCode = 'hi'
            ORDER BY CASE WHEN LanguageCode = 'en' THEN 1 ELSE 2 END;
        END
        
        -- Validate required fields
        IF @QuestionText IS NULL OR @OptionA IS NULL OR @OptionB IS NULL OR @OptionC IS NULL OR @OptionD IS NULL
        BEGIN
            RAISERROR('Question text and all options are required', 16, 1);
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
        
        -- Insert the main question
        INSERT INTO Questions (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
            CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
            IsPublished, SameExplanationForAllLanguages, CreatedBy, CreatedAt
        )
        VALUES (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
            @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, 'MCQ',
            @IsPublished, @SameExplanationForAllLanguages, @CreatedBy, GETUTCDATE()
        );
        
        SET @QuestionId = SCOPE_IDENTITY();
        
        -- Insert translations if provided
        IF @TranslationsJson IS NOT NULL AND LEN(@TranslationsJson) > 0
        BEGIN
            INSERT INTO QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation)
            SELECT 
                @QuestionId,
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
        
        -- Commit only if we started the transaction
        IF @TransactionCount = 0
            COMMIT TRANSACTION;
        
        SELECT @QuestionId AS QuestionId;
    END TRY
    BEGIN CATCH
        -- Rollback only if we started the transaction
        IF @TransactionCount = 0
            ROLLBACK TRANSACTION;
        ELSE
            ROLLBACK TRANSACTION QuestionAdminCreateProc;
        
        -- Re-throw the error
        THROW;
    END CATCH
END
GO

PRINT 'Creating Question_AdminUpdate procedure...'
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

PRINT 'Admin Question procedures created successfully!'
GO
