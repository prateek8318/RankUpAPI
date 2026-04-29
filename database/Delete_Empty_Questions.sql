-- Delete empty/incomplete questions from database
-- Questions with ID 29 and 30 have empty questionText and no options

-- First, delete from MockTestQuestions (if they are linked to mock tests)
DELETE FROM MockTestQuestions WHERE QuestionId IN (29, 30);

-- Then delete the questions themselves
DELETE FROM Questions WHERE Id IN (29, 30);

-- Verify deletion
SELECT COUNT(*) as 'Deleted Questions Count' 
FROM Questions 
WHERE Id IN (29, 30);

-- Check if any references remain
SELECT 'MockTestQuestions' as TableName, COUNT(*) as Count 
FROM MockTestQuestions 
WHERE QuestionId IN (29, 30)

UNION ALL

SELECT 'QuestionTranslations' as TableName, COUNT(*) as Count 
FROM QuestionTranslations 
WHERE QuestionId IN (29, 30)

UNION ALL

SELECT 'QuestionFeatures' as TableName, COUNT(*) as Count 
FROM QuestionFeatures 
WHERE QuestionId IN (29, 30);
