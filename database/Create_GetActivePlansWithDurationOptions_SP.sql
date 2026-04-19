USE [RankUp_SubscriptionDB]
GO

-- Create stored procedure to get active subscription plans with duration options
CREATE PROCEDURE [dbo].[GetActivePlansWithDurationOptions]
    @LanguageCode NVARCHAR(10) = 'en',
    @ExamId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- First result set: Subscription plans
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
        sp.CreatedAt,
        sp.UpdatedAt,
        sp.IsActive,
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
    ORDER BY sp.SortOrder, sp.Name;
    
    -- Second result set: Duration options for these plans
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
        pdo.UpdatedAt
    FROM PlanDurationOptions pdo
    INNER JOIN SubscriptionPlans sp ON pdo.SubscriptionPlanId = sp.Id
    WHERE sp.IsActive = 1 
    AND pdo.IsActive = 1
    AND (@ExamId IS NULL OR sp.ExamId = @ExamId OR sp.ExamId IS NULL)
    ORDER BY pdo.SubscriptionPlanId, pdo.SortOrder;
END
GO
