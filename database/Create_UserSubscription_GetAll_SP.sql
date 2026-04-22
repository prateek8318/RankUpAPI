-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'UserSubscription_GetAll')
BEGIN
    DROP PROCEDURE [dbo].[UserSubscription_GetAll]
END
GO

-- Create the procedure to get ALL user subscriptions (both active and inactive)
CREATE PROCEDURE [dbo].[UserSubscription_GetAll]
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Get all user subscriptions with plan details
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.DurationOptionId,
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
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt,
        -- Plan details
        sp.Name as PlanName,
        sp.Price as PlanPrice,
        sp.ValidityDays as PlanValidityDays,
        sp.TestPapersCount as PlanTestPapersCount,
        sp.Type as PlanType,
        sp.ExamId as PlanExamId
    FROM 
        UserSubscriptions us
    INNER JOIN 
        SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
    ORDER BY 
        us.CreatedAt DESC
END
GO

PRINT 'UserSubscription_GetAll stored procedure created successfully';
