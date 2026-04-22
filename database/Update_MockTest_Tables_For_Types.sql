-- =============================================
-- Update Mock Test Tables to Support Different Types
-- =============================================

-- 1. Add MockTestType column to MockTests table
ALTER TABLE [dbo].[MockTests] ADD [MockTestType] INT NOT NULL DEFAULT 1; -- Default to MockTest

-- 2. Add type-specific columns
ALTER TABLE [dbo].[MockTests] ADD [SubjectId] INT NULL;
ALTER TABLE [dbo].[MockTests] ADD [TopicId] INT NULL;
ALTER TABLE [dbo].[MockTests] ADD [Year] INT NULL;
ALTER TABLE [dbo].[MockTests] ADD [Difficulty] NVARCHAR(20) NULL;
ALTER TABLE [dbo].[MockTests] ADD [PaperCode] NVARCHAR(50) NULL;

-- 3. Add foreign key constraints for new columns
ALTER TABLE [dbo].[MockTests] WITH CHECK ADD CONSTRAINT [FK_MockTests_Subjects] FOREIGN KEY([SubjectId]) REFERENCES [dbo].[Subjects]([Id]);
ALTER TABLE [dbo].[MockTests] WITH CHECK ADD CONSTRAINT [FK_MockTests_Topics] FOREIGN KEY([TopicId]) REFERENCES [dbo].[Topics]([Id]);

-- 4. Add check constraints
ALTER TABLE [dbo].[MockTests] WITH CHECK ADD CONSTRAINT [CK_MockTests_MockTestType] CHECK ([MockTestType] IN (1, 2, 3, 4)); -- 1=MockTest, 2=TestSeries, 3=DeepPractice, 4=PreviousYear
ALTER TABLE [dbo].[MockTests] WITH CHECK ADD CONSTRAINT [CK_MockTests_Difficulty] CHECK ([Difficulty] IN ('Easy', 'Medium', 'Hard') OR [Difficulty] IS NULL);
ALTER TABLE [dbo].[MockTests] WITH CHECK ADD CONSTRAINT [CK_MockTests_Year] CHECK ([Year] BETWEEN 2000 AND 2100 OR [Year] IS NULL);

-- 5. Add business logic constraints based on mock test type
ALTER TABLE [dbo].[MockTests] WITH CHECK ADD CONSTRAINT [CK_MockTests_Type_Logic] CHECK (
    -- MockTest (1): Must have SubjectId, TopicId optional
    ([MockTestType] = 1 AND [SubjectId] IS NOT NULL) OR
    -- TestSeries (2): SubjectId optional (full length), TopicId null, Year optional
    ([MockTestType] = 2 AND [TopicId] IS NULL) OR
    -- DeepPractice (3): Must have SubjectId and TopicId, Difficulty required
    ([MockTestType] = 3 AND [SubjectId] IS NOT NULL AND [TopicId] IS NOT NULL AND [Difficulty] IS NOT NULL) OR
    -- PreviousYear (4): Must have Year, PaperCode optional
    ([MockTestType] = 4 AND [Year] IS NOT NULL)
);

-- 6. Update indexes for new columns
CREATE INDEX [IX_MockTests_MockTestType] ON [dbo].[MockTests]([MockTestType]);
CREATE INDEX [IX_MockTests_SubjectId] ON [dbo].[MockTests]([SubjectId]);
CREATE INDEX [IX_MockTests_TopicId] ON [dbo].[MockTests]([TopicId]);
CREATE INDEX [IX_MockTests_Year] ON [dbo].[MockTests]([Year]);

-- 7. Create composite indexes for common queries
CREATE INDEX [IX_MockTests_Type_Exam] ON [dbo].[MockTests]([MockTestType], [ExamId]);
CREATE INDEX [IX_MockTests_Type_Subject] ON [dbo].[MockTests]([MockTestType], [SubjectId]);
CREATE INDEX [IX_MockTests_Type_Topic] ON [dbo].[MockTests]([MockTestType], [TopicId]);

-- =============================================
-- Updated Stored Procedures
-- =============================================

