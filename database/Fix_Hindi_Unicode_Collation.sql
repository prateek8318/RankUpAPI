USE [RankUp_QuestionDB]
GO

-- Use proper Unicode collation for Hindi support
PRINT 'Updating to proper Unicode collation for Hindi...'

-- Change database collation to support Unicode properly
ALTER DATABASE [RankUp_QuestionDB] COLLATE SQL_Latin1_General_CP1_CI_AS;
GO

-- Update QuestionTranslations table with proper Unicode collation
ALTER TABLE dbo.QuestionTranslations 
ALTER COLUMN QuestionText NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL;

ALTER TABLE dbo.QuestionTranslations 
ALTER COLUMN OptionA NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.QuestionTranslations 
ALTER COLUMN OptionB NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.QuestionTranslations 
ALTER COLUMN OptionC NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.QuestionTranslations 
ALTER COLUMN OptionD NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.QuestionTranslations 
ALTER COLUMN Explanation NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

-- Clean all corrupted Hindi data
PRINT 'Cleaning corrupted Hindi data...'
DELETE FROM dbo.QuestionTranslations 
WHERE LanguageCode = 'hi';

-- Insert proper Hindi data with N prefix (Unicode)
PRINT 'Inserting proper Hindi Unicode data...'

-- Insert Hindi translation for question ID 17
INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
VALUES (17, 'hi', 
    N'भारतीय संविधान में नीति निदेशक सिद्धांत भाग IV में निहित हैं। ये देश के शासन में मौलिक हैं।',
    N'केवल सर्वोच्च न्यायालय',
    N'सर्वोच्च न्यायालय, उच्च न्यायालय और अधीनस्थ न्यायालय',
    N'केवल उच्च न्यायालय',
    N'केवल अधीनस्थ न्यायालय',
    N'नीति निदेशक सिद्धांत गैर-न्यायिक हैं, लेकिन शासन में मौलिक हैं।',
    GETDATE());

-- Insert Hindi translation for question ID 18
INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
VALUES (18, 'hi',
    N'भारतीय संसदीय प्रणाली ब्रिटिश मॉडल पर आधारित है।',
    N'राष्ट्रपति प्रधानमंत्री की नियुक्ति करता है।',
    N'मंत्री परिषद लोकसभा के प्रति जिम्मेदार है।',
    N'प्रधानमंत्री सरकार का प्रमुख है।',
    N'राष्ट्रपति प्रधानमंत्री को बर्खास्त कर सकता है।',
    N'संसदीय प्रणाली में राष्ट्रपति मंत्री परिषद की सलाह पर कार्य करता है।',
    GETDATE());

-- Insert simple Hindi question for testing
INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
VALUES (19, 'hi',
    N'भारत की राजधानी क्या है?',
    N'मुंबई',
    N'दिल्ली',
    N'कोलकाता',
    N'चेन्नई',
    N'दिल्ली भारत की राजधानी है।',
    GETDATE());

PRINT 'Hindi Unicode data inserted successfully!'
GO

-- Verify the Hindi data
PRINT 'Verifying Hindi Unicode data...'
SELECT QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer = 'B'
FROM dbo.QuestionTranslations 
WHERE LanguageCode = 'hi' 
ORDER BY QuestionId;
GO

PRINT 'Hindi Unicode collation fix completed!'
