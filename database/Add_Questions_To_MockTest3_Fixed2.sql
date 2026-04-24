-- Add Created Questions to Mock Test ID 3 (Fixed Version 2)
-- =============================================

-- First, let's get the current highest question number in Mock Test 3
DECLARE @MaxQuestionNumber INT = ISNULL((SELECT MAX(QuestionNumber) FROM MockTestQuestions WHERE MockTestId = 3), 0);

-- Add the newly created questions to Mock Test 3
-- Using the Question IDs that were created: 15, 16, 17, 18, 19

-- Add Question 1 (ID: 15) to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
VALUES (3, 15, @MaxQuestionNumber + 1, 2.00, 0.50);

-- Add Question 2 (ID: 16) to Mock Test 3  
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
VALUES (3, 16, @MaxQuestionNumber + 2, 2.00, 0.50);

-- Add Question 3 (ID: 17) to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
VALUES (3, 17, @MaxQuestionNumber + 3, 2.00, 0.50);

-- Add Question 4 (ID: 18) to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
VALUES (3, 18, @MaxQuestionNumber + 4, 2.00, 0.50);

-- Add Question 5 (ID: 19) to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
VALUES (3, 19, @MaxQuestionNumber + 5, 2.00, 0.50);

-- Update the total questions and total marks in Mock Test 3
DECLARE @TotalQuestions INT = (SELECT COUNT(*) FROM MockTestQuestions WHERE MockTestId = 3);
DECLARE @TotalMarks DECIMAL(10,2) = (SELECT SUM(Marks) FROM MockTestQuestions WHERE MockTestId = 3);

UPDATE MockTests 
SET 
    TotalQuestions = @TotalQuestions,
    TotalMarks = @TotalMarks,
    UpdatedAt = GETDATE()
WHERE Id = 3;

-- Verify the additions
SELECT 
    mt.Id AS MockTestId,
    mt.Name AS MockTestName,
    mt.TotalQuestions,
    mt.TotalMarks,
    mtq.QuestionNumber,
    mtq.Marks,
    LEFT(q.QuestionText, 50) + '...' AS QuestionPreview,
    qt.LanguageCode,
    CASE 
        WHEN qt.LanguageCode = 'en' THEN 'English'
        WHEN qt.LanguageCode = 'hi' THEN 'Hindi'
        ELSE qt.LanguageCode
    END AS Language
FROM MockTests mt
JOIN MockTestQuestions mtq ON mt.Id = mtq.MockTestId
JOIN Questions q ON mtq.QuestionId = q.Id
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE mt.Id = 3
ORDER BY mtq.QuestionNumber, 
         CASE WHEN qt.LanguageCode = 'en' THEN 0 ELSE 1 END,
         qt.LanguageCode;

PRINT 'Questions added to Mock Test 3 successfully!';
PRINT 'Mock Test 3 updated with new lengthy questions for Subject 7.';
PRINT 'Total questions in Mock Test 3: ' + CAST(@TotalQuestions AS VARCHAR);
PRINT 'Total marks in Mock Test 3: ' + CAST(@TotalMarks AS VARCHAR);
