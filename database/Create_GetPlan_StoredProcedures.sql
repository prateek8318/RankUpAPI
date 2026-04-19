-- Create stored procedures for getting plans with duration options
USE [RankUp_SubscriptionDB]
GO

-- Drop existing procedures if they exist
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPlanWithDurationOptions]') AND type in (N'P', 'PC'))
BEGIN
    DROP PROCEDURE [dbo].[GetPlanWithDurationOptions]
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetActivePlansWithDurationOptions]') AND type in (N'P', 'PC'))
BEGIN
    DROP PROCEDURE [dbo].[GetActivePlansWithDurationOptions]
END
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
        sp.Price,
        sp.Currency,
        sp.TestPapersCount,
        sp.Discount,
        sp.Duration,
        sp.DurationType,
        sp.ValidityDays,
        sp.ExamId,
        sp.ExamCategory,
        sp.Features,
        sp.ImageUrl,
        sp.IsPopular,
        sp.IsRecommended,
        sp.CardColorTheme,
        sp.SortOrder,
        sp.IsActive,
        sp.CreatedAt,
        sp.UpdatedAt,
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
        pdo.IsActive,
        pdo.CreatedAt,
        pdo.UpdatedAt,
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
        sp.Price,
        sp.Currency,
        sp.TestPapersCount,
        sp.Discount,
        sp.Duration,
        sp.DurationType,
        sp.ValidityDays,
        sp.ExamId,
        sp.ExamCategory,
        sp.Features,
        sp.ImageUrl,
        sp.IsPopular,
        sp.IsRecommended,
        sp.CardColorTheme,
        sp.SortOrder,
        sp.IsActive,
        sp.CreatedAt,
        sp.UpdatedAt,
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
    
    -- Get all duration options for active plans
    SELECT 
        pdo.Id,
        pdo.SubscriptionPlanId,
        pdo.DurationMonths,
        pdo.Price,
        pdo.DiscountPercentage,
        pdo.DisplayLabel,
        pdo.IsPopular,
        pdo.SortOrder,
        pdo.IsActive,
        pdo.CreatedAt,
        pdo.UpdatedAt,
        -- Calculate effective price after discount
        ROUND(pdo.Price * (1 - pdo.DiscountPercentage/100), 2) AS EffectivePrice,
        -- Calculate monthly equivalent price
        ROUND(pdo.Price * (1 - pdo.DiscountPercentage/100) / pdo.DurationMonths, 2) AS MonthlyPrice
    FROM PlanDurationOptions pdo
    WHERE pdo.SubscriptionPlanId IN (
        SELECT Id FROM SubscriptionPlans WHERE IsActive = 1 
        AND (@ExamId IS NULL OR ExamId = @ExamId OR ExamId IS NULL)
    ) AND pdo.IsActive = 1
    ORDER BY pdo.SubscriptionPlanId, pdo.SortOrder;
END
GO

PRINT 'GetPlanWithDurationOptions and GetActivePlansWithDurationOptions stored procedures created successfully'
