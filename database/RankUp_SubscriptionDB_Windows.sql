USE [master]
GO

-- Drop if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RankUp_SubscriptionDB')
BEGIN
    ALTER DATABASE [RankUp_SubscriptionDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RankUp_SubscriptionDB];
END
GO

-- Create with Windows path
CREATE DATABASE [RankUp_SubscriptionDB]
ON PRIMARY 
( NAME = N'RankUp_SubscriptionDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_SubscriptionDB.mdf', SIZE = 8192KB, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'RankUp_SubscriptionDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_SubscriptionDB_log.ldf', SIZE = 8192KB, FILEGROWTH = 65536KB )
GO

USE [RankUp_SubscriptionDB]
GO

-- Migrations History
CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId] [nvarchar](150) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
)
GO

-- SubscriptionPlans Table
CREATE TABLE [dbo].[SubscriptionPlans](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](200) NOT NULL,
    [Description] [nvarchar](max) NOT NULL,
    [Type] [int] NOT NULL, -- PlanType enum (1=Monthly, 2=Yearly, 3=ExamSpecific)
    [Price] [decimal](18,2) NOT NULL,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'INR',
    [TestPapersCount] [int] NOT NULL DEFAULT 0,
    [Discount] [decimal](5,2) NOT NULL DEFAULT 0,
    [Duration] [int] NOT NULL DEFAULT 1,
    [DurationType] [nvarchar](50) NOT NULL DEFAULT 'Monthly',
    [ValidityDays] [int] NOT NULL,
    [ExamId] [int] NULL, -- FK to Master Service Exams table
    [ExamCategory] [nvarchar](100) NULL,
    [Features] [nvarchar](max) NULL, -- JSON array of features
    [ImageUrl] [nvarchar](500) NULL,
    [IsPopular] [bit] NOT NULL DEFAULT 0,
    [IsRecommended] [bit] NOT NULL DEFAULT 0,
    [CardColorTheme] [nvarchar](50) NULL,
    [SortOrder] [int] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_SubscriptionPlans] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- SubscriptionPlanTranslations Table
CREATE TABLE [dbo].[SubscriptionPlanTranslations](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SubscriptionPlanId] [int] NOT NULL,
    [LanguageCode] [nvarchar](10) NOT NULL,
    [Name] [nvarchar](200) NOT NULL,
    [Description] [nvarchar](max) NOT NULL,
    [Features] [nvarchar](max) NULL, -- JSON array of features in local language
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    CONSTRAINT [PK_SubscriptionPlanTranslations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SubscriptionPlanTranslations_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans] ([Id]) ON DELETE CASCADE
)
GO

-- UserSubscriptions Table
CREATE TABLE [dbo].[UserSubscriptions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL, -- FK to User Service Users table
    [SubscriptionPlanId] [int] NOT NULL,
    [PaymentId] [int] NULL, -- FK to Payments table
    [RazorpayOrderId] [nvarchar](100) NOT NULL,
    [RazorpayPaymentId] [nvarchar](100) NULL,
    [RazorpaySignature] [nvarchar](100) NULL,
    [PurchasedDate] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [ValidTill] [datetime2](7) NOT NULL,
    [TestsUsed] [int] NOT NULL DEFAULT 0,
    [TestsTotal] [int] NOT NULL DEFAULT 0,
    [AmountPaid] [decimal](18,2) NOT NULL,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'INR',
    [DiscountApplied] [decimal](5,2) NOT NULL DEFAULT 0,
    [Status] [nvarchar](20) NOT NULL DEFAULT 'Active', -- Active, Expired, Cancelled
    [AutoRenewal] [bit] NOT NULL DEFAULT 0,
    [RenewalDate] [datetime2](7) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_UserSubscriptions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans] ([Id])
)
GO

