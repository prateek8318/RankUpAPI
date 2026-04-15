USE [master]
GO

-- Drop if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RankUp_QuestionDB')
BEGIN
    ALTER DATABASE [RankUp_QuestionDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RankUp_QuestionDB];
END
GO

-- Create with Windows path
CREATE DATABASE [RankUp_QuestionDB]
ON PRIMARY 
( NAME = N'RankUp_QuestionDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_QuestionDB.mdf', SIZE = 8192KB, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'RankUp_QuestionDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_QuestionDB_log.ldf', SIZE = 8192KB, FILEGROWTH = 65536KB )
GO

USE [RankUp_QuestionDB]
GO

-- Topics Table
CREATE TABLE [dbo].[Topics](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](200) NOT NULL,
    [SubjectId] [int] NOT NULL, -- FK to Master Service Subjects table
    [Description] [nvarchar](500) NULL,
    [ParentTopicId] [int] NULL, -- For hierarchical topics
    [SortOrder] [int] NOT NULL DEFAULT 0,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_Topics] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Topics_Topics_ParentTopicId] FOREIGN KEY([ParentTopicId]) REFERENCES [dbo].[Topics] ([Id])
)
GO

-- Questions Table
CREATE TABLE [dbo].[Questions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ModuleId] [int] NULL, -- FK to Master Service Modules table
    [ExamId] [int] NOT NULL, -- FK to Master Service Exams table
    [SubjectId] [int] NOT NULL, -- FK to Master Service Subjects table
    [TopicId] [int] NULL, -- FK to Topics table
    [QuestionText] [nvarchar](max) NOT NULL, -- English question text
    [OptionA] [nvarchar](500) NULL,
    [OptionB] [nvarchar](500) NULL,
    [OptionC] [nvarchar](500) NULL,
    [OptionD] [nvarchar](500) NULL,
    [CorrectAnswer] [char](1) NOT NULL, -- A, B, C, D
    [Explanation] [nvarchar](max) NULL, -- English explanation
    [Marks] [decimal](5,2) NOT NULL DEFAULT 1.00,
    [NegativeMarks] [decimal](5,2) NOT NULL DEFAULT 0.00,
    [DifficultyLevel] [nvarchar](20) NOT NULL DEFAULT 'Medium', -- Easy, Medium, Hard
    [QuestionType] [nvarchar](20) NOT NULL DEFAULT 'MCQ', -- MCQ, TrueFalse, FillInBlanks
    [QuestionImageUrl] [nvarchar](500) NULL,
    [OptionAImageUrl] [nvarchar](500) NULL,
    [OptionBImageUrl] [nvarchar](500) NULL,
    [OptionCImageUrl] [nvarchar](500) NULL,
    [OptionDImageUrl] [nvarchar](500) NULL,
    [ExplanationImageUrl] [nvarchar](500) NULL,
    [SameExplanationForAllLanguages] [bit] NOT NULL DEFAULT 0,
    [Reference] [nvarchar](500) NULL,
    [Tags] [nvarchar](max) NULL, -- JSON array of tags
    [CreatedBy] [int] NOT NULL, -- Admin user ID
    [ReviewedBy] [int] NULL, -- Reviewer admin user ID
    [IsPublished] [bit] NOT NULL DEFAULT 0,
    [PublishDate] [datetime2](7) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- QuestionTranslations Table
CREATE TABLE [dbo].[QuestionTranslations](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [QuestionId] [int] NOT NULL,
    [LanguageCode] [nvarchar](10) NOT NULL, -- en, hi, pa, ta, te
    [QuestionText] [nvarchar](max) NOT NULL,
    [OptionA] [nvarchar](500) NULL,
    [OptionB] [nvarchar](500) NULL,
    [OptionC] [nvarchar](500) NULL,
    [OptionD] [nvarchar](500) NULL,
    [Explanation] [nvarchar](max) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_QuestionTranslations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QuestionTranslations_Questions_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Questions] ([Id]) ON DELETE CASCADE
)
GO

