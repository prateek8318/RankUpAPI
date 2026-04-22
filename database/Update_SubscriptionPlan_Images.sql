-- Update Subscription Plans with actual working images
USE [RankUp_SubscriptionDB]
GO

-- Update plans with real image URLs that will actually display
UPDATE SubscriptionPlans 
SET ImageUrl = 'https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=400&h=300&fit=crop&crop=center'
WHERE ImageUrl IS NULL OR ImageUrl = 'https://example.com/engineering-plan.jpg' OR ImageUrl LIKE '/images/subscription-plans/%';

-- Add variety with different images for different plans
UPDATE SubscriptionPlans 
SET ImageUrl = 'https://images.unsplash.com/photo-1456513080510-7bf3a84b82f8?w=400&h=300&fit=crop&crop=center'
WHERE Id = 2 AND Name LIKE '%Engineering%';

UPDATE SubscriptionPlans 
SET ImageUrl = 'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=400&h=300&fit=crop&crop=center'
WHERE Name LIKE '%Basic%';

UPDATE SubscriptionPlans 
SET ImageUrl = 'https://images.unsplash.com/photo-1498243691581-b145c3f54a5a?w=400&h=300&fit=crop&crop=center'
WHERE Name LIKE '%Pro%';

UPDATE SubscriptionPlans 
SET ImageUrl = 'https://images.unsplash.com/photo-1521791136064-7986c2920216?w=400&h=300&fit=crop&crop=center'
WHERE Name LIKE '%Premium%';

-- Verify the updates
SELECT Id, Name, ImageUrl 
FROM SubscriptionPlans 
ORDER BY Id;
