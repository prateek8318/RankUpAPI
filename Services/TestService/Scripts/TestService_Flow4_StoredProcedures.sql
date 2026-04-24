PRINT 'Creating TestService Flow-4 tables and procedures...';
GO

IF OBJECT_ID('dbo.TestPlanMappings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestPlanMappings
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TestId INT NOT NULL,
        SubscriptionPlanId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX UX_TestPlanMappings_TestId
        ON dbo.TestPlanMappings(TestId)
        WHERE IsActive = 1;
END
GO

IF OBJECT_ID('dbo.AttemptAnswers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AttemptAnswers
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        AttemptId INT NOT NULL,
        QuestionId INT NOT NULL,
        Answer NVARCHAR(50) NULL,
        IsMarkedForReview BIT NOT NULL DEFAULT(0),
        IsAnswered BIT NOT NULL DEFAULT(0),
        SavedAt DATETIME2 NOT NULL DEFAULT(SYSUTCDATETIME())
    );

    CREATE UNIQUE INDEX UX_AttemptAnswers_AttemptId_QuestionId
        ON dbo.AttemptAnswers(AttemptId, QuestionId);
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestPlanMapping_CreateOrUpdate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[TestPlanMapping_CreateOrUpdate]
GO
CREATE PROCEDURE [dbo].[TestPlanMapping_CreateOrUpdate]
    @TestId INT,
    @SubscriptionPlanId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.TestPlanMappings WHERE TestId = @TestId)
    BEGIN
        UPDATE dbo.TestPlanMappings
        SET SubscriptionPlanId = @SubscriptionPlanId,
            IsActive = 1,
            UpdatedAt = SYSUTCDATETIME()
        WHERE TestId = @TestId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.TestPlanMappings (TestId, SubscriptionPlanId, IsActive, CreatedAt)
        VALUES (@TestId, @SubscriptionPlanId, 1, SYSUTCDATETIME());
    END
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Test_GetAvailableForUserPaged]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Test_GetAvailableForUserPaged]
GO
CREATE PROCEDURE [dbo].[Test_GetAvailableForUserPaged]
    @UserId INT,
    @ExamId INT,
    @PracticeModeId INT = NULL,
    @SubjectId INT = NULL,
    @SeriesId INT = NULL,
    @Year INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 20;

    ;WITH FilteredTests AS
    (
        SELECT
            t.Id,
            t.ExamId,
            t.PracticeModeId,
            t.SubjectId,
            t.SeriesId,
            t.Year,
            t.Title,
            t.Description,
            t.DurationInMinutes,
            t.TotalQuestions,
            t.TotalMarks,
            t.IsLocked,
            m.SubscriptionPlanId
        FROM dbo.Tests t
        LEFT JOIN dbo.TestPlanMappings m ON m.TestId = t.Id AND m.IsActive = 1
        WHERE t.IsActive = 1
          AND t.ExamId = @ExamId
          AND (@PracticeModeId IS NULL OR t.PracticeModeId = @PracticeModeId)
          AND (@SubjectId IS NULL OR t.SubjectId = @SubjectId)
          AND (@SeriesId IS NULL OR t.SeriesId = @SeriesId)
          AND (@Year IS NULL OR t.Year = @Year)
    )
    SELECT
        ft.Id,
        ft.ExamId,
        ft.PracticeModeId,
        ft.SubjectId,
        ft.SeriesId,
        ft.Year,
        ft.Title,
        ft.Description,
        ft.DurationInMinutes,
        ft.TotalQuestions,
        ft.TotalMarks,
        ft.IsLocked,
        CASE
            WHEN ft.IsLocked = 0 THEN CAST(1 AS BIT)
            WHEN ft.SubscriptionPlanId IS NULL THEN CAST(0 AS BIT)
            ELSE CAST(1 AS BIT)
        END AS IsUnlocked,
        ft.SubscriptionPlanId,
        CAST(NULL AS NVARCHAR(200)) AS SubscriptionPlanName
    FROM FilteredTests ft
    ORDER BY ft.Id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1)
    FROM dbo.Tests t
    WHERE t.IsActive = 1
      AND t.ExamId = @ExamId
      AND (@PracticeModeId IS NULL OR t.PracticeModeId = @PracticeModeId)
      AND (@SubjectId IS NULL OR t.SubjectId = @SubjectId)
      AND (@SeriesId IS NULL OR t.SeriesId = @SeriesId)
      AND (@Year IS NULL OR t.Year = @Year);
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Test_GetSubjectsByExamAndPracticeMode]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Test_GetSubjectsByExamAndPracticeMode]
GO
CREATE PROCEDURE [dbo].[Test_GetSubjectsByExamAndPracticeMode]
    @ExamId INT,
    @PracticeModeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        s.Id,
        s.ExamId,
        s.Name,
        s.Description,
        s.IconUrl,
        s.DisplayOrder,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM dbo.Subjects s
    INNER JOIN dbo.Tests t
        ON t.SubjectId = s.Id
       AND t.ExamId = s.ExamId
       AND t.IsActive = 1
    WHERE s.IsActive = 1
      AND s.ExamId = @ExamId
      AND t.PracticeModeId = @PracticeModeId
    ORDER BY s.DisplayOrder, s.Name;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AttemptAnswer_Save]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[AttemptAnswer_Save]
