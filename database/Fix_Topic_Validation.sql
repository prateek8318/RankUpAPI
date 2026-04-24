-- Fix Topic Validation for Different Module Types
-- Update Question_AdminCreate to handle null TopicId properly

USE [RankUp_QuestionDB]
GO

-- Drop existing procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminCreate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminCreate]
GO

PRINT 'Creating updated Question_AdminCreate procedure with flexible TopicId...'
GO

CREATE PROCEDURE [dbo].[Question_AdminCreate]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT = NULL, -- Make TopicId optional
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
    @QuestionImageUrl NVARCHAR(500) = NULL,
    @OptionAImageUrl NVARCHAR(500) = NULL,
    @OptionBImageUrl NVARCHAR(500) = NULL,
    @OptionCImageUrl NVARCHAR(500) = NULL,
    @OptionDImageUrl NVARCHAR(500) = NULL,
    @ExplanationImageUrl NVARCHAR(500) = NULL,
    @SameExplanationForAllLanguages BIT,
    @Reference NVARCHAR(500),
    @Tags NVARCHAR(MAX),
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionCount INT = @@TRANCOUNT;
    DECLARE @QuestionId INT;
    
    -- Only start transaction if not already in one
    IF @TransactionCount = 0
        BEGIN TRANSACTION;
    ELSE
        SAVE TRANSACTION QuestionAdminCreateProc;
    
    BEGIN TRY
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
        
        -- Topic validation based on ModuleId
        -- ModuleId 3 (Deep Practice) requires TopicId
        -- ModuleId 1, 2, 4 - TopicId is optional
        IF @ModuleId = 3 AND @TopicId IS NULL
        BEGIN
            RAISERROR('Topic is required for ModuleId 3 (Deep Practice)', 16, 1);
            RETURN -1;
        END
        
        -- If TopicId is provided, validate it exists and is mapped correctly
        IF @TopicId IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM Topics WHERE Id = @TopicId AND IsActive = 1)
            BEGIN
                RAISERROR('Topic with ID %d does not exist or is not active', 16, 1, @TopicId);
                RETURN -1;
            END
            
            -- Additional validation: Topic should belong to the specified Subject
            IF NOT EXISTS (SELECT 1 FROM Topics WHERE Id = @TopicId AND SubjectId = @SubjectId AND IsActive = 1)
            BEGIN
                RAISERROR('Topic %d is not mapped to Subject %d', 16, 1, @TopicId, @SubjectId);
                RETURN -1;
            END
        END
        
        -- Insert the main question with all fields (TopicId can be NULL)
        INSERT INTO Questions (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
            CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
            QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
            ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy, CreatedAt, IsActive
        )
        VALUES (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
            @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
            @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
            @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy, GETUTCDATE(), 1
        );
        
        SET @QuestionId = SCOPE_IDENTITY();
        
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

PRINT 'Question_AdminCreate procedure updated with flexible TopicId validation!'
GO

-- Test the procedure with different ModuleIds
PRINT 'Testing with ModuleId 1 (Mock Test) - TopicId should be optional...'

-- Test 1: ModuleId 1 without TopicId (should work)
DECLARE @Test1 INT;
EXEC @Test1 = [dbo].[Question_AdminCreate] 
    @ModuleId=1, @ExamId=31, @SubjectId=8, @TopicId=NULL,
    @QuestionText='Test Mock Test Question', @OptionA='A', @OptionB='B', @OptionC='C', @OptionD='D',
    @CorrectAnswer='B', @Explanation='Test Explanation', @Marks=1.0, @NegativeMarks=0.25,
    @DifficultyLevel='Medium', @QuestionType='MCQ', @SameExplanationForAllLanguages=0,
    @Reference='Test', @Tags='test', @CreatedBy=1;
PRINT CONCAT('ModuleId 1 without TopicId: QuestionId = ', ISNULL(@Test1, 'NULL'));

-- Test 2: ModuleId 3 without TopicId (should fail)
BEGIN TRY
    DECLARE @Test2 INT;
    EXEC @Test2 = [dbo].[Question_AdminCreate] 
        @ModuleId=3, @ExamId=31, @SubjectId=8, @TopicId=NULL,
        @QuestionText='Test Deep Practice Question', @OptionA='A', @OptionB='B', @OptionC='C', @OptionD='D',
        @CorrectAnswer='B', @Explanation='Test Explanation', @Marks=1.0, @NegativeMarks=0.25,
        @DifficultyLevel='Medium', @QuestionType='MCQ', @SameExplanationForAllLanguages=0,
        @Reference='Test', @Tags='test', @CreatedBy=1;
    PRINT CONCAT('ModuleId 3 without TopicId: QuestionId = ', ISNULL(@Test2, 'NULL'));
END TRY
BEGIN CATCH
    PRINT 'ModuleId 3 without TopicId: FAILED as expected - Topic is required for Deep Practice';
END CATCH

-- Test 3: ModuleId 3 with TopicId (should work)
DECLARE @Test3 INT;
EXEC @Test3 = [dbo].[Question_AdminCreate] 
    @ModuleId=3, @ExamId=31, @SubjectId=8, @TopicId=1,
    @QuestionText='Test Deep Practice Question with Topic', @OptionA='A', @OptionB='B', @OptionC='C', @OptionD='D',
    @CorrectAnswer='B', @Explanation='Test Explanation', @Marks=1.0, @NegativeMarks=0.25,
    @DifficultyLevel='Medium', @QuestionType='MCQ', @SameExplanationForAllLanguages=0,
    @Reference='Test', @Tags='test', @CreatedBy=1;
PRINT CONCAT('ModuleId 3 with TopicId: QuestionId = ', ISNULL(@Test3, 'NULL'));

PRINT 'All tests completed!'
GO
