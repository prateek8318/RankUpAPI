-- Fix empty questions and image URL formats

-- 1. First, identify empty questions (empty questionText and options)
SELECT 'Empty Questions Found:' as Info;
SELECT Id, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer
FROM Questions 
WHERE (QuestionText IS NULL OR QuestionText = '' OR QuestionText = ' ')
AND (OptionA IS NULL OR OptionA = '' OR OptionA = ' ')
AND (OptionB IS NULL OR OptionB = '' OR OptionB = ' ')
AND (OptionC IS NULL OR OptionC = '' OR OptionC = ' ')
AND (OptionD IS NULL OR OptionD = '' OR OptionD = ' ');

-- 2. Delete empty questions and their references
BEGIN TRANSACTION;

-- Delete from MockTestQuestions first
DELETE FROM MockTestQuestions 
WHERE QuestionId IN (
    SELECT Id FROM Questions 
    WHERE (QuestionText IS NULL OR QuestionText = '' OR QuestionText = ' ')
    AND (OptionA IS NULL OR OptionA = '' OR OptionA = ' ')
    AND (OptionB IS NULL OR OptionB = '' OR OptionB = ' ')
    AND (OptionC IS NULL OR OptionC = '' OR OptionC = ' ')
    AND (OptionD IS NULL OR OptionD = '' OR OptionD = ' ')
);

-- Delete from QuestionTranslations
DELETE FROM QuestionTranslations 
WHERE QuestionId IN (
    SELECT Id FROM Questions 
    WHERE (QuestionText IS NULL OR QuestionText = '' OR QuestionText = ' ')
    AND (OptionA IS NULL OR OptionA = '' OR OptionA = ' ')
    AND (OptionB IS NULL OR OptionB = '' OR OptionB = ' ')
    AND (OptionC IS NULL OR OptionC = '' OR OptionC = ' ')
    AND (OptionD IS NULL OR OptionD = '' OR OptionD = ' ')
);

-- Delete the empty questions
DELETE FROM Questions 
WHERE (QuestionText IS NULL OR QuestionText = '' OR QuestionText = ' ')
AND (OptionA IS NULL OR OptionA = '' OR OptionA = ' ')
AND (OptionB IS NULL OR OptionB = '' OR OptionB = ' ')
AND (OptionC IS NULL OR OptionC = '' OR OptionC = ' ')
AND (OptionD IS NULL OR OptionD = '' OR OptionD = ' ');

COMMIT;

-- 3. Fix image URL formats (remove S3 prefix and make relative)
SELECT 'Fixing Image URLs:' as Info;

-- Update QuestionImageUrl
UPDATE Questions 
SET QuestionImageUrl = REPLACE(
    REPLACE(QuestionImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/'),
    '\\', '/'
)
WHERE QuestionImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

-- Update OptionAImageUrl
UPDATE Questions 
SET OptionAImageUrl = REPLACE(
    REPLACE(OptionAImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/'),
    '\\', '/'
)
WHERE OptionAImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

-- Update OptionBImageUrl
UPDATE Questions 
SET OptionBImageUrl = REPLACE(
    REPLACE(OptionBImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/'),
    '\\', '/'
)
WHERE OptionBImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

-- Update OptionCImageUrl
UPDATE Questions 
SET OptionCImageUrl = REPLACE(
    REPLACE(OptionCImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/'),
    '\\', '/'
)
WHERE OptionCImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

-- Update OptionDImageUrl
UPDATE Questions 
SET OptionDImageUrl = REPLACE(
    REPLACE(OptionDImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/'),
    '\\', '/'
)
WHERE OptionDImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

-- Update ExplanationImageUrl
UPDATE Questions 
SET ExplanationImageUrl = REPLACE(
    REPLACE(ExplanationImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/'),
    '\\', '/'
)
WHERE ExplanationImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

-- 4. Show results
SELECT 'Results:' as Info;
SELECT 'Empty Questions Deleted:' as Result, COUNT(*) as Count 
FROM Questions 
WHERE (QuestionText IS NULL OR QuestionText = '' OR QuestionText = ' ')
AND (OptionA IS NULL OR OptionA = '' OR OptionA = ' ')
AND (OptionB IS NULL OR OptionB = '' OR OptionB = ' ')
AND (OptionC IS NULL OR OptionC = '' OR OptionC = ' ')
AND (OptionD IS NULL OR OptionD = '' OR OptionD = ' ');

SELECT 'Image URLs Fixed:' as Result, COUNT(*) as Count
FROM Questions 
WHERE QuestionImageUrl LIKE '/uploads/questions/%' 
   OR OptionAImageUrl LIKE '/uploads/questions/%'
   OR OptionBImageUrl LIKE '/uploads/questions/%'
   OR OptionCImageUrl LIKE '/uploads/questions/%'
   OR OptionDImageUrl LIKE '/uploads/questions/%'
   OR ExplanationImageUrl LIKE '/uploads/questions/%';

-- 5. Show sample of fixed URLs
SELECT TOP 5 'Sample Fixed URLs:' as Info, Id, QuestionImageUrl, OptionAImageUrl
FROM Questions 
WHERE QuestionImageUrl LIKE '/uploads/questions/%' 
   OR OptionAImageUrl LIKE '/uploads/questions/%';
