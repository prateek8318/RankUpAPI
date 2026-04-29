-- =============================================
-- Mock Test System Database Schema
-- =============================================

-- 1. Mock Tests Table
CREATE TABLE [dbo].[MockTests] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [ExamId] INT NOT NULL,
    [DurationInMinutes] INT NOT NULL DEFAULT 30,
    [TotalQuestions] INT NOT NULL DEFAULT 20,
    [TotalMarks] DECIMAL(10,2) NOT NULL DEFAULT 100.00,
    [PassingMarks] DECIMAL(10,2) NOT NULL DEFAULT 35.00,
    [SubscriptionPlanId] INT NULL,
    [AccessType] NVARCHAR(20) NOT NULL DEFAULT 'Free', -- Free, Paid
    [AttemptsAllowed] INT NOT NULL DEFAULT 1,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [CreatedBy] INT NOT NULL,
    
    -- Foreign Key Constraints
    CONSTRAINT [FK_MockTests_Exams] FOREIGN KEY ([ExamId]) REFERENCES [dbo].[Exams]([Id]),
    CONSTRAINT [FK_MockTests_SubscriptionPlans] FOREIGN KEY ([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans]([Id]),
    
    -- Check Constraints
    CONSTRAINT [CK_MockTests_AccessType] CHECK ([AccessType] IN ('Free', 'Paid')),
    CONSTRAINT [CK_MockTests_TotalQuestions] CHECK ([TotalQuestions] > 0),
    CONSTRAINT [CK_MockTests_DurationInMinutes] CHECK ([DurationInMinutes] > 0),
    CONSTRAINT [CK_MockTests_TotalMarks] CHECK ([TotalMarks] > 0),
    CONSTRAINT [CK_MockTests_PassingMarks] CHECK ([PassingMarks] >= 0 AND [PassingMarks] <= [TotalMarks]),
    CONSTRAINT [CK_MockTests_AttemptsAllowed] CHECK ([AttemptsAllowed] > 0)
);

-- 2. Mock Test Questions Mapping Table
CREATE TABLE [dbo].[MockTestQuestions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [MockTestId] INT NOT NULL,
    [QuestionId] INT NOT NULL,
    [QuestionNumber] INT NOT NULL,
    [Marks] DECIMAL(10,2) NOT NULL DEFAULT 1.00,
    [NegativeMarks] DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Foreign Key Constraints
    CONSTRAINT [FK_MockTestQuestions_MockTests] FOREIGN KEY ([MockTestId]) REFERENCES [dbo].[MockTests]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MockTestQuestions_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions]([Id]),
    
    -- Unique Constraints
    CONSTRAINT [UK_MockTestQuestions_MockTestQuestion] UNIQUE ([MockTestId], [QuestionId]),
    CONSTRAINT [UK_MockTestQuestions_MockTestNumber] UNIQUE ([MockTestId], [QuestionNumber]),
    
    -- Check Constraints
    CONSTRAINT [CK_MockTestQuestions_QuestionNumber] CHECK ([QuestionNumber] > 0),
    CONSTRAINT [CK_MockTestQuestions_Marks] CHECK ([Marks] > 0),
    CONSTRAINT [CK_MockTestQuestions_NegativeMarks] CHECK ([NegativeMarks] >= 0)
);

