-- Update MockTest Subject Mapping Script
-- This script updates existing mock tests to have proper subject mapping based on their exam

-- First, let's see the current state
SELECT 
    mt.Id,
    mt.Name,
    mt.ExamId,
    mt.SubjectId,
    e.Name AS ExamName,
    s.Name AS CurrentSubjectName,
    CASE 
        WHEN mt.SubjectId IS NULL THEN 'No Subject Assigned'
        WHEN EXISTS (
            SELECT 1 FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
            WHERE es.ExamId = mt.ExamId AND es.SubjectId = mt.SubjectId AND ISNULL(es.IsActive, 1) = 1
        ) THEN 'Valid Subject'
        ELSE 'Invalid Subject - Not in Exam'
    END AS SubjectStatus
FROM MockTests mt
LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
ORDER BY mt.ExamId, mt.Id;

-- Update mock tests that have invalid subject assignments
-- Assign the first valid subject from the exam for mock tests with invalid subjects

UPDATE mt
SET mt.SubjectId = (
    SELECT TOP 1 es.SubjectId 
    FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
    WHERE es.ExamId = mt.ExamId 
      AND ISNULL(es.IsActive, 1) = 1
    ORDER BY es.SubjectId
)
FROM MockTests mt
WHERE mt.SubjectId IS NULL 
   OR NOT EXISTS (
       SELECT 1 FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
       WHERE es.ExamId = mt.ExamId AND es.SubjectId = mt.SubjectId AND ISNULL(es.IsActive, 1) = 1
   );

-- Verify the updates
SELECT 
    mt.Id,
    mt.Name,
    mt.ExamId,
    mt.SubjectId,
    e.Name AS ExamName,
    s.Name AS SubjectName,
    CASE 
        WHEN mt.SubjectId IS NULL THEN 'No Subject Assigned'
        WHEN EXISTS (
            SELECT 1 FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
            WHERE es.ExamId = mt.ExamId AND es.SubjectId = mt.SubjectId AND ISNULL(es.IsActive, 1) = 1
        ) THEN 'Valid Subject'
        ELSE 'Invalid Subject - Not in Exam'
    END AS SubjectStatus
FROM MockTests mt
LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
ORDER BY mt.ExamId, mt.Id;

-- Specific updates for known exams if needed
-- For RRB Exam (ID 31) - Assign Mathematics (ID 1) if no valid subject
UPDATE MockTests
SET SubjectId = 1
WHERE ExamId = 31 
  AND (SubjectId IS NULL OR SubjectId NOT IN (
      SELECT SubjectId FROM [RankUp_MasterDB].[dbo].[ExamSubjects] 
      WHERE ExamId = 31 AND ISNULL(IsActive, 1) = 1
  ));

-- For other exams, assign their first valid subject
-- This will automatically handle all other exams

PRINT 'MockTest subject mapping updated successfully!';
PRINT 'Run the SELECT queries above to verify the changes.';
