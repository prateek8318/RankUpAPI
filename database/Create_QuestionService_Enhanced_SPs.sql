USE [RankUp_QuestionDB]
GO

-- Create Quiz Sessions Table
IF OBJECT_ID('dbo.QuizSessions', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuizSessions](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [ExamId] [int] NOT NULL,
        [UserId] [int] NOT NULL,
        [LanguageCode] [nvarchar](10) NOT NULL DEFAULT 'en',
        [StartTime] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [EndTime] [datetime2](7) NULL,
        [Duration] [int] NOT NULL DEFAULT 60, -- in minutes
        [TotalQuestions] [int] NOT NULL DEFAULT 0,
        [AnsweredQuestions] [int] NOT NULL DEFAULT 0,
        [MarkedForReview] [int] NOT NULL DEFAULT 0,
        [TotalMarks] [decimal](10,2) NOT NULL DEFAULT 0,
        [ObtainedMarks] [decimal](10,2) NOT NULL DEFAULT 0,
        [Status] [nvarchar](20) NOT NULL DEFAULT 'NotStarted', -- NotStarted, InProgress, Completed, Submitted
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_QuizSessions] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Create Quiz Answers Table
IF OBJECT_ID('dbo.QuizAnswers', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuizAnswers](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [QuizSessionId] [int] NOT NULL,
        [QuestionId] [int] NOT NULL,
        [Answer] [nvarchar](1) NOT NULL, -- A, B, C, D
        [MarkForReview] [bit] NOT NULL DEFAULT 0,
        [TimeSpent] [int] NOT NULL DEFAULT 0, -- in seconds
        [IsCorrect] [bit] NULL,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_QuizAnswers] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_QuizAnswers_QuizSessions_QuizSessionId] FOREIGN KEY([QuizSessionId]) REFERENCES [dbo].[QuizSessions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QuizAnswers_Questions_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Questions] ([Id])
    );
END
GO

-- Create Quiz Results Table
IF OBJECT_ID('dbo.QuizResults', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuizResults](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [QuizSessionId] [int] NOT NULL,
        [TotalQuestions] [int] NOT NULL,
        [CorrectAnswers] [int] NOT NULL DEFAULT 0,
        [WrongAnswers] [int] NOT NULL DEFAULT 0,
        [SkippedQuestions] [int] NOT NULL DEFAULT 0,
        [TotalMarks] [decimal](10,2) NOT NULL DEFAULT 0,
        [ObtainedMarks] [decimal](10,2) NOT NULL DEFAULT 0,
        [Percentage] [decimal](5,2) NOT NULL DEFAULT 0,
        [Grade] [nvarchar](10) NULL,
        [CompletedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_QuizResults] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_QuizResults_QuizSessions_QuizSessionId] FOREIGN KEY([QuizSessionId]) REFERENCES [dbo].[QuizSessions] ([Id]) ON DELETE CASCADE
    );
END
GO

-- Create Indexes for Quiz Tables
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuizSessions_ExamId' AND object_id = OBJECT_ID('dbo.QuizSessions'))
    CREATE INDEX [IX_QuizSessions_ExamId] ON [dbo].[QuizSessions] ([ExamId])
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuizSessions_UserId' AND object_id = OBJECT_ID('dbo.QuizSessions'))
    CREATE INDEX [IX_QuizSessions_UserId] ON [dbo].[QuizSessions] ([UserId])
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuizSessions_Status' AND object_id = OBJECT_ID('dbo.QuizSessions'))
    CREATE INDEX [IX_QuizSessions_Status] ON [dbo].[QuizSessions] ([Status])
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuizAnswers_QuizSessionId' AND object_id = OBJECT_ID('dbo.QuizAnswers'))
    CREATE INDEX [IX_QuizAnswers_QuizSessionId] ON [dbo].[QuizAnswers] ([QuizSessionId])
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_QuizAnswers_QuestionId' AND object_id = OBJECT_ID('dbo.QuizAnswers'))
    CREATE INDEX [IX_QuizAnswers_QuestionId] ON [dbo].[QuizAnswers] ([QuestionId])
GO

