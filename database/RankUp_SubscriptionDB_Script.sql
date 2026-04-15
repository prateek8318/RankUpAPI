USE [master]
GO

-- Drop if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RankUp_SubscriptionDB')
BEGIN
    ALTER DATABASE [RankUp_SubscriptionDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RankUp_SubscriptionDB];
END
GO

-- Create with Linux path
CREATE DATABASE [RankUp_SubscriptionDB]
ON PRIMARY 
( NAME = N'RankUp_SubscriptionDB', FILENAME = N'/var/opt/mssql/data/RankUp_SubscriptionDB.mdf', SIZE = 8192KB, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'RankUp_SubscriptionDB_log', FILENAME = N'/var/opt/mssql/data/RankUp_SubscriptionDB_log.ldf', SIZE = 8192KB, FILEGROWTH = 65536KB )
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
    CONSTRAINT [FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans] ([Id]),
    CONSTRAINT [FK_UserSubscriptions_Payments_PaymentId] FOREIGN KEY([PaymentId]) REFERENCES [dbo].[Payments] ([Id])
)
GO

-- Payments Table
CREATE TABLE [dbo].[Payments](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL, -- FK to User Service Users table
    [SubscriptionPlanId] [int] NOT NULL,
    [UserSubscriptionId] [int] NULL, -- FK to UserSubscriptions table (filled after successful payment)
    [Amount] [decimal](18,2) NOT NULL,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'INR',
    [DiscountAmount] [decimal](5,2) NOT NULL DEFAULT 0,
    [FinalAmount] [decimal](18,2) NOT NULL,
    [PaymentMethod] [nvarchar](50) NOT NULL, -- UPI, CreditCard, etc.
    [PaymentProvider] [nvarchar](50) NOT NULL, -- GPay, Paytm, PhonePe, etc.
    [TransactionId] [nvarchar](100) NULL, -- Provider transaction ID
    [ProviderOrderId] [nvarchar](100) NULL, -- Provider order ID
    [Status] [nvarchar](20) NOT NULL DEFAULT 'Pending', -- Pending, Success, Failed, Refunded
    [PaymentDate] [datetime2](7) NULL,
    [FailureReason] [nvarchar](500) NULL,
    [RefundAmount] [decimal](18,2) NULL,
    [RefundDate] [datetime2](7) NULL,
    [RefundReason] [nvarchar](500) NULL,
    [Metadata] [nvarchar](max) NULL, -- Additional payment metadata in JSON
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Payments_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId]) REFERENCES [dbo].[SubscriptionPlans] ([Id]),
    CONSTRAINT [FK_Payments_UserSubscriptions_UserSubscriptionId] FOREIGN KEY([UserSubscriptionId]) REFERENCES [dbo].[UserSubscriptions] ([Id])
)
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
CREATE INDEX [IX_Payments_UserId] ON [dbo].[Payments] ([UserId])
GO
CREATE INDEX [IX_Payments_SubscriptionPlanId] ON [dbo].[Payments] ([SubscriptionPlanId])
GO
CREATE INDEX [IX_Payments_Status] ON [dbo].[Payments] ([Status])
GO
CREATE INDEX [IX_Payments_TransactionId] ON [dbo].[Payments] ([TransactionId])
GO

-- Stored Procedures

-- Get Active Subscription Plans by Exam
CREATE PROCEDURE [dbo].[GetActiveSubscriptionPlansByExam]
    @ExamId INT,
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

-- Create User Subscription from Payment
CREATE PROCEDURE [dbo].[CreateUserSubscription]
    @UserId INT,
    @SubscriptionPlanId INT,
    @PaymentId INT,
    @AmountPaid DECIMAL(18,2),
    @Currency NVARCHAR(3) = 'INR',
    @DiscountApplied DECIMAL(5,2) = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ValidityDays INT;
    DECLARE @TestsTotal INT;
    
    -- Get plan details
    SELECT @ValidityDays = ValidityDays, @TestsTotal = TestPapersCount
    FROM SubscriptionPlans
    WHERE Id = @SubscriptionPlanId AND IsActive = 1;
    
    IF @ValidityDays IS NULL
    BEGIN
        RAISERROR('Invalid subscription plan', 16, 1);
        RETURN;
    END
    
    -- Insert user subscription
    INSERT INTO UserSubscriptions (
        UserId, SubscriptionPlanId, PaymentId, PurchasedDate, ValidTill,
        TestsUsed, TestsTotal, AmountPaid, Currency, DiscountApplied, Status
    )
    VALUES (
        @UserId, @SubscriptionPlanId, @PaymentId, GETDATE(), DATEADD(DAY, @ValidityDays, GETDATE()),
        0, @TestsTotal, @AmountPaid, @Currency, @DiscountApplied, 'Active'
    );
    
    SELECT SCOPE_IDENTITY() AS UserSubscriptionId;
END
GO
