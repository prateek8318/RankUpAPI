-- Check MockTestSessions table structure
SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MockTestSessions'
ORDER BY ORDINAL_POSITION;

-- Get sample data from MockTestSessions for MockTestId 12
SELECT TOP 5 *
FROM MockTestSessions
WHERE MockTestId = 12;
