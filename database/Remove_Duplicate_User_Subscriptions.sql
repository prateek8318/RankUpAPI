-- Remove Duplicate User Subscriptions - Keep Only Current Active Plan
-- This script removes duplicate subscriptions for users and keeps only the most recent active plan

USE [RankUp_SubscriptionDB]
GO

-- First, let's analyze the current state to see what we're working with
PRINT '=== Current Subscription Analysis ==='
SELECT 
    UserId,
    COUNT(*) AS TotalSubscriptions,
    SUM(CASE WHEN Status = 'Active' AND ValidTill > GETDATE() THEN 1 ELSE 0 END) AS ActiveSubscriptions,
    SUM(CASE WHEN Status = 'Expired' OR ValidTill <= GETDATE() THEN 1 ELSE 0 END) AS ExpiredSubscriptions,
    MAX(CreatedAt) AS MostRecentSubscription
FROM UserSubscriptions 
GROUP BY UserId
HAVING COUNT(*) > 1
ORDER BY TotalSubscriptions DESC
GO

-- Create a backup of UserSubscriptions before making changes
PRINT '=== Creating Backup Table ==='
DECLARE @BackupTableName NVARCHAR(100)
SET @BackupTableName = 'UserSubscriptions_Backup_' + CONVERT(VARCHAR, GETDATE(), 112)
EXEC('SELECT * INTO ' + @BackupTableName + ' FROM UserSubscriptions')
GO

-- Script to remove duplicate subscriptions
-- Logic: For each user, keep only the most recent active subscription
-- Remove all expired and older active subscriptions

PRINT '=== Removing Duplicate Subscriptions ==='

-- Remove expired subscriptions first
DELETE FROM UserSubscriptions
WHERE Id IN (
    SELECT Id FROM UserSubscriptions us
    WHERE Status = 'Expired' OR ValidTill <= GETDATE()
    AND EXISTS (
        SELECT 1 FROM UserSubscriptions us2 
        WHERE us2.UserId = us.UserId 
        AND (us2.Status = 'Active' AND us2.ValidTill > GETDATE())
    )
)

PRINT 'Expired subscriptions removed'

-- For users with multiple active subscriptions, keep only the most recent one
DELETE FROM UserSubscriptions
WHERE Id IN (
    SELECT us.Id FROM UserSubscriptions us
    WHERE us.Status = 'Active' AND us.ValidTill > GETDATE()
    AND us.Id NOT IN (
        -- Keep the most recent active subscription for each user
        SELECT TOP 1 us2.Id 
        FROM UserSubscriptions us2 
        WHERE us2.UserId = us.UserId 
        AND us2.Status = 'Active' 
        AND us2.ValidTill > GETDATE()
        ORDER BY us2.CreatedAt DESC, us2.ValidTill DESC
    )
)

PRINT 'Duplicate active subscriptions removed'

-- Update any remaining subscriptions that might be expired but still marked as Active
UPDATE UserSubscriptions
SET Status = 'Expired',
    UpdatedAt = GETDATE()
WHERE Status = 'Active' 
AND ValidTill <= GETDATE()

PRINT 'Updated expired subscriptions status'

-- Final verification
PRINT '=== Final Verification ==='
SELECT 
    UserId,
    COUNT(*) AS TotalSubscriptions,
    SUM(CASE WHEN Status = 'Active' AND ValidTill > GETDATE() THEN 1 ELSE 0 END) AS ActiveSubscriptions,
    SUM(CASE WHEN Status = 'Expired' OR ValidTill <= GETDATE() THEN 1 ELSE 0 END) AS ExpiredSubscriptions,
    MAX(CreatedAt) AS MostRecentSubscription
FROM UserSubscriptions 
GROUP BY UserId
HAVING COUNT(*) > 1
ORDER BY TotalSubscriptions DESC
GO

-- Show remaining subscriptions for verification
PRINT '=== Remaining Subscriptions ==='
SELECT 
    us.Id,
    us.UserId,
    us.SubscriptionPlanId,
    us.Status,
    us.PurchasedDate,
    us.ValidTill,
    DATEDIFF(DAY, GETDATE(), us.ValidTill) AS DaysLeft,
    sp.Name AS PlanName,
    sp.Price AS PlanPrice
FROM UserSubscriptions us
INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
ORDER BY us.UserId, us.CreatedAt DESC
GO

PRINT '=== Duplicate Subscription Removal Complete ==='
PRINT 'Each user now has at most one active subscription'
