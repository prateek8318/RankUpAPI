-- Simple Language Ordering Fix
-- =============================================

-- Create a simple stored procedure for getting mock test questions with English priority
CREATE OR ALTER PROCEDURE [dbo].[MockTest_GetQuestionsWithEnglishPriority]
    @MockTestId INT
AS
BEGIN
    SET NOCOUNT ON;
    
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
        qt.Explanation AS TranslatedExplanation
    FROM MockTestQuestions mtq
    INNER JOIN Questions q ON mtq.QuestionId = q.Id
    LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
    WHERE mtq.MockTestId = @MockTestId
    AND q.IsActive = 1
    AND q.IsPublished = 1
    ORDER BY mtq.QuestionNumber, 
             CASE WHEN qt.LanguageCode = 'en' THEN 0 ELSE 1 END,
             qt.LanguageCode;
END

-- Verify the current Mock Test 3 questions with proper ordering
SELECT 
    mtq.QuestionNumber,
    q.Id AS QuestionId,
    LEFT(q.QuestionText, 30) + '...' AS QuestionPreview,
    qt.LanguageCode,
    CASE 
        WHEN qt.LanguageCode = 'en' THEN 'English'
        WHEN qt.LanguageCode = 'hi' THEN 'Hindi'
        ELSE qt.LanguageCode
    END AS Language,
    qt.QuestionText AS TranslatedText,
    mtq.Marks
FROM MockTestQuestions mtq
INNER JOIN Questions q ON mtq.QuestionId = q.Id
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE mtq.MockTestId = 3
ORDER BY mtq.QuestionNumber, 
         CASE WHEN qt.LanguageCode = 'en' THEN 0 ELSE 1 END,
         qt.LanguageCode;

-- Also check if we need to update the Mock Test passing marks
SELECT 
    Id, Name, TotalQuestions, TotalMarks, PassingMarks
FROM MockTests 
WHERE Id = 3;

-- Update passing marks if needed (35% of total marks)
UPDATE MockTests 
SET PassingMarks = TotalMarks * 0.35
WHERE Id = 3 AND PassingMarks > TotalMarks;

PRINT 'Language ordering fix applied successfully!';
PRINT 'English translations will now appear first in mock test questions.';
PRINT 'Use MockTest_GetQuestionsWithEnglishPriority for proper ordering.';
