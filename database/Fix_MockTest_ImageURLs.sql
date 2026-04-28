-- Fix existing localhost URLs in Mock Tests to use relative paths
USE [RankUp_QuestionDB]
GO

-- First, let's see what we have currently
SELECT Id, Name, ImageUrl 
FROM MockTests 
WHERE ImageUrl IS NOT NULL 
ORDER BY Id;

-- Update localhost URLs to relative paths
UPDATE MockTests 
SET ImageUrl = REPLACE(ImageUrl, 'http://localhost:56916/images/mock-tests', '/images/mock-tests')
WHERE ImageUrl LIKE '%localhost:56916/images/mock-tests%';

-- Update any other localhost variations
UPDATE MockTests 
SET ImageUrl = REPLACE(ImageUrl, 'http://localhost:5000/images/mock-tests', '/images/mock-tests')
WHERE ImageUrl LIKE '%localhost:5000/images/mock-tests%';

-- Update any http://localhost with any port for mock tests
UPDATE MockTests 
SET ImageUrl = REPLACE(ImageUrl, 'http://localhost:', '')
WHERE ImageUrl LIKE 'http://localhost:%' AND ImageUrl LIKE '%/images/mock-tests%';

-- Extract only the relative path part
UPDATE MockTests 
SET ImageUrl = SUBSTRING(ImageUrl, CHARINDEX('/images/mock-tests', ImageUrl), LEN(ImageUrl))
WHERE ImageUrl LIKE '%/images/mock-tests%' AND ImageUrl NOT LIKE '/images/mock-tests%';

-- Clean up any double slashes that might result
UPDATE MockTests 
SET ImageUrl = REPLACE(ImageUrl, '//', '/')
WHERE ImageUrl LIKE '//%';

-- Verify the updates
SELECT Id, Name, ImageUrl 
FROM MockTests 
WHERE ImageUrl IS NOT NULL 
ORDER BY Id;

-- Check if any localhost URLs still remain
SELECT COUNT(*) as RemainingLocalhostURLs 
FROM MockTests 
WHERE ImageUrl LIKE '%localhost%';
