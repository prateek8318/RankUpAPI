USE [RankUp_SubscriptionDB]
GO

-- Drop and recreate Payment_Create with correct column names
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_Create]
GO

CREATE PROCEDURE [dbo].[Payment_Create]
    @UserId INT,
    @SubscriptionPlanId INT,
    @Amount DECIMAL(18,2),
    @DiscountAmount DECIMAL(5,2) = 0,
    @FinalAmount DECIMAL(18,2),
    @PaymentMethod NVARCHAR(50),
    @PaymentProvider NVARCHAR(50) = 'Razorpay',
    @ProviderOrderId NVARCHAR(100) = NULL,
    @TransactionId NVARCHAR(100) = NULL,
    @Status NVARCHAR(20) = 'Pending',
    @PaymentDate DATETIME2 = NULL,
    @FailureReason NVARCHAR(500) = NULL,
    @RefundAmount DECIMAL(18,2) = NULL,
    @RefundDate DATETIME2 = NULL,
    @RefundReason NVARCHAR(500) = NULL,
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
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES (
        @UserId,
        @SubscriptionPlanId,
        @Amount,
        'INR',
        @DiscountAmount,
        @FinalAmount,
        @PaymentMethod,
        @PaymentProvider,
        @TransactionId,
        @ProviderOrderId,
        @Status,
        @PaymentDate,
        @FailureReason,
        @RefundAmount,
        @RefundDate,
        @RefundReason,
        @Metadata,
        GETUTCDATE(),
        GETUTCDATE()
    );
    
    SET @CreatedId = SCOPE_IDENTITY();
    
    SELECT @CreatedId AS PaymentId;
END
GO

-- Create Payment_Update stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_Update]
GO

CREATE PROCEDURE [dbo].[Payment_Update]
    @Id INT,
    @UserId INT = NULL,
    @SubscriptionPlanId INT = NULL,
    @UserSubscriptionId INT = NULL,
    @Amount DECIMAL(18,2) = NULL,
    @Currency NVARCHAR(3) = NULL,
    @DiscountAmount DECIMAL(5,2) = NULL,
    @FinalAmount DECIMAL(18,2) = NULL,
    @PaymentMethod NVARCHAR(50) = NULL,
    @PaymentProvider NVARCHAR(50) = NULL,
    @TransactionId NVARCHAR(100) = NULL,
    @ProviderOrderId NVARCHAR(100) = NULL,
    @Status NVARCHAR(20) = NULL,
    @PaymentDate DATETIME2 = NULL,
    @FailureReason NVARCHAR(500) = NULL,
    @RefundAmount DECIMAL(18,2) = NULL,
    @RefundDate DATETIME2 = NULL,
    @RefundReason NVARCHAR(500) = NULL,
    @Metadata NVARCHAR(MAX) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Payments]
    SET 
        [UserId] = ISNULL(@UserId, [UserId]),
        [SubscriptionPlanId] = ISNULL(@SubscriptionPlanId, [SubscriptionPlanId]),
        [UserSubscriptionId] = ISNULL(@UserSubscriptionId, [UserSubscriptionId]),
        [Amount] = ISNULL(@Amount, [Amount]),
        [Currency] = ISNULL(@Currency, [Currency]),
        [DiscountAmount] = ISNULL(@DiscountAmount, [DiscountAmount]),
        [FinalAmount] = ISNULL(@FinalAmount, [FinalAmount]),
        [PaymentMethod] = ISNULL(@PaymentMethod, [PaymentMethod]),
        [PaymentProvider] = ISNULL(@PaymentProvider, [PaymentProvider]),
        [TransactionId] = ISNULL(@TransactionId, [TransactionId]),
        [ProviderOrderId] = ISNULL(@ProviderOrderId, [ProviderOrderId]),
        [Status] = ISNULL(@Status, [Status]),
        [PaymentDate] = ISNULL(@PaymentDate, [PaymentDate]),
        [FailureReason] = ISNULL(@FailureReason, [FailureReason]),
        [RefundAmount] = ISNULL(@RefundAmount, [RefundAmount]),
        [RefundDate] = ISNULL(@RefundDate, [RefundDate]),
        [RefundReason] = ISNULL(@RefundReason, [RefundReason]),
        [Metadata] = ISNULL(@Metadata, [Metadata]),
        [UpdatedAt] = GETUTCDATE(),
        [IsActive] = ISNULL(@IsActive, [IsActive])
    WHERE [Id] = @Id;
    
    SELECT * FROM [dbo].[Payments] WHERE [Id] = @Id;
