-- =============================================
-- Test Service Database Schema
-- =============================================

-- Create PracticeModes table with correct IDs (3-6)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PracticeModes' AND xtype='U')
BEGIN
    CREATE TABLE PracticeModes (
        Id INT PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IconUrl NVARCHAR(500) NULL,
        ImageUrl NVARCHAR(500) NULL,
        LinkUrl NVARCHAR(500) NULL,
        DisplayOrder INT DEFAULT 0,
        IsFeatured BIT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL
    );

    -- Insert practice modes with correct IDs as per requirements
    INSERT INTO PracticeModes (Id, Name, Description, DisplayOrder, IsFeatured, IsActive) VALUES
    (3, 'Mock Test', 'Full-length mock tests', 1, 1, 1),
    (4, 'Test Series', 'Series of practice tests', 2, 1, 1),
    (5, 'Deep Practice', 'Subject-wise focused practice', 3, 1, 1),
    (6, 'Previous Year', 'Previous year question papers', 4, 1, 1);
END

-- Create ExamMasters table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ExamMasters' AND xtype='U')
BEGIN
    CREATE TABLE ExamMasters (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500) NULL,
        IconUrl NVARCHAR(500) NULL,
        ImageUrl NVARCHAR(500) NULL,
        DisplayOrder INT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE INDEX IX_ExamMasters_IsActive ON ExamMasters(IsActive);
END

-- Create SubjectMasters table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SubjectMasters' AND xtype='U')
BEGIN
    CREATE TABLE SubjectMasters (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ExamId INT NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500) NULL,
        IconUrl NVARCHAR(500) NULL,
        DisplayOrder INT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        FOREIGN KEY (ExamId) REFERENCES ExamMasters(Id)
    );

    CREATE INDEX IX_SubjectMasters_ExamId ON SubjectMasters(ExamId);
    CREATE INDEX IX_SubjectMasters_IsActive ON SubjectMasters(IsActive);
END

-- Create TestSeries table (reuse existing if exists)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestSeries' AND xtype='U')
BEGIN
    CREATE TABLE TestSeries (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        ExamId INT NOT NULL,
        DurationInMinutes INT DEFAULT 60,
        TotalMarks INT DEFAULT 100,
        TotalQuestions INT DEFAULT 0,
        PassingMarks INT DEFAULT 35,
        InstructionsEnglish NVARCHAR(2000) NULL,
        InstructionsHindi NVARCHAR(2000) NULL,
        DisplayOrder INT DEFAULT 0,
        IsLocked BIT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        FOREIGN KEY (ExamId) REFERENCES ExamMasters(Id)
    );

    CREATE INDEX IX_TestSeries_ExamId ON TestSeries(ExamId);
    CREATE INDEX IX_TestSeries_IsActive ON TestSeries(IsActive);
END

-- Create Tests table (unified table for all practice modes)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tests' AND xtype='U')
BEGIN
    CREATE TABLE Tests (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ExamId INT NOT NULL,
        PracticeModeId INT NOT NULL,
        SeriesId INT NULL,
        SubjectId INT NULL,
        Year INT NULL,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        DurationInMinutes INT DEFAULT 60,
        TotalQuestions INT DEFAULT 0,
        TotalMarks INT DEFAULT 100,
        PassingMarks INT DEFAULT 35,
        InstructionsEnglish NVARCHAR(2000) NULL,
        InstructionsHindi NVARCHAR(2000) NULL,
        DisplayOrder INT DEFAULT 0,
        IsLocked BIT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        FOREIGN KEY (ExamId) REFERENCES ExamMasters(Id),
        FOREIGN KEY (PracticeModeId) REFERENCES PracticeModes(Id),
        FOREIGN KEY (SeriesId) REFERENCES TestSeries(Id),
        FOREIGN KEY (SubjectId) REFERENCES SubjectMasters(Id)
    );

    CREATE INDEX IX_Tests_ExamId ON Tests(ExamId);
    CREATE INDEX IX_Tests_PracticeModeId ON Tests(PracticeModeId);
    CREATE INDEX IX_Tests_SeriesId ON Tests(SeriesId);
    CREATE INDEX IX_Tests_SubjectId ON Tests(SubjectId);
    CREATE INDEX IX_Tests_Year ON Tests(Year);
    CREATE INDEX IX_Tests_IsActive ON Tests(IsActive);
