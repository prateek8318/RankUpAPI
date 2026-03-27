USE [RankUp_MasterDB]
GO

PRINT 'Migrating Countries Table...';
PRINT '====================================================';

-- First, let's see what foreign key constraints exist
SELECT 
    fk.name AS ForeignKeyName,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables AS tp ON fkc.parent_object_id = tp.object_id
INNER JOIN sys.columns AS cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
INNER JOIN sys.tables AS tr ON fkc.referenced_object_id = tr.object_id
INNER JOIN sys.columns AS cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
WHERE tp.name = 'Countries'
GO

-- Drop existing indexes if they exist
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Countries_IsActive' AND object_id = OBJECT_ID('dbo.Countries'))
    DROP INDEX [IX_Countries_IsActive] ON [dbo].[Countries]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Countries_CountryCode' AND object_id = OBJECT_ID('dbo.Countries'))
    DROP INDEX [IX_Countries_CountryCode] ON [dbo].[Countries]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Countries_Iso2' AND object_id = OBJECT_ID('dbo.Countries'))
    DROP INDEX [IX_Countries_Iso2] ON [dbo].[Countries]
GO

-- Drop foreign key constraints (you may need to adjust constraint names)
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.States') AND referenced_object_id = OBJECT_ID('dbo.Countries'))
BEGIN
    DECLARE @sql NVARCHAR(MAX)
    SELECT @sql = 'ALTER TABLE dbo.States DROP CONSTRAINT ' + name
    FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('dbo.States') AND referenced_object_id = OBJECT_ID('dbo.Countries')
    EXEC sp_executesql @sql
END
GO

-- Create a backup of existing data
SELECT * INTO Countries_Backup FROM dbo.Countries
GO

-- Drop the old table
DROP TABLE [dbo].[Countries]
GO

-- Create the new Countries table with the correct structure
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

-- Add indexes for better performance
CREATE INDEX [IX_Countries_Iso2] ON [dbo].[Countries] ([Iso2])
GO

CREATE INDEX [IX_Countries_CountryCode] ON [dbo].[Countries] ([CountryCode])
GO

CREATE INDEX [IX_Countries_IsActive] ON [dbo].[Countries] ([IsActive])
GO

-- Migrate data from backup if it exists and has the expected structure
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Countries_Backup')
BEGIN
    -- Try to migrate data from backup - adjust column mapping as needed
    INSERT INTO [dbo].[Countries] ([Name], [Iso2], [CountryCode], [PhoneLength], [CurrencyCode], [Image], [IsActive], [CreatedAt])
    SELECT 
        ISNULL([Name], 'Unknown') as [Name],
        ISNULL([Code], 'XX') as [Iso2], -- Map old Code field to Iso2
        ISNULL([Code], '+0') as [CountryCode], -- Use Code as CountryCode for migration
        10 as [PhoneLength], -- Default phone length
        'USD' as [CurrencyCode], -- Default currency
        NULL as [Image], -- No image in old structure
        ISNULL([IsActive], 1) as [IsActive],
        ISNULL([CreatedAt], GETUTCDATE()) as [CreatedAt]
    FROM Countries_Backup
    WHERE [Name] IS NOT NULL
END
GO

-- Insert sample data for India if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM [dbo].[Countries] WHERE Iso2 = 'IN')
BEGIN
    INSERT INTO [dbo].[Countries] ([Name], [Iso2], [CountryCode], [PhoneLength], [CurrencyCode], [Image], [IsActive], [CreatedAt])
    VALUES ('India', 'IN', '+91', 10, 'INR', '/uploads/flags/india.png', 1, GETUTCDATE())
END
GO

-- Recreate foreign key constraints if needed (adjust based on your States table structure)
-- Example: If States table has CountryCode column that references Countries.Iso2
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'States') AND 
   EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.States') AND name = 'CountryCode')
BEGIN
    ALTER TABLE dbo.States 
    ADD CONSTRAINT FK_States_Countries FOREIGN KEY (CountryCode)
    REFERENCES dbo.Countries(Iso2)
END
GO

PRINT '====================================================';
PRINT 'COUNTRIES TABLE MIGRATED SUCCESSFULLY!';
PRINT 'Sample data inserted for India';
PRINT '====================================================';
GO