-- Bulk Create Questions Stored Procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BulkCreateQuestions]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[BulkCreateQuestions]
GO
CREATE PROCEDURE [dbo].[BulkCreateQuestions]
    @FileName NVARCHAR(500),
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT = NULL,
    @UploadedBy INT,
    @LanguageCode NVARCHAR(10) = 'en',
    @SkipDuplicates BIT = 0,
    @ValidateOnly BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Create batch record
        INSERT INTO QuestionBatches (BatchName, FileName, FilePath, UploadedBy, Status)
        VALUES (CONCAT('Bulk Upload - ', FORMAT(GETDATE(), 'yyyy-MM-dd HH:mm')), @FileName, @FileName, @UploadedBy, 'Processing');
        
        DECLARE @BatchId INT = SCOPE_IDENTITY();
        DECLARE @SuccessCount INT = 0;
        DECLARE @FailedCount INT = 0;
        
        -- For demo purposes, we'll insert a sample question
        -- In real implementation, this would parse the Excel/CSV file
        IF @ValidateOnly = 0
        BEGIN
            INSERT INTO Questions (
                ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
                CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType, CreatedBy
            )
            VALUES (
                @ExamId, @SubjectId, @TopicId, 
                'Which of the following is the best conductor of electricity?',
                'Cold Water', 'Saline Water', 'Distilled Water', 'Warm Water',
                'B', 'Saline water contains dissolved salts which make it conductive', 1.00, 0.25, 'Easy', 'MCQ', @UploadedBy
            );
            
            SET @SuccessCount = 1;
        END
        
        -- Update batch status
        UPDATE QuestionBatches 
        SET TotalQuestions = @SuccessCount + @FailedCount,
            ProcessedQuestions = @SuccessCount + @FailedCount,
            FailedQuestions = @FailedCount,
            Status = CASE WHEN @ValidateOnly = 1 THEN 'Validated' ELSE 'Completed' END,
            UpdatedAt = GETDATE()
        WHERE Id = @BatchId;
        
        COMMIT TRANSACTION;
        
        SELECT @BatchId AS BatchId;
        SELECT @SuccessCount AS SuccessCount, @FailedCount AS FailedCount;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Update batch with error
        UPDATE QuestionBatches 
        SET Status = 'Failed',
            ErrorMessage = ERROR_MESSAGE(),
            UpdatedAt = GETDATE()
        WHERE Id = @BatchId;
        
        THROW;
    END CATCH
END
GO

-- Start Quiz Session Stored Procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StartQuizSession]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[StartQuizSession]
GO
CREATE PROCEDURE [dbo].[StartQuizSession]
    @ExamId INT,
    @UserId INT,
    @LanguageCode NVARCHAR(10) = 'en',
    @SubjectId INT = NULL,
    @TopicId INT = NULL,
    @NumberOfQuestions INT = 100,
    @DifficultyLevel NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Create quiz session
        INSERT INTO QuizSessions (ExamId, UserId, LanguageCode, Duration, Status)
        VALUES (@ExamId, @UserId, @LanguageCode, 60, 'InProgress');
        
        DECLARE @SessionId INT = SCOPE_IDENTITY();
        
        -- Get random questions for the quiz
        SELECT 
            q.Id,
            CASE 
                WHEN qt.LanguageCode = @LanguageCode THEN qt.QuestionText
                ELSE q.QuestionText
            END AS QuestionText,
            CASE 
                WHEN qt.LanguageCode = @LanguageCode THEN qt.OptionA
                ELSE q.OptionA
            END AS OptionA,
            CASE 
                WHEN qt.LanguageCode = @LanguageCode THEN qt.OptionB
                ELSE q.OptionB
            END AS OptionB,
            CASE 
                WHEN qt.LanguageCode = @LanguageCode THEN qt.OptionC
                ELSE q.OptionC
            END AS OptionC,
            CASE 
                WHEN qt.LanguageCode = @LanguageCode THEN qt.OptionD
                ELSE q.OptionD
            END AS OptionD,
            q.QuestionImageUrl,
            q.OptionAImageUrl,
            q.OptionBImageUrl,
            q.OptionCImageUrl,
            q.OptionDImageUrl,
            q.Marks,
            q.NegativeMarks,
            q.DifficultyLevel,
            q.CorrectAnswer,
            ROW_NUMBER() OVER (ORDER BY NEWID()) AS QuestionNumber
        FROM Questions q
        LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId AND qt.LanguageCode = @LanguageCode
        WHERE q.ExamId = @ExamId
        AND q.IsPublished = 1
        AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
        AND (@TopicId IS NULL OR q.TopicId = @TopicId)
        AND (@DifficultyLevel IS NULL OR q.DifficultyLevel = @DifficultyLevel)
        ORDER BY QuestionNumber
        OFFSET 0 ROWS FETCH NEXT @NumberOfQuestions ROWS ONLY;
        
        -- Update session with total questions and marks
        UPDATE QuizSessions
        SET TotalQuestions = @NumberOfQuestions,
            TotalMarks = (SELECT SUM(Marks) FROM Questions WHERE Id IN (
                SELECT TOP (@NumberOfQuestions) Id FROM Questions 
                WHERE ExamId = @ExamId AND IsPublished = 1
                ORDER BY NEWID()
            ))
        WHERE Id = @SessionId;
        
        COMMIT TRANSACTION;
        
        SELECT @SessionId AS SessionId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Save Quiz Answer Stored Procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveQuizAnswer]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SaveQuizAnswer]
