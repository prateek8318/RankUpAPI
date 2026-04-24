-- Fix Question_AdminCreate Procedure
-- Handle null image URLs and include all required fields from DTO

USE [RankUp_QuestionDB]
GO

-- Drop existing procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminCreate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminCreate]
GO

PRINT 'Creating fixed Question_AdminCreate procedure...'
GO

CREATE PROCEDURE [dbo].[Question_AdminCreate]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT,
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
        
        -- Insert the main question with all fields
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

PRINT 'Question_AdminCreate procedure fixed successfully!'
GO
