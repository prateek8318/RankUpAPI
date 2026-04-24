PRINT 'Running MockTests schema migration...';
GO

IF OBJECT_ID('dbo.MockTests', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MockTests
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        ExamId INT NOT NULL,
        DurationInMinutes INT NOT NULL DEFAULT(60),
        TotalQuestions INT NOT NULL DEFAULT(0),
        TotalMarks DECIMAL(10,2) NOT NULL DEFAULT(0),
        PassingMarks DECIMAL(10,2) NOT NULL DEFAULT(0),
        SubscriptionPlanId INT NULL,
        AccessType NVARCHAR(20) NOT NULL DEFAULT('Free'),
        AttemptsAllowed INT NOT NULL DEFAULT(1),
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedBy INT NOT NULL DEFAULT(0),
        CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NULL
    );
END
GO

-- Backfill columns expected by current repository
IF COL_LENGTH('dbo.MockTests', 'MockTestType') IS NULL
    ALTER TABLE dbo.MockTests ADD MockTestType INT NOT NULL CONSTRAINT DF_MockTests_MockTestType DEFAULT(1);
GO

IF COL_LENGTH('dbo.MockTests', 'SubjectId') IS NULL
    ALTER TABLE dbo.MockTests ADD SubjectId INT NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'TopicId') IS NULL
    ALTER TABLE dbo.MockTests ADD TopicId INT NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'MarksPerQuestion') IS NULL
    ALTER TABLE dbo.MockTests ADD MarksPerQuestion DECIMAL(10,2) NOT NULL CONSTRAINT DF_MockTests_MarksPerQuestion DEFAULT(0);
GO

IF COL_LENGTH('dbo.MockTests', 'HasNegativeMarking') IS NULL
    ALTER TABLE dbo.MockTests ADD HasNegativeMarking BIT NOT NULL CONSTRAINT DF_MockTests_HasNegativeMarking DEFAULT(0);
GO

IF COL_LENGTH('dbo.MockTests', 'NegativeMarkingValue') IS NULL
    ALTER TABLE dbo.MockTests ADD NegativeMarkingValue DECIMAL(10,2) NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'Status') IS NULL
    ALTER TABLE dbo.MockTests ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_MockTests_Status DEFAULT('Active');
GO

IF COL_LENGTH('dbo.MockTests', 'Year') IS NULL
    ALTER TABLE dbo.MockTests ADD [Year] INT NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'Difficulty') IS NULL
    ALTER TABLE dbo.MockTests ADD Difficulty NVARCHAR(50) NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'PaperCode') IS NULL
    ALTER TABLE dbo.MockTests ADD PaperCode NVARCHAR(100) NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'ExamDate') IS NULL
    ALTER TABLE dbo.MockTests ADD ExamDate DATETIME2 NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'PublishDateTime') IS NULL
    ALTER TABLE dbo.MockTests ADD PublishDateTime DATETIME2 NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'ValidTill') IS NULL
    ALTER TABLE dbo.MockTests ADD ValidTill DATETIME2 NULL;
GO

IF COL_LENGTH('dbo.MockTests', 'ShowResultType') IS NULL
    ALTER TABLE dbo.MockTests ADD ShowResultType NVARCHAR(20) NOT NULL CONSTRAINT DF_MockTests_ShowResultType DEFAULT('1');
GO

IF COL_LENGTH('dbo.MockTests', 'ImageUrl') IS NULL
    ALTER TABLE dbo.MockTests ADD ImageUrl NVARCHAR(500) NULL;
GO

PRINT 'MockTests schema migration completed.';
GO