-- Drop existing procedure if it exists
DROP PROCEDURE IF EXISTS [dbo].[GetMockTestsWithDetails];

-- Create enhanced stored procedure with type support
CREATE OR ALTER PROCEDURE [dbo].[GetMockTestsWithDetails]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @MockTestType INT = NULL,
    @ExamId INT = NULL,
    @SubjectId INT = NULL,
    @TopicId INT = NULL,
    @Year INT = NULL,
    @UserId INT = NULL -- For user-specific access check
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        mt.Id, mt.Name, mt.Description, mt.MockTestType, mt.ExamId, 
        mt.SubjectId, mt.TopicId, mt.DurationInMinutes, 
        mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
        mt.AccessType, mt.AttemptsAllowed, mt.CreatedAt,
        mt.Year, mt.Difficulty, mt.PaperCode,
        e.Name AS ExamName, e.ExamType, e.HasNegativeMarking, e.NegativeMarkingValue,
        s.Name AS SubjectName,
        t.Name AS TopicName,
        sp.Name AS SubscriptionPlanName,
        CASE mt.MockTestType
            WHEN 1 THEN 'Mock Test'
            WHEN 2 THEN 'Test Series'
            WHEN 3 THEN 'Deep Practice'
            WHEN 4 THEN 'Previous Year'
            ELSE 'Unknown'
        END AS MockTestTypeDisplay,
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
    LEFT JOIN Subjects s ON mt.SubjectId = s.Id
    LEFT JOIN Topics t ON mt.TopicId = t.Id
    LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
    WHERE mt.IsActive = 1
    AND (@MockTestType IS NULL OR mt.MockTestType = @MockTestType)
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR mt.SubjectId = @SubjectId)
    AND (@TopicId IS NULL OR mt.TopicId = @TopicId)
    AND (@Year IS NULL OR mt.Year = @Year)
    ORDER BY 
        CASE mt.MockTestType
            WHEN 1 THEN 1 -- Mock Test
            WHEN 2 THEN 2 -- Test Series
            WHEN 3 THEN 3 -- Deep Practice
            WHEN 4 THEN 4 -- Previous Year
            ELSE 5
        END,
        e.ExamType,
        mt.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM MockTests mt
    LEFT JOIN Exams e ON mt.ExamId = e.Id
    WHERE mt.IsActive = 1
    AND (@MockTestType IS NULL OR mt.MockTestType = @MockTestType)
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR mt.SubjectId = @SubjectId)
    AND (@TopicId IS NULL OR mt.TopicId = @TopicId)
    AND (@Year IS NULL OR mt.Year = @Year);
END

-- Create specialized procedures for each type
CREATE OR ALTER PROCEDURE [dbo].[GetMockTestsByType]
    @MockTestType INT,
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ExamId INT = NULL,
    @SubjectId INT = NULL,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    EXEC [dbo].[GetMockTestsWithDetails] 
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @MockTestType = @MockTestType,
        @ExamId = @ExamId,
        @SubjectId = @SubjectId,
        @UserId = @UserId;
END

-- Get Mock Tests (Subject Wise)
CREATE OR ALTER PROCEDURE [dbo].[GetMockTests_SubjectWise]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ExamId INT = NULL,
    @SubjectId INT = NULL,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    EXEC [dbo].[GetMockTestsByType] 
        @MockTestType = 1, -- MockTest type
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @ExamId = @ExamId,
        @SubjectId = @SubjectId,
        @UserId = @UserId;
END

-- Get Test Series (Full Length Papers)
CREATE OR ALTER PROCEDURE [dbo].[GetTestSeries_FullLength]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ExamId INT = NULL,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    EXEC [dbo].[GetMockTestsByType] 
        @MockTestType = 2, -- TestSeries type
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @ExamId = @ExamId,
        @UserId = @UserId;
END

