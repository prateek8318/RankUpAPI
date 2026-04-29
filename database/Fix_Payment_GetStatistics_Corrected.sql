-- Fixed Payment_GetStatistics without nested aggregates

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetStatistics]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetStatistics]
GO

CREATE PROCEDURE [dbo].[Payment_GetStatistics]
AS
BEGIN
    SET NOCOUNT ON;

    -- Get statistics with all payment statuses included
    SELECT
        COUNT(1) AS TotalPayments,
        -- Total Revenue: Include all successful payments (Status = 2)
        ISNULL(SUM(CASE WHEN p.Status = 2 THEN p.FinalAmount ELSE 0 END), 0) AS TotalRevenue,
        -- Today Revenue: All successful payments today
        ISNULL(SUM(CASE WHEN p.Status = 2 AND CAST(p.CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) THEN p.FinalAmount ELSE 0 END), 0) AS TodayRevenue,
        -- This Month Revenue: All successful payments this month
        ISNULL(SUM(CASE WHEN p.Status = 2 AND YEAR(p.CreatedAt) = YEAR(GETUTCDATE()) AND MONTH(p.CreatedAt) = MONTH(GETUTCDate()) THEN p.FinalAmount ELSE 0 END), 0) AS ThisMonthRevenue,
        -- Payment Status Counts
        ISNULL(SUM(CASE WHEN p.Status = 2 THEN 1 ELSE 0 END), 0) AS SuccessfulPayments,
        ISNULL(SUM(CASE WHEN p.Status = 3 THEN 1 ELSE 0 END), 0) AS FailedPayments,
        ISNULL(SUM(CASE WHEN p.Status = 1 THEN 1 ELSE 0 END), 0) AS PendingPayments,
        -- Average transaction amount (only successful payments)
        ISNULL(AVG(CASE WHEN p.Status = 2 THEN p.FinalAmount END), 0) AS AverageTransactionAmount,
        -- Unique paying users (users with at least one successful payment)
        ISNULL(COUNT(DISTINCT CASE WHEN p.Status = 2 THEN p.UserId END), 0) AS UniquePayingUsers,
        -- Additional stats for better tracking
        ISNULL(SUM(CASE WHEN p.Status = 4 THEN p.FinalAmount ELSE 0 END), 0) AS RefundedAmount,
        ISNULL(SUM(CASE WHEN p.Status = 4 THEN 1 ELSE 0 END), 0) AS RefundedPayments
    FROM Payments p
    INNER JOIN SubscriptionPlans sp ON p.SubscriptionPlanId = sp.Id
    WHERE p.IsActive = 1 AND sp.IsActive = 1;
END
GO

-- Create a simple view for real-time subscription statistics
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_SubscriptionStatistics]'))
    DROP VIEW [dbo].[vw_SubscriptionStatistics]
GO

CREATE VIEW [dbo].[vw_SubscriptionStatistics]
AS
SELECT 
    -- Total unique users who have ever subscribed
    COUNT(DISTINCT us.UserId) AS TotalUsers,
    -- Currently active subscribers
    COUNT(DISTINCT CASE WHEN us.Status = 'Active' AND us.EndDate > GETUTCDATE() THEN us.UserId END) AS ActiveSubscribers,
    -- Free users (users who never had a successful payment)
    COUNT(DISTINCT 
        CASE WHEN us.UserId NOT IN (
            SELECT DISTINCT p.UserId FROM Payments p WHERE p.Status = 2 AND p.IsActive = 1
        ) THEN us.UserId END
    ) AS FreeUsers,
    -- New users in last 7 days
    COUNT(DISTINCT CASE WHEN us.CreatedAt >= DATEADD(DAY, -7, GETUTCDATE()) THEN us.UserId END) AS NewUsersLast7Days,
    -- Blocked users
    COUNT(DISTINCT CASE WHEN us.Status = 'Blocked' THEN us.UserId END) AS BlockedUsers,
    -- Users expiring in next 7 days
    COUNT(DISTINCT CASE WHEN us.EndDate BETWEEN GETUTCDATE() AND DATEADD(DAY, 7, GETUTCDATE()) THEN us.UserId END) AS ExpiringNext7Days,
    -- Revenue statistics
    ISNULL((SELECT SUM(FinalAmount) FROM Payments WHERE Status = 2 AND IsActive = 1), 0) AS TotalRevenue,
    ISNULL((SELECT SUM(FinalAmount) FROM Payments WHERE Status = 2 AND CAST(CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) AND IsActive = 1), 0) AS TodayRevenue,
    -- Today's new subscribers (simplified)
    ISNULL(COUNT(DISTINCT CASE 
        WHEN us.CreatedAt >= CAST(GETUTCDATE() AS DATE) 
        AND us.Status = 'Active'
        THEN us.UserId 
    END), 0) AS NewSubscribersToday,
    GETUTCDATE() AS LastUpdated
FROM UserSubscriptions us
WHERE us.IsActive = 1;
GO

PRINT 'Payment_GetStatistics fixed and vw_SubscriptionStatistics created successfully.';
