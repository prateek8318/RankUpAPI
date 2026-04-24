-- Create Question For MockTest Procedure
-- This procedure creates a question using MockTest context and automatically maps it

USE [RankUp_QuestionDB]
GO

-- Drop existing procedure if it exists
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_CreateForMockTest]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_CreateForMockTest]
GO

PRINT 'Creating Question_CreateForMockTest procedure...'
GO

CREATE PROCEDURE [dbo].[Question_CreateForMockTest]
    @MockTestId INT,
    @QuestionText NVARCHAR(MAX),
    @OptionA NVARCHAR(500),
    @OptionB NVARCHAR(500),
    @OptionC NVARCHAR(500),
    @OptionD NVARCHAR(500),
    @CorrectAnswer CHAR(1),
    @Explanation NVARCHAR(MAX),
    @Marks DECIMAL(5,2),
    @NegativeMarks DECIMAL(5,2),
    @DifficultyLevel NVARCHAR(20),
    @QuestionType NVARCHAR(20),
    @QuestionImageUrl NVARCHAR(500),
    @OptionAImageUrl NVARCHAR(500),
    @OptionBImageUrl NVARCHAR(500),
    @OptionCImageUrl NVARCHAR(500),
    @OptionDImageUrl NVARCHAR(500),
    @ExplanationImageUrl NVARCHAR(500),
    @SameExplanationForAllLanguages BIT,
    @Reference NVARCHAR(500),
    @Tags NVARCHAR(MAX),
    @CreatedBy INT,
    @TranslationsJson NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionCount INT = @@TRANCOUNT;
    DECLARE @QuestionId INT;
    DECLARE @ModuleId INT;
    DECLARE @ExamId INT;
    DECLARE @SubjectId INT;
    DECLARE @TopicId INT;
    
    -- Only start transaction if not already in one
    IF @TransactionCount = 0
        BEGIN TRANSACTION;
    ELSE
        SAVE TRANSACTION QuestionCreateForMockTestProc;
    
    BEGIN TRY
        -- Get MockTest details to extract context
        SELECT 
            @ModuleId = MockTestType,
            @ExamId = ExamId,
            @SubjectId = SubjectId,
            @TopicId = TopicId
        FROM MockTests
        WHERE Id = @MockTestId AND IsActive = 1;
        
        -- Validate MockTest exists
        IF @ModuleId IS NULL
        BEGIN
            RAISERROR('MockTest with ID %d does not exist or is not active', 16, 1, @MockTestId);
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
        
        -- Insert the question with MockTest context
        INSERT INTO Questions (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
            CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
            QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
            ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy, CreatedAt
        )
        VALUES (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
            @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
            @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
            @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy, GETUTCDATE()
        );
        
        SET @QuestionId = SCOPE_IDENTITY();
        
        -- Insert translations if provided
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
        
        -- Automatically add question to MockTest
        DECLARE @QuestionNumber INT = (
            SELECT ISNULL(MAX(QuestionNumber), 0) + 1 
            FROM MockTestQuestions 
            WHERE MockTestId = @MockTestId
        );
        
        INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
        VALUES (@MockTestId, @QuestionId, @QuestionNumber, @Marks, @NegativeMarks);
        
        -- Commit only if we started the transaction
        IF @TransactionCount = 0
            COMMIT TRANSACTION;
        
        SELECT @QuestionId AS QuestionId, @QuestionNumber AS QuestionNumber;
    END TRY
    BEGIN CATCH
        -- Rollback only if we started the transaction
        IF @TransactionCount = 0
            ROLLBACK TRANSACTION;
        ELSE
            ROLLBACK TRANSACTION QuestionCreateForMockTestProc;
        
        -- Re-throw the error
        THROW;
    END CATCH
END
GO

PRINT 'Question_CreateForMockTest procedure created successfully!'
GO
