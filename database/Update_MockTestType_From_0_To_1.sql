-- =============================================
-- Update MockTestType from 0 to 1
-- =============================================

-- Update all mock tests with MockTestType = 0 to MockTestType = 1
UPDATE [dbo].[MockTests]
SET [MockTestType] = 1
WHERE [MockTestType] = 0;

-- Verify the update
SELECT 
    COUNT(*) AS TotalRecords,
    COUNT(CASE WHEN [MockTestType] = 1 THEN 1 END) AS Type1Count,
    COUNT(CASE WHEN [MockTestType] = 0 THEN 1 END) AS Type0Count,
    COUNT(CASE WHEN [MockTestType] > 1 THEN 1 END) AS OtherTypesCount
FROM [dbo].[MockTests];

PRINT 'MockTestType updated successfully from 0 to 1!';
