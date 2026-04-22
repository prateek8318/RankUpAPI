USE [RankUp_SubscriptionDB]
GO

-- Clear existing duration options first
DELETE FROM PlanDurationOptions;
GO

-- Add duration options for existing plans with correct IDs
INSERT INTO PlanDurationOptions (
    SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, 
    DisplayLabel, IsPopular, SortOrder, IsActive, CreatedAt
)
VALUES 
-- Plan ID 2: Engineering Monthly Plan
(2, 1, 199.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(2, 3, 539.00, 9.70, '3 Months', 0, 2, 1, GETDATE()),
(2, 6, 999.00, 16.33, '6 Months', 1, 3, 1, GETDATE()),
(2, 12, 1799.00, 24.62, '1 Year', 0, 4, 1, GETDATE()),

-- Plan ID 13: test
(13, 1, 99.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(13, 3, 269.00, 9.09, '3 Months', 0, 2, 1, GETDATE()),
(13, 6, 499.00, 15.99, '6 Months', 1, 3, 1, GETDATE()),

-- Plan ID 16: Premium Plan
(16, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(16, 3, 799.00, 10.70, '3 Months', 1, 2, 1, GETDATE()),
(16, 6, 1399.00, 22.07, '6 Months', 0, 3, 1, GETDATE()),
(16, 12, 2499.00, 30.27, '1 Year', 0, 4, 1, GETDATE()),

-- Plan ID 17: test2
(17, 1, 149.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(17, 3, 399.00, 10.74, '3 Months', 1, 2, 1, GETDATE()),
(17, 6, 699.00, 21.61, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 18: test3
(18, 1, 199.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(18, 3, 539.00, 9.70, '3 Months', 1, 2, 1, GETDATE()),
(18, 6, 999.00, 16.33, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 19: Basic Monthly
(19, 1, 99.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(19, 3, 269.00, 9.09, '3 Months', 0, 2, 1, GETDATE()),
(19, 6, 499.00, 15.99, '6 Months', 1, 3, 1, GETDATE()),
(19, 12, 899.00, 24.24, '1 Year', 0, 4, 1, GETDATE()),

-- Plan ID 20: Premium Monthly
(20, 1, 199.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(20, 3, 539.00, 9.70, '3 Months', 0, 2, 1, GETDATE()),
(20, 6, 999.00, 16.33, '6 Months', 1, 3, 1, GETDATE()),
(20, 12, 1799.00, 24.62, '1 Year', 0, 4, 1, GETDATE()),

-- Plan ID 21: Basic Yearly
(21, 6, 549.00, 5.46, '6 Months', 0, 1, 1, GETDATE()),
(21, 12, 999.00, 0.00, '1 Year', 1, 2, 1, GETDATE()),
(21, 24, 1799.00, 10.00, '2 Years', 0, 3, 1, GETDATE()),

-- Plan ID 22: Premium Yearly
(22, 6, 1099.00, 8.20, '6 Months', 0, 1, 1, GETDATE()),
(22, 12, 1999.00, 0.00, '1 Year', 1, 2, 1, GETDATE()),
(22, 24, 3499.00, 12.50, '2 Years', 0, 3, 1, GETDATE()),

-- Plan ID 23: JEE Test Series
(23, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(23, 3, 799.00, 10.70, '3 Months', 1, 2, 1, GETDATE()),
(23, 6, 1399.00, 22.07, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 24: NEET Test Series
(24, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(24, 3, 799.00, 10.70, '3 Months', 1, 2, 1, GETDATE()),
(24, 6, 1399.00, 22.07, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 25: UPSC Prelims
(25, 3, 499.00, 0.00, '3 Months', 0, 1, 1, GETDATE()),
(25, 6, 899.00, 10.00, '6 Months', 1, 2, 1, GETDATE()),
(25, 12, 1599.00, 20.00, '1 Year', 0, 3, 1, GETDATE()),

-- Plan ID 26: Bank PO Test Series
(26, 1, 199.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(26, 2, 349.00, 12.31, '2 Months', 1, 2, 1, GETDATE()),
(26, 3, 499.00, 16.42, '3 Months', 0, 3, 1, GETDATE()),

-- Plan ID 27: SSC CGL Test Series
(27, 1, 249.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(27, 3, 649.00, 13.06, '3 Months', 1, 2, 1, GETDATE()),
(27, 6, 1099.00, 26.47, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 28: CAT Test Series
(28, 2, 399.00, 0.00, '2 Months', 0, 1, 1, GETDATE()),
(28, 4, 699.00, 12.53, '4 Months', 1, 2, 1, GETDATE()),
(28, 6, 999.00, 20.88, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 29: GATE Test Series
(29, 2, 349.00, 0.00, '2 Months', 0, 1, 1, GETDATE()),
(29, 4, 599.00, 14.33, '4 Months', 1, 2, 1, GETDATE()),
(29, 6, 849.00, 19.48, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 30: CLAT Test Series
(30, 1, 299.00, 0.00, '1 Month', 0, 1, 1, GETDATE()),
(30, 3, 749.00, 16.39, '3 Months', 1, 2, 1, GETDATE()),
(30, 6, 1299.00, 27.59, '6 Months', 0, 3, 1, GETDATE()),

-- Plan ID 31: CA Foundation Test Series (as requested)
(31, 2, 399.00, 0.00, '2 Months', 0, 1, 1, GETDATE()),
(31, 4, 699.00, 12.53, '4 Months', 1, 2, 1, GETDATE()),
(31, 6, 999.00, 20.88, '6 Months', 0, 3, 1, GETDATE()),
(31, 8, 1199.00, 24.91, '8 Months', 0, 4, 1, GETDATE());

GO

PRINT 'Duration options added successfully to all existing plans!';
PRINT 'Plan ID 31 (CA Foundation) now has 4 duration options available.';

-- Verify duration options were added
SELECT 
    p.Id as PlanId,
    p.Name as PlanName,
    COUNT(pdo.Id) as DurationOptionsCount
FROM SubscriptionPlans p
LEFT JOIN PlanDurationOptions pdo ON p.Id = pdo.SubscriptionPlanId AND pdo.IsActive = 1
WHERE p.IsActive = 1
GROUP BY p.Id, p.Name
ORDER BY p.Id;