-- 3. Mock Test Sessions Table
CREATE TABLE [dbo].[MockTestSessions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [MockTestId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [StartedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CompletedAt] DATETIME2 NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'NotStarted', -- NotStarted, InProgress, Completed, Submitted
    [LanguageCode] NVARCHAR(10) NOT NULL DEFAULT 'en',
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Foreign Key Constraints
    CONSTRAINT [FK_MockTestSessions_MockTests] FOREIGN KEY ([MockTestId]) REFERENCES [dbo].[MockTests]([Id]),
    CONSTRAINT [FK_MockTestSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    
    -- Check Constraints
    CONSTRAINT [CK_MockTestSessions_Status] CHECK ([Status] IN ('NotStarted', 'InProgress', 'Completed', 'Submitted')),
    CONSTRAINT [CK_MockTestSessions_Duration] CHECK ([CompletedAt] IS NULL OR [CompletedAt] >= [StartedAt])
);

-- 4. Mock Test Session Answers Table
CREATE TABLE [dbo].[MockTestSessionAnswers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SessionId] INT NOT NULL,
    [QuestionId] INT NOT NULL,
    [SelectedAnswer] NVARCHAR(10) NULL,
    [IsMarkedForReview] BIT NOT NULL DEFAULT 0,
    [IsAnswered] BIT NOT NULL DEFAULT 0,
    [TimeSpent] INT NOT NULL DEFAULT 0, -- in seconds
    [AnsweredAt] DATETIME2 NULL,
    [IsReported] BIT NOT NULL DEFAULT 0,
    [ReportReason] NVARCHAR(500) NULL,
    [ReportedAt] DATETIME2 NULL,
    [IsBookmarked] BIT NOT NULL DEFAULT 0,
    [BookmarkedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Foreign Key Constraints
    CONSTRAINT [FK_MockTestSessionAnswers_Sessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[MockTestSessions]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MockTestSessionAnswers_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions]([Id]),
    
    -- Unique Constraints
    CONSTRAINT [UK_MockTestSessionAnswers_SessionQuestion] UNIQUE ([SessionId], [QuestionId]),
    
    -- Check Constraints
    CONSTRAINT [CK_MockTestSessionAnswers_SelectedAnswer] CHECK ([SelectedAnswer] IN ('A', 'B', 'C', 'D') OR [SelectedAnswer] IS NULL),
    CONSTRAINT [CK_MockTestSessionAnswers_TimeSpent] CHECK ([TimeSpent] >= 0)
);

-- 5. Mock Test Attempts Table
CREATE TABLE [dbo].[MockTestAttempts] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [MockTestId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [StartedAt] DATETIME2 NOT NULL,
    [CompletedAt] DATETIME2 NULL,
    [Duration] INT NULL, -- in minutes
    [TotalQuestions] INT NOT NULL,
    [AnsweredQuestions] INT NOT NULL DEFAULT 0,
    [CorrectAnswers] INT NOT NULL DEFAULT 0,
    [WrongAnswers] INT NOT NULL DEFAULT 0,
    [SkippedQuestions] INT NOT NULL DEFAULT 0,
    [ObtainedMarks] DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    [Percentage] DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'NotStarted', -- NotStarted, InProgress, Completed, Submitted
    [Grade] NVARCHAR(5) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    -- Foreign Key Constraints
    CONSTRAINT [FK_MockTestAttempts_MockTests] FOREIGN KEY ([MockTestId]) REFERENCES [dbo].[MockTests]([Id]),
    CONSTRAINT [FK_MockTestAttempts_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    
    -- Check Constraints
    CONSTRAINT [CK_MockTestAttempts_Status] CHECK ([Status] IN ('NotStarted', 'InProgress', 'Completed', 'Submitted')),
    CONSTRAINT [CK_MockTestAttempts_Duration] CHECK ([Duration] IS NULL OR [Duration] >= 0),
    CONSTRAINT [CK_MockTestAttempts_TotalQuestions] CHECK ([TotalQuestions] > 0),
    CONSTRAINT [CK_MockTestAttempts_AnsweredQuestions] CHECK ([AnsweredQuestions] >= 0 AND [AnsweredQuestions] <= [TotalQuestions]),
    CONSTRAINT [CK_MockTestAttempts_CorrectAnswers] CHECK ([CorrectAnswers] >= 0 AND [CorrectAnswers] <= [AnsweredQuestions]),
    CONSTRAINT [CK_MockTestAttempts_WrongAnswers] CHECK ([WrongAnswers] >= 0 AND [WrongAnswers] <= [AnsweredQuestions]),
    CONSTRAINT [CK_MockTestAttempts_SkippedQuestions] CHECK ([SkippedQuestions] >= 0 AND [SkippedQuestions] <= [TotalQuestions]),
    CONSTRAINT [CK_MockTestAttempts_ObtainedMarks] CHECK ([ObtainedMarks] >= 0),
    CONSTRAINT [CK_MockTestAttempts_Percentage] CHECK ([Percentage] >= 0 AND [Percentage] <= 100),
    CONSTRAINT [CK_MockTestAttempts_Grade] CHECK ([Grade] IN ('A+', 'A', 'B+', 'B', 'C', 'D', 'F') OR [Grade] IS NULL)
);

-- =============================================
-- Indexes for Performance Optimization
-- =============================================

-- Mock Tests Table Indexes
CREATE INDEX [IX_MockTests_ExamId] ON [dbo].[MockTests]([ExamId]);
CREATE INDEX [IX_MockTests_SubscriptionPlanId] ON [dbo].[MockTests]([SubscriptionPlanId]);
CREATE INDEX [IX_MockTests_AccessType] ON [dbo].[MockTests]([AccessType]);
CREATE INDEX [IX_MockTests_IsActive] ON [dbo].[MockTests]([IsActive]);
CREATE INDEX [IX_MockTests_CreatedAt] ON [dbo].[MockTests]([CreatedAt] DESC);

-- Mock Test Questions Indexes
CREATE INDEX [IX_MockTestQuestions_MockTestId] ON [dbo].[MockTestQuestions]([MockTestId]);
CREATE INDEX [IX_MockTestQuestions_QuestionId] ON [dbo].[MockTestQuestions]([QuestionId]);
CREATE INDEX [IX_MockTestQuestions_QuestionNumber] ON [dbo].[MockTestQuestions]([MockTestId], [QuestionNumber]);

-- Mock Test Sessions Indexes
CREATE INDEX [IX_MockTestSessions_MockTestId] ON [dbo].[MockTestSessions]([MockTestId]);
CREATE INDEX [IX_MockTestSessions_UserId] ON [dbo].[MockTestSessions]([UserId]);
CREATE INDEX [IX_MockTestSessions_Status] ON [dbo].[MockTestSessions]([Status]);
CREATE INDEX [IX_MockTestSessions_StartedAt] ON [dbo].[MockTestSessions]([StartedAt] DESC);

-- Mock Test Session Answers Indexes
CREATE INDEX [IX_MockTestSessionAnswers_SessionId] ON [dbo].[MockTestSessionAnswers]([SessionId]);
CREATE INDEX [IX_MockTestSessionAnswers_QuestionId] ON [dbo].[MockTestSessionAnswers]([QuestionId]);

-- Mock Test Attempts Indexes
CREATE INDEX [IX_MockTestAttempts_MockTestId] ON [dbo].[MockTestAttempts]([MockTestId]);
CREATE INDEX [IX_MockTestAttempts_UserId] ON [dbo].[MockTestAttempts]([UserId]);
CREATE INDEX [IX_MockTestAttempts_Status] ON [dbo].[MockTestAttempts]([Status]);
CREATE INDEX [IX_MockTestAttempts_StartedAt] ON [dbo].[MockTestAttempts]([StartedAt] DESC);
CREATE INDEX [IX_MockTestAttempts_Percentage] ON [dbo].[MockTestAttempts]([Percentage] DESC);

-- =============================================
-- Sample Data Insertion (Optional)
-- =============================================

-- Insert sample mock tests (assuming Exams and Questions tables exist)
-- Uncomment and modify as needed

/*
-- Sample Mock Test 1: Railway Group D Mock Test 01
INSERT INTO [dbo].[MockTests] (
    [Name], [Description], [ExamId], [DurationInMinutes], [TotalQuestions], 
    [TotalMarks], [PassingMarks], [AccessType], [AttemptsAllowed], [CreatedBy]
)
VALUES (
    'Railway Group D Mock Test 01', 
    'Comprehensive mock test for Railway Group D examination covering all subjects',
    1, -- Assuming ExamId 1 exists for Railway Group D
    30, -- 30 minutes
    20, -- 20 questions
    100.00, -- 100 total marks
    35.00, -- 35 passing marks
    'Free', -- Free access
    3, -- 3 attempts allowed
    1 -- Created by Admin user ID 1
);

-- Sample Mock Test 2: Railway Group D Mock Test 02 (Paid)
INSERT INTO [dbo].[MockTests] (
    [Name], [Description], [ExamId], [DurationInMinutes], [TotalQuestions], 
    [TotalMarks], [PassingMarks], [SubscriptionPlanId], [AccessType], [AttemptsAllowed], [CreatedBy]
)
VALUES (
    'Railway Group D Mock Test 02', 
    'Advanced mock test with detailed solutions and performance analysis',
    1, -- Assuming ExamId 1 exists for Railway Group D
    45, -- 45 minutes
    30, -- 30 questions
    150.00, -- 150 total marks
    52.50, -- 35% passing marks
    1, -- Assuming SubscriptionPlanId 1 exists for Premium plan
    'Paid', -- Paid access
    5, -- 5 attempts allowed
    1 -- Created by Admin user ID 1
);
*/

-- =============================================
-- Stored Procedures for Common Operations
-- =============================================

-- Get Mock Tests with Exam and Subject Details
CREATE OR ALTER PROCEDURE [dbo].[GetMockTestsWithDetails]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ExamId INT = NULL,
    @SubjectId INT = NULL,
    @UserId INT = NULL -- For user-specific access check
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        mt.Id, mt.Name, mt.Description, mt.ExamId, mt.DurationInMinutes, 
        mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
        mt.AccessType, mt.AttemptsAllowed, mt.CreatedAt,
        e.Name AS ExamName, e.ExamType, e.SubjectId, e.HasNegativeMarking, e.NegativeMarkingValue,
        s.Name AS SubjectName,
        sp.Name AS SubscriptionPlanName,
        CASE 
            WHEN @UserId IS NULL THEN 1 -- Admin view, show all
            WHEN mt.AccessType = 'Free' THEN 1 -- Free tests are always accessible
            WHEN EXISTS (
                SELECT 1 FROM UserSubscriptions us 
                WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ExpiresAt > GETDATE()
                AND (mt.SubscriptionPlanId IS NULL OR us.SubscriptionPlanId = mt.SubscriptionPlanId)
            ) THEN 1 -- User has valid subscription
            ELSE 0 -- No access
        END AS IsUnlocked,
        ISNULL((SELECT COUNT(*) FROM MockTestAttempts mta WHERE mta.MockTestId = mt.Id AND mta.UserId = @UserId), 0) AS AttemptsUsed
    FROM MockTests mt
    LEFT JOIN Exams e ON mt.ExamId = e.Id
    LEFT JOIN Subjects s ON e.SubjectId = s.Id
    LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
    WHERE mt.IsActive = 1
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId)
    ORDER BY e.ExamType, mt.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM MockTests mt
    LEFT JOIN Exams e ON mt.ExamId = e.Id
    WHERE mt.IsActive = 1
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId);
END

-- Get Mock Test Statistics
CREATE OR ALTER PROCEDURE [dbo].[GetMockTestStatistics]
    @ExamId INT = NULL,
    @SubjectId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TotalMockTests,
        SUM(CASE WHEN mt.AccessType = 'Free' THEN 1 ELSE 0 END) AS FreeMockTests,
        SUM(CASE WHEN mt.AccessType = 'Paid' THEN 1 ELSE 0 END) AS PaidMockTests,
        COUNT(DISTINCT mt.ExamId) AS ExamsCovered,
        COUNT(DISTINCT e.SubjectId) AS SubjectsCovered,
        AVG(mt.TotalQuestions) AS AvgQuestionsPerTest,
        AVG(mt.DurationInMinutes) AS AvgDurationPerTest,
        SUM(mt.TotalQuestions) AS TotalQuestionsAcrossAllTests
    FROM MockTests mt
    LEFT JOIN Exams e ON mt.ExamId = e.Id
    WHERE mt.IsActive = 1
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId);
END

PRINT 'Mock Test Database Schema Created Successfully!';
