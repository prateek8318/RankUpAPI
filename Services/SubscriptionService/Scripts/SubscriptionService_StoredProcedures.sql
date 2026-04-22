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
        Description,
        Type,
        Price,
        Currency,
        TestPapersCount,
        Discount,
        Duration,
        DurationType,
        ValidityDays,
        ExamId,
        ExamCategory,
        Features,
        ImageUrl,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        SortOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    ORDER BY SortOrder, Name;
END
GO

-- SubscriptionPlan_GetAllPaged (admin: can include active/inactive)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetAllPaged]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetAllPaged]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetAllPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @IncludeInactive BIT = 1,
    @ExamId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 20;

    ;WITH FilteredPlans AS
    (
        SELECT
            Id,
            Name,
            Description,
            Type,
            Price,
            Currency,
            Discount,
            Duration,
            DurationType,
            ValidityDays,
            TestPapersCount,
            ExamId,
            ExamCategory,
            Features,
            ImageUrl,
            CardColorTheme,
            IsPopular,
            IsRecommended,
            SortOrder,
            IsActive,
            CreatedAt,
            UpdatedAt
        FROM SubscriptionPlans
        WHERE (@IncludeInactive = 1 OR IsActive = 1)
          AND (@ExamId IS NULL OR ExamId = @ExamId)
    )
    SELECT
        Id,
        Name,
        Description,
        Type,
        Price,
        Currency,
        Discount,
        Duration,
        DurationType,
        ValidityDays,
        TestPapersCount,
        ExamId,
        ExamCategory,
        Features,
        ImageUrl,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        SortOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM FilteredPlans
    ORDER BY SortOrder ASC, CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM SubscriptionPlans
    WHERE (@IncludeInactive = 1 OR IsActive = 1)
      AND (@ExamId IS NULL OR ExamId = @ExamId);
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
        Description,
        Type,
        Price,
        Currency,
        TestPapersCount,
        Discount,
        Duration,
        DurationType,
        ValidityDays,
        ExamId,
        ExamCategory,
        Features,
        ImageUrl,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        SortOrder,
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
    @ValidityDays INT,
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
        Name, Description, Price, Duration, ValidityDays, Type, ExamCategory,
        ExamId, Features, IsActive, SortOrder, CreatedAt, UpdatedAt
    )
    VALUES (
        @Name, @Description, @Price, @Duration, @ValidityDays, @Type, @ExamCategory,
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
    @ValidityDays INT,
    @Type INT,
    @ExamCategory NVARCHAR(50),
    @ExamId INT,
    @Features NVARCHAR(MAX),
    @IsActive BIT,
    @SortOrder INT,
    @IsPopular BIT,
    @IsRecommended BIT,
    @CardColorTheme NVARCHAR(50),
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
        ValidityDays = @ValidityDays,
        Type = @Type,
        ExamCategory = @ExamCategory,
        ExamId = @ExamId,
        Features = @Features,
        IsActive = @IsActive,
        SortOrder = @SortOrder,
        IsPopular = @IsPopular,
        IsRecommended = @IsRecommended,
        CardColorTheme = @CardColorTheme,
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
        Description,
        Type,
        Price,
        Currency,
        TestPapersCount,
        Discount,
        Duration,
        DurationType,
        ValidityDays,
        ExamId,
        ExamCategory,
        Features,
        ImageUrl,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        SortOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE ExamCategory = @ExamCategory AND IsActive = 1
    ORDER BY SortOrder, Name;
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
        Description,
        Type,
        Price,
        Currency,
        TestPapersCount,
        Discount,
        Duration,
        DurationType,
        ValidityDays,
        ExamId,
        ExamCategory,
        Features,
        ImageUrl,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        SortOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE ExamId = @ExamId AND IsActive = 1
    ORDER BY SortOrder, Name;
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
        Description,
        Type,
        Price,
        Currency,
        TestPapersCount,
        Discount,
        Duration,
        DurationType,
        ValidityDays,
        ExamId,
        ExamCategory,
        Features,
        ImageUrl,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        SortOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE IsActive = 1
    ORDER BY SortOrder, Name;
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
        Description,
        Type,
        Price,
        Currency,
        TestPapersCount,
        Discount,
        Duration,
        DurationType,
        ValidityDays,
        ExamId,
        ExamCategory,
        Features,
        ImageUrl,
        CardColorTheme,
        IsPopular,
        IsRecommended,
        SortOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE Type = @PlanType AND IsActive = 1
    ORDER BY SortOrder, Name;
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
    @CancelledDate DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE UserSubscriptions
    SET 
        IsActive = 0,
        CancelledDate = @CancelledDate,
        Status = 'Cancelled',
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

-- =============================================
-- Payment Stored Procedures
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetById]
GO
CREATE PROCEDURE [dbo].[Payment_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id, UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider,
        RazorpayOrderId,
        RazorpayPaymentId,
        RazorpaySignature,
        Status, PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason,
        RazorpayRefundId, Metadata, IsActive, CreatedAt, UpdatedAt
    FROM Payments
    WHERE Id = @Id AND IsActive = 1;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetAll]
