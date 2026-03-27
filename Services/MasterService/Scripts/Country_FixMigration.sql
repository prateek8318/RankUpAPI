USE [RankUp_MasterDB]
GO

PRINT 'Fixing Countries Table Migration...';
PRINT '====================================================';

-- Step 1: Find and drop ALL foreign key constraints referencing Countries table
DECLARE @sql NVARCHAR(MAX) = ''
DECLARE @cursor CURSOR

SET @cursor = CURSOR FOR
SELECT 
    'ALTER TABLE ' + OBJECT_SCHEMA_NAME(fk.parent_object_id) + '.' + OBJECT_NAME(fk.parent_object_id) + 
    ' DROP CONSTRAINT ' + fk.name + ';'
FROM sys.foreign_keys AS fk
WHERE fk.referenced_object_id = OBJECT_ID('dbo.Countries')

OPEN @cursor
FETCH NEXT FROM @cursor INTO @sql
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Dropping constraint: ' + @sql
    EXEC sp_executesql @sql
    FETCH NEXT FROM @cursor INTO @sql
END
CLOSE @cursor
DEALLOCATE @cursor
GO

PRINT 'All foreign key constraints dropped.'
GO

-- Step 2: Drop the Countries table completely
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries]') AND type in (N'U'))
BEGIN
    PRINT 'Dropping Countries table...'
    DROP TABLE [dbo].[Countries]
END
GO

-- Step 3: Create new Countries table with correct structure
PRINT 'Creating new Countries table...'
CREATE TABLE [dbo].[Countries](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Iso2] [nvarchar](2) NOT NULL,
    [CountryCode] [nvarchar](5) NOT NULL,
    [PhoneLength] [int] NOT NULL DEFAULT 10,
    [CurrencyCode] [nvarchar](3) NOT NULL,
    [Image] [nvarchar](255) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Step 4: Add indexes
CREATE INDEX [IX_Countries_Iso2] ON [dbo].[Countries] ([Iso2])
GO
CREATE INDEX [IX_Countries_CountryCode] ON [dbo].[Countries] ([CountryCode])
GO
CREATE INDEX [IX_Countries_IsActive] ON [dbo].[Countries] ([IsActive])
GO

-- Step 5: Insert sample data
PRINT 'Inserting sample data...'
INSERT INTO [dbo].[Countries] ([Name], [Iso2], [CountryCode], [PhoneLength], [CurrencyCode], [Image], [IsActive], [CreatedAt])
VALUES 
('India', 'IN', '+91', 10, 'INR', '/uploads/flags/india.png', 1, GETUTCDATE()),
('United States', 'US', '+1', 10, 'USD', '/uploads/flags/usa.png', 1, GETUTCDATE()),
('United Kingdom', 'GB', '+44', 11, 'GBP', '/uploads/flags/uk.png', 1, GETUTCDATE()),
('Canada', 'CA', '+1', 10, 'CAD', '/uploads/flags/canada.png', 1, GETUTCDATE()),
('Australia', 'AU', '+61', 9, 'AUD', '/uploads/flags/australia.png', 1, GETUTCDATE())
GO

-- Step 6: Recreate foreign key constraints if States table exists and needs it
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'States')
BEGIN
    -- Check if States table has CountryCode column
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.States') AND name = 'CountryCode')
    BEGIN
        PRINT 'Recreating foreign key constraint for States table...'
        ALTER TABLE dbo.States 
        ADD CONSTRAINT FK_States_Countries FOREIGN KEY (CountryCode)
        REFERENCES dbo.Countries(Iso2)
        ON UPDATE CASCADE
        ON DELETE NO ACTION
    END
END
GO

PRINT '====================================================';
PRINT 'COUNTRIES TABLE FIXED SUCCESSFULLY!';
PRINT 'Sample countries inserted: India, US, UK, Canada, Australia';
PRINT '====================================================';
GO

-- Step 7: Verify the table structure
PRINT 'Verifying table structure...'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Countries'
ORDER BY ORDINAL_POSITION
GO

-- Step 8: Show sample data
PRINT 'Sample data in Countries table:'
SELECT TOP 5 * FROM dbo.Countries ORDER BY Id
GO
