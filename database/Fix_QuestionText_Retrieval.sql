USE [RankUp_QuestionDB]
GO

-- Fix the GetQuestionsWithFilters stored procedure to handle empty question text properly
PRINT 'Fixing GetQuestionsWithFilters stored procedure...'

-- Drop the existing procedure
IF OBJECT_ID('[dbo].[GetQuestionsWithFilters]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[GetQuestionsWithFilters];
GO

-- Create the improved stored procedure
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
        -- Improved logic: Use translation if available and not empty, otherwise use base text
        CASE 
            WHEN qt.LanguageCode = @LanguageCode 
                 AND qt.QuestionText IS NOT NULL 
                 AND LTRIM(RTRIM(qt.QuestionText)) != '' 
            THEN qt.QuestionText
            WHEN q.QuestionText IS NOT NULL 
                 AND LTRIM(RTRIM(q.QuestionText)) != '' 
            THEN q.QuestionText
            ELSE 'Question text not available'
        END AS QuestionText,
        -- Similar logic for options
        CASE 
            WHEN qt.LanguageCode = @LanguageCode 
                 AND qt.OptionA IS NOT NULL 
                 AND LTRIM(RTRIM(qt.OptionA)) != '' 
            THEN qt.OptionA
            ELSE q.OptionA
        END AS OptionA,
        CASE 
            WHEN qt.LanguageCode = @LanguageCode 
                 AND qt.OptionB IS NOT NULL 
                 AND LTRIM(RTRIM(qt.OptionB)) != '' 
            THEN qt.OptionB
            ELSE q.OptionB
        END AS OptionB,
        CASE 
            WHEN qt.LanguageCode = @LanguageCode 
                 AND qt.OptionC IS NOT NULL 
                 AND LTRIM(RTRIM(qt.OptionC)) != '' 
            THEN qt.OptionC
            ELSE q.OptionC
        END AS OptionC,
        CASE 
            WHEN qt.LanguageCode = @LanguageCode 
                 AND qt.OptionD IS NOT NULL 
                 AND LTRIM(RTRIM(qt.OptionD)) != '' 
            THEN qt.OptionD
            ELSE q.OptionD
        END AS OptionD,
        q.CorrectAnswer,
        CASE 
            WHEN qt.LanguageCode = @LanguageCode 
                 AND qt.Explanation IS NOT NULL 
                 AND LTRIM(RTRIM(qt.Explanation)) != '' 
            THEN qt.Explanation
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
        CAST(q.ExamId AS NVARCHAR(50)) AS ExamName -- Use ExamId as string since Exams table is not available
    FROM Questions q
    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId AND qt.LanguageCode = @LanguageCode
    LEFT JOIN Topics t ON q.TopicId = t.Id
    LEFT JOIN Subjects s ON q.SubjectId = s.Id -- Master Service Subjects
    -- Note: Exams table is in Master Service database, not available here
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

-- Test the fixed stored procedure
PRINT 'Testing the fixed stored procedure...'
EXEC [dbo].[GetQuestionsWithFilters] 
    @ExamId = NULL, 
    @SubjectId = NULL, 
    @TopicId = NULL, 
    @DifficultyLevel = NULL, 
    @IsPublished = NULL, 
    @LanguageCode = 'en', 
    @PageNumber = 1, 
    @PageSize = 5;
GO

-- Test with Hindi language
PRINT 'Testing with Hindi language...'
EXEC [dbo].[GetQuestionsWithFilters] 
    @ExamId = NULL, 
    @SubjectId = NULL, 
    @TopicId = NULL, 
    @DifficultyLevel = NULL, 
    @IsPublished = NULL, 
    @LanguageCode = 'hi', 
    @PageNumber = 1, 
    @PageSize = 3;
GO

PRINT 'GetQuestionsWithFilters stored procedure fixed successfully!'
