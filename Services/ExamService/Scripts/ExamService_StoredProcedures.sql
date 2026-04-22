-- =========================================================================================
-- ExamService Database Initialization Script
-- This script creates the missing tables and updates the [Exam] table,
-- then sets up all Stored Procedures required by the ExamService Dapper repositories.
-- =========================================================================================

-- 1. Create ExamCategories Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ExamCategories] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE()
    )
    
    -- Seed Default Categories
    INSERT INTO [dbo].[ExamCategories] (Name, Description, IsActive) VALUES 
    ('Test Series', 'Full length test series', 1),
    ('Mock Test', 'Subject wise mock tests', 1),
    ('Deep Practice', 'Subject and topic wise deep practice', 1),
    ('Previous Year Question', 'Previous year question papers', 1)
END
GO

-- 2. Create ExamTypes Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ExamTypes] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [ExamCategoryId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[ExamCategories](Id),
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE()
    )
    
    -- Seed Default Types
    DECLARE @TestSeriesId INT = (SELECT Id FROM [dbo].[ExamCategories] WHERE Name = 'Test Series')
    DECLARE @MockTestId INT = (SELECT Id FROM [dbo].[ExamCategories] WHERE Name = 'Mock Test')
    DECLARE @DeepPracticeId INT = (SELECT Id FROM [dbo].[ExamCategories] WHERE Name = 'Deep Practice')
    
    IF @TestSeriesId IS NOT NULL
        INSERT INTO [dbo].[ExamTypes] (ExamCategoryId, Name, IsActive) VALUES (@TestSeriesId, 'Full length', 1)
    
    IF @MockTestId IS NOT NULL
        INSERT INTO [dbo].[ExamTypes] (ExamCategoryId, Name, IsActive) VALUES (@MockTestId, 'Subject wise', 1)
        
    IF @DeepPracticeId IS NOT NULL
        INSERT INTO [dbo].[ExamTypes] (ExamCategoryId, Name, IsActive) VALUES (@DeepPracticeId, 'Subject + Topic wise', 1)
END
GO

-- 3. Modify Exam Table (If it exists, alter it; otherwise create it)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exams]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Exams] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [DurationInMinutes] INT NOT NULL DEFAULT 60,
        [TotalMarks] INT NOT NULL DEFAULT 100,
        [PassingMarks] INT NOT NULL DEFAULT 35,
        [ImageUrl] NVARCHAR(500) NULL,
        [IsInternational] BIT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
        
        [ExamCategoryId] INT NULL FOREIGN KEY REFERENCES [dbo].[ExamCategories](Id),
        [ExamTypeId] INT NULL FOREIGN KEY REFERENCES [dbo].[ExamTypes](Id),
        [SubjectId] INT NULL,
        [TotalQuestions] INT NOT NULL DEFAULT 0,
        [MarksPerQuestion] DECIMAL(10,2) NOT NULL DEFAULT 1.0,
        [HasNegativeMarking] BIT NOT NULL DEFAULT 0,
        [NegativeMarkingValue] DECIMAL(10,2) NULL,
        [AccessType] NVARCHAR(20) NOT NULL DEFAULT 'Free',
        [SubscriptionPlanId] INT NULL,
        [ExamDate] DATETIME NULL,
        [PublishDateTime] DATETIME NULL,
        [ValidTill] DATETIME NULL,
        [AttemptsAllowed] INT NOT NULL DEFAULT 1,
        [ShowResultType] NVARCHAR(50) NOT NULL DEFAULT 'Immediately',
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft'
    )
END
ELSE
BEGIN
    -- Add columns if they don't exist
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ExamCategoryId' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [ExamCategoryId] INT NULL FOREIGN KEY REFERENCES [dbo].[ExamCategories](Id);
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ExamTypeId' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [ExamTypeId] INT NULL FOREIGN KEY REFERENCES [dbo].[ExamTypes](Id);
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'SubjectId' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [SubjectId] INT NULL;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'TotalQuestions' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [TotalQuestions] INT NOT NULL DEFAULT 0;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'MarksPerQuestion' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [MarksPerQuestion] DECIMAL(10,2) NOT NULL DEFAULT 1.0;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'HasNegativeMarking' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [HasNegativeMarking] BIT NOT NULL DEFAULT 0;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'NegativeMarkingValue' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [NegativeMarkingValue] DECIMAL(10,2) NULL;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'AccessType' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [AccessType] NVARCHAR(20) NOT NULL DEFAULT 'Free';
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'SubscriptionPlanId' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [SubscriptionPlanId] INT NULL;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ExamDate' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [ExamDate] DATETIME NULL;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'PublishDateTime' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [PublishDateTime] DATETIME NULL;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ValidTill' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [ValidTill] DATETIME NULL;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'AttemptsAllowed' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [AttemptsAllowed] INT NOT NULL DEFAULT 1;
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ShowResultType' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [ShowResultType] NVARCHAR(50) NOT NULL DEFAULT 'Immediately';
        
    IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Status' AND Object_ID = Object_ID(N'dbo.Exams'))
        ALTER TABLE [dbo].[Exams] ADD [Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft';
END
GO


-- =========================================================================================
-- ExamCategory_GetActive
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamCategory_GetActive]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[ExamCategory_GetActive]
GO

