USE [RankUp_QuestionDB]
GO

-- Drop existing procedure if it exists
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminGetPaged]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminGetPaged]
GO

PRINT 'Creating Question_AdminGetPaged procedure...'
GO

CREATE PROCEDURE [dbo].[Question_AdminGetPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @ModuleId INT = NULL,
    @SubjectId INT = NULL,
    @ExamId INT = NULL,
    @TopicId INT = NULL,
    @Difficulty INT = NULL, -- 1=Easy, 2=Medium, 3=Hard
    @LanguageCode NVARCHAR(10) = NULL,
    @IsPublished BIT = NULL,
    @IncludeInactive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Main query for paginated questions
    WITH FilteredQuestions AS
    (
        SELECT 
            q.Id,
            q.ModuleId,
            q.ExamId,
            q.SubjectId,
            q.TopicId,
            q.Difficulty,
            q.Marks,
            q.NegativeMarks,
            q.IsPublished,
            q.IsActive,
            q.CreatedAt,
            ROW_NUMBER() OVER (ORDER BY q.CreatedAt DESC) AS RowNum
        FROM dbo.Questions q
        WHERE (@IncludeInactive = 1 OR q.IsActive = 1)
          AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
          AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
          AND (@ExamId IS NULL OR q.ExamId = @ExamId)
          AND (@TopicId IS NULL OR q.TopicId = @TopicId)
          AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
          AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished)
    )
    SELECT 
        f.Id,
        f.ModuleId,
        mod.Name AS ModuleName,
        f.ExamId,
        e.Name AS ExamName,
        f.SubjectId,
        s.Name AS SubjectName,
        f.TopicId,
        tp.Name AS TopicName,
        CASE f.Difficulty 
            WHEN 1 THEN 'Easy'
            WHEN 2 THEN 'Medium'
            WHEN 3 THEN 'Hard'
            ELSE 'Medium'
        END AS DifficultyLevel,
        f.Marks,
        f.NegativeMarks,
        f.IsPublished,
        f.IsActive,
        f.CreatedAt,
        (SELECT TOP 1 mtq.MockTestId FROM dbo.MockTestQuestions mtq WHERE mtq.QuestionId = f.Id) AS MockTestId,
        COALESCE(NULLIF(qt.QuestionText, ''), q.QuestionText, '') AS QuestionText,
        COALESCE(NULLIF(qt.OptionA, ''), q.OptionA) AS OptionA,
        COALESCE(NULLIF(qt.OptionB, ''), q.OptionB) AS OptionB,
        COALESCE(NULLIF(qt.OptionC, ''), q.OptionC) AS OptionC,
        COALESCE(NULLIF(qt.OptionD, ''), q.OptionD) AS OptionD,
        q.CorrectAnswer,
        COALESCE(NULLIF(qt.Explanation, ''), q.Explanation) AS Explanation,
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
        q.UpdatedAt,
        q.PublishDate,
        CASE 
            WHEN qt.LanguageCode = ISNULL(@LanguageCode, 'en') THEN qt.QuestionText
            ELSE q.QuestionText
        END AS DisplayQuestionText,
        ISNULL(qt.LanguageCode, 'en') AS LanguageCode,
        qt.OptionA AS TranslatedOptionA,
        qt.OptionB AS TranslatedOptionB,
        qt.OptionC AS TranslatedOptionC,
        qt.OptionD AS TranslatedOptionD,
        qt.Explanation AS TranslatedExplanation
    FROM FilteredQuestions f
    INNER JOIN dbo.Questions q ON f.Id = q.Id
    OUTER APPLY
    (
        SELECT TOP 1 m.Name
        FROM [RankUp_MasterDB].[dbo].[Modules] m
        WHERE m.Id = f.ModuleId
    ) mod
    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON e.Id = f.ExamId
    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON s.Id = f.SubjectId
    LEFT JOIN dbo.Topics tp ON tp.Id = f.TopicId
    OUTER APPLY
    (
        SELECT TOP 1 qt.QuestionText, qt.LanguageCode, qt.OptionA, qt.OptionB, qt.OptionC, qt.OptionD, qt.Explanation
        FROM dbo.QuestionTranslations qt
        WHERE qt.QuestionId = f.Id
          AND (@LanguageCode IS NULL OR qt.LanguageCode = @LanguageCode)
        ORDER BY CASE WHEN qt.LanguageCode = ISNULL(@LanguageCode, 'en') THEN 0 ELSE 1 END
    ) qt
    WHERE f.RowNum > @Offset AND f.RowNum <= @Offset + @PageSize
    ORDER BY f.CreatedAt DESC;
    
    -- Total count query
    SELECT COUNT(1) AS TotalCount
    FROM dbo.Questions q
    WHERE (@IncludeInactive = 1 OR q.IsActive = 1)
      AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
      AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
      AND (@ExamId IS NULL OR q.ExamId = @ExamId)
      AND (@TopicId IS NULL OR q.TopicId = @TopicId)
      AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
      AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished);
END
GO

PRINT 'Question_AdminGetPaged procedure created successfully!'
GO
