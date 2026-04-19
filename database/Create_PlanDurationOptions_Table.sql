-- Create PlanDurationOptions table to support multiple duration options for each subscription plan
USE [RankUp_SubscriptionDB]
GO

-- Drop table if exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlanDurationOptions]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[PlanDurationOptions]
END
GO

-- Create PlanDurationOptions table
CREATE TABLE [dbo].[PlanDurationOptions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SubscriptionPlanId] [int] NOT NULL,
    [DurationMonths] [int] NOT NULL, -- 3, 6, 12 months
    [Price] [decimal](18,2) NOT NULL, -- Price for this specific duration
    [DiscountPercentage] [decimal](5,2) NOT NULL DEFAULT 0, -- Discount for longer durations
    [DisplayLabel] [nvarchar](50) NOT NULL, -- "3 Months", "6 Months", "12 Months"
    [IsPopular] [bit] NOT NULL DEFAULT 0, -- Mark as popular option (like 6 months)
    [SortOrder] [int] NOT NULL DEFAULT 0, -- Order for display
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_PlanDurationOptions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PlanDurationOptions_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans] ([Id]) ON DELETE CASCADE
)
GO

-- Create indexes for performance
CREATE INDEX [IX_PlanDurationOptions_SubscriptionPlanId] ON [dbo].[PlanDurationOptions] ([SubscriptionPlanId])
GO
CREATE INDEX [IX_PlanDurationOptions_IsActive] ON [dbo].[PlanDurationOptions] ([IsActive])
GO
CREATE INDEX [IX_PlanDurationOptions_DurationMonths] ON [dbo].[PlanDurationOptions] ([DurationMonths])
GO

-- Insert sample duration options for existing plans
-- This will be populated when admin creates plans
-- Example data for a "Bihar Beltron" plan:
INSERT INTO [dbo].[PlanDurationOptions] (
    SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, DisplayLabel, IsPopular, SortOrder
)
VALUES 
-- Assuming plan ID 1 exists for Bihar Beltron
(1, 3, 1299.00, 0, '3 Months', 0, 1),
(1, 6, 2299.00, 12, '6 Months', 1, 2), -- Popular option with 12% discount
(1, 12, 3999.00, 23, '12 Months', 0, 3) -- Best value with 23% discount
GO

-- Create stored procedure to get plan with duration options
CREATE PROCEDURE [dbo].[GetPlanWithDurationOptions]
    @PlanId INT,
    @LanguageCode NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get main plan details
    SELECT 
        sp.Id,
        sp.Name,
        sp.Description,
        sp.Type,
        sp.TestPapersCount,
        sp.ExamId,
        sp.ExamCategory,
        sp.Features,
        sp.ImageUrl,
        sp.CardColorTheme,
        sp.SortOrder,
        sp.IsActive,
        -- Localized content
        CASE 
            WHEN spt.LanguageCode = @LanguageCode THEN spt.Name
            ELSE sp.Name
        END AS LocalizedName,
        CASE 
            WHEN spt.LanguageCode = @LanguageCode THEN spt.Description
            ELSE sp.Description
        END AS LocalizedDescription,
        CASE 
            WHEN spt.LanguageCode = @LanguageCode THEN spt.Features
            ELSE sp.Features
        END AS LocalizedFeatures
    FROM SubscriptionPlans sp
    LEFT JOIN SubscriptionPlanTranslations spt ON sp.Id = spt.SubscriptionPlanId AND spt.LanguageCode = @LanguageCode
    WHERE sp.Id = @PlanId AND sp.IsActive = 1;
    
    -- Get duration options for this plan
    SELECT 
        pdo.Id,
        pdo.SubscriptionPlanId,
        pdo.DurationMonths,
        pdo.Price,
        pdo.DiscountPercentage,
        pdo.DisplayLabel,
        pdo.IsPopular,
        pdo.SortOrder,
        -- Calculate effective price after discount
        ROUND(pdo.Price * (1 - pdo.DiscountPercentage/100), 2) AS EffectivePrice,
        -- Calculate monthly equivalent price
        ROUND(pdo.Price * (1 - pdo.DiscountPercentage/100) / pdo.DurationMonths, 2) AS MonthlyPrice
    FROM PlanDurationOptions pdo
    WHERE pdo.SubscriptionPlanId = @PlanId AND pdo.IsActive = 1
    ORDER BY pdo.SortOrder;
END
GO

-- Create stored procedure to get all active plans with their duration options
CREATE PROCEDURE [dbo].[GetActivePlansWithDurationOptions]
    @LanguageCode NVARCHAR(10) = 'en',
    @ExamId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all active plans
    SELECT 
        sp.Id,
        sp.Name,
        sp.Description,
        sp.Type,
        sp.TestPapersCount,
        sp.ExamId,
        sp.ExamCategory,
        sp.Features,
        sp.ImageUrl,
        sp.CardColorTheme,
        sp.SortOrder,
        sp.IsActive,
        -- Localized content
        CASE 
            WHEN spt.LanguageCode = @LanguageCode THEN spt.Name
            ELSE sp.Name
        END AS LocalizedName,
        CASE 
            WHEN spt.LanguageCode = @LanguageCode THEN spt.Description
            ELSE sp.Description
        END AS LocalizedDescription,
        CASE 
            WHEN spt.LanguageCode = @LanguageCode THEN spt.Features
            ELSE sp.Features
        END AS LocalizedFeatures
    FROM SubscriptionPlans sp
    LEFT JOIN SubscriptionPlanTranslations spt ON sp.Id = spt.SubscriptionPlanId AND spt.LanguageCode = @LanguageCode
    WHERE sp.IsActive = 1 
    AND (@ExamId IS NULL OR sp.ExamId = @ExamId OR sp.ExamId IS NULL)
    ORDER BY sp.SortOrder, sp.Id;
END
GO

-- Create stored procedure to add duration options to a plan
CREATE PROCEDURE [dbo].[AddPlanDurationOptions]
    @SubscriptionPlanId INT,
    @DurationOptions NVARCHAR(MAX) -- JSON array of duration options
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @JsonData NVARCHAR(MAX) = @DurationOptions;
    
    -- Parse and insert duration options
    INSERT INTO PlanDurationOptions (
        SubscriptionPlanId, DurationMonths, Price, DiscountPercentage, 
        DisplayLabel, IsPopular, SortOrder
    )
    SELECT 
        @SubscriptionPlanId,
        JSON_VALUE(value, '$.durationMonths'),
        JSON_VALUE(value, '$.price'),
        ISNULL(JSON_VALUE(value, '$.discountPercentage'), 0),
        JSON_VALUE(value, '$.displayLabel'),
        ISNULL(JSON_VALUE(value, '$.isPopular'), 0),
        ISNULL(JSON_VALUE(value, '$.sortOrder'), 0)
    FROM OPENJSON(@JsonData)
    WHERE JSON_VALUE(value, '$.durationMonths') IS NOT NULL;
    
    SELECT SCOPE_IDENTITY() AS LastInsertedId;
END
GO

PRINT 'PlanDurationOptions table and stored procedures created successfully'
