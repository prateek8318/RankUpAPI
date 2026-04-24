-- =============================================
-- Add MockTestType column to MockTests table
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
END