CREATE PROCEDURE [dbo].[ExamCategory_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[ExamCategories] WHERE [IsActive] = 1;
END
GO

-- =========================================================================================
-- ExamType_GetByCategoryId
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamType_GetByCategoryId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[ExamType_GetByCategoryId]
GO

CREATE PROCEDURE [dbo].[ExamType_GetByCategoryId]
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[ExamTypes] WHERE [IsActive] = 1 AND [ExamCategoryId] = @CategoryId;
END
GO

-- =========================================================================================
-- Exam_Create
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_Create]
GO

CREATE PROCEDURE [dbo].[Exam_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    
    @ExamCategoryId INT,
    @ExamTypeId INT,
    @SubjectId INT,
    @TotalQuestions INT,
    @MarksPerQuestion DECIMAL(10,2),
    @HasNegativeMarking BIT,
    @NegativeMarkingValue DECIMAL(10,2),
    @AccessType NVARCHAR(20),
    @SubscriptionPlanId INT,
    @ExamDate DATETIME,
    @PublishDateTime DATETIME,
    @ValidTill DATETIME,
    @AttemptsAllowed INT,
    @ShowResultType NVARCHAR(50),
    @Status NVARCHAR(20),
    @DurationInMinutes INT,
    @TotalMarks INT,
    @PassingMarks INT,
    @ImageUrl NVARCHAR(500),
    @IsInternational BIT,

    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Exams] (
        Name, Description, IsActive, CreatedAt, UpdatedAt,
        ExamCategoryId, ExamTypeId, SubjectId, TotalQuestions, MarksPerQuestion,
        HasNegativeMarking, NegativeMarkingValue, AccessType, SubscriptionPlanId,
        ExamDate, PublishDateTime, ValidTill, AttemptsAllowed, ShowResultType, Status,
        DurationInMinutes, TotalMarks, PassingMarks, ImageUrl, IsInternational
    )
    VALUES (
        @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt,
        @ExamCategoryId, @ExamTypeId, @SubjectId, @TotalQuestions, @MarksPerQuestion,
        @HasNegativeMarking, @NegativeMarkingValue, @AccessType, @SubscriptionPlanId,
        @ExamDate, @PublishDateTime, @ValidTill, @AttemptsAllowed, @ShowResultType, @Status,
        @DurationInMinutes, @TotalMarks, @PassingMarks, @ImageUrl, @IsInternational
    );
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- =========================================================================================
-- Exam_Update
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_Update]
GO

CREATE PROCEDURE [dbo].[Exam_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT,
    @UpdatedAt DATETIME,

    @ExamCategoryId INT,
    @ExamTypeId INT,
    @SubjectId INT,
    @TotalQuestions INT,
    @MarksPerQuestion DECIMAL(10,2),
    @HasNegativeMarking BIT,
    @NegativeMarkingValue DECIMAL(10,2),
    @AccessType NVARCHAR(20),
    @SubscriptionPlanId INT,
    @ExamDate DATETIME,
    @PublishDateTime DATETIME,
    @ValidTill DATETIME,
    @AttemptsAllowed INT,
    @ShowResultType NVARCHAR(50),
    @Status NVARCHAR(20),
    @DurationInMinutes INT,
    @TotalMarks INT,
    @PassingMarks INT,
    @ImageUrl NVARCHAR(500),
    @IsInternational BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Exams]
    SET 
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt,
        ExamCategoryId = @ExamCategoryId,
        ExamTypeId = @ExamTypeId,
        SubjectId = @SubjectId,
        TotalQuestions = @TotalQuestions,
        MarksPerQuestion = @MarksPerQuestion,
        HasNegativeMarking = @HasNegativeMarking,
        NegativeMarkingValue = @NegativeMarkingValue,
        AccessType = @AccessType,
        SubscriptionPlanId = @SubscriptionPlanId,
        ExamDate = @ExamDate,
        PublishDateTime = @PublishDateTime,
        ValidTill = @ValidTill,
        AttemptsAllowed = @AttemptsAllowed,
        ShowResultType = @ShowResultType,
        Status = @Status,
        DurationInMinutes = @DurationInMinutes,
        TotalMarks = @TotalMarks,
        PassingMarks = @PassingMarks,
        ImageUrl = @ImageUrl,
        IsInternational = @IsInternational
    WHERE Id = @Id;
