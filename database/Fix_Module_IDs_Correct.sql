USE [RankUp_MasterDB]
GO

-- Delete the incorrectly inserted module
DELETE FROM [dbo].[Modules] WHERE Name = 'Deep Practice'

-- Enable identity insert to insert with correct ID
SET IDENTITY_INSERT [dbo].[Modules] ON

INSERT INTO [dbo].[Modules] (Id, Name, Description, IsActive, CreatedAt, UpdatedAt)
VALUES (3, 'Deep Practice', 'Deep practice questions for advanced learning', 1, GETDATE(), GETDATE())

SET IDENTITY_INSERT [dbo].[Modules] OFF

-- Display current modules to verify
SELECT * FROM [dbo].[Modules] ORDER BY Id
GO
