USE [RankUp_QuestionDB]
GO

-- Update QuestionTranslations table collation for proper Hindi/Unicode support
PRINT 'Updating QuestionTranslations table collation for Unicode support...'

-- Drop existing constraints if any
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.QuestionTranslations'))
BEGIN
    DECLARE @sql NVARCHAR(MAX) = ''
    SELECT @sql = @sql + 'ALTER TABLE dbo.QuestionTranslations DROP CONSTRAINT [' + name + '];' + CHAR(13)
    FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.QuestionTranslations')
    EXEC sp_executesql @sql
END

-- Update column collations to support Unicode (Hindi)
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

-- Recreate foreign key constraints
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_QuestionTranslations_Questions')
BEGIN
    ALTER TABLE dbo.QuestionTranslations 
    ADD CONSTRAINT FK_QuestionTranslations_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id) ON DELETE CASCADE;
END

-- Also update Questions table for better Unicode support
ALTER TABLE dbo.Questions 
ALTER COLUMN QuestionText NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL;

ALTER TABLE dbo.Questions 
ALTER COLUMN OptionA NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.Questions 
ALTER COLUMN OptionB NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.Questions 
ALTER COLUMN OptionC NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.Questions 
ALTER COLUMN OptionD NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

ALTER TABLE dbo.Questions 
ALTER COLUMN Explanation NVARCHAR(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL;

PRINT 'Hindi/Unicode encoding fix completed successfully!'
GO

-- Clean up existing corrupted Hindi data and insert proper sample
PRINT 'Cleaning and inserting sample Hindi data...'

-- Delete any corrupted Hindi translations
DELETE FROM dbo.QuestionTranslations 
WHERE QuestionText LIKE '%à☼%' OR QuestionText LIKE '%☼%' 
   OR OptionA LIKE '%à☼%' OR OptionA LIKE '%☼%'
   OR OptionB LIKE '%à☼%' OR OptionB LIKE '%☼%'
   OR OptionC LIKE '%à☼%' OR OptionC LIKE '%☼%'
   OR OptionD LIKE '%à☼%' OR OptionD LIKE '%☼%'
   OR Explanation LIKE '%à☼%' OR Explanation LIKE '%☼%';

-- Insert sample Hindi translation for question ID 17
IF NOT EXISTS (SELECT 1 FROM dbo.QuestionTranslations WHERE QuestionId = 17 AND LanguageCode = 'hi')
BEGIN
    INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
    VALUES (17, 'hi', 
        N'भारतीय संविधान में नीति निदेशक सिद्धांत भाग IV (अनुच्छेद 36-51) में निहित हैं। ये देश के शासन में मौलिक हैं और राज्य के लिए इन सिद्धांतों को लागू करना एक कर्तव्य है।',
        N'सिर्फ सर्वोच्च न्यायालय',
        N'सर्वोच्च न्यायालय, उच्च न्यायालय और अधीनस्थ न्यायालय',
        N'केवल उच्च न्यायालय',
        N'केवल अधीनस्थ न्यायालय',
        N'यह कथन गलत है कि नीति निदेशक सिद्धांत न्यायिक हैं और न्यायालयों द्वारा लागू किए जा सकते हैं। नीति निदेशक सिद्धांत गैर-न्यायिक हैं, लेकिन शासन में मौलिक हैं।',
        GETDATE());
END

-- Insert sample Hindi translation for question ID 18
IF NOT EXISTS (SELECT 1 FROM dbo.QuestionTranslations WHERE QuestionId = 18 AND LanguageCode = 'hi')
BEGIN
    INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
    VALUES (18, 'hi',
        N'भारतीय संसदीय प्रणाली ब्रिटिश मॉडल पर आधारित है और वेस्टमिंस्टर प्रणाली का अनुसरण करती है। राष्ट्रपति देश का संवैधानिक प्रमुख है जबकि प्रधानमंत्री वास्तविक कार्यकारी है।',
        N'राष्ट्रपति प्रधानमंत्री और अन्य मंत्रियों की नियुक्ति प्रधानमंत्री की सलाह पर करता है।',
        N'मंत्री परिषद लोकसभा के प्रति सामूहिक रूप से जिम्मेदार है और अविश्वास प्रस्ताव द्वारा हटाई जा सकती है।',
        N'प्रधानमंत्री सरकार का प्रमुख है और वास्तविक कार्यकारी शक्तियां भोगता है।',
        N'राष्ट्रपति किसी भी समय बिना संसदीय अनुमोदन के प्रधानमंत्री को बर्खास्त कर सकता है।',
        N'यह कथन भारतीय संसदीय प्रणाली की विशेषता नहीं है कि राष्ट्रपति बिना संसदीय अनुमोदन के प्रधानमंत्री को बर्खास्त कर सकता है। संसदीय प्रणाली में राष्ट्रपति मंत्री परिषद की सलाह पर कार्य करता है।',
        GETDATE());
END

PRINT 'Sample Hindi data inserted successfully!'
GO

-- Verify the Hindi data
PRINT 'Verifying Hindi data storage...'
SELECT QuestionId, LanguageCode, LEFT(QuestionText, 100) AS SampleQuestionText, LEFT(OptionA, 50) AS SampleOptionA
FROM dbo.QuestionTranslations 
WHERE LanguageCode = 'hi' 
ORDER BY QuestionId;
GO

PRINT 'Hindi encoding fix and data cleanup completed!'
