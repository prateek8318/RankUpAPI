-- Fix Language Ordering to Prioritize English as Default
-- =============================================

-- This script fixes the issue where Hindi translations were appearing first
-- instead of English being the default language

-- Update stored procedures to prioritize English language
-- First, let's check if there are any stored procedures that need fixing

-- Fix the GetQuestionById procedure to prioritize English
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_GetById]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE OR ALTER PROCEDURE [dbo].[Question_GetById]
        @Id INT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT q.*, t.*, qt.*
        FROM Questions q
        LEFT JOIN Topics t ON q.TopicId = t.Id
        LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
        WHERE q.Id = @Id AND q.IsActive = 1
        ORDER BY CASE WHEN qt.LanguageCode = ''en'' THEN 0 ELSE 1 END, qt.LanguageCode;
    END
    ')
    
    PRINT 'Fixed Question_GetById procedure to prioritize English';
END

-- Fix the GetQuestions procedure to prioritize English
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_GetAll]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE OR ALTER PROCEDURE [dbo].[Question_GetAll]
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT q.*, t.*, qt.*
        FROM Questions q
        LEFT JOIN Topics t ON q.TopicId = t.Id
        LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
        WHERE q.IsActive = 1
        ORDER BY q.CreatedAt DESC, CASE WHEN qt.LanguageCode = ''en'' THEN 0 ELSE 1 END, qt.LanguageCode;
    END
    ')
    
    PRINT 'Fixed Question_GetAll procedure to prioritize English';
END

-- Fix the Question_GetByMockTest procedure to prioritize English
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_GetByMockTest]') AND type in (N'P', N'PC'))
BEGIN
    EXEC('
    CREATE OR ALTER PROCEDURE [dbo].[Question_GetByMockTest]
        @MockTestId INT,
        @LanguageCode NVARCHAR(10) = ''en''
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT q.*, mtq.QuestionNumber, mtq.Marks, mtq.NegativeMarks,
               qt.QuestionText AS TranslatedQuestionText,
               qt.OptionA AS TranslatedOptionA,
               qt.OptionB AS TranslatedOptionB,
               qt.OptionC AS TranslatedOptionC,
               qt.OptionD AS TranslatedOptionD,
               qt.Explanation AS TranslatedExplanation
        FROM MockTestQuestions mtq
        INNER JOIN Questions q ON mtq.QuestionId = q.Id
        LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
        WHERE mtq.MockTestId = @MockTestId
        AND q.IsActive = 1
        AND (@LanguageCode IS NULL OR qt.LanguageCode = @LanguageCode)
        ORDER BY mtq.QuestionNumber, CASE WHEN qt.LanguageCode = ''en'' THEN 0 ELSE 1 END, qt.LanguageCode;
    END
    ')
    
    PRINT 'Fixed Question_GetByMockTest procedure to prioritize English';
END

-- Create a new stored procedure specifically for getting questions with proper language ordering
CREATE OR ALTER PROCEDURE [dbo].[Question_GetByMockTestWithLanguagePriority]
    @MockTestId INT,
    @PreferredLanguage NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get questions for mock test with language prioritization
    -- English (en) should always come first, then other languages alphabetically
    SELECT 
        q.Id,
        q.QuestionText,
        q.OptionA,
        q.OptionB,
        q.OptionC,
        q.OptionD,
        q.CorrectAnswer,
        q.Explanation,
        q.Marks,
        q.NegativeMarks,
        q.DifficultyLevel,
        q.QuestionType,
        q.SubjectId,
        q.ExamId,
        q.TopicId,
        q.IsPublished,
        q.CreatedAt,
        q.UpdatedAt,
        q.IsActive,
        mtq.QuestionNumber,
        mtq.Marks AS MockTestMarks,
        mtq.NegativeMarks AS MockTestNegativeMarks,
        qt.LanguageCode,
        qt.QuestionText AS TranslatedQuestionText,
        qt.OptionA AS TranslatedOptionA,
        qt.OptionB AS TranslatedOptionB,
        qt.OptionC AS TranslatedOptionC,
        qt.OptionD AS TranslatedOptionD,
        qt.Explanation AS TranslatedExplanation,
        -- Priority ordering: English first, then preferred language, then others
        CASE 
            WHEN qt.LanguageCode = 'en' THEN 0
            WHEN qt.LanguageCode = @PreferredLanguage THEN 1
            ELSE 2
        END AS LanguagePriority
    FROM MockTestQuestions mtq
    INNER JOIN Questions q ON mtq.QuestionId = q.Id
    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
    WHERE mtq.MockTestId = @MockTestId
    AND q.IsActive = 1
    AND q.IsPublished = 1
    ORDER BY mtq.QuestionNumber, 
             CASE 
                 WHEN qt.LanguageCode = 'en' THEN 0
                 WHEN qt.LanguageCode = @PreferredLanguage THEN 1
                 ELSE 2
             END,
             qt.LanguageCode;
END

-- Also fix the QuestionTranslation ordering in general queries
CREATE OR ALTER PROCEDURE [dbo].[Question_GetTranslationsByPriority]
    @QuestionId INT,
    @PreferredLanguage NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        qt.*,
        CASE 
            WHEN qt.LanguageCode = 'en' THEN 0
            WHEN qt.LanguageCode = @PreferredLanguage THEN 1
            ELSE 2
        END AS LanguagePriority
    FROM QuestionTranslations qt
    WHERE qt.QuestionId = @QuestionId
    ORDER BY 
        CASE 
            WHEN qt.LanguageCode = 'en' THEN 0
            WHEN qt.LanguageCode = @PreferredLanguage THEN 1
            ELSE 2
        END,
        qt.LanguageCode;
END

-- Update existing QuestionTranslations to ensure English exists first for all questions
-- Check if there are questions without English translations
SELECT 
    q.Id AS QuestionId,
    q.QuestionText,
    COUNT(qt.QuestionId) AS TranslationCount,
    SUM(CASE WHEN qt.LanguageCode = 'en' THEN 1 ELSE 0 END) AS HasEnglishTranslation
FROM Questions q
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE q.IsActive = 1
AND q.IsPublished = 1
GROUP BY q.Id, q.QuestionText
HAVING SUM(CASE WHEN qt.LanguageCode = 'en' THEN 1 ELSE 0 END) = 0
OR COUNT(qt.QuestionId) = 0;

PRINT 'Language ordering fixes applied successfully!';
PRINT 'English will now appear as the default language in all queries.';
PRINT 'Use Question_GetByMockTestWithLanguagePriority for mock test questions.';
