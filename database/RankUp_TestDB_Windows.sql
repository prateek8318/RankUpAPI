USE [master]
GO

-- Drop if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RankUp_TestDB')
BEGIN
    ALTER DATABASE [RankUp_TestDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RankUp_TestDB];
END
GO

-- Create with Windows path
CREATE DATABASE [RankUp_TestDB]
ON PRIMARY 
( NAME = N'RankUp_TestDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_TestDB.mdf', SIZE = 8192KB, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'RankUp_TestDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_TestDB_log.ldf', SIZE = 8192KB, FILEGROWTH = 65536KB )
GO

USE [RankUp_TestDB]
GO

-- Tests Table
CREATE TABLE [dbo].[Tests](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](200) NOT NULL,
    [Description] [nvarchar](max) NULL,
    [ExamId] [int] NOT NULL, -- FK to Master Service Exams table
    [SubjectId] [int] NOT NULL, -- FK to Master Service Subjects table
    [TestType] [nvarchar](50) NOT NULL DEFAULT 'Practice', -- Practice, Mock, PreviousYear, ChapterWise
    [DifficultyLevel] [nvarchar](20) NOT NULL DEFAULT 'Mixed', -- Easy, Medium, Hard, Mixed
    [TotalQuestions] [int] NOT NULL DEFAULT 0,
    [DurationMinutes] [int] NOT NULL DEFAULT 60,
    [TotalMarks] [decimal](10,2) NOT NULL DEFAULT 0,
    [PassingMarks] [decimal](10,2) NOT NULL DEFAULT 0,
    [NegativeMarking] [bit] NOT NULL DEFAULT 1,
    [NegativeMarksPerQuestion] [decimal](5,2) NOT NULL DEFAULT 0.25,
    [Instructions] [nvarchar](max) NULL,
    [IsFree] [bit] NOT NULL DEFAULT 0,
    [Price] [decimal](18,2) NOT NULL DEFAULT 0,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'INR',
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [IsPublished] [bit] NOT NULL DEFAULT 0,
    [PublishDate] [datetime2](7) NULL,
    [CreatedBy] [int] NOT NULL, -- Admin user ID
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_Tests] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- TestQuestions Table (Questions in a test)
CREATE TABLE [dbo].[TestQuestions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TestId] [int] NOT NULL,
    [QuestionId] [int] NOT NULL, -- FK to QuestionService Questions table
    [QuestionNumber] [int] NOT NULL,
    [Marks] [decimal](5,2) NOT NULL DEFAULT 1.00,
    [NegativeMarks] [decimal](5,2) NOT NULL DEFAULT 0.00,
    [IsOptional] [bit] NOT NULL DEFAULT 0, -- For optional questions
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TestQuestions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TestQuestions_Tests_TestId] FOREIGN KEY([TestId]) REFERENCES [dbo].[Tests] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_TestQuestions_TestId_QuestionId] UNIQUE ([TestId], [QuestionId])
)
GO

-- TestAttempts Table (User test attempts)
CREATE TABLE [dbo].[TestAttempts](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL, -- FK to User Service Users table
    [TestId] [int] NOT NULL,
    [AttemptNumber] [int] NOT NULL DEFAULT 1,
    [Status] [nvarchar](20) NOT NULL DEFAULT 'InProgress', -- InProgress, Completed, Abandoned, TimeExpired
    [StartTime] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [EndTime] [datetime2](7) NULL,
    [DurationTaken] [int] NULL, -- In seconds
    [TotalQuestions] [int] NOT NULL DEFAULT 0,
    [QuestionsAttempted] [int] NOT NULL DEFAULT 0,
    [CorrectAnswers] [int] NOT NULL DEFAULT 0,
    [WrongAnswers] [int] NOT NULL DEFAULT 0,
    [Unanswered] [int] NOT NULL DEFAULT 0,
    [TotalMarks] [decimal](10,2) NOT NULL DEFAULT 0,
    [ObtainedMarks] [decimal](10,2) NOT NULL DEFAULT 0,
    [Percentage] [decimal](5,2) NOT NULL DEFAULT 0,
    [Rank] [int] NULL,
    [TotalParticipants] [int] NULL,
    [IsPassed] [bit] NULL,
    [IpAddress] [nvarchar](45) NULL,
    [DeviceInfo] [nvarchar](max) NULL, -- Browser/device info
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_TestAttempts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TestAttempts_Tests_TestId] FOREIGN KEY([TestId]) REFERENCES [dbo].[Tests] ([Id])
)
GO