END
GO

-- Create Payment_GetById stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_GetById]
GO

CREATE PROCEDURE [dbo].[Payment_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [UserId],
        [SubscriptionPlanId],
        [UserSubscriptionId],
        [Amount],
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt],
        [IsActive]
    FROM [dbo].[Payments]
    WHERE [Id] = @Id AND [IsActive] = 1;
END
GO

-- Create Payment_GetAll stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_GetAll]
GO

CREATE PROCEDURE [dbo].[Payment_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [UserId],
        [SubscriptionPlanId],
        [UserSubscriptionId],
        [Amount],
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt],
        [IsActive]
    FROM [dbo].[Payments]
    WHERE [IsActive] = 1
    ORDER BY [CreatedAt] DESC;
END
GO

-- Create Payment_GetByTransactionId stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByTransactionId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_GetByTransactionId]
GO

CREATE PROCEDURE [dbo].[Payment_GetByTransactionId]
    @TransactionId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [UserId],
        [SubscriptionPlanId],
        [UserSubscriptionId],
        [Amount],
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt],
        [IsActive]
    FROM [dbo].[Payments]
    WHERE [TransactionId] = @TransactionId AND [IsActive] = 1;
END
GO

-- Create Payment_GetByProviderOrderId stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByProviderOrderId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_GetByProviderOrderId]
GO

CREATE PROCEDURE [dbo].[Payment_GetByProviderOrderId]
    @ProviderOrderId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [UserId],
        [SubscriptionPlanId],
        [UserSubscriptionId],
        [Amount],
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt],
        [IsActive]
    FROM [dbo].[Payments]
    WHERE [ProviderOrderId] = @ProviderOrderId AND [IsActive] = 1;
END
GO

-- Create Payment_GetByUserId stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByUserId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_GetByUserId]
GO

CREATE PROCEDURE [dbo].[Payment_GetByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [UserId],
        [SubscriptionPlanId],
        [UserSubscriptionId],
        [Amount],
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt],
        [IsActive]
    FROM [dbo].[Payments]
    WHERE [UserId] = @UserId AND [IsActive] = 1
    ORDER BY [CreatedAt] DESC;
END
GO

-- Create Payment_GetByStatus stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_GetByStatus]
GO

CREATE PROCEDURE [dbo].[Payment_GetByStatus]
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [UserId],
        [SubscriptionPlanId],
        [UserSubscriptionId],
        [Amount],
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt],
        [IsActive]
    FROM [dbo].[Payments]
    WHERE [Status] = @Status AND [IsActive] = 1
    ORDER BY [CreatedAt] DESC;
END
GO

-- Create Payment_GetPaged stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetPaged]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Payment_GetPaged]
GO

CREATE PROCEDURE [dbo].[Payment_GetPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @UserId INT = NULL,
    @Status NVARCHAR(20) = NULL,
    @PaymentProvider NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Skip INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        [Id],
        [UserId],
        [SubscriptionPlanId],
        [UserSubscriptionId],
        [Amount],
        [Currency],
        [DiscountAmount],
        [FinalAmount],
        [PaymentMethod],
        [PaymentProvider],
        [TransactionId],
        [ProviderOrderId],
        [Status],
        [PaymentDate],
        [FailureReason],
        [RefundAmount],
        [RefundDate],
        [RefundReason],
        [Metadata],
        [CreatedAt],
        [UpdatedAt],
        [IsActive]
    FROM [dbo].[Payments]
    WHERE [IsActive] = 1
        AND (@UserId IS NULL OR [UserId] = @UserId)
        AND (@Status IS NULL OR [Status] = @Status)
        AND (@PaymentProvider IS NULL OR [PaymentProvider] = @PaymentProvider)
    ORDER BY [CreatedAt] DESC
    OFFSET @Skip ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Payments]
    WHERE [IsActive] = 1
        AND (@UserId IS NULL OR [UserId] = @UserId)
        AND (@Status IS NULL OR [Status] = @Status)
        AND (@PaymentProvider IS NULL OR [PaymentProvider] = @PaymentProvider);
END
GO

PRINT 'All Payment stored procedures created successfully'
