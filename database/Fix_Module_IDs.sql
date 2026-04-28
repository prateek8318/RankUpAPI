USE [RankUp_MasterDB]
GO

-- Fix the Module ID issue - update the incorrectly inserted module
UPDATE [dbo].[Modules] 
SET Id = 3 
WHERE Name = 'Deep Practice' AND Id = 2

-- Enable identity insert to correct the ID
SET IDENTITY_INSERT [dbo].[Modules] ON

-- Delete the incorrectly inserted module and re-insert with correct ID
DELETE FROM [dbo].[Modules] WHERE Name = 'Deep Practice' AND Id = 2

INSERT INTO [dbo].[Modules] (Id, Name, Description, IsActive, CreatedAt, UpdatedAt)
VALUES (3, 'Deep Practice', 'Deep practice questions for advanced learning', 1, GETDATE(), GETDATE())

SET IDENTITY_INSERT [dbo].[Modules] OFF

-- Display current modules to verify
SELECT * FROM [dbo].[Modules] ORDER BY Id
GO
