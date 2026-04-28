USE [RankUp_QuestionDB]
GO

-- Create a simple test table for Hindi data
PRINT 'Creating Hindi test table...'

IF OBJECT_ID('dbo.HindiTest', 'U') IS NOT NULL
    DROP TABLE dbo.HindiTest;

CREATE TABLE dbo.HindiTest
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    HindiText NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    EnglishText NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Insert simple Hindi test data
PRINT 'Inserting Hindi test data...'

INSERT INTO dbo.HindiTest (HindiText, EnglishText)
VALUES 
    (N'भारत की राजधानी दिल्ली है।', 'Capital of India is Delhi.'),
    (N'गणित एक महत्वपूर्ण विषय है।', 'Mathematics is an important subject.'),
    (N'संविधान भारत का सर्वोच्च विधान है।', 'Constitution is the supreme law of India.');

GO

-- Test retrieval
PRINT 'Testing Hindi data retrieval...'

SELECT Id, HindiText, EnglishText 
FROM dbo.HindiTest 
ORDER BY Id;
GO

-- Update QuestionTranslations with proper Hindi data using the test approach
PRINT 'Updating QuestionTranslations with proper Hindi data...'

-- Clean existing Hindi data
DELETE FROM dbo.QuestionTranslations WHERE LanguageCode = 'hi';

-- Insert Hindi translations using proper Unicode strings
INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
SELECT 17, 'hi', 
    N'भारतीय संविधान में नीति निदेशक सिद्धांत भाग IV में हैं।',
    N'केवल सर्वोच्च न्यायालय',
    N'सर्वोच्च न्यायालय, उच्च न्यायालय और अधीनस्थ न्यायालय',
    N'केवल उच्च न्यायालय',
    N'केवल अधीनस्थ न्यायालय',
    N'नीति निदेशक सिद्धांत गैर-न्यायिक हैं।',
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.QuestionTranslations WHERE QuestionId = 17 AND LanguageCode = 'hi');

INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
SELECT 18, 'hi',
    N'भारतीय संसदीय प्रणाली ब्रिटिश मॉडल पर आधारित है।',
    N'राष्ट्रपति प्रधानमंत्री की नियुक्ति करता है।',
    N'मंत्री परिषद लोकसभा के प्रति जिम्मेदार है।',
    N'प्रधानमंत्री सरकार का प्रमुख है।',
    N'राष्ट्रपति प्रधानमंत्री को बर्खास्त कर सकता है।',
    N'संसदीय प्रणाली में राष्ट्रपति मंत्री परिषद की सलाह पर कार्य करता है।',
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.QuestionTranslations WHERE QuestionId = 18 AND LanguageCode = 'hi');

INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
SELECT 19, 'hi',
    N'भारत की राजधानी क्या है?',
    N'मुंबई',
    N'दिल्ली',
    N'कोलकाता',
    N'चेन्नई',
    N'दिल्ली भारत की राजधानी है।',
    GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.QuestionTranslations WHERE QuestionId = 19 AND LanguageCode = 'hi');

GO

-- Test the stored procedure with Hindi data
PRINT 'Testing stored procedure with Hindi data...'

EXEC Question_AdminGetPaged @PageNumber=1, @PageSize=2, @LanguageCode='hi';

GO

PRINT 'Hindi data setup completed!'