-- Get Deep Practice (Topic Wise MCQs)
CREATE OR ALTER PROCEDURE [dbo].[GetDeepPractice_TopicWise]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ExamId INT = NULL,
    @SubjectId INT = NULL,
    @TopicId INT = NULL,
    @Difficulty NVARCHAR(20) = NULL,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        mt.Id, mt.Name, mt.Description, mt.MockTestType, mt.ExamId, 
        mt.SubjectId, mt.TopicId, mt.DurationInMinutes, 
        mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
        mt.AccessType, mt.AttemptsAllowed, mt.CreatedAt,
        mt.Year, mt.Difficulty, mt.PaperCode,
        e.Name AS ExamName, e.ExamType, e.HasNegativeMarking, e.NegativeMarkingValue,
        s.Name AS SubjectName,
        t.Name AS TopicName,
        sp.Name AS SubscriptionPlanName,
        'Deep Practice' AS MockTestTypeDisplay,
        CASE 
            WHEN @UserId IS NULL THEN 1
            WHEN mt.AccessType = 'Free' THEN 1
            WHEN EXISTS (
                SELECT 1 FROM UserSubscriptions us 
                WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ExpiresAt > GETDATE()
                AND (mt.SubscriptionPlanId IS NULL OR us.SubscriptionPlanId = mt.SubscriptionPlanId)
            ) THEN 1
            ELSE 0
        END AS IsUnlocked,
        ISNULL((SELECT COUNT(*) FROM MockTestAttempts mta WHERE mta.MockTestId = mt.Id AND mta.UserId = @UserId), 0) AS AttemptsUsed
    FROM MockTests mt
    LEFT JOIN Exams e ON mt.ExamId = e.Id
    LEFT JOIN Subjects s ON mt.SubjectId = s.Id
    LEFT JOIN Topics t ON mt.TopicId = t.Id
    LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
    WHERE mt.IsActive = 1
    AND mt.MockTestType = 3 -- DeepPractice type
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR mt.SubjectId = @SubjectId)
    AND (@TopicId IS NULL OR mt.TopicId = @TopicId)
    AND (@Difficulty IS NULL OR mt.Difficulty = @Difficulty)
    ORDER BY mt.Difficulty, s.Name, t.Name, mt.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM MockTests mt
    WHERE mt.IsActive = 1
    AND mt.MockTestType = 3
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR mt.SubjectId = @SubjectId)
    AND (@TopicId IS NULL OR mt.TopicId = @TopicId)
    AND (@Difficulty IS NULL OR mt.Difficulty = @Difficulty);
END

-- Get Previous Years Solved Papers
CREATE OR ALTER PROCEDURE [dbo].[GetPreviousYearPapers]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ExamId INT = NULL,
    @Year INT = NULL,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        mt.Id, mt.Name, mt.Description, mt.MockTestType, mt.ExamId, 
        mt.SubjectId, mt.TopicId, mt.DurationInMinutes, 
        mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
        mt.AccessType, mt.AttemptsAllowed, mt.CreatedAt,
        mt.Year, mt.Difficulty, mt.PaperCode,
        e.Name AS ExamName, e.ExamType, e.HasNegativeMarking, e.NegativeMarkingValue,
        s.Name AS SubjectName,
        sp.Name AS SubscriptionPlanName,
        'Previous Year' AS MockTestTypeDisplay,
        CASE 
            WHEN @UserId IS NULL THEN 1
            WHEN mt.AccessType = 'Free' THEN 1
            WHEN EXISTS (
                SELECT 1 FROM UserSubscriptions us 
                WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ExpiresAt > GETDATE()
                AND (mt.SubscriptionPlanId IS NULL OR us.SubscriptionPlanId = mt.SubscriptionPlanId)
            ) THEN 1
            ELSE 0
        END AS IsUnlocked,
        ISNULL((SELECT COUNT(*) FROM MockTestAttempts mta WHERE mta.MockTestId = mt.Id AND mta.UserId = @UserId), 0) AS AttemptsUsed
    FROM MockTests mt
    LEFT JOIN Exams e ON mt.ExamId = e.Id
    LEFT JOIN Subjects s ON mt.SubjectId = s.Id
    LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
    WHERE mt.IsActive = 1
    AND mt.MockTestType = 4 -- PreviousYear type
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@Year IS NULL OR mt.Year = @Year)
    ORDER BY mt.Year DESC, e.ExamType, mt.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM MockTests mt
    WHERE mt.IsActive = 1
    AND mt.MockTestType = 4
    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
    AND (@Year IS NULL OR mt.Year = @Year);
