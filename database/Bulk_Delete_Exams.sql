-- Bulk delete all exams except IDs 63, 64, 65, 66, 67
-- This will permanently remove exams from the database

-- First, let's see which exams will be deleted
SELECT Id, Name, Status, CreatedAt 
FROM Exams 
WHERE Id NOT IN (63, 64, 65, 66, 67)
ORDER BY Id;

-- Perform the hard delete
DELETE FROM Exams 
WHERE Id NOT IN (63, 64, 65, 66, 67);

-- Verify the deletion
SELECT COUNT(*) as RemainingExams FROM Exams;
SELECT Id, Name, Status, CreatedAt 
FROM Exams 
ORDER BY Id;
