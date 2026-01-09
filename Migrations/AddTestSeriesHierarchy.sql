-- Migration: Add Test Series Hierarchy Tables
-- Run this script directly on your SQL Server database
-- This script creates tables first, then adds foreign key constraints

-- Check if Exams table exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Exams')
BEGIN
    PRINT 'ERROR: Exams table does not exist. Please create Exams table first or run EF Core migrations.';
    RETURN;
END
GO

-- Step 1: Create Subjects table WITHOUT foreign key
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Subjects')
BEGIN
    CREATE TABLE [Subjects] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [ExamId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
    );
    PRINT 'Subjects table created.';
END
ELSE
BEGIN
    PRINT 'Subjects table already exists.';
END
GO

-- Step 2: Create Chapters table WITHOUT foreign key
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Chapters')
BEGIN
    CREATE TABLE [Chapters] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [SubjectId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Chapters] PRIMARY KEY ([Id])
    );
    PRINT 'Chapters table created.';
END
ELSE
BEGIN
    PRINT 'Chapters table already exists.';
END
GO

-- Step 3: Create TestSeries table WITHOUT foreign key
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestSeries')
BEGIN
    CREATE TABLE [TestSeries] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [ExamId] int NOT NULL,
        [DurationInMinutes] int NOT NULL DEFAULT 60,
        [TotalMarks] int NOT NULL DEFAULT 100,
        [TotalQuestions] int NOT NULL DEFAULT 0,
        [PassingMarks] int NOT NULL DEFAULT 35,
        [InstructionsEnglish] nvarchar(2000) NULL,
        [InstructionsHindi] nvarchar(2000) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [IsLocked] bit NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_TestSeries] PRIMARY KEY ([Id])
    );
    PRINT 'TestSeries table created.';
END
ELSE
BEGIN
    PRINT 'TestSeries table already exists.';
END
GO

-- Step 4: Create Questions table WITHOUT foreign key
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Questions')
BEGIN
    CREATE TABLE [Questions] (
        [Id] int NOT NULL IDENTITY,
        [QuestionTextEnglish] nvarchar(2000) NOT NULL,
        [QuestionTextHindi] nvarchar(2000) NOT NULL,
        [Type] int NOT NULL DEFAULT 1,
        [QuestionImageUrlEnglish] nvarchar(500) NULL,
        [QuestionImageUrlHindi] nvarchar(500) NULL,
        [QuestionVideoUrlEnglish] nvarchar(500) NULL,
        [QuestionVideoUrlHindi] nvarchar(500) NULL,
        [OptionAEnglish] nvarchar(500) NOT NULL,
        [OptionBEnglish] nvarchar(500) NOT NULL,
        [OptionCEnglish] nvarchar(500) NOT NULL,
        [OptionDEnglish] nvarchar(500) NOT NULL,
        [OptionAHindi] nvarchar(500) NOT NULL,
        [OptionBHindi] nvarchar(500) NOT NULL,
        [OptionCHindi] nvarchar(500) NOT NULL,
        [OptionDHindi] nvarchar(500) NOT NULL,
        [OptionImageAUrl] nvarchar(500) NULL,
        [OptionImageBUrl] nvarchar(500) NULL,
        [OptionImageCUrl] nvarchar(500) NULL,
        [OptionImageDUrl] nvarchar(500) NULL,
        [CorrectAnswer] nvarchar(1) NOT NULL,
        [ExplanationEnglish] nvarchar(2000) NULL,
        [ExplanationHindi] nvarchar(2000) NULL,
        [SolutionImageUrlEnglish] nvarchar(500) NULL,
        [SolutionImageUrlHindi] nvarchar(500) NULL,
        [SolutionVideoUrlEnglish] nvarchar(500) NULL,
        [SolutionVideoUrlHindi] nvarchar(500) NULL,
        [Difficulty] int NOT NULL DEFAULT 1,
        [ChapterId] int NOT NULL,
        [Marks] int NOT NULL DEFAULT 1,
        [NegativeMarks] decimal(18,2) NOT NULL DEFAULT 0,
        [EstimatedTimeInSeconds] int NOT NULL DEFAULT 120,
        [IsMcq] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([Id])
    );
    PRINT 'Questions table created.';
END
ELSE
BEGIN
    PRINT 'Questions table already exists.';
END
GO

-- Step 5: Create TestSeriesQuestions join table WITHOUT foreign keys
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestSeriesQuestions')
BEGIN
    CREATE TABLE [TestSeriesQuestions] (
        [TestSeriesId] int NOT NULL,
        [QuestionId] int NOT NULL,
        [QuestionOrder] int NOT NULL DEFAULT 0,
        [Marks] int NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_TestSeriesQuestions] PRIMARY KEY ([TestSeriesId], [QuestionId])
    );
    PRINT 'TestSeriesQuestions table created.';
END
ELSE
BEGIN
    PRINT 'TestSeriesQuestions table already exists.';
END
GO

-- Step 6: Add Foreign Key Constraints (after all tables are created)

-- Add FK: Subjects -> Exams
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Subjects_Exams_ExamId')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Exams')
    BEGIN
        ALTER TABLE [Subjects]
        ADD CONSTRAINT [FK_Subjects_Exams_ExamId] 
        FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE CASCADE;
        PRINT 'Foreign key FK_Subjects_Exams_ExamId added.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: Cannot add FK_Subjects_Exams_ExamId - Exams table does not exist.';
    END
END
ELSE
BEGIN
    PRINT 'Foreign key FK_Subjects_Exams_ExamId already exists.';