END

-- Create Questions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Questions' AND xtype='U')
BEGIN
    CREATE TABLE Questions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        QuestionText NVARCHAR(2000) NOT NULL,
        ImageUrl NVARCHAR(500) NULL,
        VideoUrl NVARCHAR(500) NULL,
        Explanation NVARCHAR(2000) NULL,
        Difficulty INT DEFAULT 2, -- 1=Easy, 2=Medium, 3=Hard
        Marks INT DEFAULT 1,
        DisplayOrder INT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE INDEX IX_Questions_IsActive ON Questions(IsActive);
END

-- Create TestQuestions table (junction table)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TestQuestions' AND xtype='U')
BEGIN
    CREATE TABLE TestQuestions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TestId INT NOT NULL,
        QuestionId INT NOT NULL,
        DisplayOrder INT DEFAULT 0,
        Marks INT DEFAULT 1,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        FOREIGN KEY (TestId) REFERENCES Tests(Id),
        FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
    );

    CREATE INDEX IX_TestQuestions_TestId ON TestQuestions(TestId);
    CREATE INDEX IX_TestQuestions_QuestionId ON TestQuestions(QuestionId);
    CREATE INDEX IX_TestQuestions_IsActive ON TestQuestions(IsActive);
END

-- Create UserTestAttempts table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserTestAttempts' AND xtype='U')
BEGIN
    CREATE TABLE UserTestAttempts (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TestId INT NOT NULL,
        UserId INT NOT NULL,
        StartedAt DATETIME2 NULL,
        CompletedAt DATETIME2 NULL,
        CurrentQuestionIndex INT DEFAULT 0,
        Score INT DEFAULT 0,
        TotalMarks INT DEFAULT 0,
        Accuracy DECIMAL(5,2) DEFAULT 0,
        Status INT DEFAULT 0, -- 0=NotStarted, 1=InProgress, 2=Completed, 3=Expired, 4=Abandoned
        AnswersJson NVARCHAR(1000) NULL,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        FOREIGN KEY (TestId) REFERENCES Tests(Id)
    );

    CREATE INDEX IX_UserTestAttempts_TestId ON UserTestAttempts(TestId);
    CREATE INDEX IX_UserTestAttempts_UserId ON UserTestAttempts(UserId);
    CREATE INDEX IX_UserTestAttempts_Status ON UserTestAttempts(Status);
    CREATE INDEX IX_UserTestAttempts_IsActive ON UserTestAttempts(IsActive);
END

-- =============================================
-- Sample Data (for testing)
-- =============================================

-- Insert sample exam
IF NOT EXISTS (SELECT * FROM ExamMasters WHERE Name = 'UPSC Civil Services')
BEGIN
    INSERT INTO ExamMasters (Name, Description, DisplayOrder, IsActive) VALUES
    ('UPSC Civil Services', 'Union Public Service Commission Civil Services Examination', 1, 1);
END

-- Insert sample subjects
DECLARE @ExamId INT = (SELECT TOP 1 Id FROM ExamMasters WHERE Name = 'UPSC Civil Services');

IF @ExamId IS NOT NULL AND NOT EXISTS (SELECT * FROM SubjectMasters WHERE ExamId = @ExamId AND Name = 'History')
BEGIN
    INSERT INTO SubjectMasters (ExamId, Name, Description, DisplayOrder, IsActive) VALUES
    (@ExamId, 'History', 'Indian History and World History', 1, 1),
    (@ExamId, 'Geography', 'Indian and World Geography', 2, 1),
    (@ExamId, 'Polity', 'Indian Constitution and Political System', 3, 1),
    (@ExamId, 'Economy', 'Indian Economy and Economic Development', 4, 1);
END

-- Insert sample test series
IF @ExamId IS NOT NULL AND NOT EXISTS (SELECT * FROM TestSeries WHERE Name = 'UPSC Prelims Test Series 2024')
BEGIN
    INSERT INTO TestSeries (Name, Description, ExamId, DurationInMinutes, TotalMarks, TotalQuestions, PassingMarks, DisplayOrder, IsActive) VALUES
    ('UPSC Prelims Test Series 2024', 'Comprehensive test series for UPSC Prelims examination', @ExamId, 120, 200, 100, 66, 1, 1);
END

PRINT 'Test Service database schema created successfully!';