END

-- =============================================
-- Sample Data for Different Mock Test Types
-- =============================================

-- Sample Mock Tests (Subject Wise)
/*
INSERT INTO [dbo].[MockTests] (
    [Name], [Description], [MockTestType], [ExamId], [SubjectId], [DurationInMinutes], 
    [TotalQuestions], [TotalMarks], [PassingMarks], [AccessType], [AttemptsAllowed], [CreatedBy]
)
VALUES 
('Railway Reasoning Mock Test 01', 'Subject wise mock test for Reasoning section', 1, 1, 1, 30, 25, 50.00, 17.50, 'Free', 3, 1),
('Railway Mathematics Mock Test 01', 'Subject wise mock test for Mathematics section', 1, 1, 2, 30, 25, 50.00, 17.50, 'Free', 3, 1),
('Railway English Mock Test 01', 'Subject wise mock test for English section', 1, 1, 3, 30, 25, 50.00, 17.50, 'Free', 3, 1);

-- Sample Test Series (Full Length Papers)
INSERT INTO [dbo].[MockTests] (
    [Name], [Description], [MockTestType], [ExamId], [DurationInMinutes], 
    [TotalQuestions], [TotalMarks], [PassingMarks], [PaperCode], [AccessType], [AttemptsAllowed], [CreatedBy]
)
VALUES 
('Railway Group D Full Test 01', 'Complete full length paper for Railway Group D exam', 2, 1, 90, 100, 100.00, 35.00, 'RGD-2023-01', 'Free', 2, 1),
('Railway Group D Full Test 02', 'Complete full length paper with advanced difficulty', 2, 1, 90, 100, 100.00, 35.00, 'RGD-2023-02', 'Paid', 3, 1);

-- Sample Deep Practice (Topic Wise MCQs)
INSERT INTO [dbo].[MockTests] (
    [Name], [Description], [MockTestType], [ExamId], [SubjectId], [TopicId], [Difficulty],
    [DurationInMinutes], [TotalQuestions], [TotalMarks], [PassingMarks], [AccessType], [AttemptsAllowed], [CreatedBy]
)
VALUES 
('Reasoning - Syllogism Easy', 'Topic wise practice for Syllogism topic', 3, 1, 1, 1, 'Easy', 15, 20, 20.00, 7.00, 'Free', 5, 1),
('Reasoning - Syllogism Medium', 'Medium difficulty Syllogism practice', 3, 1, 1, 1, 'Medium', 20, 25, 25.00, 8.75, 'Free', 5, 1),
('Mathematics - Percentage Hard', 'Hard difficulty Percentage practice', 3, 1, 2, 2, 'Hard', 25, 30, 30.00, 10.50, 'Paid', 3, 1);

-- Sample Previous Years Solved Papers
INSERT INTO [dbo].[MockTests] (
    [Name], [Description], [MockTestType], [ExamId], [Year], [PaperCode],
    [DurationInMinutes], [TotalQuestions], [TotalMarks], [PassingMarks], [AccessType], [AttemptsAllowed], [CreatedBy]
)
VALUES 
('Railway Group D 2022 Paper', 'Previous year solved paper for 2022', 4, 1, 2022, 'RGD-2022-ACT', 90, 100, 100.00, 35.00, 'Free', 2, 1),
('Railway Group D 2021 Paper', 'Previous year solved paper for 2021', 4, 1, 2021, 'RGD-2021-ACT', 90, 100, 100.00, 35.00, 'Free', 2, 1),
('Railway Group D 2020 Paper', 'Previous year solved paper for 2020', 4, 1, 2020, 'RGD-2020-ACT', 90, 100, 100.00, 35.00, 'Paid', 2, 1);
*/

PRINT 'Mock Test Tables Updated Successfully with Type Support!';