END
GO

-- Add FK: Chapters -> Subjects
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Chapters_Subjects_SubjectId')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Subjects')
    BEGIN
        ALTER TABLE [Chapters]
        ADD CONSTRAINT [FK_Chapters_Subjects_SubjectId] 
        FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE;
        PRINT 'Foreign key FK_Chapters_Subjects_SubjectId added.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: Cannot add FK_Chapters_Subjects_SubjectId - Subjects table does not exist.';
    END
END
ELSE
BEGIN
    PRINT 'Foreign key FK_Chapters_Subjects_SubjectId already exists.';
END
GO

-- Add FK: TestSeries -> Exams
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TestSeries_Exams_ExamId')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Exams')
    BEGIN
        ALTER TABLE [TestSeries]
        ADD CONSTRAINT [FK_TestSeries_Exams_ExamId] 
        FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE CASCADE;
        PRINT 'Foreign key FK_TestSeries_Exams_ExamId added.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: Cannot add FK_TestSeries_Exams_ExamId - Exams table does not exist.';
    END
END
ELSE
BEGIN
    PRINT 'Foreign key FK_TestSeries_Exams_ExamId already exists.';
END
GO

-- Add FK: Questions -> Chapters
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Questions_Chapters_ChapterId')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Chapters')
    BEGIN
        ALTER TABLE [Questions]
        ADD CONSTRAINT [FK_Questions_Chapters_ChapterId] 
        FOREIGN KEY ([ChapterId]) REFERENCES [Chapters] ([Id]) ON DELETE CASCADE;
        PRINT 'Foreign key FK_Questions_Chapters_ChapterId added.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: Cannot add FK_Questions_Chapters_ChapterId - Chapters table does not exist.';
    END
END
ELSE
BEGIN
    PRINT 'Foreign key FK_Questions_Chapters_ChapterId already exists.';
END
GO

-- Add FK: TestSeriesQuestions -> TestSeries
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TestSeriesQuestions_TestSeries_TestSeriesId')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'TestSeries')
    BEGIN
        ALTER TABLE [TestSeriesQuestions]
        ADD CONSTRAINT [FK_TestSeriesQuestions_TestSeries_TestSeriesId] 
        FOREIGN KEY ([TestSeriesId]) REFERENCES [TestSeries] ([Id]) ON DELETE CASCADE;
        PRINT 'Foreign key FK_TestSeriesQuestions_TestSeries_TestSeriesId added.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: Cannot add FK_TestSeriesQuestions_TestSeries_TestSeriesId - TestSeries table does not exist.';
    END
END
ELSE
BEGIN
    PRINT 'Foreign key FK_TestSeriesQuestions_TestSeries_TestSeriesId already exists.';
END
GO

-- Add FK: TestSeriesQuestions -> Questions
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TestSeriesQuestions_Questions_QuestionId')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Questions')
    BEGIN
        ALTER TABLE [TestSeriesQuestions]
        ADD CONSTRAINT [FK_TestSeriesQuestions_Questions_QuestionId] 
        FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE;
        PRINT 'Foreign key FK_TestSeriesQuestions_Questions_QuestionId added.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: Cannot add FK_TestSeriesQuestions_Questions_QuestionId - Questions table does not exist.';
    END
END
ELSE
BEGIN
    PRINT 'Foreign key FK_TestSeriesQuestions_Questions_QuestionId already exists.';
END
GO

-- Step 7: Create Indexes

-- Index on Subjects.ExamId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subjects_ExamId' AND object_id = OBJECT_ID('Subjects'))
BEGIN
    CREATE INDEX [IX_Subjects_ExamId] ON [Subjects] ([ExamId]);
    PRINT 'Index IX_Subjects_ExamId created.';
END
GO

-- Index on Chapters.SubjectId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chapters_SubjectId' AND object_id = OBJECT_ID('Chapters'))
BEGIN
    CREATE INDEX [IX_Chapters_SubjectId] ON [Chapters] ([SubjectId]);
    PRINT 'Index IX_Chapters_SubjectId created.';
END
GO

-- Index on TestSeries.ExamId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TestSeries_ExamId' AND object_id = OBJECT_ID('TestSeries'))
BEGIN
    CREATE INDEX [IX_TestSeries_ExamId] ON [TestSeries] ([ExamId]);
    PRINT 'Index IX_TestSeries_ExamId created.';
END
GO

-- Index on Questions.ChapterId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Questions_ChapterId' AND object_id = OBJECT_ID('Questions'))
BEGIN
    CREATE INDEX [IX_Questions_ChapterId] ON [Questions] ([ChapterId]);
    PRINT 'Index IX_Questions_ChapterId created.';
END
GO

-- Index on TestSeriesQuestions.TestSeriesId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TestSeriesQuestions_TestSeriesId' AND object_id = OBJECT_ID('TestSeriesQuestions'))
BEGIN
    CREATE INDEX [IX_TestSeriesQuestions_TestSeriesId] ON [TestSeriesQuestions] ([TestSeriesId]);
    PRINT 'Index IX_TestSeriesQuestions_TestSeriesId created.';
END
GO

-- Index on TestSeriesQuestions.QuestionId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TestSeriesQuestions_QuestionId' AND object_id = OBJECT_ID('TestSeriesQuestions'))
BEGIN
    CREATE INDEX [IX_TestSeriesQuestions_QuestionId] ON [TestSeriesQuestions] ([QuestionId]);
    PRINT 'Index IX_TestSeriesQuestions_QuestionId created.';
END
GO

PRINT 'Migration completed successfully!';
GO
