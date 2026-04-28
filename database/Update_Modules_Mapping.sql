USE [RankUp_MasterDB]
GO

-- Delete module 0 (General)
DELETE FROM [dbo].[Modules] WHERE Id = 0
PRINT 'Module 0 (General) deleted'

-- Update existing module to have correct IDs
DELETE FROM [dbo].[Modules] WHERE Name = 'Deep Practice'
PRINT 'Deleted existing Deep Practice module'

-- Insert modules with correct IDs matching MockTestType enum
SET IDENTITY_INSERT [dbo].[Modules] ON

INSERT INTO [dbo].[Modules] (Id, Name, Description, IsActive, CreatedAt, UpdatedAt)
VALUES 
(1, 'Mock Test', 'Subject wise mock tests', 1, GETDATE(), GETDATE()),
(2, 'Test Series', 'Full length test series papers', 1, GETDATE(), GETDATE()),
(3, 'Deep Practice', 'Topic wise deep practice questions', 1, GETDATE(), GETDATE()),
(4, 'Previous Year', 'Previous years solved papers', 1, GETDATE(), GETDATE())

SET IDENTITY_INSERT [dbo].[Modules] OFF

PRINT 'Modules updated with correct MockTestType mapping'

-- Display current modules
SELECT * FROM [dbo].[Modules] ORDER BY Id
GO
