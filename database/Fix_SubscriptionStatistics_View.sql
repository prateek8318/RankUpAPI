-- Fix the subscription statistics view syntax error

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
    -- Today's new subscribers
    ISNULL(COUNT(DISTINCT CASE 
        WHEN us.CreatedAt >= CAST(GETUTCDATE() AS DATE) 
        AND EXISTS (SELECT 1 FROM Payments p WHERE p.UserId = us.UserId AND p.Status = 2 AND p.IsActive = 1)
        THEN us.UserId 
    END), 0) AS NewSubscribersToday,
    GETUTCDATE() AS LastUpdated
FROM UserSubscriptions us
WHERE us.IsActive = 1;
GO

PRINT 'vw_SubscriptionStatistics view created successfully.';