GO
CREATE PROCEDURE [dbo].[SaveQuizAnswer]
    @QuizSessionId INT,
    @QuestionId INT,
    @Answer NVARCHAR(1),
    @MarkForReview BIT = 0,
    @TimeSpent INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Check if answer already exists
        IF EXISTS (SELECT 1 FROM QuizAnswers WHERE QuizSessionId = @QuizSessionId AND QuestionId = @QuestionId)
        BEGIN
            -- Update existing answer
            UPDATE QuizAnswers
            SET Answer = @Answer,
                MarkForReview = @MarkForReview,
                TimeSpent = @TimeSpent,
                IsCorrect = CASE WHEN @Answer = (SELECT CorrectAnswer FROM Questions WHERE Id = @QuestionId) THEN 1 ELSE 0 END
            WHERE QuizSessionId = @QuizSessionId AND QuestionId = @QuestionId;
        END
        ELSE
        BEGIN
            -- Insert new answer
            INSERT INTO QuizAnswers (QuizSessionId, QuestionId, Answer, MarkForReview, TimeSpent, IsCorrect)
            VALUES (@QuizSessionId, @QuestionId, @Answer, @MarkForReview, @TimeSpent, 
                    CASE WHEN @Answer = (SELECT CorrectAnswer FROM Questions WHERE Id = @QuestionId) THEN 1 ELSE 0 END);
        END
        
        -- Update session statistics
        UPDATE QuizSessions
        SET AnsweredQuestions = (
                SELECT COUNT(*) FROM QuizAnswers 
                WHERE QuizSessionId = @QuizSessionId
            ),
            MarkedForReview = (
                SELECT COUNT(*) FROM QuizAnswers 
                WHERE QuizSessionId = @QuizSessionId AND MarkForReview = 1
            ),
            ObtainedMarks = (
                SELECT SUM(
                    CASE WHEN IsCorrect = 1 THEN q.Marks 
                    ELSE -q.NegativeMarks 
                    END
                ) FROM QuizAnswers qa
                INNER JOIN Questions q ON qa.QuestionId = q.Id
                WHERE qa.QuizSessionId = @QuizSessionId
            ),
            UpdatedAt = GETDATE()
        WHERE Id = @QuizSessionId;
        
        COMMIT TRANSACTION;
        
        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Submit Quiz Stored Procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubmitQuiz]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubmitQuiz]
