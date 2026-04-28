-- Fix existing localhost URLs in Subscription Plans to use relative paths
USE [RankUp_SubscriptionDB]
GO

-- First, let's see what we have currently
SELECT Id, Name, ImageUrl 
FROM SubscriptionPlans 
WHERE ImageUrl LIKE '%localhost%' OR ImageUrl LIKE 'http://%'
ORDER BY Id;

-- Update localhost URLs to relative paths
UPDATE SubscriptionPlans 
SET ImageUrl = REPLACE(ImageUrl, 'http://localhost:56925/uploads', '/uploads')
WHERE ImageUrl LIKE '%localhost:56925/uploads%';

-- Update any other localhost variations
UPDATE SubscriptionPlans 
SET ImageUrl = REPLACE(ImageUrl, 'http://localhost:8000/uploads', '/uploads')
WHERE ImageUrl LIKE '%localhost:8000/uploads%';

-- Update any remaining localhost URLs to relative paths
UPDATE SubscriptionPlans 
SET ImageUrl = REPLACE(ImageUrl, 'http://localhost:', '')
WHERE ImageUrl LIKE 'http://localhost:%' AND ImageUrl NOT LIKE '/uploads/%';

-- Remove port numbers and keep only the /uploads part
UPDATE SubscriptionPlans 
SET ImageUrl = SUBSTRING(ImageUrl, CHARINDEX('/uploads', ImageUrl), LEN(ImageUrl))
WHERE ImageUrl LIKE '%/uploads%' AND ImageUrl NOT LIKE '/uploads/%';

-- Clean up any double slashes that might result
UPDATE SubscriptionPlans 
SET ImageUrl = REPLACE(ImageUrl, '//', '/')
WHERE ImageUrl LIKE '//%';

-- Verify the updates
SELECT Id, Name, ImageUrl 
FROM SubscriptionPlans 
ORDER BY Id;

-- Check if any localhost URLs still remain
SELECT COUNT(*) as RemainingLocalhostURLs 
FROM SubscriptionPlans 
WHERE ImageUrl LIKE '%localhost%';
