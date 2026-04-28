-- Check if MockTest ID 31 exists
USE [RankUp_QuestionDB]
GO

PRINT 'Checking MockTest ID 31...'
SELECT 
    Id,
    Name,
    ExamId,
    SubjectId,
    IsActive,
    CreatedAt
FROM MockTests 
WHERE Id = 31
GO

-- Check available MockTests
PRINT 'Available MockTests:'
SELECT TOP 10
    Id,
    Name,
    ExamId,
    SubjectId,
    IsActive,
    CreatedAt
FROM MockTests 
WHERE IsActive = 1
ORDER BY Id
GO

-- Check questions available for MockTest 31
PRINT 'Questions available for MockTest mapping:'
SELECT TOP 10
    q.Id,
    q.QuestionText,
    q.ExamId,
    q.SubjectId,
    q.ModuleId,
    q.IsActive
FROM Questions q
WHERE q.IsActive = 1
    AND (q.ModuleId = 1 OR q.ModuleId IS NULL)  -- Mock Test questions
ORDER BY q.Id
GO

-- Check MockTestQuestions table for any mappings
PRINT 'Current MockTestQuestion mappings:'
SELECT TOP 10
    mtq.MockTestId,
    mtq.QuestionId,
    mtq.QuestionNumber,
    mtq.Marks,
    mtq.NegativeMarks,
    mtq.CreatedAt
FROM MockTestQuestions mtq
ORDER BY mtq.MockTestId, mtq.QuestionNumber
GO
