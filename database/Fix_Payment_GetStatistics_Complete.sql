-- Complete fix for Payment_GetStatistics to return all fields properly

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetStatistics]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetStatistics]
GO

CREATE PROCEDURE [dbo].[Payment_GetStatistics]
AS
BEGIN
    SET NOCOUNT ON;

    -- Get all statistics in a single query
    SELECT 
        -- Payment Statistics
        ISNULL(COUNT(1), 0) AS TotalPayments,
        ISNULL(SUM(CASE WHEN p.Status = 2 THEN p.FinalAmount ELSE 0 END), 0) AS TotalRevenue,
        ISNULL(SUM(CASE WHEN p.Status = 2 AND CAST(p.CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) THEN p.FinalAmount ELSE 0 END), 0) AS TodayRevenue,
        ISNULL(SUM(CASE WHEN p.Status = 2 AND YEAR(p.CreatedAt) = YEAR(GETUTCDATE()) AND MONTH(p.CreatedAt) = MONTH(GETUTCDate()) THEN p.FinalAmount ELSE 0 END), 0) AS ThisMonthRevenue,
        ISNULL(SUM(CASE WHEN p.Status = 2 THEN 1 ELSE 0 END), 0) AS SuccessfulPayments,
        ISNULL(SUM(CASE WHEN p.Status = 3 THEN 1 ELSE 0 END), 0) AS FailedPayments,
        ISNULL(SUM(CASE WHEN p.Status = 1 THEN 1 ELSE 0 END), 0) AS PendingPayments,
        ISNULL(AVG(CASE WHEN p.Status = 2 THEN p.FinalAmount END), 0) AS AverageTransactionAmount,
        ISNULL(COUNT(DISTINCT CASE WHEN p.Status = 2 THEN p.UserId END), 0) AS UniquePayingUsers,
        ISNULL(SUM(CASE WHEN p.Status = 4 THEN p.FinalAmount ELSE 0 END), 0) AS RefundedAmount,
        ISNULL(SUM(CASE WHEN p.Status = 4 THEN 1 ELSE 0 END), 0) AS RefundedPayments,
        
        -- Plan Statistics
        ISNULL((SELECT COUNT(*) FROM SubscriptionPlans WHERE IsActive = 1), 0) AS ActivePlansCount,
        
        -- User Subscription Statistics
        ISNULL((SELECT COUNT(*) FROM UserSubscriptions WHERE Status = 'Active' AND EndDate > GETUTCDATE() AND IsActive = 1), 0) AS ActiveSubscribers,
        ISNULL((SELECT COUNT(*) FROM UserSubscriptions WHERE Status = 'Active' AND EndDate BETWEEN GETUTCDATE() AND DATEADD(DAY, 7, GETUTCDATE()) AND IsActive = 1), 0) AS ExpiringSoon,
        ISNULL((SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE CreatedAt >= DATEADD(DAY, -7, GETUTCDATE()) AND IsActive = 1), 0) AS NewUsersLast7Days,
        ISNULL((SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE Status = 'Blocked' AND IsActive = 1), 0) AS BlockedUsers,
        ISNULL((SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE IsActive = 1), 0) AS TotalUsers,
        ISNULL((
            SELECT COUNT(DISTINCT us.UserId) 
            FROM UserSubscriptions us 
            WHERE us.IsActive = 1 
            AND us.UserId NOT IN (SELECT DISTINCT p.UserId FROM Payments p WHERE p.Status = 2 AND p.IsActive = 1)
        ), 0) AS FreeUsers,
        ISNULL((
            SELECT COUNT(DISTINCT us.UserId) 
            FROM UserSubscriptions us 
            WHERE CAST(us.CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) 
            AND us.IsActive = 1
            AND EXISTS (SELECT 1 FROM Payments p WHERE p.UserId = us.UserId AND p.Status = 2 AND p.IsActive = 1)
        ), 0) AS NewSubscribersToday,
        
        GETUTCDATE() AS LastUpdated
    FROM Payments p
    WHERE p.IsActive = 1;
END
GO

PRINT 'Complete Payment_GetStatistics created successfully with all fields.';
