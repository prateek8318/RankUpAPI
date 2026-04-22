-- Add ExamType column to Exams table
-- This script adds the missing ExamType column to support MockTest/TestSeries categorization

-- Check if column exists, if not add it
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Exams]') AND name = N'ExamType')
BEGIN
    ALTER TABLE [dbo].[Exams] 
    ADD [ExamType] [nvarchar](20) NOT NULL DEFAULT 'MockTest';
    
    PRINT 'ExamType column added successfully to Exams table';
END
ELSE
BEGIN
    PRINT 'ExamType column already exists in Exams table';
END

-- Verify the column was added
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Exams' 
    AND COLUMN_NAME = 'ExamType';
