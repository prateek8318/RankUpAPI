-- Fix MockTest Status and Subjects Script
-- This script fixes mock test statuses and ensures proper subject assignments

-- Step 1: Update statuses based on PublishDateTime
-- Set status to 'Scheduled' for future publish dates
UPDATE MockTests
SET Status = 'Scheduled'
WHERE PublishDateTime > GETUTCDATE()
  AND Status IN ('Active', 'Inactive');

-- Set status to 'Active' for past publish dates
UPDATE MockTests  
SET Status = 'Active'
WHERE PublishDateTime <= GETUTCDATE()
  AND Status IN ('Active', 'Inactive', 'Scheduled');

-- Step 2: Fix subject assignments for mock tests
-- Update RRB mock tests (ExamId 31) to use Mathematics (SubjectId 1)
UPDATE MockTests
SET SubjectId = 1
WHERE ExamId = 31 
  AND (SubjectId IS NULL OR SubjectId NOT IN (
      SELECT SubjectId FROM [RankUp_MasterDB].[dbo].[ExamSubjects] 
      WHERE ExamId = 31 AND ISNULL(IsActive, 1) = 1
  ));

-- Update other exams to use their first valid subject
UPDATE mt
SET mt.SubjectId = (
    SELECT TOP 1 es.SubjectId 
    FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
    WHERE es.ExamId = mt.ExamId 
      AND ISNULL(es.IsActive, 1) = 1
      AND es.SubjectId <> mt.SubjectId -- Avoid updating if already valid
    ORDER BY es.SubjectId
)
FROM MockTests mt
WHERE mt.SubjectId IS NULL 
   OR NOT EXISTS (
       SELECT 1 FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
       WHERE es.ExamId = mt.ExamId AND es.SubjectId = mt.SubjectId AND ISNULL(es.IsActive, 1) = 1
   );

-- Step 3: Verify the changes
-- Check status distribution
SELECT 
    Status,
    COUNT(*) AS Count,
    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM MockTests) AS DECIMAL(10,2)) AS Percentage
FROM MockTests 
GROUP BY Status
ORDER BY Count DESC;

-- Check subject assignments by exam
SELECT 
    e.Name AS ExamName,
    mt.ExamId,
    s.Name AS SubjectName,
    mt.SubjectId,
    COUNT(*) AS MockTestCount
FROM MockTests mt
LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
GROUP BY e.Name, mt.ExamId, s.Name, mt.SubjectId
ORDER BY e.Name, mt.SubjectId;

-- Check for any remaining issues
SELECT 
    mt.Id,
    mt.Name,
    mt.Status,
    mt.ExamId,
    mt.SubjectId,
    e.Name AS ExamName,
    s.Name AS SubjectName,
    CASE 
        WHEN mt.SubjectId IS NULL THEN 'No Subject'
        WHEN NOT EXISTS (
            SELECT 1 FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
            WHERE es.ExamId = mt.ExamId AND es.SubjectId = mt.SubjectId AND ISNULL(es.IsActive, 1) = 1
        ) THEN 'Invalid Subject'
        ELSE 'Valid'
    END AS SubjectValidation
FROM MockTests mt
LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
WHERE mt.SubjectId IS NULL 
   OR NOT EXISTS (
       SELECT 1 FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es 
       WHERE es.ExamId = mt.ExamId AND es.SubjectId = mt.SubjectId AND ISNULL(es.IsActive, 1) = 1
   )
ORDER BY mt.ExamId, mt.Id;

PRINT 'MockTest statuses and subjects updated successfully!';
PRINT 'Check the results above to verify all changes.';