-- QuestionBatches Table (for bulk uploads)
CREATE TABLE [dbo].[QuestionBatches](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BatchName] [nvarchar](200) NOT NULL,
    [Description] [nvarchar](500) NULL,
    [FileName] [nvarchar](500) NOT NULL,
    [FilePath] [nvarchar](500) NOT NULL,
    [TotalQuestions] [int] NOT NULL DEFAULT 0,
    [ProcessedQuestions] [int] NOT NULL DEFAULT 0,
    [FailedQuestions] [int] NOT NULL DEFAULT 0,
    [Status] [nvarchar](20) NOT NULL DEFAULT 'Pending', -- Pending, Processing, Completed, Failed
    [ErrorMessage] [nvarchar](max) NULL,
    [UploadedBy] [int] NOT NULL, -- Admin user ID
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_QuestionBatches] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- QuestionErrors Table (for bulk upload errors)
CREATE TABLE [dbo].[QuestionErrors](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BatchId] [int] NOT NULL,
    [RowNumber] [int] NOT NULL,
    [ErrorMessage] [nvarchar](max) NOT NULL,
    [RawData] [nvarchar](max) NULL, -- The raw Excel/CSV row data
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_QuestionErrors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QuestionErrors_QuestionBatches_BatchId] FOREIGN KEY([BatchId]) REFERENCES [dbo].[QuestionBatches] ([Id]) ON DELETE CASCADE
)
GO

-- Indexes for performance
CREATE INDEX [IX_Topics_SubjectId] ON [dbo].[Topics] ([SubjectId])
GO
CREATE INDEX [IX_Topics_ParentTopicId] ON [dbo].[Topics] ([ParentTopicId])
GO
CREATE INDEX [IX_Topics_IsActive] ON [dbo].[Topics] ([IsActive])
GO

CREATE INDEX [IX_Questions_ExamId] ON [dbo].[Questions] ([ExamId])
GO
CREATE INDEX [IX_Questions_SubjectId] ON [dbo].[Questions] ([SubjectId])
GO
CREATE INDEX [IX_Questions_TopicId] ON [dbo].[Questions] ([TopicId])
GO
CREATE INDEX [IX_Questions_ModuleId] ON [dbo].[Questions] ([ModuleId])
GO
CREATE INDEX [IX_Questions_DifficultyLevel] ON [dbo].[Questions] ([DifficultyLevel])
GO
CREATE INDEX [IX_Questions_IsPublished] ON [dbo].[Questions] ([IsPublished])
GO
CREATE INDEX [IX_Questions_CreatedBy] ON [dbo].[Questions] ([CreatedBy])
GO
CREATE INDEX [IX_Questions_CreatedAt] ON [dbo].[Questions] ([CreatedAt])
GO

CREATE INDEX [IX_QuestionTranslations_QuestionId] ON [dbo].[QuestionTranslations] ([QuestionId])
GO
CREATE INDEX [IX_QuestionTranslations_LanguageCode] ON [dbo].[QuestionTranslations] ([LanguageCode])
GO

CREATE INDEX [IX_QuestionBatches_UploadedBy] ON [dbo].[QuestionBatches] ([UploadedBy])
GO
CREATE INDEX [IX_QuestionBatches_Status] ON [dbo].[QuestionBatches] ([Status])
GO

CREATE INDEX [IX_QuestionErrors_BatchId] ON [dbo].[QuestionErrors] ([BatchId])
GO

-- Stored Procedures