GO
CREATE PROCEDURE [dbo].[SubmitQuiz]
    @QuizSessionId INT,
    @Answers NVARCHAR(MAX) = NULL -- JSON string of answers
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Update session status
        UPDATE QuizSessions
        SET Status = 'Submitted',
            EndTime = GETDATE(),
            UpdatedAt = GETDATE()
        WHERE Id = @QuizSessionId;
        
        -- Calculate and create quiz result
        DECLARE @TotalQuestions INT = (SELECT TotalQuestions FROM QuizSessions WHERE Id = @QuizSessionId);
        DECLARE @CorrectAnswers INT = (
            SELECT COUNT(*) FROM QuizAnswers 
            WHERE QuizSessionId = @QuizSessionId AND IsCorrect = 1
        );
        DECLARE @WrongAnswers INT = (
            SELECT COUNT(*) FROM QuizAnswers 
            WHERE QuizSessionId = @QuizSessionId AND IsCorrect = 0
        );
        DECLARE @SkippedQuestions INT = @TotalQuestions - @CorrectAnswers - @WrongAnswers;
        DECLARE @TotalMarks DECIMAL(10,2) = (SELECT TotalMarks FROM QuizSessions WHERE Id = @QuizSessionId);
        DECLARE @ObtainedMarks DECIMAL(10,2) = (SELECT ObtainedMarks FROM QuizSessions WHERE Id = @QuizSessionId);
        DECLARE @Percentage DECIMAL(5,2) = CASE WHEN @TotalMarks > 0 THEN (@ObtainedMarks / @TotalMarks) * 100 ELSE 0 END;
        DECLARE @Grade NVARCHAR(10) = CASE 
            WHEN @Percentage >= 90 THEN 'A+'
            WHEN @Percentage >= 80 THEN 'A'
            WHEN @Percentage >= 70 THEN 'B+'
            WHEN @Percentage >= 60 THEN 'B'
            WHEN @Percentage >= 50 THEN 'C'
            ELSE 'D'
        END;
        
        INSERT INTO QuizResults (
            QuizSessionId, TotalQuestions, CorrectAnswers, WrongAnswers, SkippedQuestions,
            TotalMarks, ObtainedMarks, Percentage, Grade
        )
        VALUES (
            @QuizSessionId, @TotalQuestions, @CorrectAnswers, @WrongAnswers, @SkippedQuestions,
            @TotalMarks, @ObtainedMarks, @Percentage, @Grade
        );
        
        DECLARE @ResultId INT = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        -- Return quiz result with question details
        SELECT 
            qr.*,
            qs.StartTime,
            qs.EndTime,
            DATEDIFF(SECOND, qs.StartTime, qs.EndTime) AS Duration
        FROM QuizResults qr
        INNER JOIN QuizSessions qs ON qr.QuizSessionId = qs.Id
        WHERE qr.Id = @ResultId;
        
        -- Return question results
        SELECT 
            q.Id AS QuestionId,
            CASE 
                WHEN qt.LanguageCode = qs.LanguageCode THEN qt.QuestionText
                ELSE q.QuestionText
            END AS QuestionText,
            CASE 
                WHEN qt.LanguageCode = qs.LanguageCode THEN qt.OptionA
                ELSE q.OptionA
            END AS OptionA,
            CASE 
                WHEN qt.LanguageCode = qs.LanguageCode THEN qt.OptionB
                ELSE q.OptionB
            END AS OptionB,
            CASE 
                WHEN qt.LanguageCode = qs.LanguageCode THEN qt.OptionC
                ELSE q.OptionC
            END AS OptionC,
            CASE 
                WHEN qt.LanguageCode = qs.LanguageCode THEN qt.OptionD
                ELSE q.OptionD
            END AS OptionD,
            q.CorrectAnswer,
            qa.Answer AS UserAnswer,
            qa.IsCorrect,
            q.Marks,
            q.NegativeMarks,
            CASE WHEN qa.IsCorrect = 1 THEN q.Marks ELSE -q.NegativeMarks END AS ObtainedMarks,
            qa.MarkForReview AS WasMarkedForReview,
            qa.TimeSpent
        FROM QuizAnswers qa
        INNER JOIN Questions q ON qa.QuestionId = q.Id
        INNER JOIN QuizSessions qs ON qa.QuizSessionId = qs.Id
        LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId AND qt.LanguageCode = qs.LanguageCode
        WHERE qa.QuizSessionId = @QuizSessionId
        ORDER BY q.Id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Get Quiz Session Stored Procedure
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetQuizSession]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[GetQuizSession]
GO
CREATE PROCEDURE [dbo].[GetQuizSession]
    @SessionId INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        qs.Id,
        qs.ExamId,
        e.Name AS ExamName,
        qs.UserId,
        qs.LanguageCode,
        qs.StartTime,
        qs.EndTime,
        qs.Duration,
        qs.TotalQuestions,
        qs.AnsweredQuestions,
        qs.MarkedForReview,
        qs.TotalMarks,
        qs.ObtainedMarks,
        qs.Status,
        qa.QuestionId,
        qa.Answer AS SelectedAnswer,
        qa.MarkForReview AS IsMarkedForReview,
        qa.TimeSpent,
        q.QuestionText,
        q.OptionA,
        q.OptionB,
        q.OptionC,
        q.OptionD,
        q.QuestionImageUrl,
        q.OptionAImageUrl,
        q.OptionBImageUrl,
        q.OptionCImageUrl,
        q.OptionDImageUrl,
        q.Marks,
        q.NegativeMarks,
        q.DifficultyLevel,
        ROW_NUMBER() OVER (ORDER BY q.Id) AS QuestionNumber
    FROM QuizSessions qs
    INNER JOIN Exams e ON qs.ExamId = e.Id
    LEFT JOIN QuizAnswers qa ON qs.Id = qa.QuizSessionId
    LEFT JOIN Questions q ON qa.QuestionId = q.Id
    WHERE qs.Id = @SessionId AND qs.UserId = @UserId
    ORDER BY q.Id;
END
GO

PRINT 'QuestionService Enhanced Stored Procedures Created Successfully'
