-- Simple fix: Just add ModuleId condition to existing topic validation
USE [RankUp_QuestionDB]
GO

-- Get the current procedure content
DECLARE @sql NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID('[dbo].[Question_AdminCreate]'));

-- Replace the topic validation line to include ModuleId check
SET @sql = REPLACE(@sql, 
'IF @TopicId IS NOT NULL',
'IF @TopicId IS NOT NULL AND @ModuleId = 3');

-- Drop and recreate with the fix
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Question_AdminCreate]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Question_AdminCreate]
GO

EXEC(@sql)
GO

PRINT 'Topic validation fix applied - now only validates for ModuleId = 3'
GO
