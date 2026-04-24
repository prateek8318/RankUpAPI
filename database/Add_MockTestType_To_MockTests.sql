-- =============================================
-- Add MockTestType column to MockTests table and update from 0 to 1
-- =============================================

-- Add MockTestType column if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MockTests' AND COLUMN_NAME = 'MockTestType')
BEGIN
    ALTER TABLE [dbo].[MockTests] ADD [MockTestType] INT NOT NULL DEFAULT 1;
    PRINT 'MockTestType column added successfully with default value 1';
END
ELSE
BEGIN
    PRINT 'MockTestType column already exists';
    
    -- Update all records to ensure MockTestType is 1 (in case any have 0)
    UPDATE [dbo].[MockTests]
    SET [MockTestType] = 1
    WHERE [MockTestType] = 0 OR [MockTestType] IS NULL;
    
    -- Verify the update
    SELECT 
        COUNT(*) AS TotalRecords,
        COUNT(CASE WHEN [MockTestType] = 1 THEN 1 END) AS Type1Count,
        COUNT(CASE WHEN [MockTestType] = 0 THEN 1 END) AS Type0Count,
        COUNT(CASE WHEN [MockTestType] IS NULL THEN 1 END) AS NullCount
    FROM [dbo].[MockTests];
END

PRINT 'MockTestType column setup completed successfully!';
