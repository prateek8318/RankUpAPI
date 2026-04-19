USE [RankUp_SubscriptionDB]
GO

-- Drop stored procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_Create]
GO

-- Create Payment_Create stored procedure
CREATE PROCEDURE [dbo].[Payment_Create]
    @UserId INT,
    @SubscriptionPlanId INT,
    @Amount DECIMAL(18,2),
    @DiscountAmount DECIMAL(5,2) = 0,
    @FinalAmount DECIMAL(18,2),
    @PaymentMethod NVARCHAR(50),
    @PaymentProvider NVARCHAR(50) = 'Razorpay',
    @RazorpayOrderId NVARCHAR(100),
    @RazorpayPaymentId NVARCHAR(100) = NULL,
    @RazorpaySignature NVARCHAR(100) = NULL,
    @Status NVARCHAR(20) = 'Pending',
    @PaymentDate DATETIME2 = NULL,
    @FailureReason NVARCHAR(500) = NULL,
    @RefundAmount DECIMAL(18,2) = NULL,
    @RefundDate DATETIME2 = NULL,
    @RefundReason NVARCHAR(500) = NULL,
    @RazorpayRefundId NVARCHAR(100) = NULL,
    @Metadata NVARCHAR(MAX) = NULL,
    @CreatedId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @PaymentDate IS NULL
        SET @PaymentDate = GETUTCDATE();
    
    INSERT INTO [dbo].[Payments] (
        [UserId],
        [SubscriptionPlanId],
        [Amount],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [RazorpayOrderId],
        [RazorpayPaymentId],
        [RazorpaySignature],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [RazorpayRefundId],
        [Metadata],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES (
        @UserId,
        @SubscriptionPlanId,
        @Amount,
        @DiscountAmount,
        @FinalAmount,
        @PaymentMethod,
        @PaymentProvider,
        @RazorpayOrderId,
        @RazorpayPaymentId,
        @RazorpaySignature,
        @Status,
        @PaymentDate,
        @FailureReason,
        @RefundAmount,
        @RefundDate,
        @RefundReason,
        @RazorpayRefundId,
        @Metadata,
        GETUTCDATE(),
        GETUTCDATE()
    );
    
    SET @CreatedId = SCOPE_IDENTITY();
    
    SELECT @CreatedId AS PaymentId;
END
GO

PRINT 'Payment_Create stored procedure created successfully'
