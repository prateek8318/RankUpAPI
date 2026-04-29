-- Check User ID 42 Current Subscriptions
-- This script shows all subscriptions for user ID 42

USE [RankUp_SubscriptionDB]
GO

PRINT '=== User ID 42 Subscription Analysis ==='

-- Show all subscriptions for user 42
SELECT 
    us.Id,
    us.UserId,
    us.SubscriptionPlanId,
    us.Status,
    us.PurchasedDate,
    us.ValidTill,
    us.TestsUsed,
    us.TestsTotal,
    us.AmountPaid,
    us.Currency,
    us.IsActive,
    DATEDIFF(DAY, GETDATE(), us.ValidTill) AS DaysLeft,
    CASE 
        WHEN us.Status = 'Active' AND us.ValidTill > GETDATE() THEN 'Currently Active'
        WHEN us.Status = 'Active' AND us.ValidTill <= GETDATE() THEN 'Expired (Still Marked Active)'
        ELSE us.Status
    END AS CurrentStatus,
    sp.Name AS PlanName,
    sp.Description AS PlanDescription,
    sp.Type AS PlanType,
    sp.Price AS PlanPrice,
    sp.ValidityDays AS PlanValidityDays,
    sp.ExamCategory AS PlanExamCategory
FROM UserSubscriptions us
INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
WHERE us.UserId = 42
ORDER BY us.CreatedAt DESC
GO

-- Check if there are any truly active subscriptions
PRINT '=== Active Subscriptions Check ==='
SELECT 
    us.Id,
    us.UserId,
    us.Status,
    us.ValidTill,
    sp.Name AS PlanName,
    CASE 
        WHEN us.Status = 'Active' AND us.ValidTill > GETDATE() THEN 'TRULY ACTIVE'
        ELSE 'NOT ACTIVE'
    END AS IsActiveCheck
FROM UserSubscriptions us
INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
WHERE us.UserId = 42
AND us.Status = 'Active' 
AND us.ValidTill > GETDATE()
GO

PRINT '=== User 42 Analysis Complete ==='