-- AttemptAnswers Table (User's answers in a test attempt)
CREATE TABLE [dbo].[AttemptAnswers](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TestAttemptId] [int] NOT NULL,
    [QuestionId] [int] NOT NULL,
    [SelectedAnswer] [nvarchar](10) NULL, -- A, B, C, D, True, False
    [IsCorrect] [bit] NULL,
    [MarksObtained] [decimal](5,2) NOT NULL DEFAULT 0,
    [NegativeMarks] [decimal](5,2) NOT NULL DEFAULT 0,
    [TimeTaken] [int] NULL, -- In seconds to answer this question
    [IsSkipped] [bit] NOT NULL DEFAULT 0,
    [IsMarkedForReview] [bit] NOT NULL DEFAULT 0,
    [AnsweredAt] [datetime2](7) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_AttemptAnswers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AttemptAnswers_TestAttempts_TestAttemptId] FOREIGN KEY([TestAttemptId]) REFERENCES [dbo].[TestAttempts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_AttemptAnswers_TestAttemptId_QuestionId] UNIQUE ([TestAttemptId], [QuestionId])
)
GO

-- TestResults Table (Final test results and analysis)
CREATE TABLE [dbo].[TestResults](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TestAttemptId] [int] NOT NULL,
    [Section] [nvarchar](100) NULL, -- For section-wise analysis
    [TotalQuestions] [int] NOT NULL DEFAULT 0,
    [CorrectAnswers] [int] NOT NULL DEFAULT 0,
    [WrongAnswers] [int] NOT NULL DEFAULT 0,
    [Unanswered] [int] NOT NULL DEFAULT 0,
    [TotalMarks] [decimal](10,2) NOT NULL DEFAULT 0,
    [ObtainedMarks] [decimal](10,2) NOT NULL DEFAULT 0,
    [Percentage] [decimal](5,2) NOT NULL DEFAULT 0,
    [Accuracy] [decimal](5,2) NOT NULL DEFAULT 0,
    [TimeTaken] [int] NOT NULL DEFAULT 0, -- In seconds
    [AverageTimePerQuestion] [decimal](5,2) NOT NULL DEFAULT 0,
    [DifficultyLevel] [nvarchar](20) NULL,
    [TopicId] [int] NULL, -- For topic-wise analysis
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TestResults] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TestResults_TestAttempts_TestAttemptId] FOREIGN KEY([TestAttemptId]) REFERENCES [dbo].[TestAttempts] ([Id]) ON DELETE CASCADE
)
GO

-- TestAnalytics Table (Analytics data for tests)
CREATE TABLE [dbo].[TestAnalytics](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TestId] [int] NOT NULL,
    [Date] [date] NOT NULL,
    [TotalAttempts] [int] NOT NULL DEFAULT 0,
    [CompletedAttempts] [int] NOT NULL DEFAULT 0,
    [AverageScore] [decimal](5,2) NOT NULL DEFAULT 0,
    [AverageTime] [int] NOT NULL DEFAULT 0,
    [PassPercentage] [decimal](5,2) NOT NULL DEFAULT 0,
    [UniqueUsers] [int] NOT NULL DEFAULT 0,
    [DifficultyWiseStats] [nvarchar](max) NULL, -- JSON data for difficulty-wise stats
    [TopicWiseStats] [nvarchar](max) NULL, -- JSON data for topic-wise stats
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_TestAnalytics] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_TestAnalytics_TestId_Date] UNIQUE ([TestId], [Date])
)
GO

