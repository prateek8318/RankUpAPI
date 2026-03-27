USE [RankUp_MasterDB]
GO

PRINT 'Fixing States Foreign Key...';
PRINT '====================================================';

-- Check the structure of States table first
PRINT 'States table structure:'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'States'
ORDER BY ORDINAL_POSITION
GO

-- Check Countries table structure
PRINT 'Countries table structure:'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Countries'
ORDER BY ORDINAL_POSITION
GO

-- Check if States table has CountryCode column and what type it is
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.States') AND name = 'CountryCode')
BEGIN
    PRINT 'States table has CountryCode column. Checking data...'
    
    -- Show sample data from States to see what CountryCode values exist
    SELECT TOP 5 CountryCode, COUNT(*) as Count 
    FROM dbo.States 
    WHERE CountryCode IS NOT NULL
    GROUP BY CountryCode
    GO
    
    -- Update States table to use valid Iso2 codes from Countries table
    PRINT 'Updating States.CountryCode to match Countries.Iso2...'
    UPDATE s
    SET s.CountryCode = c.Iso2
    FROM dbo.States s
    INNER JOIN dbo.Countries c ON s.CountryCode = c.Iso2 OR s.CountryCode LIKE '%' + c.Iso2 + '%'
    WHERE s.CountryCode IS NOT NULL
    GO
END
ELSE
BEGIN
    PRINT 'States table does not have CountryCode column. Checking for other country-related columns...'
    
    -- Check if States has CountryId column
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.States') AND name = 'CountryId')
    BEGIN
        PRINT 'States table has CountryId column. Adding CountryCode column...'
        
        -- Add CountryCode column
        ALTER TABLE dbo.States 
        ADD CountryCode NVARCHAR(2) NULL
        GO
        
        -- Update CountryCode based on CountryId mapping (assuming CountryId references Countries.Id)
        PRINT 'Updating CountryCode from CountryId...'
        UPDATE s
        SET s.CountryCode = c.Iso2
        FROM dbo.States s
        INNER JOIN dbo.Countries c ON s.CountryId = c.Id
        WHERE s.CountryId IS NOT NULL
        GO
    END
END
GO

-- Now create the foreign key constraint
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.States') AND name = 'CountryCode')
BEGIN
    PRINT 'Creating foreign key constraint for States.CountryCode -> Countries.Iso2...'
    
    -- First, remove any invalid CountryCode values
    DELETE FROM dbo.States 
    WHERE CountryCode IS NOT NULL 
    AND CountryCode NOT IN (SELECT Iso2 FROM dbo.Countries)
    GO
    
    -- Create the foreign key
    ALTER TABLE dbo.States 
    ADD CONSTRAINT FK_States_Countries FOREIGN KEY (CountryCode)
    REFERENCES dbo.Countries(Iso2)
    ON UPDATE CASCADE
    ON DELETE NO ACTION
    GO
    
    PRINT 'Foreign key constraint created successfully!'
END
ELSE
BEGIN
    PRINT 'Could not create foreign key - States table does not have CountryCode column'
END
GO

PRINT '====================================================';
PRINT 'STATES FOREIGN KEY FIX COMPLETED!';
PRINT '====================================================';
GO

-- Verify the foreign key
PRINT 'Verifying foreign key constraints:'
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_SCHEMA_NAME(fk.parent_object_id) + '.' + OBJECT_NAME(fk.parent_object_id) AS ParentTable,
    pc.name AS ParentColumn,
    OBJECT_SCHEMA_NAME(fk.referenced_object_id) + '.' + OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    rc.name AS ReferencedColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns AS pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
INNER JOIN sys.columns AS rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
WHERE OBJECT_NAME(fk.referenced_object_id) = 'Countries'
GO