END
GO

-- =========================================================================================
-- Exam_GetById
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetById]
GO

CREATE PROCEDURE [dbo].[Exam_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[Exams] WHERE Id = @Id AND IsDeleted = 0; -- Assuming IsDeleted exists from BaseEntity
END
GO

-- =========================================================================================
-- Exam_GetAll
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetAll]
GO

CREATE PROCEDURE [dbo].[Exam_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[Exams] WHERE IsDeleted = 0;
END
GO

-- =========================================================================================
-- Exam_GetActive
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetActive]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetActive]
GO

CREATE PROCEDURE [dbo].[Exam_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT *
    FROM [dbo].[Exams]
    WHERE IsDeleted = 0
      AND IsActive = 1
      AND Status = 'Active'
      AND (PublishDateTime IS NULL OR PublishDateTime <= GETUTCDATE())
      AND (ValidTill IS NULL OR ValidTill > GETUTCDATE());
END
GO

-- =========================================================================================
-- Exam_GetAllIncludingInactive
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetAllIncludingInactive]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetAllIncludingInactive]
GO

CREATE PROCEDURE [dbo].[Exam_GetAllIncludingInactive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [dbo].[Exams] WHERE IsDeleted = 0;
END
GO

-- =========================================================================================
-- Exam_HardDeleteById
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_HardDeleteById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_HardDeleteById]
GO

CREATE PROCEDURE [dbo].[Exam_HardDeleteById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[Exams] WHERE Id = @Id;
END
GO

-- =========================================================================================
-- Exam_GetByQualificationId
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetByQualificationId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetByQualificationId]
GO

CREATE PROCEDURE [dbo].[Exam_GetByQualificationId]
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT e.* 
    FROM [dbo].[Exams] e
    INNER JOIN [dbo].[ExamQualifications] eq ON e.Id = eq.ExamId
    WHERE eq.QualificationId = @QualificationId
      AND e.IsDeleted = 0
END
GO

-- =========================================================================================
-- Exam_GetByQualificationAndStream
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetByQualificationAndStream]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetByQualificationAndStream]
GO

CREATE PROCEDURE [dbo].[Exam_GetByQualificationAndStream]
    @QualificationId INT,
    @StreamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT e.* 
    FROM [dbo].[Exams] e
    INNER JOIN [dbo].[ExamQualifications] eq ON e.Id = eq.ExamId
    WHERE eq.QualificationId = @QualificationId
      AND (eq.StreamId = @StreamId OR eq.StreamId IS NULL)
      AND e.IsDeleted = 0
END
GO

-- =========================================================================================
-- Exam_GetByIdWithQualifications
-- =========================================================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetByIdWithQualifications]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetByIdWithQualifications]
GO

CREATE PROCEDURE [dbo].[Exam_GetByIdWithQualifications]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT e.*, eq.*
    FROM [dbo].[Exams] e
    LEFT JOIN [dbo].[ExamQualifications] eq ON e.Id = eq.ExamId
    WHERE e.Id = @Id
      AND e.IsDeleted = 0
END
GO

-- =========================================================================================
-- Admin Stored Procedures for Exam Management
-- =========================================================================================

-- Exam_GetStats - Get exam statistics for admin dashboard
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetStats]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetStats]
GO

CREATE PROCEDURE [dbo].[Exam_GetStats]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) as TotalExams,
        COUNT(CASE WHEN IsActive = 1 AND Status = 'Active' THEN 1 END) as ActiveExams,
        COUNT(CASE WHEN Status = 'Scheduled' THEN 1 END) as ScheduledExams,
        COUNT(CASE WHEN Status = 'Draft' THEN 1 END) as DraftExams,
        COUNT(CASE WHEN AccessType = 'Paid' THEN 1 END) as PaidExams,
        COUNT(CASE WHEN ExamCategoryId = 1 THEN 1 END) as TestSeriesCount,
        COUNT(CASE WHEN ExamCategoryId = 2 THEN 1 END) as MockTestCount,
        COUNT(CASE WHEN ExamCategoryId = 3 THEN 1 END) as DeepPracticeCount,
        COUNT(CASE WHEN ExamCategoryId = 4 THEN 1 END) as PreviousYearCount
    FROM [dbo].[Exams]
    WHERE IsDeleted = 0
END
GO

-- ExamCategory_GetAll - Get all exam categories
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamCategory_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[ExamCategory_GetAll]
GO

