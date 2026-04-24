-- Add Created Questions to Mock Test ID 3
-- =============================================

-- First, let's get the current highest question number in Mock Test 3
DECLARE @MaxQuestionNumber INT = ISNULL((SELECT MAX(QuestionNumber) FROM MockTestQuestions WHERE MockTestId = 3), 0);

-- Add the newly created questions to Mock Test 3
-- Note: These question IDs will be generated after running the previous script
-- For now, let's assume the question IDs and create the mapping

-- Add Question 1 to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
SELECT 3, Id, @MaxQuestionNumber + 1, 2.00, 0.50
FROM Questions 
WHERE Id IN (
    SELECT TOP 1 Id FROM Questions 
    WHERE SubjectId = 7 
    AND QuestionText LIKE '%Indian Constitution, adopted on 26th November 1949%'
    ORDER BY Id DESC
);

-- Add Question 2 to Mock Test 3  
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
SELECT 3, Id, @MaxQuestionNumber + 2, 2.00, 0.50
FROM Questions 
WHERE Id IN (
    SELECT TOP 1 Id FROM Questions 
    WHERE SubjectId = 7 
    AND QuestionText LIKE '%Fundamental Rights in the Indian Constitution are enshrined in Part III%'
    ORDER BY Id DESC
);

-- Add Question 3 to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
SELECT 3, Id, @MaxQuestionNumber + 3, 2.00, 0.50
FROM Questions 
WHERE Id IN (
    SELECT TOP 1 Id FROM Questions 
    WHERE SubjectId = 7 
    AND QuestionText LIKE '%The Directive Principles of State Policy (DPSP) are contained in Part IV%'
    ORDER BY Id DESC
);

-- Add Question 4 to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
SELECT 3, Id, @MaxQuestionNumber + 4, 2.00, 0.50
FROM Questions 
WHERE Id IN (
    SELECT TOP 1 Id FROM Questions 
    WHERE SubjectId = 7 
    AND QuestionText LIKE '%The Indian parliamentary system is based on the British model%'
    ORDER BY Id DESC
);

-- Add Question 5 to Mock Test 3
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
SELECT 3, Id, @MaxQuestionNumber + 5, 2.00, 0.50
FROM Questions 
WHERE Id IN (
    SELECT TOP 1 Id FROM Questions 
    WHERE SubjectId = 7 
    AND QuestionText LIKE '%The Indian judicial system is an integrated and independent judiciary%'
    ORDER BY Id DESC
);

-- Update the total questions and total marks in Mock Test 3
UPDATE MockTests 
SET 
    TotalQuestions = (SELECT COUNT(*) FROM MockTestQuestions WHERE MockTestId = 3),
    TotalMarks = (SELECT SUM(Marks) FROM MockTestQuestions WHERE MockTestId = 3),
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
    q.QuestionText,
    qt.LanguageCode,
    qt.QuestionText AS TranslatedQuestion
FROM MockTests mt
JOIN MockTestQuestions mtq ON mt.Id = mtq.MockTestId
JOIN Questions q ON mtq.QuestionId = q.Id
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE mt.Id = 3
ORDER BY mtq.QuestionNumber;

PRINT 'Questions added to Mock Test 3 successfully!';
PRINT 'Mock Test 3 updated with new lengthy questions for Subject 7.';