-- Payments Table (Razorpay only)
CREATE TABLE [dbo].[Payments](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL, -- FK to User Service Users table
    [SubscriptionPlanId] [int] NOT NULL,
    [UserSubscriptionId] [int] NULL, -- FK to UserSubscriptions table (filled after successful payment)
    [Amount] [decimal](18,2) NOT NULL,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'INR',
    [DiscountAmount] [decimal](5,2) NOT NULL DEFAULT 0,
    [FinalAmount] [decimal](18,2) NOT NULL,
    [PaymentMethod] [nvarchar](50) NOT NULL, -- Razorpay methods: card, upi, netbanking, wallet
    [PaymentProvider] [nvarchar](50) NOT NULL DEFAULT 'Razorpay',
    [RazorpayOrderId] [nvarchar](100) NOT NULL,
    [RazorpayPaymentId] [nvarchar](100) NULL,
    [RazorpaySignature] [nvarchar](100) NULL,
    [Status] [nvarchar](20) NOT NULL DEFAULT 'Pending', -- Pending, Success, Failed, Refunded
    [PaymentDate] [datetime2](7) NULL,
    [FailureReason] [nvarchar](500) NULL,
    [RefundAmount] [decimal](18,2) NULL,
    [RefundDate] [datetime2](7) NULL,
    [RefundReason] [nvarchar](500) NULL,
    [RazorpayRefundId] [nvarchar](100) NULL,
    [Metadata] [nvarchar](max) NULL, -- Additional payment metadata in JSON
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Payments_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans] ([Id])
)
GO

-- Add foreign key constraints after tables are created
ALTER TABLE [dbo].[UserSubscriptions] WITH CHECK ADD CONSTRAINT [FK_UserSubscriptions_Payments_PaymentId] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payments] ([Id])
GO

ALTER TABLE [dbo].[Payments] WITH CHECK ADD CONSTRAINT [FK_Payments_UserSubscriptions_UserSubscriptionId] FOREIGN KEY([UserSubscriptionId])
REFERENCES [dbo].[UserSubscriptions] ([Id])
GO

-- Indexes for performance
CREATE INDEX [IX_SubscriptionPlans_ExamId] ON [dbo].[SubscriptionPlans] ([ExamId])
GO
CREATE INDEX [IX_SubscriptionPlans_IsActive] ON [dbo].[SubscriptionPlans] ([IsActive])
GO
CREATE INDEX [IX_SubscriptionPlans_Type] ON [dbo].[SubscriptionPlans] ([Type])
GO
CREATE INDEX [IX_SubscriptionPlanTranslations_SubscriptionPlanId] ON [dbo].[SubscriptionPlanTranslations] ([SubscriptionPlanId])
GO
CREATE INDEX [IX_SubscriptionPlanTranslations_LanguageCode] ON [dbo].[SubscriptionPlanTranslations] ([LanguageCode])
GO
CREATE INDEX [IX_UserSubscriptions_UserId] ON [dbo].[UserSubscriptions] ([UserId])
GO
CREATE INDEX [IX_UserSubscriptions_SubscriptionPlanId] ON [dbo].[UserSubscriptions] ([SubscriptionPlanId])
GO
CREATE INDEX [IX_UserSubscriptions_ValidTill] ON [dbo].[UserSubscriptions] ([ValidTill])
GO
CREATE INDEX [IX_UserSubscriptions_Status] ON [dbo].[UserSubscriptions] ([Status])
GO
CREATE INDEX [IX_UserSubscriptions_RazorpayOrderId] ON [dbo].[UserSubscriptions] ([RazorpayOrderId])
GO
CREATE INDEX [IX_Payments_UserId] ON [dbo].[Payments] ([UserId])
GO
CREATE INDEX [IX_Payments_SubscriptionPlanId] ON [dbo].[Payments] ([SubscriptionPlanId])
GO
CREATE INDEX [IX_Payments_Status] ON [dbo].[Payments] ([Status])
GO
CREATE INDEX [IX_Payments_RazorpayOrderId] ON [dbo].[Payments] ([RazorpayOrderId])
GO
CREATE INDEX [IX_Payments_RazorpayPaymentId] ON [dbo].[Payments] ([RazorpayPaymentId])
GO

-- Stored Procedures

