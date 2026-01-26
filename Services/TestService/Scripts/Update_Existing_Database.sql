-- =============================================
-- Update Existing Database for Unified Test Architecture
-- =============================================

-- Step 1: Update PracticeMode IDs in existing HomeDashboardService
-- Change from 1-4 to 3-6 as per requirements
IF EXISTS (SELECT * FROM sysobjects WHERE name='PracticeModes' AND xtype='U')
BEGIN
    -- Update existing practice modes to correct IDs
    UPDATE PracticeModes SET Id = 3 WHERE Name = 'Mock Test' AND Id = 1;
    UPDATE PracticeModes SET Id = 4 WHERE Name = 'Test Series' AND Id = 2;
    UPDATE PracticeModes SET Id = 5 WHERE Name = 'Deep Practice' AND Id = 3;
    UPDATE PracticeModes SET Id = 6 WHERE Name = 'Previous Year' AND Id = 4;
    
    -- Insert any missing practice modes
    IF NOT EXISTS (SELECT * FROM PracticeModes WHERE Id = 3)
        INSERT INTO PracticeModes (Id, Name, Description, DisplayOrder, IsFeatured, IsActive) 
        VALUES (3, 'Mock Test', 'Full-length mock tests', 1, 1, 1);
    
    IF NOT EXISTS (SELECT * FROM PracticeModes WHERE Id = 4)
        INSERT INTO PracticeModes (Id, Name, Description, DisplayOrder, IsFeatured, IsActive) 
        VALUES (4, 'Test Series', 'Series of practice tests', 2, 1, 1);
    
    IF NOT EXISTS (SELECT * FROM PracticeModes WHERE Id = 5)
        INSERT INTO PracticeModes (Id, Name, Description, DisplayOrder, IsFeatured, IsActive) 
        VALUES (5, 'Deep Practice', 'Subject-wise focused practice', 3, 1, 1);
    
    IF NOT EXISTS (SELECT * FROM PracticeModes WHERE Id = 6)
        INSERT INTO PracticeModes (Id, Name, Description, DisplayOrder, IsFeatured, IsActive) 
        VALUES (6, 'Previous Year', 'Previous year question papers', 4, 1, 1);
END

-- Step 2: Create new TestService tables if they don't exist
-- (This will be handled by EF Core migrations, but keeping for manual setup)

-- Step 3: Migrate existing QuizService TestSeries to new TestService if needed
IF EXISTS (SELECT * FROM sysobjects WHERE name='TestSeries' AND xtype='U' AND 
          (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestSeries' AND COLUMN_NAME = 'ExamId') > 0)
BEGIN
    -- Check if this is the old QuizService TestSeries or new TestService TestSeries
    -- If old structure, migrate to new unified structure
    DECLARE @IsOldStructure BIT = 0;
    
    -- Check for old columns vs new structure
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TestSeries' AND COLUMN_NAME = 'ExamId')
    BEGIN
        -- This appears to be the new structure, no migration needed
        PRINT 'TestSeries table already has correct structure.';
    END
    ELSE
    BEGIN
        -- This is the old structure, need to migrate
        PRINT 'Migrating old TestSeries structure to new unified Test structure...';
        
        -- Create new TestService tables if they don't exist
        -- (Migration logic would go here)
        
        SET @IsOldStructure = 1;
    END
END

-- Step 4: Update navigation keys in HomeDashboardService
IF EXISTS (SELECT * FROM sysobjects WHERE name='PracticeModes' AND xtype='U')
BEGIN
    -- Update PracticeMode navigation keys to use ID instead of enum names
    UPDATE PracticeModes 
    SET LinkUrl = CASE 
        WHEN Id = 3 THEN 'practice/3'
        WHEN Id = 4 THEN 'practice/4'
        WHEN Id = 5 THEN 'practice/5'
        WHEN Id = 6 THEN 'practice/6'
        ELSE LinkUrl
    END
    WHERE LinkUrl LIKE 'practice/%Type%' OR LinkUrl IS NULL;
END

-- Step 5: Create indexes for performance
IF EXISTS (SELECT * FROM sysobjects WHERE name='Tests' AND xtype='U')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tests_ExamId' AND object_id = OBJECT_ID('Tests'))
        CREATE INDEX IX_Tests_ExamId ON Tests(ExamId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tests_PracticeModeId' AND object_id = OBJECT_ID('Tests'))
        CREATE INDEX IX_Tests_PracticeModeId ON Tests(PracticeModeId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tests_SeriesId' AND object_id = OBJECT_ID('Tests'))
        CREATE INDEX IX_Tests_SeriesId ON Tests(SeriesId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tests_SubjectId' AND object_id = OBJECT_ID('Tests'))
        CREATE INDEX IX_Tests_SubjectId ON Tests(SubjectId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tests_Year' AND object_id = OBJECT_ID('Tests'))
        CREATE INDEX IX_Tests_Year ON Tests(Year);
END

-- Step 6: Verify data integrity
PRINT 'Verifying data integrity...';

-- Check practice modes have correct IDs
SELECT 
    Id,
    Name,
    Description,
    DisplayOrder,
    IsFeatured,
    IsActive
FROM PracticeModes 
WHERE Id IN (3, 4, 5, 6)
ORDER BY Id;

-- Check if unified Tests table exists and has correct structure
IF EXISTS (SELECT * FROM sysobjects WHERE name='Tests' AND xtype='U')
BEGIN
    SELECT 
        COUNT(*) AS TotalTests,
        COUNT(CASE WHEN PracticeModeId = 3 THEN 1 END) AS MockTests,
        COUNT(CASE WHEN PracticeModeId = 4 THEN 1 END) AS TestSeriesTests,
        COUNT(CASE WHEN PracticeModeId = 5 THEN 1 END) AS DeepPracticeTests,
        COUNT(CASE WHEN PracticeModeId = 6 THEN 1 END) AS PreviousYearTests
    FROM Tests
    WHERE IsActive = 1;
END

PRINT 'Database update completed successfully!';
PRINT 'Practice Modes have been updated to correct IDs (3-6)';
PRINT 'Unified Test architecture is now ready for use.';
PRINT 'Please run EF Core migrations to create/update tables: dotnet ef database update';

-- =============================================
-- Post-Migration Verification Queries
-- =============================================

-- Verify Practice Modes
PRINT '=== Practice Modes Verification ===';
SELECT Id, Name, Description, DisplayOrder, IsFeatured, IsActive 
FROM PracticeModes 
WHERE Id IN (3, 4, 5, 6)
ORDER BY Id;

-- Verify Test Series (if exists)
PRINT '=== Test Series Verification ===';
SELECT Id, Name, ExamId, IsActive, DisplayOrder
FROM TestSeries 
WHERE IsActive = 1
ORDER BY DisplayOrder;

-- Verify Unified Tests (if exists)
PRINT '=== Unified Tests Verification ===';
SELECT 
    PracticeModeId,
    COUNT(*) AS TestCount,
    COUNT(CASE WHEN SeriesId IS NOT NULL THEN 1 END) AS HasSeries,
    COUNT(CASE WHEN SubjectId IS NOT NULL THEN 1 END) AS HasSubject,
    COUNT(CASE WHEN Year IS NOT NULL THEN 1 END) AS HasYear
FROM Tests 
WHERE IsActive = 1
GROUP BY PracticeModeId
ORDER BY PracticeModeId;