GO
CREATE PROCEDURE [dbo].[Payment_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id, UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider,
        RazorpayOrderId,
        RazorpayPaymentId,
        RazorpaySignature,
        Status, PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason,
        RazorpayRefundId, Metadata, IsActive, CreatedAt, UpdatedAt
    FROM Payments
    WHERE IsActive = 1
    ORDER BY CreatedAt DESC;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_Create]
GO
CREATE PROCEDURE [dbo].[Payment_Create]
    @UserId INT,
    @SubscriptionPlanId INT,
    @UserSubscriptionId INT = NULL,
    @Amount DECIMAL(18,2),
    @Currency NVARCHAR(3),
    @DiscountAmount DECIMAL(18,2),
    @FinalAmount DECIMAL(18,2),
    @PaymentMethod INT,
    @PaymentProvider NVARCHAR(50),
    @RazorpayOrderId NVARCHAR(100),
    @RazorpayPaymentId NVARCHAR(100) = NULL,
    @RazorpaySignature NVARCHAR(100) = NULL,
    @Status INT,
    @PaymentDate DATETIME = NULL,
    @FailureReason NVARCHAR(500) = NULL,
    @RefundAmount DECIMAL(18,2) = NULL,
    @RefundDate DATETIME = NULL,
    @RefundReason NVARCHAR(500) = NULL,
    @RazorpayRefundId NVARCHAR(100) = NULL,
    @Metadata NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Payments
    (
        UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider, RazorpayOrderId, RazorpayPaymentId, RazorpaySignature, Status,
        PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason, RazorpayRefundId, Metadata,
        IsActive, CreatedAt
    )
    VALUES
    (
        @UserId, @SubscriptionPlanId, @UserSubscriptionId, @Amount, @Currency, @DiscountAmount, @FinalAmount,
        @PaymentMethod, @PaymentProvider, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature, @Status,
        @PaymentDate, @FailureReason, @RefundAmount, @RefundDate, @RefundReason, @RazorpayRefundId, @Metadata,
        1, GETUTCDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_Update]