-- Get Questions with Filters
CREATE PROCEDURE [dbo].[GetQuestionsWithFilters]
    @ExamId INT = NULL,
    @SubjectId INT = NULL,
    @TopicId INT = NULL,
    @DifficultyLevel NVARCHAR(20) = NULL,
    @IsPublished BIT = NULL,
    @LanguageCode NVARCHAR(10) = 'en',
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        q.Id,
        q.ModuleId,
        q.ExamId,
        q.SubjectId,
        q.TopicId,
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
        q.CorrectAnswer,
        CASE 
            WHEN qt.LanguageCode = @LanguageCode THEN qt.Explanation
            ELSE q.Explanation
        END AS Explanation,
        q.Marks,
        q.NegativeMarks,
        q.DifficultyLevel,
        q.QuestionType,
        q.QuestionImageUrl,
        q.OptionAImageUrl,
        q.OptionBImageUrl,
        q.OptionCImageUrl,
        q.OptionDImageUrl,
        q.ExplanationImageUrl,
        q.SameExplanationForAllLanguages,
        q.Reference,
        q.Tags,
        q.CreatedBy,
        q.ReviewedBy,
        q.IsPublished,
        q.PublishDate,
        q.CreatedAt,
        q.UpdatedAt,
        q.IsActive,
        t.Name AS TopicName,
        s.Name AS SubjectName,
        e.Name AS ExamName
    FROM Questions q
    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId AND qt.LanguageCode = @LanguageCode
    LEFT JOIN Topics t ON q.TopicId = t.Id
    LEFT JOIN Subjects s ON q.SubjectId = s.Id -- Master Service Subjects
    LEFT JOIN Exams e ON q.ExamId = e.Id -- Master Service Exams
    WHERE q.IsActive = 1
    AND (@ExamId IS NULL OR q.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
    AND (@TopicId IS NULL OR q.TopicId = @TopicId)
    AND (@DifficultyLevel IS NULL OR q.DifficultyLevel = @DifficultyLevel)
    AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished)
    ORDER BY q.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM Questions q
    WHERE q.IsActive = 1
    AND (@ExamId IS NULL OR q.ExamId = @ExamId)
    AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
    AND (@TopicId IS NULL OR q.TopicId = @TopicId)
    AND (@DifficultyLevel IS NULL OR q.DifficultyLevel = @DifficultyLevel)
    AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished);
END
GO

