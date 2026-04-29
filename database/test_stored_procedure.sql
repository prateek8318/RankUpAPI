-- Test the stored procedure directly
EXEC Payment_GetStatistics

-- Test individual queries
SELECT 'Active Plans Count' as Metric, COUNT(*) as Value FROM SubscriptionPlans WHERE IsActive = 1
UNION ALL
SELECT 'Active Subscribers' as Metric, COUNT(*) as Value FROM UserSubscriptions WHERE Status = 'Active' AND EndDate > GETUTCDATE() AND IsActive = 1
UNION ALL
SELECT 'Expiring Soon' as Metric, COUNT(*) as Value FROM UserSubscriptions WHERE Status = 'Active' AND EndDate BETWEEN GETUTCDATE() AND DATEADD(DAY, 7, GETUTCDATE()) AND IsActive = 1
UNION ALL
SELECT 'Total Users' as Metric, COUNT(DISTINCT UserId) as Value FROM UserSubscriptions WHERE IsActive = 1
UNION ALL
SELECT 'New Users Last 7 Days' as Metric, COUNT(DISTINCT UserId) as Value FROM UserSubscriptions WHERE CreatedAt >= DATEADD(DAY, -7, GETUTCDATE()) AND IsActive = 1;
