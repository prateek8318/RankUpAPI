-- Final fixed Payment_GetStatistics with all requested fields

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetStatistics]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetStatistics]
GO

CREATE PROCEDURE [dbo].[Payment_GetStatistics]
AS
BEGIN
    SET NOCOUNT ON;

    -- Payment statistics
    SELECT 
        COUNT(1) AS TotalPayments,
        ISNULL(SUM(CASE WHEN Status = 2 THEN FinalAmount ELSE 0 END), 0) AS TotalRevenue,
        ISNULL(SUM(CASE WHEN Status = 2 AND CAST(CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) THEN FinalAmount ELSE 0 END), 0) AS TodayRevenue,
        ISNULL(SUM(CASE WHEN Status = 2 AND YEAR(CreatedAt) = YEAR(GETUTCDATE()) AND MONTH(CreatedAt) = MONTH(GETUTCDate()) THEN FinalAmount ELSE 0 END), 0) AS ThisMonthRevenue,
        ISNULL(SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END), 0) AS SuccessfulPayments,
        ISNULL(SUM(CASE WHEN Status = 3 THEN 1 ELSE 0 END), 0) AS FailedPayments,
        ISNULL(SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END), 0) AS PendingPayments,
        ISNULL(AVG(CASE WHEN Status = 2 THEN FinalAmount END), 0) AS AverageTransactionAmount,
        ISNULL(COUNT(DISTINCT CASE WHEN Status = 2 THEN UserId END), 0) AS UniquePayingUsers,
        ISNULL(SUM(CASE WHEN Status = 4 THEN FinalAmount ELSE 0 END), 0) AS RefundedAmount,
        ISNULL(SUM(CASE WHEN Status = 4 THEN 1 ELSE 0 END), 0) AS RefundedPayments
    INTO #PaymentStats
    FROM Payments 
    WHERE IsActive = 1;

    -- Additional statistics
    SELECT 
        (SELECT COUNT(*) FROM SubscriptionPlans WHERE IsActive = 1) AS ActivePlansCount,
        (SELECT COUNT(*) FROM UserSubscriptions WHERE Status = 'Active' AND EndDate > GETUTCDATE() AND IsActive = 1) AS ActiveSubscribers,
        (SELECT COUNT(*) FROM UserSubscriptions WHERE Status = 'Active' AND EndDate BETWEEN GETUTCDATE() AND DATEADD(DAY, 7, GETUTCDATE()) AND IsActive = 1) AS ExpiringSoon,
        (SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE CreatedAt >= DATEADD(DAY, -7, GETUTCDATE()) AND IsActive = 1) AS NewUsersLast7Days,
        (SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE Status = 'Blocked' AND IsActive = 1) AS BlockedUsers,
        (SELECT COUNT(DISTINCT UserId) FROM UserSubscriptions WHERE IsActive = 1) AS TotalUsers,
        (SELECT COUNT(DISTINCT us.UserId) FROM UserSubscriptions us 
         WHERE us.IsActive = 1 
         AND us.UserId NOT IN (SELECT DISTINCT p.UserId FROM Payments p WHERE p.Status = 2 AND p.IsActive = 1)
        ) AS FreeUsers,
        ISNULL((
            SELECT COUNT(DISTINCT us.UserId) 
            FROM UserSubscriptions us 
            WHERE CAST(us.CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) 
            AND us.IsActive = 1
            AND EXISTS (SELECT 1 FROM Payments p WHERE p.UserId = us.UserId AND p.Status = 2 AND p.IsActive = 1)
        ), 0) AS NewSubscribersToday,
        GETUTCDATE() AS LastUpdated
    INTO #UserStats;

    -- Combine all statistics
    SELECT 
        ps.*,
        us.*
    FROM #PaymentStats ps
    CROSS JOIN #UserStats us;

    -- Clean up temp tables
    DROP TABLE #PaymentStats;
    DROP TABLE #UserStats;
END
GO

PRINT 'Final Payment_GetStatistics created successfully with all requested fields.';
