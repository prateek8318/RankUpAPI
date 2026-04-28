USE [RankUp_MasterDB]
GO

-- Create Modules table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Modules' AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Modules](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](200) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
    PRINT 'Modules table created successfully'
END
ELSE
BEGIN
    PRINT 'Modules table already exists'
END
GO

-- Insert common modules based on the module IDs seen in the data
-- Check if modules with these IDs already exist
IF NOT EXISTS (SELECT 1 FROM [dbo].[Modules] WHERE Id = 0)
BEGIN
    -- Allow manual ID insertion for this special case
    SET IDENTITY_INSERT [dbo].[Modules] ON
    
    INSERT INTO [dbo].[Modules] (Id, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (0, 'General', 'General questions and topics', 1, GETDATE(), GETDATE())
    
    SET IDENTITY_INSERT [dbo].[Modules] OFF
    PRINT 'Module 0 (General) inserted'
END
ELSE
BEGIN
    PRINT 'Module 0 already exists'
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Modules] WHERE Id = 1)
BEGIN
    INSERT INTO [dbo].[Modules] (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES ('Mock Test', 'Mock test questions and practice sets', 1, GETDATE(), GETDATE())
    PRINT 'Module 1 (Mock Test) inserted'
END
ELSE
BEGIN
    PRINT 'Module 1 already exists'
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Modules] WHERE Id = 3)
BEGIN
    INSERT INTO [dbo].[Modules] (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES ('Deep Practice', 'Deep practice questions for advanced learning', 1, GETDATE(), GETDATE())
    PRINT 'Module 3 (Deep Practice) inserted'
END
ELSE
BEGIN
    PRINT 'Module 3 already exists'
END
GO

-- Display current modules
SELECT * FROM [dbo].[Modules] ORDER BY Id
GO
