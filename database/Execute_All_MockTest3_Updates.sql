-- Complete Script for Mock Test 3 Updates
-- =============================================
-- Execute this script in order to:
-- 1. Add lengthy questions for Subject ID 7
-- 2. Add them to Mock Test ID 3
-- 3. Fix language ordering (English as default)

-- Step 1: Execute the lengthy questions creation
-- Run: Add_Lengthy_Questions_MockTest3_Subject7.sql

-- Step 2: Add questions to Mock Test 3
-- Run: Add_Questions_To_MockTest3.sql

-- Step 3: Fix language ordering issues
-- Run: Fix_Language_Ordering.sql

-- After executing all scripts, verify the results:

-- Verify Mock Test 3 has the new questions
SELECT 
    mt.Id AS MockTestId,
    mt.Name AS MockTestName,
    mt.TotalQuestions,
    mt.TotalMarks,
    COUNT(mtq.QuestionId) AS ActualQuestions,
    SUM(mtq.Marks) AS ActualMarks
FROM MockTests mt
LEFT JOIN MockTestQuestions mtq ON mt.Id = mtq.MockTestId
WHERE mt.Id = 3
GROUP BY mt.Id, mt.Name, mt.TotalQuestions, mt.TotalMarks;

-- Verify the new questions with proper language ordering
SELECT 
    mtq.QuestionNumber,
    q.Id AS QuestionId,
    LEFT(q.QuestionText, 50) + '...' AS QuestionPreview,
    qt.LanguageCode,
    CASE 
        WHEN qt.LanguageCode = 'en' THEN 'English'
        WHEN qt.LanguageCode = 'hi' THEN 'Hindi'
        ELSE qt.LanguageCode
    END AS Language,
    qt.QuestionText AS TranslatedText,
    mtq.Marks
FROM MockTestQuestions mtq
INNER JOIN Questions q ON mtq.QuestionId = q.Id
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE mtq.MockTestId = 3
ORDER BY mtq.QuestionNumber, 
         CASE WHEN qt.LanguageCode = 'en' THEN 0 ELSE 1 END,
         qt.LanguageCode;

-- Check Subject 7 details
SELECT 
    s.Id AS SubjectId,
    s.Name AS SubjectName,
    COUNT(q.Id) AS TotalQuestions,
    COUNT(CASE WHEN q.IsPublished = 1 THEN 1 END) AS PublishedQuestions
FROM Subjects s
LEFT JOIN Questions q ON s.Id = q.SubjectId
WHERE s.Id = 7
GROUP BY s.Id, s.Name;

-- Verify language translations are properly ordered
SELECT 
    q.Id AS QuestionId,
    LEFT(q.QuestionText, 30) + '...' AS QuestionPreview,
    COUNT(qt.QuestionId) AS TranslationCount,
    STRING_AGG(qt.LanguageCode, ', ') WITHIN GROUP (ORDER BY 
        CASE WHEN qt.LanguageCode = 'en' THEN 0 ELSE 1 END, qt.LanguageCode) AS Languages
FROM Questions q
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE q.SubjectId = 7
AND q.IsPublished = 1
GROUP BY q.Id, q.QuestionText
ORDER BY q.Id;

PRINT '========================================';
PRINT 'Mock Test 3 Update Summary:';
PRINT '1. Added 5 lengthy questions for Subject ID 7';
PRINT '2. Questions cover: Constitution, Fundamental Rights, DPSP, Parliamentary System, Judiciary';
PRINT '3. All questions have English and Hindi translations';
PRINT '4. Language ordering fixed - English appears first by default';
PRINT '5. Mock Test 3 total questions and marks updated';
PRINT '========================================';
PRINT 'Execute the individual scripts in the order mentioned above.';
