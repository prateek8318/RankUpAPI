USE [RankUp_QuestionDB];
GO

-- =====================================================
-- ADD MISSING COLUMNS TO MOCKTESTS TABLE
-- =====================================================

-- Add missing columns to MockTests table
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'SubjectId')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [SubjectId] INT NULL;
    PRINT 'Added SubjectId column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'TopicId')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [TopicId] INT NULL;
    PRINT 'Added TopicId column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'MockTestType')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [MockTestType] INT NOT NULL DEFAULT 1; -- 1 = Practice, 2 = Live, etc.
    PRINT 'Added MockTestType column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'MarksPerQuestion')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [MarksPerQuestion] DECIMAL(10,2) NOT NULL DEFAULT 1.00;
    PRINT 'Added MarksPerQuestion column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'HasNegativeMarking')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [HasNegativeMarking] BIT NOT NULL DEFAULT 0;
    PRINT 'Added HasNegativeMarking column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'NegativeMarkingValue')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [NegativeMarkingValue] DECIMAL(10,2) NULL;
    PRINT 'Added NegativeMarkingValue column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [Status] NVARCHAR(20) NOT NULL DEFAULT 'Active';
    PRINT 'Added Status column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'Year')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [Year] INT NULL;
    PRINT 'Added Year column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'Difficulty')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [Difficulty] NVARCHAR(20) NULL;
    PRINT 'Added Difficulty column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'PaperCode')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [PaperCode] NVARCHAR(50) NULL;
    PRINT 'Added PaperCode column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'ExamDate')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [ExamDate] DATE NULL;
    PRINT 'Added ExamDate column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'PublishDateTime')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [PublishDateTime] DATETIME2 NULL;
    PRINT 'Added PublishDateTime column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'ValidTill')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [ValidTill] DATETIME2 NULL;
    PRINT 'Added ValidTill column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'ShowResultType')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [ShowResultType] INT NOT NULL DEFAULT 1; -- 1 = Immediate, 2 = AfterDate, etc.
    PRINT 'Added ShowResultType column to MockTests table';
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'ImageUrl')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [ImageUrl] NVARCHAR(500) NULL;
    PRINT 'Added ImageUrl column to MockTests table';
END
GO

-- =====================================================
-- ADD CONSTRAINTS FOR NEW COLUMNS
-- =====================================================

-- Add check constraints
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_MockTests_Status')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD CONSTRAINT [CK_MockTests_Status] CHECK ([Status] IN ('Active', 'Inactive', 'Draft', 'Archived'));
    PRINT 'Added Status check constraint';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_MockTests_Difficulty')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD CONSTRAINT [CK_MockTests_Difficulty] CHECK ([Difficulty] IN ('Easy', 'Medium', 'Hard') OR [Difficulty] IS NULL);
    PRINT 'Added Difficulty check constraint';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_MockTests_ShowResultType')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD CONSTRAINT [CK_MockTests_ShowResultType] CHECK ([ShowResultType] IN (1, 2, 3)); -- 1=Immediate, 2=AfterDate, 3=Never
    PRINT 'Added ShowResultType check constraint';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_MockTests_MockTestType')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD CONSTRAINT [CK_MockTests_MockTestType] CHECK ([MockTestType] IN (1, 2, 3)); -- 1=Practice, 2=Live, 3=PreviousYear
    PRINT 'Added MockTestType check constraint';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_MockTests_MarksPerQuestion')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD CONSTRAINT [CK_MockTests_MarksPerQuestion] CHECK ([MarksPerQuestion] > 0);
    PRINT 'Added MarksPerQuestion check constraint';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_MockTests_NegativeMarkingValue')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD CONSTRAINT [CK_MockTests_NegativeMarkingValue] CHECK ([NegativeMarkingValue] >= 0 OR [NegativeMarkingValue] IS NULL);
    PRINT 'Added NegativeMarkingValue check constraint';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_MockTests_Year')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD CONSTRAINT [CK_MockTests_Year] CHECK ([Year] >= 2000 AND [Year] <= 2100 OR [Year] IS NULL);
    PRINT 'Added Year check constraint';
END
GO

-- =====================================================
-- ADD INDEXES FOR NEW COLUMNS
-- =====================================================

-- Create indexes for performance
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MockTests_SubjectId' AND object_id = OBJECT_ID('MockTests'))
BEGIN
    CREATE INDEX [IX_MockTests_SubjectId] ON [dbo].[MockTests]([SubjectId]);
    PRINT 'Created SubjectId index';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MockTests_TopicId' AND object_id = OBJECT_ID('MockTests'))
BEGIN
    CREATE INDEX [IX_MockTests_TopicId] ON [dbo].[MockTests]([TopicId]);
    PRINT 'Created TopicId index';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MockTests_Status' AND object_id = OBJECT_ID('MockTests'))
BEGIN
    CREATE INDEX [IX_MockTests_Status] ON [dbo].[MockTests]([Status]);
    PRINT 'Created Status index';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MockTests_ExamDate' AND object_id = OBJECT_ID('MockTests'))
BEGIN
    CREATE INDEX [IX_MockTests_ExamDate] ON [dbo].[MockTests]([ExamDate]);
    PRINT 'Created ExamDate index';
END
GO

PRINT 'All missing MockTest columns added successfully!';
GO
