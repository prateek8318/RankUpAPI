-- Fix Question Creation Issues
-- 1. Create Subjects table in QuestionDB or add cross-database reference
-- 2. Fix transaction handling in CreateQuestion procedure

USE [RankUp_QuestionDB]
GO

-- Check if Subjects table exists in QuestionDB, if not create it
IF OBJECT_ID('dbo.Subjects', 'U') IS NULL
BEGIN
    PRINT 'Creating Subjects table in QuestionDB...'
    
    CREATE TABLE [dbo].[Subjects](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [NameEn] [nvarchar](100) NULL,
        [NameHi] [nvarchar](100) NULL,
        [Description] [nvarchar](500) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    -- Create indexes
    CREATE INDEX [IX_Subjects_IsActive] ON [dbo].[Subjects] ([IsActive]);
    CREATE INDEX [IX_Subjects_Name] ON [dbo].[Subjects] ([Name]);
    
    PRINT 'Subjects table created successfully.'
END
ELSE
BEGIN
    PRINT 'Subjects table already exists in QuestionDB.'
END
GO

-- Sync subjects from MasterDB if needed
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'RankUp_MasterDB')
BEGIN
    PRINT 'Syncing subjects from MasterDB...'
    
    -- Insert subjects that don't exist in QuestionDB
    INSERT INTO Subjects (Name, NameEn, NameHi, Description, IsActive, CreatedAt)
    SELECT 
        s.Name,
        s.Name,
        NULL AS NameHi,
        s.Description,
        s.IsActive,
        s.CreatedAt
    FROM [RankUp_MasterDB].[dbo].[Subjects] s
    WHERE NOT EXISTS (
        SELECT 1 FROM Subjects qs 
        WHERE qs.Name = s.Name
    );
    
    PRINT 'Subjects synced from MasterDB.'
END
GO

-- Drop and recreate CreateQuestion procedure with proper transaction handling
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateQuestion]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[CreateQuestion]
GO

PRINT 'Creating fixed CreateQuestion procedure...'
GO

CREATE PROCEDURE [dbo].[CreateQuestion]
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
    @QuestionImageUrl NVARCHAR(500),
    @OptionAImageUrl NVARCHAR(500),
    @OptionBImageUrl NVARCHAR(500),
    @OptionCImageUrl NVARCHAR(500),
    @OptionDImageUrl NVARCHAR(500),
    @ExplanationImageUrl NVARCHAR(500),
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
        SAVE TRANSACTION CreateQuestionProc;
    
    BEGIN TRY
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
        
        -- Insert the question
        INSERT INTO Questions (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
            CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
            QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
            ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy
        )
        VALUES (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
            @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
            @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
            @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy
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
            ROLLBACK TRANSACTION CreateQuestionProc;
        
        -- Re-throw the error
        THROW;
    END CATCH
END
GO

-- Also fix CreateQuestionWithTranslation procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateQuestionWithTranslation]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[CreateQuestionWithTranslation]
GO

PRINT 'Creating fixed CreateQuestionWithTranslation procedure...'
GO

CREATE PROCEDURE [dbo].[CreateQuestionWithTranslation]
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
    @LanguageCode NVARCHAR(10) = 'en',
    @TranslatedQuestionText NVARCHAR(MAX) = NULL,
    @TranslatedOptionA NVARCHAR(500) = NULL,
    @TranslatedOptionB NVARCHAR(500) = NULL,
    @TranslatedOptionC NVARCHAR(500) = NULL,
    @TranslatedOptionD NVARCHAR(500) = NULL,
    @TranslatedExplanation NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionCount INT = @@TRANCOUNT;
    DECLARE @QuestionId INT;
    
    -- Only start transaction if not already in one
    IF @TransactionCount = 0
        BEGIN TRANSACTION;
    ELSE
        SAVE TRANSACTION CreateQuestionWithTransProc;
    
    BEGIN TRY
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
        
        -- Insert the question
        INSERT INTO Questions (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
            CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
            QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
            ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy
        )
        VALUES (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
            @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
            @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
            @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy
        );
        
        SET @QuestionId = SCOPE_IDENTITY();
        
        -- Insert translation if provided
        IF @TranslatedQuestionText IS NOT NULL OR @TranslatedOptionA IS NOT NULL OR 
           @TranslatedOptionB IS NOT NULL OR @TranslatedOptionC IS NOT NULL OR 
           @TranslatedOptionD IS NOT NULL OR @TranslatedExplanation IS NOT NULL
        BEGIN
            INSERT INTO QuestionTranslations (
                QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
            )
            VALUES (
                @QuestionId, @LanguageCode, 
                ISNULL(@TranslatedQuestionText, @QuestionText),
                ISNULL(@TranslatedOptionA, @OptionA),
                ISNULL(@TranslatedOptionB, @OptionB),
                ISNULL(@TranslatedOptionC, @OptionC),
                ISNULL(@TranslatedOptionD, @OptionD),
                ISNULL(@TranslatedExplanation, @Explanation)
            );
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
            ROLLBACK TRANSACTION CreateQuestionWithTransProc;
        
        -- Re-throw the error
        THROW;
    END CATCH
END
GO

PRINT 'Question creation fixes applied successfully!'
GO
