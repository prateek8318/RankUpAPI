USE [RankUp_MasterDB]
GO

-- Simple direct fix for States foreign key
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.States') AND name = 'CountryCode')
BEGIN
    -- Drop existing constraint if it exists
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_States_Countries' AND parent_object_id = OBJECT_ID('dbo.States'))
    BEGIN
        ALTER TABLE dbo.States DROP CONSTRAINT FK_States_Countries
    END
    
    -- Create the foreign key
    ALTER TABLE dbo.States ADD CONSTRAINT FK_States_Countries FOREIGN KEY (CountryCode) REFERENCES dbo.Countries(Iso2)
    
    PRINT 'Foreign key created successfully!'
END
ELSE
BEGIN
    PRINT 'States table does not have CountryCode column'
END
GO

-- Show final result
SELECT * FROM dbo.Countries
GO