-- Get Question Statistics
CREATE PROCEDURE [dbo].[GetQuestionStatistics]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TotalQuestions,
        SUM(CASE WHEN CreatedAt >= CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS AddedToday,
        SUM(CASE WHEN NegativeMarks > 0 THEN 1 ELSE 0 END) AS NegativeMarksCount,
        SUM(CASE WHEN IsPublished = 0 THEN 1 ELSE 0 END) AS UnpublishedCount,
        SUM(CASE WHEN DifficultyLevel = 'Easy' THEN 1 ELSE 0 END) AS EasyCount,
        SUM(CASE WHEN DifficultyLevel = 'Medium' THEN 1 ELSE 0 END) AS MediumCount,
        SUM(CASE WHEN DifficultyLevel = 'Hard' THEN 1 ELSE 0 END) AS HardCount
    FROM Questions
    WHERE IsActive = 1;
    
    -- Questions by subject
    SELECT 
        s.Name AS SubjectName,
        COUNT(q.Id) AS QuestionCount
    FROM Questions q
    INNER JOIN Subjects s ON q.SubjectId = s.Id -- Master Service Subjects
    WHERE q.IsActive = 1
    GROUP BY s.Name
    ORDER BY QuestionCount DESC;
    
    -- Questions by exam
    SELECT 
        e.Name AS ExamName,
        COUNT(q.Id) AS QuestionCount
    FROM Questions q
    INNER JOIN Exams e ON q.ExamId = e.Id -- Master Service Exams
    WHERE q.IsActive = 1
    GROUP BY e.Name
    ORDER BY QuestionCount DESC;
END
GO

-- Create Question (Single)
CREATE PROCEDURE [dbo].[CreateQuestion]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT,
    @QuestionText NVARCHAR(MAX),
    @OptionA NVARCHAR(500),
    @OptionB NVARCHAR(500),
    @OptionC NVARCHAR(500),
    @OptionD NVARCHAR(500),
    @CorrectAnswer CHAR(1),
    @Explanation NVARCHAR(MAX),
    @Marks DECIMAL(5,2),
    @NegativeMarks DECIMAL(5,2),
    @DifficultyLevel NVARCHAR(20),
    @QuestionType NVARCHAR(20),
    @QuestionImageUrl NVARCHAR(500),
    @OptionAImageUrl NVARCHAR(500),
    @OptionBImageUrl NVARCHAR(500),
    @OptionCImageUrl NVARCHAR(500),
    @OptionDImageUrl NVARCHAR(500),
    @ExplanationImageUrl NVARCHAR(500),
    @SameExplanationForAllLanguages BIT,
    @Reference NVARCHAR(500),
    @Tags NVARCHAR(MAX),
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Questions (
        ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
        CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
        QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
        ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy
    )
    VALUES (
        @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
        @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
        @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
        @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy
    );
    
    SELECT SCOPE_IDENTITY() AS QuestionId;
END
GO

-- Create Question with Translation
CREATE PROCEDURE [dbo].[CreateQuestionWithTranslation]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT,
    @QuestionText NVARCHAR(MAX),
    @OptionA NVARCHAR(500),
    @OptionB NVARCHAR(500),
    @OptionC NVARCHAR(500),
    @OptionD NVARCHAR(500),
    @CorrectAnswer CHAR(1),
    @Explanation NVARCHAR(MAX),
    @Marks DECIMAL(5,2),
    @NegativeMarks DECIMAL(5,2),
    @DifficultyLevel NVARCHAR(20),
    @QuestionType NVARCHAR(20),
    @QuestionImageUrl NVARCHAR(500),
    @OptionAImageUrl NVARCHAR(500),
    @OptionBImageUrl NVARCHAR(500),
    @OptionCImageUrl NVARCHAR(500),
    @OptionDImageUrl NVARCHAR(500),
    @ExplanationImageUrl NVARCHAR(500),
    @SameExplanationForAllLanguages BIT,
    @Reference NVARCHAR(500),
    @Tags NVARCHAR(MAX),
    @CreatedBy INT,
    @LanguageCode NVARCHAR(10),
    @TranslatedQuestionText NVARCHAR(MAX),
    @TranslatedOptionA NVARCHAR(500),
    @TranslatedOptionB NVARCHAR(500),
    @TranslatedOptionC NVARCHAR(500),
    @TranslatedOptionD NVARCHAR(500),
    @TranslatedExplanation NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Insert main question
        INSERT INTO Questions (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
            CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
            QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
            ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy
        )
        VALUES (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
            @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
            @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
            @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy
        );
        
        DECLARE @QuestionId INT = SCOPE_IDENTITY();
        
        -- Insert translation if provided
        IF @LanguageCode IS NOT NULL AND @LanguageCode != 'en'
        BEGIN
            INSERT INTO QuestionTranslations (
                QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
            )
            VALUES (
                @QuestionId, @LanguageCode, @TranslatedQuestionText, @TranslatedOptionA, 
                @TranslatedOptionB, @TranslatedOptionC, @TranslatedOptionD, @TranslatedExplanation
            );
        END
        
        COMMIT TRANSACTION;
        SELECT @QuestionId AS QuestionId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Publish/Unpublish Question
CREATE PROCEDURE [dbo].[ToggleQuestionPublishStatus]
    @QuestionId INT,
    @IsPublished BIT,
    @ReviewedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Questions
    SET IsPublished = @IsPublished,
        ReviewedBy = @ReviewedBy,
        PublishDate = CASE WHEN @IsPublished = 1 THEN GETDATE() ELSE NULL END,
        UpdatedAt = GETDATE()
    WHERE Id = @QuestionId AND IsActive = 1;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Get Topics by Subject
CREATE PROCEDURE [dbo].[GetTopicsBySubject]
    @SubjectId INT,
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.Id,
        t.Name,
        t.SubjectId,
        t.Description,
        t.ParentTopicId,
        t.SortOrder,
        t.IsActive,
        t.CreatedAt,
        t.UpdatedAt,
        pt.Name AS ParentTopicName
    FROM Topics t
    LEFT JOIN Topics pt ON t.ParentTopicId = pt.Id
    WHERE t.SubjectId = @SubjectId
    AND (@IncludeInactive = 1 OR t.IsActive = 1)
    ORDER BY t.SortOrder, t.Name;
END
GO
