-- Enhanced Payment_GetStatistics with Active Plans, Expiring Soon, and New Subscribers

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetStatistics]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetStatistics]
GO

CREATE PROCEDURE [dbo].[Payment_GetStatistics]
AS
BEGIN
    SET NOCOUNT ON;

    -- Get comprehensive statistics
    SELECT
        -- Payment Statistics
        COUNT(1) AS TotalPayments,
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
        (SELECT COUNT(*) FROM SubscriptionPlans WHERE IsActive = 1) AS ActivePlansCount,
        
        -- User Subscription Statistics
        (SELECT COUNT(*) FROM UserSubscriptions WHERE Status = 'Active' AND EndDate > GETUTCDATE() AND IsActive = 1) AS ActiveSubscribers,
        (SELECT COUNT(*) FROM UserSubscriptions WHERE Status = 'Active' AND EndDate BETWEEN GETUTCDATE() AND DATEADD(DAY, 7, GETUTCDATE()) AND IsActive = 1) AS ExpiringSoon,
        
        -- New Subscribers (users who created subscription today)
        ISNULL(COUNT(DISTINCT CASE 
            WHEN CAST(us.CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) 
            AND EXISTS (SELECT 1 FROM Payments p WHERE p.UserId = us.UserId AND p.Status = 2 AND p.IsActive = 1)
            THEN us.UserId 
        END), 0) AS NewSubscribersToday,
        
        -- New Users Last 7 Days
        (SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE CreatedAt >= DATEADD(DAY, -7, GETUTCDATE()) AND IsActive = 1) AS NewUsersLast7Days,
        
        -- Blocked Users
        (SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE Status = 'Blocked' AND IsActive = 1) AS BlockedUsers,
        
        -- Total Users
        (SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE IsActive = 1) AS TotalUsers,
        
        -- Free Users (users who never had successful payment)
        ISNULL(COUNT(DISTINCT 
            CASE WHEN us.UserId NOT IN (
                SELECT DISTINCT p.UserId FROM Payments p WHERE p.Status = 2 AND p.IsActive = 1
            ) THEN us.UserId END
        ), 0) AS FreeUsers,
        
        GETUTCDATE() AS LastUpdated
    FROM Payments p
    INNER JOIN SubscriptionPlans sp ON p.SubscriptionPlanId = sp.Id
    LEFT JOIN UserSubscriptions us ON p.UserId = us.UserId
    WHERE p.IsActive = 1 AND sp.IsActive = 1;
END
GO

PRINT 'Enhanced Payment_GetStatistics created successfully with Active Plans and Expiring Soon statistics.';