-- TestReviews Table (User reviews and ratings for tests)
CREATE TABLE [dbo].[TestReviews](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TestId] [int] NOT NULL,
    [UserId] [int] NOT NULL,
    [Rating] [int] NOT NULL CHECK ([Rating] BETWEEN 1 AND 5),
    [Review] [nvarchar](1000) NULL,
    [Pros] [nvarchar](max) NULL, -- JSON array of pros
    [Cons] [nvarchar](max) NULL, -- JSON array of cons
    [IsVerified] [bit] NOT NULL DEFAULT 0,
    [HelpfulCount] [int] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_TestReviews] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_TestReviews_TestId_UserId] UNIQUE ([TestId], [UserId])
)
GO

-- Indexes for performance
CREATE INDEX [IX_Tests_ExamId] ON [dbo].[Tests] ([ExamId])
GO
CREATE INDEX [IX_Tests_SubjectId] ON [dbo].[Tests] ([SubjectId])
GO
CREATE INDEX [IX_Tests_TestType] ON [dbo].[Tests] ([TestType])
GO
CREATE INDEX [IX_Tests_DifficultyLevel] ON [dbo].[Tests] ([DifficultyLevel])
GO
CREATE INDEX [IX_Tests_IsActive] ON [dbo].[Tests] ([IsActive])
GO
CREATE INDEX [IX_Tests_IsPublished] ON [dbo].[Tests] ([IsPublished])
GO
CREATE INDEX [IX_Tests_IsFree] ON [dbo].[Tests] ([IsFree])
GO
CREATE INDEX [IX_Tests_CreatedBy] ON [dbo].[Tests] ([CreatedBy])
GO

CREATE INDEX [IX_TestQuestions_TestId] ON [dbo].[TestQuestions] ([TestId])
GO
CREATE INDEX [IX_TestQuestions_QuestionId] ON [dbo].[TestQuestions] ([QuestionId])
GO

CREATE INDEX [IX_TestAttempts_UserId] ON [dbo].[TestAttempts] ([UserId])
GO
CREATE INDEX [IX_TestAttempts_TestId] ON [dbo].[TestAttempts] ([TestId])
GO
CREATE INDEX [IX_TestAttempts_Status] ON [dbo].[TestAttempts] ([Status])
GO
CREATE INDEX [IX_TestAttempts_StartTime] ON [dbo].[TestAttempts] ([StartTime])
GO
CREATE INDEX [IX_TestAttempts_Percentage] ON [dbo].[TestAttempts] ([Percentage])
GO

CREATE INDEX [IX_AttemptAnswers_TestAttemptId] ON [dbo].[AttemptAnswers] ([TestAttemptId])
GO
CREATE INDEX [IX_AttemptAnswers_QuestionId] ON [dbo].[AttemptAnswers] ([QuestionId])
GO
CREATE INDEX [IX_AttemptAnswers_IsCorrect] ON [dbo].[AttemptAnswers] ([IsCorrect])
GO

CREATE INDEX [IX_TestResults_TestAttemptId] ON [dbo].[TestResults] ([TestAttemptId])
GO
CREATE INDEX [IX_TestResults_TopicId] ON [dbo].[TestResults] ([TopicId])
GO

CREATE INDEX [IX_TestAnalytics_TestId] ON [dbo].[TestAnalytics] ([TestId])
GO
CREATE INDEX [IX_TestAnalytics_Date] ON [dbo].[TestAnalytics] ([Date])
GO

CREATE INDEX [IX_TestReviews_TestId] ON [dbo].[TestReviews] ([TestId])
GO
CREATE INDEX [IX_TestReviews_UserId] ON [dbo].[TestReviews] ([UserId])
GO
CREATE INDEX [IX_TestReviews_Rating] ON [dbo].[TestReviews] ([Rating])
GO