CREATE PROCEDURE [dbo].[ExamCategory_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[ExamCategories]
    ORDER BY Name
END
GO

-- ExamType_GetByCategoryId - Get exam types by category ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamType_GetByCategoryId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[ExamType_GetByCategoryId]
GO

CREATE PROCEDURE [dbo].[ExamType_GetByCategoryId]
    @ExamCategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        ExamCategoryId,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[ExamTypes]
    WHERE ExamCategoryId = @ExamCategoryId
      AND IsActive = 1
    ORDER BY Name
END
GO

-- Exam_GetFiltered - Get filtered exams for admin
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetFiltered]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetFiltered]
GO

CREATE PROCEDURE [dbo].[Exam_GetFiltered]
    @ExamCategoryId INT = NULL,
    @ExamTypeId INT = NULL,
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Description,
        e.DurationInMinutes,
        e.TotalMarks,
        e.PassingMarks,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive,
        e.ExamCategoryId,
        e.ExamTypeId,
        e.SubjectId,
        e.TotalQuestions,
        e.MarksPerQuestion,
        e.HasNegativeMarking,
        e.NegativeMarkingValue,
        e.AccessType,
        e.SubscriptionPlanId,
        e.ExamDate,
        e.PublishDateTime,
        e.ValidTill,
        e.AttemptsAllowed,
        e.ShowResultType,
        e.Status,
        e.CreatedAt,
        e.UpdatedAt,
        ec.Name as CategoryName,
        et.Name as TypeName
    FROM [dbo].[Exams] e
    LEFT JOIN [dbo].[ExamCategories] ec ON e.ExamCategoryId = ec.Id
    LEFT JOIN [dbo].[ExamTypes] et ON e.ExamTypeId = et.Id
    WHERE e.IsDeleted = 0
      AND (@ExamCategoryId IS NULL OR e.ExamCategoryId = @ExamCategoryId)
      AND (@ExamTypeId IS NULL OR e.ExamTypeId = @ExamTypeId)
      AND (@Status IS NULL OR e.Status = @Status)
    ORDER BY e.CreatedAt DESC
END
GO

-- Exam_UpdateStatus - Update exam status
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_UpdateStatus]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_UpdateStatus]
GO

CREATE PROCEDURE [dbo].[Exam_UpdateStatus]
    @Id INT,
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Exams]
    SET Status = @Status,
        IsActive = CASE
            WHEN @Status = 'Active' THEN 1
            WHEN @Status IN ('Draft', 'Inactive', 'Completed') THEN 0
            ELSE IsActive
        END,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id
      AND IsDeleted = 0
    
    SELECT @@ROWCOUNT as AffectedRows
END
GO

-- Exam_GetDashboard - Get complete dashboard data for admin
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetDashboard]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Exam_GetDashboard]
GO

CREATE PROCEDURE [dbo].[Exam_GetDashboard]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get stats
    DECLARE @TotalExams INT = (SELECT COUNT(*) FROM [dbo].[Exams] WHERE IsDeleted = 0)
    DECLARE @ActiveExams INT = (SELECT COUNT(*) FROM [dbo].[Exams] WHERE IsActive = 1 AND Status = 'Active' AND IsDeleted = 0)
    DECLARE @ScheduledExams INT = (SELECT COUNT(*) FROM [dbo].[Exams] WHERE Status = 'Scheduled' AND IsDeleted = 0)
    DECLARE @DraftExams INT = (SELECT COUNT(*) FROM [dbo].[Exams] WHERE Status = 'Draft' AND IsDeleted = 0)
    DECLARE @PaidExams INT = (SELECT COUNT(*) FROM [dbo].[Exams] WHERE AccessType = 'Paid' AND IsDeleted = 0)
    
    -- Get recent exams
    SELECT TOP 10
        e.Id,
        e.Name,
        e.Status,
        e.AccessType,
        ec.Name as CategoryName,
        e.CreatedAt
    FROM [dbo].[Exams] e
    LEFT JOIN [dbo].[ExamCategories] ec ON e.ExamCategoryId = ec.Id
    WHERE e.IsDeleted = 0
    ORDER BY e.CreatedAt DESC
    
    -- Get category distribution
    SELECT 
        ec.Name as CategoryName,
        COUNT(e.Id) as ExamCount
    FROM [dbo].[ExamCategories] ec
    LEFT JOIN [dbo].[Exams] e ON ec.Id = e.ExamCategoryId AND e.IsDeleted = 0
    GROUP BY ec.Name, ec.Id
    ORDER BY ExamCount DESC
    
    -- Return stats as single row
    SELECT 
        @TotalExams as TotalExams,
        @ActiveExams as ActiveExams,
        @ScheduledExams as ScheduledExams,
        @DraftExams as DraftExams,
        @PaidExams as PaidExams
END
GO
