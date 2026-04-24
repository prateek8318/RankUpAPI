-- =============================================
-- Add Enhanced Fields to MockTests Table
-- Database: RankUp_QuestionDB
-- Server: ABHIJEET
-- =============================================

USE [RankUp_QuestionDB];
GO

-- Check if MockTestType column already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'MockTestType')
BEGIN
    -- Add new fields to MockTests table
    ALTER TABLE [dbo].[MockTests]
    ADD 
        [MockTestType] INT NOT NULL DEFAULT 1, -- 1=MockTest, 2=TestSeries, 3=DeepPractice, 4=PreviousYear
        [SubjectId] INT NULL, -- For specific subject tests
        [TopicId] INT NULL, -- For topic-wise tests (DeepPractice)
        [HasNegativeMarking] BIT NOT NULL DEFAULT 0,
        [NegativeMarkingValue] DECIMAL(5,2) NULL,
        [MarksPerQuestion] DECIMAL(5,2) NOT NULL DEFAULT 5.00,
        [ExamDate] DATE NULL, -- For scheduled exams
        [PublishDateTime] DATETIME2 NULL, -- When to publish the test
        [ValidTill] DATETIME2 NULL, -- When test expires
        [ShowResultType] NVARCHAR(20) NOT NULL DEFAULT 'Immediate', -- Immediate, Scheduled, AfterDeadline
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Inactive, Draft
        [Year] INT NULL, -- For PreviousYear papers
        [Difficulty] NVARCHAR(20) NULL, -- Easy, Medium, Hard (for DeepPractice)
        [PaperCode] NVARCHAR(50) NULL, -- For TestSeries
        [ImageUrl] NVARCHAR(500) NULL; -- For uploaded images

    PRINT 'New columns added to MockTests table';
END
ELSE
BEGIN
    PRINT 'MockTestType column already exists, skipping column addition';
END
GO

-- Add foreign key constraints for new fields (only if they don't exist)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MockTests_Subjects')
BEGIN
    ALTER TABLE [dbo].[MockTests]
    ADD CONSTRAINT [FK_MockTests_Subjects] FOREIGN KEY ([SubjectId]) REFERENCES [dbo].[Subjects]([Id]);
    PRINT 'FK_MockTests_Subjects constraint added';
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MockTests_Topics')
BEGIN
    ALTER TABLE [dbo].[MockTests]
    ADD CONSTRAINT [FK_MockTests_Topics] FOREIGN KEY ([TopicId]) REFERENCES [dbo].[Topics]([Id]);
    PRINT 'FK_MockTests_Topics constraint added';
END
GO

-- Add check constraints (only if they don't exist)
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_MockTests_MockTestType')
BEGIN
    ALTER TABLE [dbo].[MockTests]
    ADD CONSTRAINT [CK_MockTests_MockTestType] CHECK ([MockTestType] IN (1, 2, 3, 4));
    PRINT 'CK_MockTests_MockTestType constraint added';
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_MockTests_ShowResultType')
BEGIN
    ALTER TABLE [dbo].[MockTests]
    ADD CONSTRAINT [CK_MockTests_ShowResultType] CHECK ([ShowResultType] IN ('Immediate', 'Scheduled', 'AfterDeadline'));
    PRINT 'CK_MockTests_ShowResultType constraint added';
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_MockTests_Status')
BEGIN
    ALTER TABLE [dbo].[MockTests]
    ADD CONSTRAINT [CK_MockTests_Status] CHECK ([Status] IN ('Active', 'Inactive', 'Draft'));
    PRINT 'CK_MockTests_Status constraint added';
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_MockTests_Difficulty')
BEGIN
    ALTER TABLE [dbo].[MockTests]
    ADD CONSTRAINT [CK_MockTests_Difficulty] CHECK ([Difficulty] IN ('Easy', 'Medium', 'Hard') OR [Difficulty] IS NULL);
    PRINT 'CK_MockTests_Difficulty constraint added';
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_MockTests_NegativeMarkingValue')
BEGIN
    ALTER TABLE [dbo].[MockTests]
    ADD CONSTRAINT [CK_MockTests_NegativeMarkingValue] CHECK (
        ([HasNegativeMarking] = 0 AND [NegativeMarkingValue] IS NULL) OR 
        ([HasNegativeMarking] = 1 AND [NegativeMarkingValue] IS NOT NULL AND [NegativeMarkingValue] >= 0 AND [NegativeMarkingValue] <= 10)
    );
    PRINT 'CK_MockTests_NegativeMarkingValue constraint added';
END
GO

-- Add indexes for new fields (only if they don't exist)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MockTests_MockTestType' AND object_id = OBJECT_ID('[dbo].[MockTests]'))
BEGIN
    CREATE INDEX [IX_MockTests_MockTestType] ON [dbo].[MockTests]([MockTestType]);
    PRINT 'IX_MockTests_MockTestType index added';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MockTests_SubjectId' AND object_id = OBJECT_ID('[dbo].[MockTests]'))
BEGIN
    CREATE INDEX [IX_MockTests_SubjectId] ON [dbo].[MockTests]([SubjectId]);
    PRINT 'IX_MockTests_SubjectId index added';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MockTests_TopicId' AND object_id = OBJECT_ID('[dbo].[MockTests]'))
BEGIN
    CREATE INDEX [IX_MockTests_TopicId] ON [dbo].[MockTests]([TopicId]);
    PRINT 'IX_MockTests_TopicId index added';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MockTests_Status' AND object_id = OBJECT_ID('[dbo].[MockTests]'))
BEGIN
    CREATE INDEX [IX_MockTests_Status] ON [dbo].[MockTests]([Status]);
    PRINT 'IX_MockTests_Status index added';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MockTests_ExamDate' AND object_id = OBJECT_ID('[dbo].[MockTests]'))
BEGIN
    CREATE INDEX [IX_MockTests_ExamDate] ON [dbo].[MockTests]([ExamDate]);
    PRINT 'IX_MockTests_ExamDate index added';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MockTests_PublishDateTime' AND object_id = OBJECT_ID('[dbo].[MockTests]'))
BEGIN
    CREATE INDEX [IX_MockTests_PublishDateTime] ON [dbo].[MockTests]([PublishDateTime]);
    PRINT 'IX_MockTests_PublishDateTime index added';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MockTests_ValidTill' AND object_id = OBJECT_ID('[dbo].[MockTests]'))
BEGIN
    CREATE INDEX [IX_MockTests_ValidTill] ON [dbo].[MockTests]([ValidTill]);
    PRINT 'IX_MockTests_ValidTill index added';
END
GO

-- Update existing records to have proper default values
UPDATE [dbo].[MockTests] 
SET [Status] = 'Active' 
WHERE [IsActive] = 1 AND [Status] IS NULL;

UPDATE [dbo].[MockTests] 
SET [Status] = 'Inactive' 
WHERE [IsActive] = 0 AND [Status] IS NULL;

PRINT 'Enhanced MockTest fields added successfully!';
GO

-- Show current table structure
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MockTests' 
ORDER BY ORDINAL_POSITION;
GO