-- Create Subscription Plan
CREATE PROCEDURE [dbo].[CreateSubscriptionPlan]
    @Name NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @Type INT,
    @Price DECIMAL(18,2),
    @Currency NVARCHAR(3) = 'INR',
    @TestPapersCount INT = 0,
    @Discount DECIMAL(5,2) = 0,
    @Duration INT = 1,
    @DurationType NVARCHAR(50) = 'Monthly',
    @ValidityDays INT,
    @ExamId INT = NULL,
    @ExamCategory NVARCHAR(100) = NULL,
    @Features NVARCHAR(MAX) = NULL,
    @ImageUrl NVARCHAR(500) = NULL,
    @IsPopular BIT = 0,
    @IsRecommended BIT = 0,
    @CardColorTheme NVARCHAR(50) = NULL,
    @SortOrder INT = 0,
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO SubscriptionPlans (
        Name, Description, Type, Price, Currency, TestPapersCount, Discount, 
        Duration, DurationType, ValidityDays, ExamId, ExamCategory, Features, 
        ImageUrl, IsPopular, IsRecommended, CardColorTheme, SortOrder, CreatedAt, IsActive
    )
    VALUES (
        @Name, @Description, @Type, @Price, @Currency, @TestPapersCount, @Discount,
        @Duration, @DurationType, @ValidityDays, @ExamId, @ExamCategory, @Features,
        @ImageUrl, @IsPopular, @IsRecommended, @CardColorTheme, @SortOrder, GETDATE(), 1
    );
    
    SELECT SCOPE_IDENTITY() AS PlanId;
END
GO

-- Get Active Subscription Plans by Exam
CREATE PROCEDURE [dbo].[GetActiveSubscriptionPlansByExam]
    @ExamId INT = NULL,
    @LanguageCode NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
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
    ORDER BY sp.SortOrder, sp.Name
END
GO

-- Create Razorpay Order for Subscription
CREATE PROCEDURE [dbo].[CreateRazorpayOrder]
    @UserId INT,
    @SubscriptionPlanId INT,
    @Currency NVARCHAR(3) = 'INR',
    @Receipt NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get subscription plan details
    DECLARE @Price DECIMAL(18,2);
    DECLARE @Discount DECIMAL(5,2);
    DECLARE @FinalAmount DECIMAL(18,2);
    
    SELECT @Price = Price, @Discount = Discount
    FROM SubscriptionPlans
    WHERE Id = @SubscriptionPlanId AND IsActive = 1;
    
    IF @Price IS NULL
    BEGIN
        RAISERROR('Subscription plan not found', 16, 1);
        RETURN;
    END
    
    SET @FinalAmount = @Price - (@Price * @Discount / 100);
    
    -- Generate unique receipt if not provided
    IF @Receipt IS NULL
        SET @Receipt = 'order_' + CAST(@UserId AS NVARCHAR) + '_' + CAST(@SubscriptionPlanId AS NVARCHAR) + '_' + REPLACE(CONVERT(NVARCHAR, GETDATE(), 120), '-', '');
    
    -- Insert payment record
    INSERT INTO Payments (
        UserId, SubscriptionPlanId, Amount, Currency, DiscountAmount, FinalAmount,
        PaymentMethod, PaymentProvider, RazorpayOrderId, Status, CreatedAt, IsActive
    )
    VALUES (
        @UserId, @SubscriptionPlanId, @Price, @Currency, @Price * @Discount / 100, @FinalAmount,
        'razorpay', 'Razorpay', @Receipt, 'Pending', GETDATE(), 1
    );
    
    DECLARE @PaymentId INT = SCOPE_IDENTITY();
    
    -- Return order details for Razorpay
    SELECT 
        @PaymentId AS PaymentId,
        @Receipt AS OrderId,
        @FinalAmount AS Amount,
        @Currency,
        @Receipt AS Receipt,
        'created' AS Status,
        GETDATE() AS CreatedAt;
END
GO