-- Stored Procedures

-- Create Test
CREATE PROCEDURE [dbo].[CreateTest]
    @Name NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @ExamId INT,
    @SubjectId INT,
    @TestType NVARCHAR(50),
    @DifficultyLevel NVARCHAR(20),
    @TotalQuestions INT,
    @DurationMinutes INT,
    @TotalMarks DECIMAL(10,2),
    @PassingMarks DECIMAL(10,2),
    @NegativeMarking BIT,
    @NegativeMarksPerQuestion DECIMAL(5,2),
    @Instructions NVARCHAR(MAX),
    @IsFree BIT,
    @Price DECIMAL(18,2),
    @Currency NVARCHAR(3),
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Tests (
        Name, Description, ExamId, SubjectId, TestType, DifficultyLevel, TotalQuestions,
        DurationMinutes, TotalMarks, PassingMarks, NegativeMarking, NegativeMarksPerQuestion,
        Instructions, IsFree, Price, Currency, CreatedBy, CreatedAt, IsActive
    )
    VALUES (
        @Name, @Description, @ExamId, @SubjectId, @TestType, @DifficultyLevel, @TotalQuestions,
        @DurationMinutes, @TotalMarks, @PassingMarks, @NegativeMarking, @NegativeMarksPerQuestion,
        @Instructions, @IsFree, @Price, @Currency, @CreatedBy, GETDATE(), 1
    );
    
    SELECT SCOPE_IDENTITY() AS TestId;
END
GO

-- Add Questions to Test
CREATE PROCEDURE [dbo].[AddQuestionsToTest]
    @TestId INT,
    @QuestionIds NVARCHAR(MAX), -- Comma-separated question IDs
    @Marks NVARCHAR(MAX) = NULL -- Comma-separated marks (optional, defaults to 1 per question)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @QuestionIdTable TABLE (QuestionId INT);
    DECLARE @MarksTable TABLE (QuestionId INT, Marks DECIMAL(5,2));
    
    -- Parse question IDs
    INSERT INTO @QuestionIdTable
    SELECT value FROM STRING_SPLIT(@QuestionIds, ',');
    
    -- Parse marks if provided
    IF @Marks IS NOT NULL
    BEGIN
        DECLARE @Index INT = 1;
        DECLARE @CurrentMark DECIMAL(5,2);
        
        DECLARE @MarkList TABLE (Id INT IDENTITY(1,1), Mark DECIMAL(5,2));
        INSERT INTO @MarkList (Mark)
        SELECT CAST(value AS DECIMAL(5,2)) FROM STRING_SPLIT(@Marks, ',');
        
        INSERT INTO @MarksTable
        SELECT q.QuestionId, m.Mark
        FROM @QuestionIdTable q
        JOIN @MarkList m ON m.Id = @Index;
    END
    
    -- Insert test questions
    IF @Marks IS NULL
    BEGIN
        INSERT INTO TestQuestions (TestId, QuestionId, QuestionNumber, Marks)
        SELECT @TestId, QuestionId, ROW_NUMBER() OVER (ORDER BY QuestionId), 1.00
        FROM @QuestionIdTable;
    END
    ELSE
    BEGIN
        INSERT INTO TestQuestions (TestId, QuestionId, QuestionNumber, Marks)
        SELECT @TestId, q.QuestionId, ROW_NUMBER() OVER (ORDER BY q.QuestionId), ISNULL(m.Marks, 1.00)
        FROM @QuestionIdTable q
        LEFT JOIN @MarksTable m ON q.QuestionId = m.QuestionId;
    END
    
    SELECT @@ROWCOUNT AS QuestionsAdded;
END
GO

