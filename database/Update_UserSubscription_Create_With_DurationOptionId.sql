-- Update UserSubscription_Create Stored Procedure to include DurationOptionId
-- This procedure creates a new user subscription with DurationOptionId support

USE [RankUp_SubscriptionDB]
GO

-- Drop the existing procedure
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'UserSubscription_Create')
    DROP PROCEDURE [dbo].[UserSubscription_Create]
GO

-- Create the updated procedure with DurationOptionId
CREATE PROCEDURE [dbo].[UserSubscription_Create]
    @UserId INT,
    @SubscriptionPlanId INT,
    @DurationOptionId INT,
    @RazorpayOrderId NVARCHAR(100) = NULL,
    @RazorpayPaymentId NVARCHAR(100) = NULL,
    @RazorpaySignature NVARCHAR(500) = NULL,
    @OriginalAmount DECIMAL(18,2) = NULL,
    @FinalAmount DECIMAL(18,2) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @Status NVARCHAR(50) = 'Pending',
    @AutoRenewal BIT = 0,
    @RazorpaySubscriptionId NVARCHAR(100) = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Set default values if not provided
    SET @StartDate = ISNULL(@StartDate, GETDATE());
    SET @CreatedAt = ISNULL(@CreatedAt, GETDATE());
    SET @UpdatedAt = ISNULL(@UpdatedAt, GETDATE());
    
    -- Get plan details for validity and tests
    DECLARE @ValidityDays INT;
    DECLARE @TestsTotal INT;
    DECLARE @PlanPrice DECIMAL(18,2);
    
    SELECT 
        @ValidityDays = ValidityDays,
        @TestsTotal = TestPapersCount,
        @PlanPrice = Price
    FROM SubscriptionPlans
    WHERE Id = @SubscriptionPlanId AND IsActive = 1;
    
    IF @ValidityDays IS NULL
    BEGIN
        RAISERROR('Invalid subscription plan', 16, 1);
        RETURN;
    END
    
    -- Set default amounts if not provided
    SET @OriginalAmount = ISNULL(@OriginalAmount, @PlanPrice);
    SET @FinalAmount = ISNULL(@FinalAmount, @PlanPrice);
    
    -- Set end date if not provided
    SET @EndDate = ISNULL(@EndDate, DATEADD(DAY, @ValidityDays, @StartDate));
    
    -- Insert user subscription with DurationOptionId
    INSERT INTO UserSubscriptions (
        UserId, 
        SubscriptionPlanId,
        DurationOptionId,
        RazorpayOrderId, 
        RazorpayPaymentId, 
        RazorpaySignature,
        PurchasedDate,
        ValidTill,
        TestsUsed, 
        TestsTotal,
        AmountPaid,
        Currency,
        DiscountApplied,
        Status, 
        AutoRenewal,
        CreatedAt, 
        UpdatedAt
    )
    VALUES (
        @UserId, 
        @SubscriptionPlanId,
        @DurationOptionId,
        @RazorpayOrderId, 
        @RazorpayPaymentId, 
        @RazorpaySignature,
        @StartDate, -- PurchasedDate same as StartDate
        @EndDate, -- ValidTill same as EndDate
        0, -- TestsUsed
        @TestsTotal,
        @FinalAmount, -- AmountPaid
        'INR', -- Currency
        (@OriginalAmount - @FinalAmount), -- DiscountApplied
        @Status, 
        @AutoRenewal,
        @CreatedAt, 
        @UpdatedAt
    );
    
    -- Return the new subscription ID
    SELECT SCOPE_IDENTITY() AS UserSubscriptionId;
    
    -- Also return the created subscription details
    SELECT 
        Id,
        UserId,
        SubscriptionPlanId,
        DurationOptionId,
        RazorpayOrderId,
        RazorpayPaymentId,
        RazorpaySignature,
        OriginalAmount,
        FinalAmount,
        StartDate,
        EndDate,
        Status,
        AutoRenewal,
        RazorpaySubscriptionId,
        PurchasedDate,
        TestsUsed,
        TestsTotal,
        CreatedAt,
        UpdatedAt
    FROM UserSubscriptions
    WHERE Id = SCOPE_IDENTITY();
END
GO

PRINT 'UserSubscription_Create stored procedure updated successfully with DurationOptionId support';
