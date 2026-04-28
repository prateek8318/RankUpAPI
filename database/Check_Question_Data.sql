USE [RankUp_QuestionDB]
GO

-- Check if Questions table has actual data
PRINT 'Checking Questions table data...'
SELECT TOP 5 
    Id, 
    ModuleId, 
    ExamId, 
    SubjectId, 
    TopicId,
    LEN(QuestionText) as QuestionTextLength,
    LEFT(QuestionText, 100) as QuestionTextSample,
    CorrectAnswer,
    IsPublished,
    IsActive
FROM Questions 
WHERE IsActive = 1
ORDER BY CreatedAt DESC;
GO

-- Check QuestionTranslations table
PRINT 'Checking QuestionTranslations table data...'
SELECT TOP 5
    QuestionId,
    LanguageCode,
    LEN(QuestionText) as QuestionTextLength,
    LEFT(QuestionText, 100) as QuestionTextSample,
    CreatedAt
FROM QuestionTranslations
ORDER BY CreatedAt DESC;
GO

-- Test the stored procedure directly
PRINT 'Testing GetQuestionsWithFilters stored procedure...'
EXEC [dbo].[GetQuestionsWithFilters] 
    @ExamId = NULL, 
    @SubjectId = NULL, 
    @TopicId = NULL, 
    @DifficultyLevel = NULL, 
    @IsPublished = NULL, 
    @LanguageCode = 'en', 
    @PageNumber = 1, 
    @PageSize = 3;
GO

-- Check specific question IDs mentioned in the user's data
PRINT 'Checking specific question IDs from user data...'
SELECT 
    Id,
    ModuleId,
    ExamId,
    SubjectId,
    TopicId,
    LEN(QuestionText) as QuestionTextLength,
    QuestionText,
    CorrectAnswer,
    IsPublished,
    IsActive
FROM Questions 
WHERE Id IN (17, 18, 19, 16, 15, 24, 23, 22, 21, 20)
ORDER BY Id;
GO

-- Check translations for these specific questions
PRINT 'Checking translations for specific question IDs...'
SELECT 
    qt.QuestionId,
    qt.LanguageCode,
    qt.QuestionText,
    qt.OptionA,
    qt.OptionB,
    qt.OptionC,
    qt.OptionD,
    q.CorrectAnswer
FROM QuestionTranslations qt
INNER JOIN Questions q ON qt.QuestionId = q.Id
WHERE qt.QuestionId IN (17, 18, 19, 16, 15, 24, 23, 22, 21, 20)
ORDER BY qt.QuestionId, qt.LanguageCode;
GO
