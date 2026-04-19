-- Remove auto-generated duration options and keep only user-entered ones
USE [RankUp_SubscriptionDB]
GO

-- Delete all auto-generated duration options that were created by the Fix_Database_Issues.sql script
-- These typically have pattern-based prices and labels
DELETE FROM [dbo].[PlanDurationOptions]
WHERE SubscriptionPlanId IN (
    SELECT Id FROM [dbo].[SubscriptionPlans] WHERE IsActive = 1
)
AND (
    -- Remove auto-generated patterns
    (DurationMonths = 3 AND Price = 599.00 AND DiscountPercentage = 0 AND DisplayLabel = '3 Months') OR
    (DurationMonths = 6 AND Price = 999.00 AND DiscountPercentage = 10 AND DisplayLabel = '6 Months') OR
    (DurationMonths = 12 AND Price = 1499.00 AND DiscountPercentage = 20 AND DisplayLabel = '12 Months')
)
GO

PRINT 'Auto-generated duration options removed successfully'
