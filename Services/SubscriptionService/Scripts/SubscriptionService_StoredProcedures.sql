-- SubscriptionService Stored Procedures
-- This file contains all stored procedures needed for the SubscriptionService

PRINT 'Creating SubscriptionService procedures...';

-- SubscriptionPlan_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetAll]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Type,
        Price,
        Duration,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE IsActive = 1
    ORDER BY Price;
END
GO

-- SubscriptionPlan_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetById]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Type,
        Price,
        Duration,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE Id = @Id;
END
GO

-- SubscriptionPlan_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_Create]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(10,2),
    @Duration INT,
    @Type INT,
    @ExamCategory NVARCHAR(50),
    @ExamId INT,
    @Features NVARCHAR(MAX),
    @IsActive BIT,
    @SortOrder INT,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO SubscriptionPlans (
        Name, Description, Price, Duration, Type, ExamCategory,
        ExamId, Features, IsActive, SortOrder, CreatedAt, UpdatedAt
    )
    VALUES (
        @Name, @Description, @Price, @Duration, @Type, @ExamCategory,
        @ExamId, @Features, @IsActive, @SortOrder, @CreatedAt, @UpdatedAt
    );
END
GO

-- SubscriptionPlan_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_Update]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(10,2),
    @Duration INT,
    @Type INT,
    @ExamCategory NVARCHAR(50),
    @ExamId INT,
    @Features NVARCHAR(MAX),
    @IsActive BIT,
    @SortOrder INT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE SubscriptionPlans
    SET 
        Name = @Name,
        Description = @Description,
        Price = @Price,
        Duration = @Duration,
        Type = @Type,
        ExamCategory = @ExamCategory,
        ExamId = @ExamId,
        Features = @Features,
        IsActive = @IsActive,
        SortOrder = @SortOrder,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- SubscriptionPlan_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_Delete]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM SubscriptionPlans WHERE Id = @Id;
END
GO

-- SubscriptionPlan_GetByExamCategory
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetByExamCategory]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetByExamCategory]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetByExamCategory]
    @ExamCategory NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Type,
        Price,
        Duration,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE ExamCategory = @ExamCategory AND IsActive = 1
    ORDER BY Price;
END
GO

-- SubscriptionPlan_GetByExamId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetByExamId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetByExamId]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetByExamId]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Type,
        Price,
        Duration,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE ExamId = @ExamId AND IsActive = 1
    ORDER BY Price;
END
GO

-- SubscriptionPlan_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetActive]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetActive]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Type,
        Price,
        Duration,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE IsActive = 1
    ORDER BY SortOrder, Price;
END
GO

-- SubscriptionPlan_GetByPlanType
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetByPlanType]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetByPlanType]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetByPlanType]
    @PlanType INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Type,
        Price,
        Duration,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE Type = @PlanType AND IsActive = 1
    ORDER BY Price;
END
GO

-- SubscriptionPlan_ExistsByName
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_ExistsByName]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_ExistsByName]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_ExistsByName]
    @Name NVARCHAR(100),
    @ExamCategory NVARCHAR(50),
    @PlanType INT,
    @ExcludeId INT = NULL,
    @ExamId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) 
    FROM SubscriptionPlans 
    WHERE Name = @Name 
        AND (@ExamCategory IS NULL OR ExamCategory = @ExamCategory)
        AND (@PlanType IS NULL OR Type = @PlanType)
        AND (@ExcludeId IS NULL OR Id != @ExcludeId)
        AND (@ExamId IS NULL OR ExamId = @ExamId);
END
GO

PRINT 'SubscriptionService stored procedures created successfully.';

-- =============================================
-- User Subscription Stored Procedures
-- =============================================

-- UserSubscription_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetAll]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    ORDER BY us.CreatedAt DESC;
END
GO

-- UserSubscription_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetById]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.Id = @Id;
END
GO

-- UserSubscription_GetByUserId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetByUserId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetByUserId]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.UserId = @UserId
    ORDER BY us.CreatedAt DESC;
END
GO

-- UserSubscription_GetActiveByUserId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetActiveByUserId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetActiveByUserId]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetActiveByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.UserId = @UserId AND us.Status = 'Active' AND us.EndDate >= GETUTCDATE()
    ORDER BY us.EndDate DESC;
END
GO

-- UserSubscription_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_Create]
GO

CREATE PROCEDURE [dbo].[UserSubscription_Create]
    @UserId INT,
    @SubscriptionPlanId INT,
    @RazorpayOrderId NVARCHAR(100),
    @RazorpayPaymentId NVARCHAR(100),
    @RazorpaySignature NVARCHAR(100),
    @OriginalAmount DECIMAL(10,2),
    @FinalAmount DECIMAL(10,2),
    @StartDate DATETIME,
    @EndDate DATETIME,
    @Status NVARCHAR(50),
    @AutoRenew BIT,
    @RazorpaySubscriptionId NVARCHAR(100),
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO UserSubscriptions (
        UserId, SubscriptionPlanId, RazorpayOrderId, RazorpayPaymentId, RazorpaySignature,
        OriginalAmount, FinalAmount, StartDate, EndDate, Status, AutoRenew,
        RazorpaySubscriptionId, CreatedAt, UpdatedAt
    )
    VALUES (
        @UserId, @SubscriptionPlanId, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature,
        @OriginalAmount, @FinalAmount, @StartDate, @EndDate, @Status, @AutoRenew,
        @RazorpaySubscriptionId, @CreatedAt, @UpdatedAt
    );
    
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- UserSubscription_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_Update]
GO

