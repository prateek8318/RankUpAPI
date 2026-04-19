-- Create UserSubscription_GetActiveByUserId Stored Procedure
-- This procedure gets the active subscription for a specific user with plan details

USE [RankUp_SubscriptionDB]
GO

-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'UserSubscription_GetActiveByUserId')
BEGIN
    DROP PROCEDURE [dbo].[UserSubscription_GetActiveByUserId]
END
GO

-- Create the procedure
CREATE PROCEDURE [dbo].[UserSubscription_GetActiveByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.PaymentId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.PurchasedDate,
        us.ValidTill,
        us.TestsUsed,
        us.TestsTotal,
        us.AmountPaid,
        us.Currency,
        us.DiscountApplied,
        us.Status,
        us.AutoRenewal,
        us.RenewalDate,
        us.CreatedAt,
        us.UpdatedAt,
        us.IsActive,
        
        -- Calculate days left
        DATEDIFF(DAY, GETDATE(), us.ValidTill) AS DaysLeft,
        
        -- Calculate current status based on dates
        CASE 
            WHEN us.Status = 'Active' AND us.ValidTill > GETDATE() THEN 'Active'
            WHEN us.Status = 'Active' AND us.ValidTill <= GETDATE() THEN 'Expired'
            ELSE us.Status
        END AS CurrentStatus,
        
        -- Subscription Plan details
        sp.Id AS SubscriptionPlan_Id,
        sp.Name AS SubscriptionPlan_Name,
        sp.Description AS SubscriptionPlan_Description,
        sp.Type AS SubscriptionPlan_Type,
        sp.Price AS SubscriptionPlan_Price,
        sp.Currency AS SubscriptionPlan_Currency,
        sp.TestPapersCount AS SubscriptionPlan_TestPapersCount,
        sp.Discount AS SubscriptionPlan_Discount,
        sp.Duration AS SubscriptionPlan_Duration,
        sp.DurationType AS SubscriptionPlan_DurationType,
        sp.ValidityDays AS SubscriptionPlan_ValidityDays,
        sp.ExamId AS SubscriptionPlan_ExamId,
        sp.ExamCategory AS SubscriptionPlan_ExamCategory,
        sp.Features AS SubscriptionPlan_Features,
        sp.ImageUrl AS SubscriptionPlan_ImageUrl,
        sp.IsPopular AS SubscriptionPlan_IsPopular,
        sp.IsRecommended AS SubscriptionPlan_IsRecommended,
        sp.CardColorTheme AS SubscriptionPlan_CardColorTheme,
        sp.SortOrder AS SubscriptionPlan_SortOrder,
        sp.CreatedAt AS SubscriptionPlan_CreatedAt,
        sp.UpdatedAt AS SubscriptionPlan_UpdatedAt,
        sp.IsActive AS SubscriptionPlan_IsActive
        
    FROM UserSubscriptions us
    INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
    WHERE us.UserId = @UserId 
    AND us.IsActive = 1 
    AND us.Status = 'Active'
    AND us.ValidTill > GETDATE() -- Only return currently active subscriptions
    
    ORDER BY us.CreatedAt DESC
END
GO

-- Grant execute permissions
GRANT EXECUTE ON [dbo].[UserSubscription_GetActiveByUserId] TO [dbo]
GO

PRINT 'UserSubscription_GetActiveByUserId stored procedure created successfully'