GO
CREATE PROCEDURE [dbo].[Payment_Update]
    @Id INT,
    @UserId INT,
    @SubscriptionPlanId INT,
    @UserSubscriptionId INT = NULL,
    @Amount DECIMAL(18,2),
    @Currency NVARCHAR(3),
    @DiscountAmount DECIMAL(18,2),
    @FinalAmount DECIMAL(18,2),
    @PaymentMethod INT,
    @PaymentProvider NVARCHAR(50),
    @RazorpayOrderId NVARCHAR(100),
    @RazorpayPaymentId NVARCHAR(100) = NULL,
    @RazorpaySignature NVARCHAR(100) = NULL,
    @Status INT,
    @PaymentDate DATETIME = NULL,
    @FailureReason NVARCHAR(500) = NULL,
    @RefundAmount DECIMAL(18,2) = NULL,
    @RefundDate DATETIME = NULL,
    @RefundReason NVARCHAR(500) = NULL,
    @RazorpayRefundId NVARCHAR(100) = NULL,
    @Metadata NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Payments
    SET
        UserId = @UserId,
        SubscriptionPlanId = @SubscriptionPlanId,
        UserSubscriptionId = @UserSubscriptionId,
        Amount = @Amount,
        Currency = @Currency,
        DiscountAmount = @DiscountAmount,
        FinalAmount = @FinalAmount,
        PaymentMethod = @PaymentMethod,
        PaymentProvider = @PaymentProvider,
        RazorpayOrderId = @RazorpayOrderId,
        RazorpayPaymentId = @RazorpayPaymentId,
        RazorpaySignature = @RazorpaySignature,
        Status = @Status,
        PaymentDate = @PaymentDate,
        FailureReason = @FailureReason,
        RefundAmount = @RefundAmount,
        RefundDate = @RefundDate,
        RefundReason = @RefundReason,
        RazorpayRefundId = @RazorpayRefundId,
        Metadata = @Metadata,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND IsActive = 1;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_Delete]
GO
CREATE PROCEDURE [dbo].[Payment_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Payments SET IsActive = 0, UpdatedAt = GETUTCDATE() WHERE Id = @Id AND IsActive = 1;
    SELECT @@ROWCOUNT;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByTransactionId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetByTransactionId]
GO
CREATE PROCEDURE [dbo].[Payment_GetByTransactionId]
    @TransactionId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id, UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider,
        RazorpayOrderId,
        RazorpayPaymentId,
        RazorpaySignature,
        Status, PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason,
        RazorpayRefundId, Metadata, IsActive, CreatedAt, UpdatedAt
    FROM Payments
    WHERE RazorpayPaymentId = @TransactionId AND IsActive = 1;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByProviderOrderId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetByProviderOrderId]
GO
CREATE PROCEDURE [dbo].[Payment_GetByProviderOrderId]
    @ProviderOrderId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id, UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider,
        RazorpayOrderId,
        RazorpayPaymentId,
        RazorpaySignature,
        Status, PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason,
        RazorpayRefundId, Metadata, IsActive, CreatedAt, UpdatedAt
    FROM Payments
    WHERE RazorpayOrderId = @ProviderOrderId AND IsActive = 1;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByUserId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetByUserId]
GO
CREATE PROCEDURE [dbo].[Payment_GetByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id, UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider,
        RazorpayOrderId,
        RazorpayPaymentId,
        RazorpaySignature,
        Status, PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason,
        RazorpayRefundId, Metadata, IsActive, CreatedAt, UpdatedAt
    FROM Payments
    WHERE UserId = @UserId AND IsActive = 1
    ORDER BY CreatedAt DESC;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetByStatus]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetByStatus]
