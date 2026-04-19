USE [RankUp_SubscriptionDB]
GO

-- Add duration options to the existing test2 plan (id: 17)
-- This will fix the empty durationOptions array issue

INSERT INTO PlanDurationOptions (
    SubscriptionPlanId, 
    DurationMonths, 
    Price, 
    DiscountPercentage, 
    DisplayLabel, 
    IsPopular, 
    SortOrder, 
    IsActive, 
    CreatedAt
) VALUES 
-- 1 Month option
(17, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),

-- 3 Months option  
(17, 3, 799.00, 11.00, '3 Months', 0, 2, 1, GETDATE()),

-- 6 Months option (Popular)
(17, 6, 1399.00, 22.00, '6 Months', 1, 3, 1, GETDATE()),

-- 12 Months option
(17, 12, 2399.00, 33.00, '12 Months', 0, 4, 1, GETDATE());

GO

-- Verify the duration options were added
SELECT * FROM PlanDurationOptions WHERE SubscriptionPlanId = 17 ORDER BY SortOrder;
GO
