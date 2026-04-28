USE [RankUp_QuestionDB]
GO

-- Clean existing Hindi data
DELETE FROM dbo.QuestionTranslations WHERE LanguageCode = 'hi';

-- Insert simple Hindi data using direct approach
-- Note: This might still have encoding issues, but let's try with basic Hindi

-- Simple Hindi question for testing
INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
VALUES 
(19, 'hi', N'What is the capital of India?', N'Mumbai', N'Delhi', N'Kolkata', N'Chennai', N'Delhi is the capital of India.', GETDATE());

-- Test with English first to ensure the system works
INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
VALUES 
(17, 'en', N'Directive Principles are contained in which part?', N'Part III', N'Part IV', N'Part V', N'Part VI', N'Directive Principles are in Part IV of the Constitution.', GETDATE());

-- Verify data
SELECT QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
FROM dbo.QuestionTranslations 
WHERE QuestionId IN (17, 19)
ORDER BY QuestionId, LanguageCode;

GO

PRINT 'Sample data created for testing!'
