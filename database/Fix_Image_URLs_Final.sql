-- Fix image URL formats for all questions

-- First, check current image URLs
SELECT 'Current Image URLs in Database:' as Info;
SELECT Id, QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl, ExplanationImageUrl
FROM Questions 
WHERE (QuestionImageUrl IS NOT NULL AND QuestionImageUrl <> '') 
   OR (OptionAImageUrl IS NOT NULL AND OptionAImageUrl <> '')
   OR (OptionBImageUrl IS NOT NULL AND OptionBImageUrl <> '')
   OR (OptionCImageUrl IS NOT NULL AND OptionCImageUrl <> '')
   OR (OptionDImageUrl IS NOT NULL AND OptionDImageUrl <> '')
   OR (ExplanationImageUrl IS NOT NULL AND ExplanationImageUrl <> '');

-- Fix all S3 URLs to relative paths
UPDATE Questions 
SET QuestionImageUrl = REPLACE(QuestionImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/')
WHERE QuestionImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

UPDATE Questions 
SET OptionAImageUrl = REPLACE(OptionAImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/')
WHERE OptionAImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

UPDATE Questions 
SET OptionBImageUrl = REPLACE(OptionBImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/')
WHERE OptionBImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

UPDATE Questions 
SET OptionCImageUrl = REPLACE(OptionCImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/')
WHERE OptionCImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

UPDATE Questions 
SET OptionDImageUrl = REPLACE(OptionDImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/')
WHERE OptionDImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

UPDATE Questions 
SET ExplanationImageUrl = REPLACE(ExplanationImageUrl, 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\', '/uploads/questions/')
WHERE ExplanationImageUrl LIKE 'https://rankup-api.s3.amazonaws.com/uploads\\questions\\%';

-- Also fix backslashes to forward slashes
UPDATE Questions 
SET QuestionImageUrl = REPLACE(QuestionImageUrl, '\\', '/')
WHERE QuestionImageUrl LIKE '%\\%';

UPDATE Questions 
SET OptionAImageUrl = REPLACE(OptionAImageUrl, '\\', '/')
WHERE OptionAImageUrl LIKE '%\\%';

UPDATE Questions 
SET OptionBImageUrl = REPLACE(OptionBImageUrl, '\\', '/')
WHERE OptionBImageUrl LIKE '%\\%';

UPDATE Questions 
SET OptionCImageUrl = REPLACE(OptionCImageUrl, '\\', '/')
WHERE OptionCImageUrl LIKE '%\\%';

UPDATE Questions 
SET OptionDImageUrl = REPLACE(OptionDImageUrl, '\\', '/')
WHERE OptionDImageUrl LIKE '%\\%';

UPDATE Questions 
SET ExplanationImageUrl = REPLACE(ExplanationImageUrl, '\\', '/')
WHERE ExplanationImageUrl LIKE '%\\%';

-- Show updated URLs
SELECT 'Updated Image URLs:' as Info;
SELECT Id, QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl, ExplanationImageUrl
FROM Questions 
WHERE (QuestionImageUrl IS NOT NULL AND QuestionImageUrl <> '') 
   OR (OptionAImageUrl IS NOT NULL AND OptionAImageUrl <> '')
   OR (OptionBImageUrl IS NOT NULL AND OptionBImageUrl <> '')
   OR (OptionCImageUrl IS NOT NULL AND OptionCImageUrl <> '')
   OR (OptionDImageUrl IS NOT NULL AND OptionDImageUrl <> '')
   OR (ExplanationImageUrl IS NOT NULL AND ExplanationImageUrl <> '');
