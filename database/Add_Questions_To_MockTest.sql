-- Add sample questions to MockTest ID 31
USE [RankUp_QuestionDB]
GO

PRINT 'Adding questions to MockTest ID 31...'

-- First check if MockTest 31 exists
IF NOT EXISTS (SELECT 1 FROM MockTests WHERE Id = 31)
BEGIN
    PRINT 'MockTest ID 31 does not exist. Creating it...'
    
    -- Create MockTest 31 (assuming exam and subject exist)
    INSERT INTO MockTests (Name, ExamId, SubjectId, Duration, TotalMarks, IsActive, CreatedAt)
    VALUES (
        'Sample Mock Test for Demo',
        1, -- Assuming Exam ID 1 exists
        1, -- Assuming Subject ID 1 exists
        60, -- 60 minutes
        100, -- Total marks
        1, -- Active
        GETDATE()
    )
END
ELSE
BEGIN
    PRINT 'MockTest ID 31 exists.'
END

-- Check if we have questions to add
IF EXISTS (SELECT 1 FROM Questions WHERE IsActive = 1 AND ModuleId = 1)
BEGIN
    PRINT 'Adding questions to MockTest 31...'
    
    -- Add first 5 questions to MockTest 31
    INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks, CreatedAt)
    SELECT 
        31 as MockTestId,
        q.Id as QuestionId,
        ROW_NUMBER() OVER (ORDER BY q.Id) as QuestionNumber,
        q.Marks,
        q.NegativeMarks,
        GETDATE() as CreatedAt
    FROM Questions q
    WHERE q.IsActive = 1 
        AND q.ModuleId = 1  -- Mock Test questions
        AND q.Id NOT IN (SELECT QuestionId FROM MockTestQuestions WHERE MockTestId = 31)
    ORDER BY q.Id
    LIMIT 5
    
    PRINT 'Questions added successfully!'
END
ELSE
BEGIN
    PRINT 'No questions found with ModuleId = 1. Creating sample questions...'
    
    -- Create sample questions
    INSERT INTO Questions (QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer, Explanation, Marks, NegativeMarks, ModuleId, ExamId, SubjectId, IsActive, CreatedAt)
    VALUES 
    ('What is the capital of India?', 'Mumbai', 'Delhi', 'Kolkata', 'Chennai', 'B', 'Delhi is the capital of India', 4, 1, 1, 1, 1, 1, GETDATE()),
    ('What is 2 + 2?', '3', '4', '5', '6', 'B', '2 + 2 = 4', 4, 1, 1, 1, 1, 1, GETDATE()),
    ('Which river is called the Ganga of the South?', 'Godavari', 'Krishna', 'Cauvery', 'Mahanadi', 'C', 'Cauvery is known as the Ganga of the South', 4, 1, 1, 1, 1, 1, GETDATE()),
    ('Who is the current Prime Minister of India?', 'Narendra Modi', 'Rahul Gandhi', 'Amit Shah', 'Yogi Adityanath', 'A', 'Narendra Modi is the current PM', 4, 1, 1, 1, 1, 1, GETDATE()),
    ('What is the largest planet in our solar system?', 'Earth', 'Mars', 'Jupiter', 'Saturn', 'C', 'Jupiter is the largest planet', 4, 1, 1, 1, 1, 1, GETDATE())
    
    PRINT 'Sample questions created. Now adding to MockTest 31...'
    
    -- Add the newly created questions to MockTest 31
    INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks, CreatedAt)
    SELECT 
        31 as MockTestId,
        q.Id as QuestionId,
        ROW_NUMBER() OVER (ORDER BY q.Id) as QuestionNumber,
        q.Marks,
        q.NegativeMarks,
        GETDATE() as CreatedAt
    FROM Questions q
    WHERE q.QuestionText IN (
        'What is the capital of India?',
        'What is 2 + 2?',
        'Which river is called the Ganga of the South?',
        'Who is the current Prime Minister of India?',
        'What is the largest planet in our solar system?'
    )
    ORDER BY q.Id
END

-- Verify the results
PRINT 'Verification - Questions in MockTest 31:'
SELECT 
    mtq.QuestionNumber,
    mtq.QuestionId,
    q.QuestionText,
    mtq.Marks,
    mtq.NegativeMarks
FROM MockTestQuestions mtq
INNER JOIN Questions q ON mtq.QuestionId = q.Id
WHERE mtq.MockTestId = 31
ORDER BY mtq.QuestionNumber
GO
