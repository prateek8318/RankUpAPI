-- Fix existing MockTests by adding questions automatically
USE [RankUp_QuestionDB]
GO

PRINT 'Checking existing MockTest data...'

-- Step 1: Check existing MockTests without questions
SELECT 
    mt.Id as MockTestId,
    mt.Name as MockTestName,
    mt.ExamId,
    mt.SubjectId,
    COUNT(mtq.QuestionId) as QuestionCount
FROM MockTests mt
LEFT JOIN MockTestQuestions mtq ON mt.Id = mtq.MockTestId
WHERE mt.IsActive = 1
GROUP BY mt.Id, mt.Name, mt.ExamId, mt.SubjectId
HAVING COUNT(mtq.QuestionId) = 0 OR COUNT(mtq.QuestionId) IS NULL
ORDER BY mt.Id
GO

-- Step 2: Add questions to existing MockTests that don't have questions
PRINT 'Adding questions to existing MockTests...'

-- Add questions to MockTests with no questions
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks, CreatedAt)
SELECT 
    mt.Id as MockTestId,
    q.Id as QuestionId,
    ROW_NUMBER() OVER (PARTITION BY mt.Id ORDER BY q.Id) as QuestionNumber,
    COALESCE(q.Marks, 4) as Marks,
    COALESCE(q.NegativeMarks, 1) as NegativeMarks,
    GETDATE() as CreatedAt
FROM MockTests mt
CROSS APPLY (
    SELECT TOP 5 q.Id, q.Marks, q.NegativeMarks
    FROM Questions q
    WHERE q.IsActive = 1 
        AND q.ModuleId = 1  -- Mock Test questions
        AND (mt.SubjectId IS NULL OR q.SubjectId = mt.SubjectId)
        AND q.Id NOT IN (
            SELECT mtq2.QuestionId 
            FROM MockTestQuestions mtq2 
            WHERE mtq2.MockTestId = mt.Id
        )
    ORDER BY q.Id
) q
WHERE mt.IsActive = 1
    AND NOT EXISTS (
        SELECT 1 FROM MockTestQuestions mtq_check 
        WHERE mtq_check.MockTestId = mt.Id
    )
GO

-- Step 3: Add more questions to MockTests that have some questions but need more
PRINT 'Adding more questions to MockTests with insufficient questions...'

-- Add questions to MockTests that have less than 5 questions
INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks, CreatedAt)
SELECT 
    mt.Id as MockTestId,
    q.Id as QuestionId,
    (
        SELECT COUNT(*) + 1 
        FROM MockTestQuestions mtq_count 
        WHERE mtq_count.MockTestId = mt.Id
    ) as QuestionNumber,
    COALESCE(q.Marks, 4) as Marks,
    COALESCE(q.NegativeMarks, 1) as NegativeMarks,
    GETDATE() as CreatedAt
FROM MockTests mt
CROSS APPLY (
    SELECT TOP (5 - COALESCE(existing_q.QuestionCount, 0)) q.Id, q.Marks, q.NegativeMarks
    FROM Questions q
    WHERE q.IsActive = 1 
        AND q.ModuleId = 1  -- Mock Test questions
        AND (mt.SubjectId IS NULL OR q.SubjectId = mt.SubjectId)
        AND q.Id NOT IN (
            SELECT mtq2.QuestionId 
            FROM MockTestQuestions mtq2 
            WHERE mtq2.MockTestId = mt.Id
        )
    ORDER BY q.Id
) q
INNER JOIN (
    SELECT 
        mtq.MockTestId,
        COUNT(mtq.QuestionId) as QuestionCount
    FROM MockTestQuestions mtq
    GROUP BY mtq.MockTestId
) existing_q ON existing_q.MockTestId = mt.Id
WHERE mt.IsActive = 1
    AND existing_q.QuestionCount < 5
GO

-- Step 4: Verification - Check results
PRINT 'Verification - Updated MockTest question counts:'
SELECT 
    mt.Id as MockTestId,
    mt.Name as MockTestName,
    mt.ExamId,
    mt.SubjectId,
    COUNT(mtq.QuestionId) as QuestionCount
FROM MockTests mt
LEFT JOIN MockTestQuestions mtq ON mt.Id = mtq.MockTestId
WHERE mt.IsActive = 1
GROUP BY mt.Id, mt.Name, mt.ExamId, mt.SubjectId
ORDER BY mt.Id
GO

-- Step 5: Show sample questions added
PRINT 'Sample questions added to MockTests:'
SELECT TOP 10
    mtq.MockTestId,
    mt.Name as MockTestName,
    mtq.QuestionId,
    mtq.QuestionNumber,
    q.QuestionText,
    mtq.Marks,
    mtq.NegativeMarks
FROM MockTestQuestions mtq
INNER JOIN MockTests mt ON mtq.MockTestId = mt.Id
INNER JOIN Questions q ON mtq.QuestionId = q.Id
WHERE mt.IsActive = 1
ORDER BY mtq.MockTestId, mtq.QuestionNumber
GO

PRINT 'Existing MockTests have been populated with questions!'
PRINT 'Check your MockTest endpoints now:'
PRINT 'GET /api/mock-tests/admin/mock-tests/{id}/questions'
