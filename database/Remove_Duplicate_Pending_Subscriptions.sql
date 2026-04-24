-- Remove Duplicate Pending Subscriptions - Keep Only Most Recent
-- This script removes duplicate pending subscriptions for users and keeps only the most recent one

USE [RankUp_SubscriptionDB]
GO

PRINT '=== Current Pending Subscription Analysis ==='

-- Show users with multiple pending subscriptions
SELECT 
    UserId,
    COUNT(*) AS PendingSubscriptions,
    MAX(CreatedAt) AS MostRecentPending
FROM UserSubscriptions 
WHERE Status = 'Pending'
GROUP BY UserId
HAVING COUNT(*) > 1
ORDER BY PendingSubscriptions DESC
GO

-- Show all pending subscriptions before cleanup
PRINT '=== All Pending Subscriptions Before Cleanup ==='
SELECT 
    us.Id,
    us.UserId,
    us.SubscriptionPlanId,
    us.Status,
    us.PurchasedDate,
    us.ValidTill,
    us.AmountPaid,
    us.CreatedAt,
    DATEDIFF(DAY, GETDATE(), us.ValidTill) AS DaysLeft,
    sp.Name AS PlanName,
    sp.Price AS PlanPrice,
    ROW_NUMBER() OVER (PARTITION BY us.UserId ORDER BY us.CreatedAt DESC) AS SubscriptionRank
FROM UserSubscriptions us
INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
WHERE us.Status = 'Pending'
ORDER BY us.UserId, us.CreatedAt DESC
GO

-- Remove duplicate pending subscriptions - keep only the most recent one for each user
PRINT '=== Removing Duplicate Pending Subscriptions ==='

DELETE FROM UserSubscriptions
WHERE Id IN (
    SELECT us.Id FROM UserSubscriptions us
    WHERE us.Status = 'Pending'
    AND us.Id NOT IN (
        -- Keep the most recent pending subscription for each user
        SELECT TOP 1 us2.Id 
        FROM UserSubscriptions us2 
        WHERE us2.UserId = us.UserId 
        AND us2.Status = 'Pending'
        ORDER BY us2.CreatedAt DESC, us2.ValidTill DESC
    )
)

PRINT 'Duplicate pending subscriptions removed'

-- Final verification
PRINT '=== Final Verification ==='
SELECT 
    UserId,
    COUNT(*) AS PendingSubscriptions,
    MAX(CreatedAt) AS MostRecentPending
FROM UserSubscriptions 
WHERE Status = 'Pending'
GROUP BY UserId
HAVING COUNT(*) > 1
ORDER BY PendingSubscriptions DESC
GO

-- Show remaining pending subscriptions
PRINT '=== Remaining Pending Subscriptions ==='
SELECT 
    us.Id,
    us.UserId,
    us.SubscriptionPlanId,
    us.Status,
    us.PurchasedDate,
    us.ValidTill,
    us.AmountPaid,
    us.CreatedAt,
    DATEDIFF(DAY, GETDATE(), us.ValidTill) AS DaysLeft,
    sp.Name AS PlanName,
    sp.Price AS PlanPrice
FROM UserSubscriptions us
INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
WHERE us.Status = 'Pending'
ORDER BY us.UserId, us.CreatedAt DESC
GO

PRINT '=== Pending Subscription Cleanup Complete ==='