CREATE PROCEDURE [dbo].[UserSubscription_Update]
    @Id INT,
    @UserId INT,
    @SubscriptionPlanId INT,
    @RazorpayOrderId NVARCHAR(100),
    @RazorpayPaymentId NVARCHAR(100),
    @RazorpaySignature NVARCHAR(100),
    @OriginalAmount DECIMAL(10,2),
    @FinalAmount DECIMAL(10,2),
    @StartDate DATETIME,
    @EndDate DATETIME,
    @Status NVARCHAR(50),
    @AutoRenew BIT,
    @RazorpaySubscriptionId NVARCHAR(100),
    @LastRenewalDate DATETIME,
    @CancelledDate DATETIME,
    @CancellationReason NVARCHAR(500),
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE UserSubscriptions
    SET 
        UserId = @UserId,
        SubscriptionPlanId = @SubscriptionPlanId,
        RazorpayOrderId = @RazorpayOrderId,
        RazorpayPaymentId = @RazorpayPaymentId,
        RazorpaySignature = @RazorpaySignature,
        OriginalAmount = @OriginalAmount,
        FinalAmount = @FinalAmount,
        StartDate = @StartDate,
        EndDate = @EndDate,
        Status = @Status,
        AutoRenew = @AutoRenew,
        RazorpaySubscriptionId = @RazorpaySubscriptionId,
        LastRenewalDate = @LastRenewalDate,
        CancelledDate = @CancelledDate,
        CancellationReason = @CancellationReason,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- UserSubscription_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_Delete]
GO

CREATE PROCEDURE [dbo].[UserSubscription_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM UserSubscriptions WHERE Id = @Id;
END
GO

-- UserSubscription_Cancel
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_Cancel]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_Cancel]
GO

CREATE PROCEDURE [dbo].[UserSubscription_Cancel]
    @Id INT,
    @CancelledAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE UserSubscriptions
    SET 
        IsActive = 0,
        CancelledAt = @CancelledAt,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- UserSubscription_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetActive]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetActive]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.Status = 'Active' AND us.EndDate >= GETUTCDATE()
    ORDER BY us.EndDate ASC;
END
GO

-- UserSubscription_GetExpiring
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetExpiring]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetExpiring]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetExpiring]
    @DaysBeforeExpiry INT = 7
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.Status = 'Active' 
        AND us.EndDate >= GETUTCDATE() 
        AND us.EndDate <= DATEADD(DAY, @DaysBeforeExpiry, GETUTCDATE())
    ORDER BY us.EndDate ASC;
END
GO

-- UserSubscription_Renew
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_Renew]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_Renew]
GO

CREATE PROCEDURE [dbo].[UserSubscription_Renew]
    @Id INT,
    @NewEndDate DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE UserSubscriptions
    SET 
        EndDate = @NewEndDate,
        IsActive = 1,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

PRINT 'UserSubscription stored procedures created successfully.';

-- =============================================
-- Additional Missing User Subscription Stored Procedures
-- =============================================

-- UserSubscription_GetByUserIdWithHistory
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetByUserIdWithHistory]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetByUserIdWithHistory]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetByUserIdWithHistory]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.UserId = @UserId
    ORDER BY us.CreatedAt DESC;
END
GO

-- UserSubscription_GetByRazorpayOrderId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetByRazorpayOrderId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetByRazorpayOrderId]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetByRazorpayOrderId]
    @OrderId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.RazorpayOrderId = @OrderId;
END
GO

-- UserSubscription_GetByRazorpayPaymentId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserSubscription_GetByRazorpayPaymentId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[UserSubscription_GetByRazorpayPaymentId]
GO

CREATE PROCEDURE [dbo].[UserSubscription_GetByRazorpayPaymentId]
    @PaymentId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        us.Id,
        us.UserId,
        us.SubscriptionPlanId,
        us.RazorpayOrderId,
        us.RazorpayPaymentId,
        us.RazorpaySignature,
        us.OriginalAmount,
        us.FinalAmount,
        us.StartDate,
        us.EndDate,
        us.Status,
        us.AutoRenew,
        us.RazorpaySubscriptionId,
        us.LastRenewalDate,
        us.CancelledDate,
        us.CancellationReason,
        us.CreatedAt,
        us.UpdatedAt
    FROM UserSubscriptions us
    WHERE us.RazorpayPaymentId = @PaymentId;
END
GO

PRINT 'All UserSubscription stored procedures created successfully.';