-- Verify Razorpay Payment and Create Subscription
CREATE PROCEDURE [dbo].[VerifyRazorpayPayment]
    @RazorpayOrderId NVARCHAR(100),
    @RazorpayPaymentId NVARCHAR(100),
    @RazorpaySignature NVARCHAR(100),
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Get payment details
        DECLARE @PaymentId INT;
        DECLARE @SubscriptionPlanId INT;
        DECLARE @FinalAmount DECIMAL(18,2);
        DECLARE @DiscountAmount DECIMAL(18,2);
        DECLARE @ValidityDays INT;
        DECLARE @TestsTotal INT;
        
        SELECT @PaymentId = Id, @SubscriptionPlanId = SubscriptionPlanId, @FinalAmount = FinalAmount, @DiscountAmount = DiscountAmount
        FROM Payments
        WHERE RazorpayOrderId = @RazorpayOrderId AND UserId = @UserId AND IsActive = 1;
        
        IF @PaymentId IS NULL
        BEGIN
            RAISERROR('Payment record not found', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Get subscription plan details
        SELECT @ValidityDays = ValidityDays, @TestsTotal = TestPapersCount
        FROM SubscriptionPlans
        WHERE Id = @SubscriptionPlanId AND IsActive = 1;
        
        -- Update payment record
        UPDATE Payments
        SET RazorpayPaymentId = @RazorpayPaymentId,
            RazorpaySignature = @RazorpaySignature,
            Status = 'Success',
            PaymentDate = GETDATE(),
            UpdatedAt = GETDATE()
        WHERE Id = @PaymentId;
        
        -- Create user subscription
        INSERT INTO UserSubscriptions (
            UserId, SubscriptionPlanId, PaymentId, RazorpayOrderId, RazorpayPaymentId, RazorpaySignature,
            PurchasedDate, ValidTill, TestsUsed, TestsTotal, AmountPaid, Currency, DiscountApplied, Status, CreatedAt, IsActive
        )
        VALUES (
            @UserId, @SubscriptionPlanId, @PaymentId, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature,
            GETDATE(), DATEADD(DAY, @ValidityDays, GETDATE()), 0, @TestsTotal, @FinalAmount, 'INR', @DiscountAmount, 'Active', GETDATE(), 1
        );
        
        DECLARE @UserSubscriptionId INT = SCOPE_IDENTITY();
        
        -- Update payment with user subscription ID
        UPDATE Payments
        SET UserSubscriptionId = @UserSubscriptionId
        WHERE Id = @PaymentId;
        
        COMMIT TRANSACTION;
        
        -- Return success result
        SELECT 
            1 AS IsSuccess,
            'Payment verified successfully' AS Message,
            @UserSubscriptionId AS UserSubscriptionId,
            @PaymentId AS PaymentId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Update payment status to failed
        UPDATE Payments
        SET Status = 'Failed',
            FailureReason = ERROR_MESSAGE(),
            UpdatedAt = GETDATE()
        WHERE RazorpayOrderId = @RazorpayOrderId AND UserId = @UserId;
        
        SELECT 
            0 AS IsSuccess,
            ERROR_MESSAGE() AS Message,
            NULL AS UserSubscriptionId,
            NULL AS PaymentId;
    END CATCH
END
GO

-- Get User Active Subscriptions
CREATE PROCEDURE [dbo].[GetUserActiveSubscriptions]
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
        sp.Name AS PlanName,
        sp.Description AS PlanDescription,
        sp.Type AS PlanType,
        sp.Price AS PlanPrice,
        sp.TestPapersCount,
        sp.Duration,
        sp.DurationType,
        sp.ExamId,
        sp.ExamCategory,
        sp.Features,
        sp.ImageUrl,
        sp.IsPopular,
        sp.IsRecommended,
        sp.CardColorTheme,
        DATEDIFF(DAY, GETDATE(), us.ValidTill) AS DaysLeft,
        CASE 
            WHEN us.ValidTill > GETDATE() THEN 'Active'
            ELSE 'Expired'
        END AS CurrentStatus
    FROM UserSubscriptions us
    INNER JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
    WHERE us.UserId = @UserId 
    AND us.IsActive = 1 
    AND us.Status = 'Active'
    AND us.ValidTill > GETDATE()
    ORDER BY us.ValidTill DESC
END
GO

-- Get Subscription Statistics
CREATE PROCEDURE [dbo].[GetSubscriptionStatistics]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS ActivePlansCount,
        SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS TotalPlansCount,
        SUM(CASE WHEN IsPopular = 1 AND IsActive = 1 THEN 1 ELSE 0 END) AS PopularPlansCount
    FROM SubscriptionPlans
    
    SELECT 
        COUNT(*) AS MonthlyRevenue,
        SUM(FinalAmount) AS TotalRevenue
    FROM Payments
    WHERE Status = 'Success'
    AND PaymentDate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
    
    SELECT 
        COUNT(*) AS ExpiringSoonCount
    FROM UserSubscriptions
    WHERE Status = 'Active'
    AND ValidTill BETWEEN GETDATE() AND DATEADD(DAY, 7, GETDATE())
    AND IsActive = 1
    
    SELECT 
        COUNT(*) AS NewSubscribersCount
    FROM UserSubscriptions
    WHERE PurchasedDate >= DATEADD(DAY, -30, GETDATE())
    AND IsActive = 1
END
GO

-- Get Paginated Subscription Plans
CREATE PROCEDURE [dbo].[GetSubscriptionPlansPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @IncludeInactive BIT = 0,
    @ExamId INT = NULL,
    @LanguageCode NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
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
    WHERE (@IncludeInactive = 1 OR sp.IsActive = 1)
    AND (@ExamId IS NULL OR sp.ExamId = @ExamId OR sp.ExamId IS NULL)
    ORDER BY sp.SortOrder, sp.Name
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM SubscriptionPlans sp
    WHERE (@IncludeInactive = 1 OR sp.IsActive = 1)
    AND (@ExamId IS NULL OR sp.ExamId = @ExamId OR sp.ExamId IS NULL);
END
GO

-- Update Subscription Plan
CREATE PROCEDURE [dbo].[UpdateSubscriptionPlan]
    @Id INT,
    @Name NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @Type INT,
    @Price DECIMAL(18,2),
    @Currency NVARCHAR(3) = 'INR',
    @TestPapersCount INT = 0,
    @Discount DECIMAL(5,2) = 0,
    @Duration INT = 1,
    @DurationType NVARCHAR(50) = 'Monthly',
    @ValidityDays INT,
    @ExamId INT = NULL,
    @ExamCategory NVARCHAR(100) = NULL,
    @Features NVARCHAR(MAX) = NULL,
    @ImageUrl NVARCHAR(500) = NULL,
    @IsPopular BIT = 0,
    @IsRecommended BIT = 0,
    @CardColorTheme NVARCHAR(50) = NULL,
    @SortOrder INT = 0,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE SubscriptionPlans
    SET Name = @Name,
        Description = @Description,
        Type = @Type,
        Price = @Price,
        Currency = @Currency,
        TestPapersCount = @TestPapersCount,
        Discount = @Discount,
        Duration = @Duration,
        DurationType = @DurationType,
        ValidityDays = @ValidityDays,
        ExamId = @ExamId,
        ExamCategory = @ExamCategory,
        Features = @Features,
        ImageUrl = @ImageUrl,
        IsPopular = @IsPopular,
        IsRecommended = @IsRecommended,
        CardColorTheme = @CardColorTheme,
        SortOrder = @SortOrder,
        IsActive = @IsActive,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Delete Subscription Plan (Soft Delete)
CREATE PROCEDURE [dbo].[DeleteSubscriptionPlan]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE SubscriptionPlans
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Process Refund
CREATE PROCEDURE [dbo].[ProcessRefund]
    @PaymentId INT,
    @RefundAmount DECIMAL(18,2),
    @RefundReason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Get payment details
        DECLARE @UserId INT;
        DECLARE @RazorpayPaymentId NVARCHAR(100);
        DECLARE @CurrentStatus NVARCHAR(20);
        
        SELECT @UserId = UserId, @RazorpayPaymentId = RazorpayPaymentId, @CurrentStatus = Status
        FROM Payments
        WHERE Id = @PaymentId AND IsActive = 1;
        
        IF @PaymentId IS NULL OR @CurrentStatus != 'Success'
        BEGIN
            RAISERROR('Invalid payment or payment not successful', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Update payment record
        UPDATE Payments
        SET RefundAmount = @RefundAmount,
            RefundDate = GETDATE(),
            RefundReason = @RefundReason,
            UpdatedAt = GETDATE()
        WHERE Id = @PaymentId;
        
        -- Cancel user subscription if full refund
        IF @RefundAmount >= (SELECT FinalAmount FROM Payments WHERE Id = @PaymentId)
        BEGIN
            UPDATE UserSubscriptions
            SET Status = 'Cancelled',
                UpdatedAt = GETDATE()
            WHERE PaymentId = @PaymentId;
        END
        
        COMMIT TRANSACTION;
        
        SELECT 1 AS Success, 'Refund processed successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- Get Paginated Payments
CREATE PROCEDURE [dbo].[GetPaymentsPaged]
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @UserId INT = NULL,
    @PlanId INT = NULL,
    @Status NVARCHAR(20) = NULL,
    @PaymentMethod NVARCHAR(50) = NULL,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        p.Id,
        p.UserId,
        p.SubscriptionPlanId,
        p.UserSubscriptionId,
        p.Amount,
        p.Currency,
        p.DiscountAmount,
        p.FinalAmount,
        p.PaymentMethod,
        p.PaymentProvider,
        p.RazorpayOrderId,
        p.RazorpayPaymentId,
        p.RazorpaySignature,
        p.Status,
        p.PaymentDate,
        p.FailureReason,
        p.RefundAmount,
        p.RefundDate,
        p.RefundReason,
        p.RazorpayRefundId,
        p.Metadata,
        p.CreatedAt,
        p.UpdatedAt,
        p.IsActive,
        sp.Name AS PlanName,
        sp.Description AS PlanDescription,
        sp.Type AS PlanType,
        sp.Price AS PlanPrice,
        sp.TestPapersCount,
        sp.Duration,
        sp.DurationType,
        sp.ExamId,
        sp.ExamCategory,
        sp.Features,
        sp.ImageUrl,
        sp.IsPopular,
        sp.IsRecommended,
        sp.CardColorTheme,
        us.UserId AS SubscriptionUserId,
        us.PurchasedDate,
        us.ValidTill,
        us.TestsUsed,
        us.TestsTotal,
        us.AmountPaid AS SubscriptionAmountPaid,
        us.Status AS SubscriptionStatus
    FROM Payments p
    LEFT JOIN SubscriptionPlans sp ON p.SubscriptionPlanId = sp.Id
    LEFT JOIN UserSubscriptions us ON p.UserSubscriptionId = us.Id
    WHERE p.IsActive = 1
    AND (@UserId IS NULL OR p.UserId = @UserId)
    AND (@PlanId IS NULL OR p.SubscriptionPlanId = @PlanId)
    AND (@Status IS NULL OR p.Status = @Status)
    AND (@PaymentMethod IS NULL OR p.PaymentMethod = @PaymentMethod)
    AND (@StartDate IS NULL OR p.CreatedAt >= @StartDate)
    AND (@EndDate IS NULL OR p.CreatedAt <= @EndDate)
    ORDER BY p.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM Payments p
    WHERE p.IsActive = 1
    AND (@UserId IS NULL OR p.UserId = @UserId)
    AND (@PlanId IS NULL OR p.SubscriptionPlanId = @PlanId)
    AND (@Status IS NULL OR p.Status = @Status)
    AND (@PaymentMethod IS NULL OR p.PaymentMethod = @PaymentMethod)
    AND (@StartDate IS NULL OR p.CreatedAt >= @StartDate)
    AND (@EndDate IS NULL OR p.CreatedAt <= @EndDate);
END
GO