-- Start Test Attempt
CREATE PROCEDURE [dbo].[StartTestAttempt]
    @UserId INT,
    @TestId INT,
    @IpAddress NVARCHAR(45),
    @DeviceInfo NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Get test details
        DECLARE @TotalQuestions INT;
        DECLARE @DurationMinutes INT;
        DECLARE @TotalMarks DECIMAL(10,2);
        DECLARE @PassingMarks DECIMAL(10,2);
        DECLARE @NegativeMarking BIT;
        DECLARE @NegativeMarksPerQuestion DECIMAL(5,2);
        
        SELECT @TotalQuestions = TotalQuestions, @DurationMinutes = DurationMinutes,
               @TotalMarks = TotalMarks, @PassingMarks = PassingMarks,
               @NegativeMarking = NegativeMarking, @NegativeMarksPerQuestion = NegativeMarksPerQuestion
        FROM Tests
        WHERE Id = @TestId AND IsActive = 1 AND IsPublished = 1;
        
        IF @TotalQuestions IS NULL
        BEGIN
            RAISERROR('Test not found or not published', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Get next attempt number
        DECLARE @AttemptNumber INT = ISNULL((SELECT MAX(AttemptNumber) FROM TestAttempts WHERE UserId = @UserId AND TestId = @TestId), 0) + 1;
        
        -- Create test attempt
        INSERT INTO TestAttempts (
            UserId, TestId, AttemptNumber, Status, StartTime, TotalQuestions,
            TotalMarks, PassingMarks, IpAddress, DeviceInfo, CreatedAt
        )
        VALUES (
            @UserId, @TestId, @AttemptNumber, 'InProgress', GETDATE(), @TotalQuestions,
            @TotalMarks, @PassingMarks, @IpAddress, @DeviceInfo, GETDATE()
        );
        
        DECLARE @TestAttemptId INT = SCOPE_IDENTITY();
        
        -- Create initial result record
        INSERT INTO TestResults (TestAttemptId, TotalQuestions, TotalMarks, CreatedAt)
        VALUES (@TestAttemptId, @TotalQuestions, @TotalMarks, GETDATE());
        
        COMMIT TRANSACTION;
        
        SELECT @TestAttemptId AS TestAttemptId, @AttemptNumber AS AttemptNumber;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Submit Answer
CREATE PROCEDURE [dbo].[SubmitAnswer]
    @TestAttemptId INT,
    @QuestionId INT,
    @SelectedAnswer NVARCHAR(10),
    @TimeTaken INT,
    @IsMarkedForReview BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Get question details and correct answer
        DECLARE @CorrectAnswer NVARCHAR(10);
        DECLARE @NegativeMarking BIT;
        DECLARE @NegativeMarksPerQuestion DECIMAL(5,2);
        DECLARE @QuestionMarks DECIMAL(5,2);
        
        SELECT q.CorrectAnswer, t.NegativeMarking, t.NegativeMarksPerQuestion, tq.Marks
        FROM Questions q
        JOIN TestQuestions tq ON q.Id = tq.QuestionId
        JOIN Tests t ON tq.TestId = t.Id
        JOIN TestAttempts ta ON tq.TestId = ta.TestId
        WHERE q.Id = @QuestionId AND tq.TestId = ta.TestId AND ta.Id = @TestAttemptId;
        
        IF @CorrectAnswer IS NULL
        BEGIN
            RAISERROR('Question not found in this test', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Check if answer already exists
        IF EXISTS (SELECT 1 FROM AttemptAnswers WHERE TestAttemptId = @TestAttemptId AND QuestionId = @QuestionId)
        BEGIN
            -- Update existing answer
            UPDATE AttemptAnswers
            SET SelectedAnswer = @SelectedAnswer,
                IsCorrect = CASE WHEN @SelectedAnswer = @CorrectAnswer THEN 1 ELSE 0 END,
                MarksObtained = CASE WHEN @SelectedAnswer = @CorrectAnswer THEN @QuestionMarks ELSE 0 END,
                NegativeMarks = CASE WHEN @SelectedAnswer != @CorrectAnswer AND @NegativeMarking = 1 THEN @NegativeMarksPerQuestion ELSE 0 END,
                TimeTaken = @TimeTaken,
                IsSkipped = CASE WHEN @SelectedAnswer IS NULL THEN 1 ELSE 0 END,
                IsMarkedForReview = @IsMarkedForReview,
                AnsweredAt = GETDATE()
            WHERE TestAttemptId = @TestAttemptId AND QuestionId = @QuestionId;
        END
        ELSE
        BEGIN
            -- Insert new answer
            INSERT INTO AttemptAnswers (
                TestAttemptId, QuestionId, SelectedAnswer, IsCorrect, MarksObtained,
                NegativeMarks, TimeTaken, IsSkipped, IsMarkedForReview, AnsweredAt, CreatedAt
            )
            VALUES (
                @TestAttemptId, @QuestionId, @SelectedAnswer,
                CASE WHEN @SelectedAnswer = @CorrectAnswer THEN 1 ELSE 0 END,
                CASE WHEN @SelectedAnswer = @CorrectAnswer THEN @QuestionMarks ELSE 0 END,
                CASE WHEN @SelectedAnswer != @CorrectAnswer AND @NegativeMarking = 1 THEN @NegativeMarksPerQuestion ELSE 0 END,
                @TimeTaken,
                CASE WHEN @SelectedAnswer IS NULL THEN 1 ELSE 0 END,
                @IsMarkedForReview, GETDATE(), GETDATE()
            );
        END
        
        -- Update attempt statistics
        UPDATE TestAttempts
        SET QuestionsAttempted = (
                SELECT COUNT(*) FROM AttemptAnswers 
                WHERE TestAttemptId = @TestAttemptId AND IsSkipped = 0
            ),
            CorrectAnswers = (
                SELECT COUNT(*) FROM AttemptAnswers 
                WHERE TestAttemptId = @TestAttemptId AND IsCorrect = 1
            ),
            WrongAnswers = (
                SELECT COUNT(*) FROM AttemptAnswers 
                WHERE TestAttemptId = @TestAttemptId AND IsCorrect = 0 AND IsSkipped = 0
            ),
            Unanswered = (
                SELECT COUNT(*) FROM AttemptAnswers 
                WHERE TestAttemptId = @TestAttemptId AND IsSkipped = 1
            ),
            ObtainedMarks = (
                SELECT SUM(MarksObtained - NegativeMarks) FROM AttemptAnswers 
                WHERE TestAttemptId = @TestAttemptId
            ),
            Percentage = CASE 
                WHEN (SELECT TotalMarks FROM Tests WHERE Id = (SELECT TestId FROM TestAttempts WHERE Id = @TestAttemptId)) > 0
                THEN (SELECT SUM(MarksObtained - NegativeMarks) * 100.0 / TotalMarks FROM AttemptAnswers WHERE TestAttemptId = @TestAttemptId) / 
                     (SELECT TotalMarks FROM Tests WHERE Id = (SELECT TestId FROM TestAttempts WHERE Id = @TestAttemptId))
                ELSE 0 
            END,
            UpdatedAt = GETDATE()
        WHERE Id = @TestAttemptId;
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success;
    END CATCH
END
GO

-- Complete Test Attempt
CREATE PROCEDURE [dbo].[CompleteTestAttempt]
    @TestAttemptId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Update attempt completion details
        DECLARE @StartTime DATETIME2;
        DECLARE @TotalMarks DECIMAL(10,2);
        DECLARE @PassingMarks DECIMAL(10,2);
        
        SELECT @StartTime = StartTime, @TotalMarks = ta.TotalMarks, @PassingMarks = ta.PassingMarks
        FROM TestAttempts ta
        WHERE ta.Id = @TestAttemptId;
        
        UPDATE TestAttempts
        SET Status = 'Completed',
            EndTime = GETDATE(),
            DurationTaken = DATEDIFF(SECOND, @StartTime, GETDATE()),
            IsPassed = CASE WHEN ObtainedMarks >= @PassingMarks THEN 1 ELSE 0 END,
            UpdatedAt = GETDATE()
        WHERE Id = @TestAttemptId;
        
        -- Update final results
        UPDATE TestResults
        SET CorrectAnswers = ta.CorrectAnswers,
            WrongAnswers = ta.WrongAnswers,
            Unanswered = ta.Unanswered,
            ObtainedMarks = ta.ObtainedMarks,
            Percentage = ta.Percentage,
            Accuracy = CASE WHEN ta.QuestionsAttempted > 0 THEN (ta.CorrectAnswers * 100.0 / ta.QuestionsAttempted) ELSE 0 END,
            TimeTaken = ta.DurationTaken,
            AverageTimePerQuestion = CASE WHEN ta.QuestionsAttempted > 0 THEN CAST(ta.DurationTaken AS DECIMAL) / ta.QuestionsAttempted ELSE 0 END
        FROM TestResults tr
        JOIN TestAttempts ta ON tr.TestAttemptId = ta.Id
        WHERE tr.TestAttemptId = @TestAttemptId;
        
        -- Update analytics
        DECLARE @TestId INT;
        SELECT @TestId = TestId FROM TestAttempts WHERE Id = @TestAttemptId;
        
        MERGE TestAnalytics AS target
        USING (
            SELECT @TestId AS TestId, CAST(GETDATE() AS DATE) AS Date
        ) AS source
        ON (target.TestId = source.TestId AND target.Date = source.Date)
        WHEN MATCHED THEN
            UPDATE SET
                TotalAttempts = TotalAttempts + 1,
                CompletedAttempts = CompletedAttempts + 1,
                AverageScore = ((AverageScore * (TotalAttempts - 1)) + ta.Percentage) / TotalAttempts,
                AverageTime = ((AverageTime * (CompletedAttempts - 1)) + ta.DurationTaken) / CompletedAttempts,
                PassPercentage = ((PassPercentage * (CompletedAttempts - 1)) + CASE WHEN ta.IsPassed = 1 THEN 100 ELSE 0 END) / CompletedAttempts,
                UniqueUsers = (SELECT COUNT(DISTINCT UserId) FROM TestAttempts WHERE TestId = @TestId),
                UpdatedAt = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT (TestId, Date, TotalAttempts, CompletedAttempts, AverageScore, AverageTime, PassPercentage, UniqueUsers)
            VALUES (@TestId, CAST(GETDATE() AS DATE), 1, 1, ta.Percentage, ta.DurationTaken, CASE WHEN ta.IsPassed = 1 THEN 100 ELSE 0 END, 1);
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success;
    END CATCH
END
GO

-- Get Test Details with Questions
CREATE PROCEDURE [dbo].[GetTestWithQuestions]
    @TestId INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get test details
    SELECT 
        t.Id, t.Name, t.Description, t.ExamId, t.SubjectId, t.TestType,
        t.DifficultyLevel, t.TotalQuestions, t.DurationMinutes, t.TotalMarks,
        t.PassingMarks, t.NegativeMarking, t.NegativeMarksPerQuestion,
        t.Instructions, t.IsFree, t.Price, t.Currency, t.IsActive, t.IsPublished,
        t.PublishDate, t.CreatedAt, t.UpdatedAt,
        CASE 
            WHEN @UserId IS NOT NULL THEN (
                SELECT COUNT(*) FROM TestAttempts WHERE TestId = t.Id AND UserId = @UserId
            )
            ELSE 0
        END AS UserAttempts,
        CASE 
            WHEN @UserId IS NOT NULL THEN (
                SELECT TOP 1 Percentage FROM TestAttempts 
                WHERE TestId = t.Id AND UserId = @UserId AND Status = 'Completed'
                ORDER BY CreatedAt DESC
            )
            ELSE NULL
        END AS BestScore
    FROM Tests t
    WHERE t.Id = @TestId AND t.IsActive = 1;
    
    -- Get test questions
    SELECT 
        tq.Id, tq.TestId, tq.QuestionId, tq.QuestionNumber, tq.Marks,
        tq.NegativeMarks, tq.IsOptional,
        q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.OptionD,
        q.CorrectAnswer, q.Explanation, q.QuestionType, q.QuestionImageUrl,
        q.ExplanationImageUrl, q.DifficultyLevel
    FROM TestQuestions tq
    JOIN Questions q ON tq.QuestionId = q.Id
    WHERE tq.TestId = @TestId
    ORDER BY tq.QuestionNumber;
END
GO

-- Get User Test History
CREATE PROCEDURE [dbo].[GetUserTestHistory]
    @UserId INT,
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @TestType NVARCHAR(50) = NULL,
    @ExamId INT = NULL,
    @SubjectId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        ta.Id, ta.TestId, ta.AttemptNumber, ta.Status, ta.StartTime, ta.EndTime,
        ta.DurationTaken, ta.TotalQuestions, ta.QuestionsAttempted, ta.CorrectAnswers,
        ta.WrongAnswers, ta.Unanswered, ta.TotalMarks, ta.ObtainedMarks, ta.Percentage,
        ta.Rank, ta.TotalParticipants, ta.IsPassed,
        t.Name AS TestName, t.TestType, t.DifficultyLevel, t.DurationMinutes,
        e.Name AS ExamName, s.Name AS SubjectName
    FROM TestAttempts ta
    JOIN Tests t ON ta.TestId = t.Id
    LEFT JOIN Exams e ON t.ExamId = e.Id
    LEFT JOIN Subjects s ON t.SubjectId = s.Id
    WHERE ta.UserId = @UserId
    AND (@TestType IS NULL OR t.TestType = @TestType)
    AND (@ExamId IS NULL OR t.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR t.SubjectId = @SubjectId)
    ORDER BY ta.StartTime DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM TestAttempts ta
    JOIN Tests t ON ta.TestId = t.Id
    WHERE ta.UserId = @UserId
    AND (@TestType IS NULL OR t.TestType = @TestType)
    AND (@ExamId IS NULL OR t.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR t.SubjectId = @SubjectId);
END
GO

-- Get Test Statistics
CREATE PROCEDURE [dbo].[GetTestStatistics]
    @TestId INT = NULL,
    @DateFrom DATE = NULL,
    @DateTo DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TotalTests,
        SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS ActiveTests,
        SUM(CASE WHEN IsPublished = 1 THEN 1 ELSE 0 END) AS PublishedTests,
        SUM(CASE WHEN IsFree = 1 THEN 1 ELSE 0 END) AS FreeTests
    FROM Tests
    WHERE (@TestId IS NULL OR Id = @TestId);
    
    SELECT 
        COUNT(*) AS TotalAttempts,
        SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedAttempts,
        SUM(CASE WHEN Status = 'InProgress' THEN 1 ELSE 0 END) AS InProgressAttempts,
        AVG(CASE WHEN Status = 'Completed' THEN Percentage ELSE NULL END) AS AverageScore,
        AVG(CASE WHEN Status = 'Completed' THEN DurationTaken ELSE NULL END) AS AverageTime,
        SUM(CASE WHEN IsPassed = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS PassPercentage
    FROM TestAttempts
    WHERE (@TestId IS NULL OR TestId = @TestId)
    AND (@DateFrom IS NULL OR CAST(StartTime AS DATE) >= @DateFrom)
    AND (@DateTo IS NULL OR CAST(StartTime AS DATE) <= @DateTo);
END
GO
