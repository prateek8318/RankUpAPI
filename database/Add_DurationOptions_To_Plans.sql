USE [RankUp_SubscriptionDB]
GO

-- Add duration options to existing subscription plans
-- This will add multiple duration options for each plan to allow users to choose different pricing tiers

-- Clear existing duration options first (if any)
DELETE FROM PlanDurationOptions;
GO

-- Insert duration options for each plan
INSERT INTO PlanDurationOptions (
    SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, 
    DisplayLabel, IsPopular, SortOrder, IsActive, CreatedAt
)
VALUES 
-- Plan ID 1: Basic Monthly - Different duration options
(1, 1, 99.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(1, 3, 269.00, 9.09, '3 Months', 0, 2, 1, GETDATE()),
(1, 6, 499.00, 15.99, '6 Months', 1, 3, 1, GETDATE()),
(1, 12, 899.00, 24.24, '1 Year', 0, 4, 1, GETDATE()),

-- Plan ID 2: Premium Monthly - Different duration options
(2, 1, 199.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(2, 3, 539.00, 9.70, '3 Months', 0, 2, 1, GETDATE()),
(2, 6, 999.00, 16.33, '6 Months', 1, 3, 1, GETDATE()),
(2, 12, 1799.00, 24.62, '1 Year', 0, 4, 1, GETDATE()),

-- Plan ID 3: Basic Yearly - Different duration options
(3, 6, 549.00, 5.46, '6 Months', 0, 1, 1, GETDATE()),
(3, 12, 999.00, 0.00, '1 Year', 1, 2, 1, GETDATE()),
(3, 24, 1799.00, 10.00, '2 Years', 0, 3, 1, GETDATE()),

-- Plan ID 4: Premium Yearly - Different duration options
(4, 6, 1099.00, 8.20, '6 Months', 0, 1, 1, GETDATE()),
(4, 12, 1999.00, 0.00, '1 Year', 1, 2, 1, GETDATE()),
(4, 24, 3499.00, 12.50, '2 Years', 0, 3, 1, GETDATE()),

-- Plan ID 5: JEE Test Series - Different duration options
(5, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(5, 3, 799.00, 10.70, '3 Months', 1, 2, 1, GETDATE()),
(5, 6, 1399.00, 22.07, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 6: NEET Test Series - Different duration options
(6, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(6, 3, 799.00, 10.70, '3 Months', 1, 2, 1, GETDATE()),
(6, 6, 1399.00, 22.07, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 7: UPSC Prelims - Different duration options
(7, 3, 499.00, 0.00, '3 Months', 0, 1, 1, GETDATE()),
(7, 6, 899.00, 10.00, '6 Months', 1, 2, 1, GETDATE()),
(7, 12, 1599.00, 20.00, '1 Year', 0, 3, 1, GETDATE()),

-- Plan ID 8: Bank PO Test Series - Different duration options
(8, 1, 199.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(8, 2, 349.00, 12.31, '2 Months', 1, 2, 1, GETDATE()),
(8, 3, 499.00, 16.42, '3 Months', 0, 3, 1, GETDATE()),

-- Plan ID 9: SSC CGL Test Series - Different duration options
(9, 1, 249.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(9, 3, 649.00, 13.06, '3 Months', 1, 2, 1, GETDATE()),
(9, 6, 1099.00, 26.47, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 10: CAT Test Series - Different duration options
(10, 2, 399.00, 0.00, '2 Months', 0, 1, 1, GETDATE()),
(10, 4, 699.00, 12.53, '4 Months', 1, 2, 1, GETDATE()),
(10, 6, 999.00, 20.88, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 11: GATE Test Series - Different duration options
(11, 2, 349.00, 0.00, '2 Months', 0, 1, 1, GETDATE()),
(11, 4, 599.00, 14.33, '4 Months', 1, 2, 1, GETDATE()),
(11, 6, 849.00, 19.48, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 12: CLAT Test Series - Different duration options
(12, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(12, 3, 749.00, 16.39, '3 Months', 1, 2, 1, GETDATE()),
(12, 6, 1299.00, 27.59, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 13: CA Foundation Test Series - Different duration options (as requested)
(13, 2, 399.00, 0.00, '2 Months', 0, 1, 1, GETDATE()),
(13, 4, 699.00, 12.53, '4 Months', 1, 2, 1, GETDATE()),
(13, 6, 999.00, 20.88, '6 Months', 0, 3, 1, GETDATE()),
(13, 8, 1199.00, 24.91, '8 Months', 0, 4, 1, GETDATE());

GO

PRINT 'Duration options added successfully to all subscription plans!';
PRINT 'Plan ID 13 (CA Foundation) now has 4 duration options available.';
PRINT 'Total duration options created: 52';

-- Verify the duration options were added
SELECT 
    p.Id as PlanId,
    p.Name as PlanName,
    pdo.DurationMonths,
    pdo.Price,
    pdo.DisplayLabel,
    pdo.IsPopular
FROM SubscriptionPlans p
INNER JOIN PlanDurationOptions pdo ON p.Id = pdo.SubscriptionPlanId
WHERE p.IsActive = 1 AND pdo.IsActive = 1
ORDER BY p.Id, pdo.SortOrder;
