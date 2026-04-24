USE [RankUp_SubscriptionDB]
GO

-- Insert sample subscription plans
INSERT INTO SubscriptionPlans (
    Name, Description, Type, Price, Currency, TestPapersCount, Discount, 
    Duration, DurationType, ValidityDays, ExamId, ExamCategory, Features, 
    ImageUrl, IsPopular, IsRecommended, CardColorTheme, SortOrder, CreatedAt, IsActive
)
VALUES 
-- Plan ID 1
('Basic Monthly', 'Basic monthly subscription plan', 1, 99.00, 'INR', 10, 0.00, 
 1, 'Monthly', 30, NULL, 'General', 
 '["Mock Tests", "Basic Analytics", "Email Support"]', 
 '/images/plans/basic.svg', 0, 0, '#6366f1', 1, GETDATE(), 1),

-- Plan ID 2
('Premium Monthly', 'Premium monthly subscription plan', 1, 199.00, 'INR', 25, 10.00, 
 1, 'Monthly', 30, NULL, 'General', 
 '["Mock Tests", "Advanced Analytics", "Priority Support", "Study Materials"]', 
 '/images/plans/basic.svg', 1, 0, '#8b5cf6', 2, GETDATE(), 1),

-- Plan ID 3
('Basic Yearly', 'Basic yearly subscription plan', 2, 999.00, 'INR', 120, 15.00, 
 12, 'Yearly', 365, NULL, 'General', 
 '["Mock Tests", "Basic Analytics", "Email Support", "Offline Access"]', 
 '/images/plans/basic.svg', 0, 1, '#10b981', 3, GETDATE(), 1),

-- Plan ID 4
('Premium Yearly', 'Premium yearly subscription plan', 2, 1999.00, 'INR', 300, 20.00, 
 12, 'Yearly', 365, NULL, 'General', 
 '["Mock Tests", "Advanced Analytics", "Priority Support", "Study Materials", "Offline Access", "Live Sessions"]', 
 '/images/plans/basic.svg', 1, 1, '#f59e0b', 4, GETDATE(), 1),

-- Plan ID 5
('JEE Test Series', 'JEE exam specific test series', 3, 299.00, 'INR', 15, 0.00, 
 3, 'Months', 90, 1, 'Engineering', 
 '["JEE Mock Tests", "Physics Special", "Chemistry Special", "Math Special", "Performance Analysis"]', 
 '/images/plans/basic.svg', 1, 0, '#ef4444', 5, GETDATE(), 1),

-- Plan ID 6
('NEET Test Series', 'NEET exam specific test series', 3, 299.00, 'INR', 15, 0.00, 
 3, 'Months', 90, 2, 'Medical', 
 '["NEET Mock Tests", "Biology Special", "Physics Special", "Chemistry Special", "Performance Analysis"]', 
 '/images/plans/basic.svg', 1, 0, '#22c55e', 6, GETDATE(), 1),

-- Plan ID 7
('UPSC Prelims', 'UPSC Prelims exam specific test series', 3, 499.00, 'INR', 20, 0.00, 
 6, 'Months', 180, 3, 'Civil Services', 
 '["UPSC Mock Tests", "GS Papers", "CSAT", "Current Affairs", "Performance Analysis"]', 
 '/images/plans/basic.svg', 1, 0, '#3b82f6', 7, GETDATE(), 1),

-- Plan ID 8
('Bank PO Test Series', 'Bank PO exam specific test series', 3, 199.00, 'INR', 12, 0.00, 
 2, 'Months', 60, 4, 'Banking', 
 '["Bank PO Mock Tests", "Quantitative Aptitude", "Reasoning", "English", "General Awareness"]', 
 '/images/plans/basic.svg', 0, 0, '#f97316', 8, GETDATE(), 1),

-- Plan ID 9
('SSC CGL Test Series', 'SSC CGL exam specific test series', 3, 249.00, 'INR', 15, 0.00, 
 3, 'Months', 90, 5, 'Government', 
 '["SSC CGL Mock Tests", "Tier 1 Papers", "Tier 2 Papers", "Previous Year Papers"]', 
 '/images/plans/basic.svg', 0, 0, '#a855f7', 9, GETDATE(), 1),

-- Plan ID 10
('CAT Test Series', 'CAT exam specific test series', 3, 399.00, 'INR', 18, 0.00, 
 4, 'Months', 120, 6, 'MBA', 
 '["CAT Mock Tests", "Quantitative Ability", "Verbal Ability", "Logical Reasoning", "DI & LR"]', 
 '/images/plans/basic.svg', 1, 0, '#06b6d4', 10, GETDATE(), 1),

-- Plan ID 11
('GATE Test Series', 'GATE exam specific test series', 3, 349.00, 'INR', 16, 0.00, 
 4, 'Months', 120, 7, 'Engineering', 
 '["GATE Mock Tests", "Technical Papers", "General Aptitude", "Previous Year Papers"]', 
 '/images/plans/basic.svg', 0, 0, '#84cc16', 11, GETDATE(), 1),

-- Plan ID 12
('CLAT Test Series', 'CLAT exam specific test series', 3, 299.00, 'INR', 14, 0.00, 
 3, 'Months', 90, 8, 'Law', 
 '["CLAT Mock Tests", "Legal Aptitude", "General Knowledge", "English", "Logical Reasoning"]', 
 '/images/plans/basic.svg', 0, 0, '#ec4899', 12, GETDATE(), 1),

-- Plan ID 13 (as requested)
('CA Foundation Test Series', 'CA Foundation exam specific test series', 3, 399.00, 'INR', 20, 0.00, 
 4, 'Months', 120, 9, 'Commerce', 
 '["CA Foundation Mock Tests", "Principles of Accounting", "Business Law", "Business Mathematics", "Economics"]', 
 '/images/plans/basic.svg', 1, 1, '#14b8a6', 13, GETDATE(), 1);

GO

PRINT 'Subscription plans seeded successfully!';
PRINT 'Plan ID 13 (CA Foundation Test Series) has been created as requested.';