GO
CREATE PROCEDURE [dbo].[Payment_GetByStatus]
    @Status INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id, UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider,
        RazorpayOrderId,
        RazorpayPaymentId,
        RazorpaySignature,
        Status, PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason,
        RazorpayRefundId, Metadata, IsActive, CreatedAt, UpdatedAt
    FROM Payments
    WHERE Status = @Status AND IsActive = 1
    ORDER BY CreatedAt DESC;
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetPaged]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetPaged]
GO
CREATE PROCEDURE [dbo].[Payment_GetPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @UserId INT = NULL,
    @SearchTerm NVARCHAR(100) = NULL,
    @Status INT = NULL,
    @PaymentMethod INT = NULL,
    @AmountFrom DECIMAL(18,2) = NULL,
    @AmountTo DECIMAL(18,2) = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL,
    @Reference NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 20;

    ;WITH Filtered AS
    (
        SELECT
            Id, UserId, SubscriptionPlanId, UserSubscriptionId, Amount, Currency, DiscountAmount, FinalAmount,
            PaymentMethod, PaymentProvider,
            RazorpayOrderId,
            RazorpayPaymentId,
            RazorpaySignature,
            Status, PaymentDate, FailureReason, RefundAmount, RefundDate, RefundReason,
            RazorpayRefundId, Metadata, IsActive, CreatedAt, UpdatedAt
        FROM Payments
        WHERE IsActive = 1
          AND (@UserId IS NULL OR UserId = @UserId)
          AND (@Status IS NULL OR Status = @Status)
          AND (@PaymentMethod IS NULL OR PaymentMethod = @PaymentMethod)
          AND (@AmountFrom IS NULL OR FinalAmount >= @AmountFrom)
          AND (@AmountTo IS NULL OR FinalAmount <= @AmountTo)
          AND (@DateFrom IS NULL OR CreatedAt >= @DateFrom)
          AND (@DateTo IS NULL OR CreatedAt <= @DateTo)
          AND (
                @SearchTerm IS NULL
                OR RazorpayOrderId LIKE '%' + @SearchTerm + '%'
                OR RazorpayPaymentId LIKE '%' + @SearchTerm + '%'
              )
          AND (
                @Reference IS NULL
                OR RazorpayOrderId = @Reference
                OR RazorpayPaymentId = @Reference
              )
    )
    SELECT *
    FROM Filtered
    ORDER BY CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM Payments
    WHERE IsActive = 1
      AND (@UserId IS NULL OR UserId = @UserId)
      AND (@Status IS NULL OR Status = @Status)
      AND (@PaymentMethod IS NULL OR PaymentMethod = @PaymentMethod)
      AND (@AmountFrom IS NULL OR FinalAmount >= @AmountFrom)
      AND (@AmountTo IS NULL OR FinalAmount <= @AmountTo)
      AND (@DateFrom IS NULL OR CreatedAt >= @DateFrom)
      AND (@DateTo IS NULL OR CreatedAt <= @DateTo)
      AND (
            @SearchTerm IS NULL
            OR ProviderOrderId LIKE '%' + @SearchTerm + '%'
            OR TransactionId LIKE '%' + @SearchTerm + '%'
          )
      AND (
            @Reference IS NULL
            OR ProviderOrderId = @Reference
            OR TransactionId = @Reference
          );
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payment_GetStatistics]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Payment_GetStatistics]
GO
CREATE PROCEDURE [dbo].[Payment_GetStatistics]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(1) AS TotalPayments,
        ISNULL(SUM(CASE WHEN p.Status = 2 THEN p.FinalAmount ELSE 0 END), 0) AS TotalRevenue,
        ISNULL(SUM(CASE WHEN p.Status = 2 AND CAST(p.CreatedAt AS DATE) = CAST(GETUTCDATE() AS DATE) THEN p.FinalAmount ELSE 0 END), 0) AS TodayRevenue,
        ISNULL(SUM(CASE WHEN p.Status = 2 AND YEAR(p.CreatedAt) = YEAR(GETUTCDATE()) AND MONTH(p.CreatedAt) = MONTH(GETUTCDATE()) THEN p.FinalAmount ELSE 0 END), 0) AS ThisMonthRevenue,
        ISNULL(SUM(CASE WHEN p.Status = 2 THEN 1 ELSE 0 END), 0) AS SuccessfulPayments,
        ISNULL(SUM(CASE WHEN p.Status = 3 THEN 1 ELSE 0 END), 0) AS FailedPayments,
        ISNULL(SUM(CASE WHEN p.Status = 1 THEN 1 ELSE 0 END), 0) AS PendingPayments,
        ISNULL(AVG(CASE WHEN p.Status = 2 THEN p.FinalAmount END), 0) AS AverageTransactionAmount,
        ISNULL(COUNT(DISTINCT CASE WHEN p.Status = 2 THEN p.UserId END), 0) AS UniquePayingUsers
    FROM Payments p
    INNER JOIN SubscriptionPlans sp ON p.SubscriptionPlanId = sp.Id
    WHERE p.IsActive = 1 AND sp.IsActive = 1;
END
GO

PRINT 'Payment stored procedures created successfully.';
