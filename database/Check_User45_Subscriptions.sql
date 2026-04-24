-- Check User ID 45 Current Subscriptions
-- This script shows all subscriptions for user ID 45 before and after cleanup

USE [RankUp_SubscriptionDB]
GO

PRINT '=== User ID 45 Subscription Analysis ==='

-- Show all subscriptions for user 45
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
WHERE us.UserId = 45
ORDER BY us.CreatedAt DESC
GO

-- Show what would be kept after cleanup
PRINT '=== What Will Be Kept After Cleanup ==='
SELECT TOP 1
    us.Id AS SubscriptionToKeep,
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
    sp.Name AS PlanName,
    sp.Description AS PlanDescription,
    sp.Price AS PlanPrice
FROM UserSubscriptions us
INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
WHERE us.UserId = 45 
AND us.Status = 'Active' 
AND us.ValidTill > GETDATE()
ORDER BY us.CreatedAt DESC, us.ValidTill DESC
GO

-- Show what would be removed
PRINT '=== What Will Be Removed ==='
SELECT 
    us.Id AS SubscriptionToRemove,
    us.UserId,
    us.SubscriptionPlanId,
    us.Status,
    us.PurchasedDate,
    us.ValidTill,
    DATEDIFF(DAY, GETDATE(), us.ValidTill) AS DaysLeft,
    sp.Name AS PlanName,
    sp.Price AS PlanPrice,
    CASE 
        WHEN us.Status = 'Expired' OR us.ValidTill <= GETDATE() THEN 'Expired Subscription'
        WHEN us.Status = 'Active' AND us.ValidTill > GETDATE() THEN 'Older Active Subscription'
        ELSE 'Other'
    END AS RemovalReason
FROM UserSubscriptions us
INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
WHERE us.UserId = 45 
AND (
    (us.Status = 'Expired' OR us.ValidTill <= GETDATE())
    OR 
    (us.Status = 'Active' AND us.ValidTill > GETDATE() AND us.Id NOT IN (
        SELECT TOP 1 us2.Id 
        FROM UserSubscriptions us2 
        WHERE us2.UserId = 45 
        AND us2.Status = 'Active' 
        AND us2.ValidTill > GETDATE()
        ORDER BY us2.CreatedAt DESC, us2.ValidTill DESC
    ))
)
ORDER BY us.CreatedAt DESC
GO

PRINT '=== User 45 Analysis Complete ==='
