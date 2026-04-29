-- Fix GetPlanWithDurationOptions procedure to move timestamps to plan level only
-- Remove CreatedAt and UpdatedAt from duration options

USE [RankUp_SubscriptionDB];
GO

-- Drop existing procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPlanWithDurationOptions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPlanWithDurationOptions];
GO

-- Create updated procedure with timestamps only at plan level
CREATE PROCEDURE [dbo].[GetPlanWithDurationOptions]
    @PlanId INT,
    @LanguageCode NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;

    -- Get main plan details with timestamps
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

    -- Get duration options for this plan WITHOUT timestamps
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
        -- Calculate effective price after discount
        ROUND(pdo.Price * (1 - pdo.DiscountPercentage/100), 2) AS EffectivePrice,
        -- Calculate monthly equivalent price
        ROUND(pdo.Price * (1 - pdo.DiscountPercentage/100) / pdo.DurationMonths, 2) AS MonthlyPrice
    FROM PlanDurationOptions pdo
    WHERE pdo.SubscriptionPlanId = @PlanId AND pdo.IsActive = 1
    ORDER BY pdo.SortOrder;
END
GO

PRINT 'GetPlanWithDurationOptions procedure updated successfully - timestamps moved to plan level only';