GO
CREATE PROCEDURE [dbo].[AttemptAnswer_Save]
    @AttemptId INT,
    @QuestionId INT,
    @Answer NVARCHAR(50) = NULL,
    @IsMarkedForReview BIT = 0,
    @IsAnswered BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.AttemptAnswers WHERE AttemptId = @AttemptId AND QuestionId = @QuestionId)
    BEGIN
        UPDATE dbo.AttemptAnswers
        SET Answer = @Answer,
            IsMarkedForReview = @IsMarkedForReview,
            IsAnswered = @IsAnswered,
            SavedAt = SYSUTCDATETIME()
        WHERE AttemptId = @AttemptId AND QuestionId = @QuestionId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.AttemptAnswers (AttemptId, QuestionId, Answer, IsMarkedForReview, IsAnswered, SavedAt)
        VALUES (@AttemptId, @QuestionId, @Answer, @IsMarkedForReview, @IsAnswered, SYSUTCDATETIME());
    END
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AttemptAnswer_GetByAttemptId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[AttemptAnswer_GetByAttemptId]
GO
CREATE PROCEDURE [dbo].[AttemptAnswer_GetByAttemptId]
    @AttemptId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AttemptId, QuestionId, Answer, IsMarkedForReview, IsAnswered, SavedAt
    FROM dbo.AttemptAnswers
    WHERE AttemptId = @AttemptId
    ORDER BY QuestionId;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Test_GetLeaderboard]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Test_GetLeaderboard]
GO
CREATE PROCEDURE [dbo].[Test_GetLeaderboard]
    @TestId INT,
    @Top INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@Top)
        ROW_NUMBER() OVER (ORDER BY ISNULL(a.Score, 0) DESC, DATEDIFF(SECOND, a.StartedAt, a.CompletedAt) ASC) AS Rank,
        a.UserId,
        CAST(CONCAT('User ', a.UserId) AS NVARCHAR(200)) AS UserName,
        CAST(ISNULL(a.Score, 0) AS DECIMAL(10,2)) AS Score,
        ISNULL(a.Accuracy, 0) AS Accuracy,
        ISNULL(DATEDIFF(SECOND, a.StartedAt, a.CompletedAt), 0) AS TimeTakenSeconds
    FROM dbo.UserTestAttempts a
    WHERE a.TestId = @TestId
      AND a.IsActive = 1
      AND a.CompletedAt IS NOT NULL
    ORDER BY ISNULL(a.Score, 0) DESC, DATEDIFF(SECOND, a.StartedAt, a.CompletedAt) ASC;
END
GO

PRINT 'TestService Flow-4 procedures created successfully.';
GO
