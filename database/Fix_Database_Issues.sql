-- Fix database schema issues
USE [RankUp_SubscriptionDB]
GO

-- 1. Add missing columns to Payments table if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = N'ProviderOrderId')
BEGIN
    ALTER TABLE [dbo].[Payments] ADD [ProviderOrderId] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND name = N'TransactionId')
BEGIN
    ALTER TABLE [dbo].[Payments] ADD [TransactionId] [nvarchar](100) NULL
END
GO

-- 2. Create PlanDurationOptions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlanDurationOptions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PlanDurationOptions](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SubscriptionPlanId] [int] NOT NULL,
        [DurationMonths] [int] NOT NULL,
        [Price] [decimal](18,2) NOT NULL,
        [DiscountPercentage] [decimal](5,2) NOT NULL DEFAULT 0,
        [DisplayLabel] [nvarchar](50) NOT NULL,
        [IsPopular] [bit] NOT NULL DEFAULT 0,
        [SortOrder] [int] NOT NULL DEFAULT 0,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime2](7) NULL,
        CONSTRAINT [PK_PlanDurationOptions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_PlanDurationOptions_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans] ([Id]) ON DELETE CASCADE
    )
    
    -- Create indexes
    CREATE INDEX [IX_PlanDurationOptions_SubscriptionPlanId] ON [dbo].[PlanDurationOptions] ([SubscriptionPlanId])
    CREATE INDEX [IX_PlanDurationOptions_IsActive] ON [dbo].[PlanDurationOptions] ([IsActive])
    CREATE INDEX [IX_PlanDurationOptions_DurationMonths] ON [dbo].[PlanDurationOptions] ([DurationMonths])
END
GO

-- 3. Add ImageUrl column to SubscriptionPlans if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlans]') AND name = N'ImageUrl')
BEGIN
    ALTER TABLE [dbo].[SubscriptionPlans] ADD [ImageUrl] [nvarchar](500) NULL
END
GO

-- 4. Add sample duration options for existing plans (only if no duration options exist)
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlanDurationOptions])
BEGIN
    -- Add duration options for existing subscription plans
    -- These are sample prices - adjust as needed
    INSERT INTO [dbo].[PlanDurationOptions] (
        SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, DisplayLabel, IsPopular, SortOrder
    )
    SELECT 
        Id,
        CASE 
            WHEN Id % 3 = 1 THEN 3  -- Some plans get 3 months
            WHEN Id % 3 = 2 THEN 6  -- Some plans get 6 months
            ELSE 12                 -- Others get 12 months
        END,
        CASE 
            WHEN Id % 3 = 1 THEN 599.00   -- 3 months price
            WHEN Id % 3 = 2 THEN 999.00   -- 6 months price
            ELSE 1499.00                  -- 12 months price
        END,
        CASE 
            WHEN Id % 3 = 1 THEN 0        -- No discount for 3 months
            WHEN Id % 3 = 2 THEN 10       -- 10% discount for 6 months
            ELSE 20                       -- 20% discount for 12 months
        END,
        CASE 
            WHEN Id % 3 = 1 THEN '3 Months'
            WHEN Id % 3 = 2 THEN '6 Months'
            ELSE '12 Months'
        END,
        CASE 
            WHEN Id % 3 = 2 THEN 1  -- 6 months is popular
            ELSE 0
        END,
        CASE 
            WHEN Id % 3 = 1 THEN 1
            WHEN Id % 3 = 2 THEN 2
            ELSE 3
        END
    FROM [dbo].[SubscriptionPlans]
    WHERE IsActive = 1
END
GO

-- 5. Update existing plans with sample image URLs if ImageUrl is null
UPDATE [dbo].[SubscriptionPlans]
SET ImageUrl = '/images/subscription-plans/plan-' + CAST(Id AS VARCHAR(10)) + '.jpg'
WHERE ImageUrl IS NULL AND IsActive = 1
GO

PRINT 'Database schema issues fixed successfully'
