-- Delete inactive exams from MasterService database
-- WARNING: This will permanently delete all inactive exams and their related data

USE [RankUp_MasterDB];
GO

PRINT 'Starting cleanup of inactive exams...';

-- First, delete related ExamLanguage entries for inactive exams
DELETE FROM [dbo].[ExamLanguages]
WHERE ExamId IN (
    SELECT Id FROM [dbo].[Exams] 
    WHERE IsActive = 0
);
PRINT 'Deleted ExamLanguages for inactive exams: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' rows';

-- Delete related ExamQualification entries for inactive exams  
DELETE FROM [dbo].[ExamQualifications]
WHERE ExamId IN (
    SELECT Id FROM [dbo].[Exams] 
    WHERE IsActive = 0
);
PRINT 'Deleted ExamQualifications for inactive exams: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' rows';

-- Finally, delete the inactive exams themselves
DELETE FROM [dbo].[Exams]
WHERE IsActive = 0;
PRINT 'Deleted inactive exams: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' rows';

PRINT 'Cleanup completed successfully!';
GO

-- Optional: Show remaining active exams count
SELECT COUNT(*) as ActiveExamsCount 
FROM [dbo].[Exams] 
WHERE IsActive = 1;
PRINT 'Total active exams remaining: ' + CAST((SELECT COUNT(*) FROM [dbo].[Exams] WHERE IsActive = 1) AS VARCHAR);
GO
