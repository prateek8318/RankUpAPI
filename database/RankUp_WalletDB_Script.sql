USE [master]
GO

-- Drop if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'RankUp_WalletDB')
BEGIN
    ALTER DATABASE [RankUp_WalletDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [RankUp_WalletDB];
END
GO

-- Create with Linux path
CREATE DATABASE [RankUp_WalletDB]
ON PRIMARY 
( NAME = N'RankUp_WalletDB', FILENAME = N'/var/opt/mssql/data/RankUp_WalletDB.mdf', SIZE = 8192KB, FILEGROWTH = 65536KB )
LOG ON 
( NAME = N'RankUp_WalletDB_log', FILENAME = N'/var/opt/mssql/data/RankUp_WalletDB_log.ldf', SIZE = 8192KB, FILEGROWTH = 65536KB )
GO

USE [RankUp_WalletDB]
GO

-- Migrations History
CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId] [nvarchar](150) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
)
GO

-- UserWallets Table
CREATE TABLE [dbo].[UserWallets](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL, -- FK to User Service Users table
    [Balance] [decimal](18,2) NOT NULL DEFAULT 0.00,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'INR',
    [LastRechargeDate] [datetime2](7) NULL,
    [TotalRecharged] [decimal](18,2) NOT NULL DEFAULT 0.00,
    [TotalSpent] [decimal](18,2) NOT NULL DEFAULT 0.00,
    [IsBlocked] [bit] NOT NULL DEFAULT 0,
    [BlockReason] [nvarchar](500) NULL,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_UserWallets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_UserWallets_UserId] UNIQUE ([UserId])
)
GO

-- WalletTransactions Table
CREATE TABLE [dbo].[WalletTransactions](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [WalletId] [int] NOT NULL,
    [TransactionId] [nvarchar](100) NOT NULL, -- Unique transaction identifier
    [TransactionType] [nvarchar](20) NOT NULL, -- RECHARGE, PAYMENT, REFUND, WITHDRAWAL
    [Amount] [decimal](18,2) NOT NULL,
    [BalanceBefore] [decimal](18,2) NOT NULL,
    [BalanceAfter] [decimal](18,2) NOT NULL,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'INR',
    [PaymentMethod] [nvarchar](50) NULL, -- RAZORPAY, UPI, CARD, etc.
    [PaymentProvider] [nvarchar](50) NULL, -- RAZORPAY, GPAY, PAYTM, etc.
    [ProviderTransactionId] [nvarchar](100) NULL, -- Razorpay payment ID, UPI transaction ID
    [Description] [nvarchar](500) NULL,
    [Status] [nvarchar](20) NOT NULL DEFAULT 'PENDING', -- PENDING, SUCCESS, FAILED
    [FailureReason] [nvarchar](500) NULL,
    [ReferenceId] [int] NULL, -- Reference to payment, subscription, etc.
    [ReferenceType] [nvarchar](50) NULL, -- PAYMENT, SUBSCRIPTION, REFUND
    [Metadata] [nvarchar](max) NULL, -- Additional transaction metadata
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime2](7) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    CONSTRAINT [PK_WalletTransactions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WalletTransactions_UserWallets_WalletId] FOREIGN KEY([WalletId]) REFERENCES [dbo].[UserWallets] ([Id]),
    CONSTRAINT [UQ_WalletTransactions_TransactionId] UNIQUE ([TransactionId])
)
GO

-- Indexes for performance
CREATE INDEX [IX_UserWallets_UserId] ON [dbo].[UserWallets] ([UserId])
GO
CREATE INDEX [IX_UserWallets_IsActive] ON [dbo].[UserWallets] ([UserId])
GO

CREATE INDEX [IX_WalletTransactions_WalletId] ON [dbo].[WalletTransactions] ([WalletId])
GO
CREATE INDEX [IX_WalletTransactions_TransactionType] ON [dbo].[WalletTransactions] ([TransactionType])
GO
CREATE INDEX [IX_WalletTransactions_Status] ON [dbo].[WalletTransactions] ([Status])
GO
CREATE INDEX [IX_WalletTransactions_ProviderTransactionId] ON [dbo].[WalletTransactions] ([ProviderTransactionId])
GO
CREATE INDEX [IX_WalletTransactions_CreatedAt] ON [dbo].[WalletTransactions] ([CreatedAt])
GO

-- Stored Procedures

-- Get or Create User Wallet
CREATE PROCEDURE [dbo].[GetOrCreateUserWallet]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Try to get existing wallet
    SELECT Id, UserId, Balance, Currency, LastRechargeDate, TotalRecharged, TotalSpent, IsBlocked, BlockReason, CreatedAt, UpdatedAt, IsActive
    FROM UserWallets
    WHERE UserId = @UserId AND IsActive = 1;
    
    -- If not found, create new wallet
    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO UserWallets (UserId, Balance, Currency, CreatedAt, IsActive)
        VALUES (@UserId, 0.00, 'INR', GETDATE(), 1);
        
        SELECT Id, UserId, Balance, Currency, LastRechargeDate, TotalRecharged, TotalSpent, IsBlocked, BlockReason, CreatedAt, UpdatedAt, IsActive
        FROM UserWallets
        WHERE Id = SCOPE_IDENTITY();
    END
END
GO

-- Add Wallet Transaction
CREATE PROCEDURE [dbo].[AddWalletTransaction]
    @WalletId INT,
    @TransactionType NVARCHAR(20),
    @Amount DECIMAL(18,2),
    @PaymentMethod NVARCHAR(50),
    @PaymentProvider NVARCHAR(50),
    @ProviderTransactionId NVARCHAR(100),
    @Description NVARCHAR(500),
    @ReferenceId INT,
    @ReferenceType NVARCHAR(50),
    @Metadata NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionId NVARCHAR(100) = 'WT_' + CAST(@WalletId AS NVARCHAR) + '_' + REPLACE(CONVERT(NVARCHAR, GETDATE(), 120), '-', '') + '_' + RIGHT('000000' + CAST((SELECT COUNT(*) + 1 FROM WalletTransactions WHERE WalletId = @WalletId) AS NVARCHAR), 6);
    DECLARE @BalanceBefore DECIMAL(18,2);
    DECLARE @BalanceAfter DECIMAL(18,2);
    
    -- Get current balance
    SELECT @BalanceBefore = Balance FROM UserWallets WHERE Id = @WalletId;
    
    -- Calculate new balance based on transaction type
    IF @TransactionType IN ('RECHARGE', 'REFUND')
        SET @BalanceAfter = @BalanceBefore + @Amount;
    ELSE
        SET @BalanceAfter = @BalanceBefore - @Amount;
    
    -- Insert transaction
    INSERT INTO WalletTransactions (
        WalletId, TransactionId, TransactionType, Amount, BalanceBefore, BalanceAfter,
        PaymentMethod, PaymentProvider, ProviderTransactionId, Description, Status,
        ReferenceId, ReferenceType, Metadata, CreatedAt, IsActive
    )
    VALUES (
        @WalletId, @TransactionId, @TransactionType, @Amount, @BalanceBefore, @BalanceAfter,
        @PaymentMethod, @PaymentProvider, @ProviderTransactionId, @Description, 'SUCCESS',
        @ReferenceId, @ReferenceType, @Metadata, GETDATE(), 1
    );
    
    -- Update wallet balance
    UPDATE UserWallets
    SET Balance = @BalanceAfter,
        UpdatedAt = GETDATE(),
        LastRechargeDate = CASE WHEN @TransactionType = 'RECHARGE' THEN GETDATE() ELSE LastRechargeDate END,
        TotalRecharged = CASE WHEN @TransactionType = 'RECHARGE' THEN TotalRecharged + @Amount ELSE TotalRecharged END,
        TotalSpent = CASE WHEN @TransactionType IN ('PAYMENT', 'WITHDRAWAL') THEN TotalSpent + @Amount ELSE TotalSpent END
    WHERE Id = @WalletId;
    
    SELECT @TransactionId AS TransactionId, @BalanceAfter AS NewBalance;
END
GO

-- Get Wallet Transactions with Pagination
CREATE PROCEDURE [dbo].[GetWalletTransactions]
    @UserId INT,
    @TransactionType NVARCHAR(20) = NULL,
    @Status NVARCHAR(20) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        wt.Id,
        wt.TransactionId,
        wt.TransactionType,
        wt.Amount,
        wt.BalanceBefore,
        wt.BalanceAfter,
        wt.Currency,
        wt.PaymentMethod,
        wt.PaymentProvider,
        wt.ProviderTransactionId,
        wt.Description,
        wt.Status,
        wt.FailureReason,
        wt.ReferenceId,
        wt.ReferenceType,
        wt.Metadata,
        wt.CreatedAt,
        wt.UpdatedAt,
        uw.UserId
    FROM WalletTransactions wt
    INNER JOIN UserWallets uw ON wt.WalletId = uw.Id
    WHERE uw.UserId = @UserId 
    AND wt.IsActive = 1
    AND (@TransactionType IS NULL OR wt.TransactionType = @TransactionType)
    AND (@Status IS NULL OR wt.Status = @Status)
    ORDER BY wt.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM WalletTransactions wt
    INNER JOIN UserWallets uw ON wt.WalletId = uw.Id
    WHERE uw.UserId = @UserId 
    AND wt.IsActive = 1
    AND (@TransactionType IS NULL OR wt.TransactionType = @TransactionType)
    AND (@Status IS NULL OR wt.Status = @Status);
END
GO

-- Get Wallet Statistics
CREATE PROCEDURE [dbo].[GetWalletStatistics]
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @UserId IS NOT NULL
    BEGIN
        -- Individual user wallet stats
        SELECT 
            uw.Balance,
            uw.TotalRecharged,
            uw.TotalSpent,
            uw.LastRechargeDate,
            COUNT(wt.Id) AS TotalTransactions,
            SUM(CASE WHEN wt.TransactionType = 'RECHARGE' THEN 1 ELSE 0 END) AS RechargeTransactions,
            SUM(CASE WHEN wt.TransactionType = 'PAYMENT' THEN 1 ELSE 0 END) AS PaymentTransactions,
            SUM(CASE WHEN wt.TransactionType = 'REFUND' THEN 1 ELSE 0 END) AS RefundTransactions
        FROM UserWallets uw
        LEFT JOIN WalletTransactions wt ON uw.Id = wt.WalletId AND wt.IsActive = 1
        WHERE uw.UserId = @UserId AND uw.IsActive = 1
        GROUP BY uw.Balance, uw.TotalRecharged, uw.TotalSpent, uw.LastRechargeDate;
    END
    ELSE
    BEGIN
        -- Overall wallet statistics
        SELECT 
            COUNT(*) AS TotalWallets,
            SUM(uw.Balance) AS TotalBalance,
            SUM(uw.TotalRecharged) AS TotalRechargedAmount,
            SUM(uw.TotalSpent) AS TotalSpentAmount,
            COUNT(CASE WHEN uw.Balance > 0 THEN 1 END) AS ActiveWallets,
            AVG(uw.Balance) AS AverageBalance,
            MAX(uw.Balance) AS HighestBalance,
            COUNT(wt.Id) AS TotalTransactions,
            SUM(CASE WHEN wt.TransactionType = 'RECHARGE' THEN 1 ELSE 0 END) AS TotalRecharges,
            SUM(CASE WHEN wt.TransactionType = 'RECHARGE' THEN wt.Amount ELSE 0 END) AS TotalRechargeAmount,
            SUM(CASE WHEN wt.TransactionType = 'PAYMENT' THEN 1 ELSE 0 END) AS TotalPayments,
            SUM(CASE WHEN wt.TransactionType = 'PAYMENT' THEN wt.Amount ELSE 0 END) AS TotalPaymentAmount
        FROM UserWallets uw
        LEFT JOIN WalletTransactions wt ON uw.Id = wt.WalletId AND wt.IsActive = 1
        WHERE uw.IsActive = 1;
    END
END
GO

-- Check Wallet Balance
CREATE PROCEDURE [dbo].[CheckWalletBalance]
    @UserId INT,
    @RequiredAmount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        uw.Id AS WalletId,
        uw.Balance,
        CASE 
            WHEN uw.Balance >= @RequiredAmount AND uw.IsBlocked = 0 AND uw.IsActive = 1 
            THEN 1 
            ELSE 0 
        END AS IsSufficient,
        CASE 
            WHEN uw.IsBlocked = 1 THEN 'Wallet is blocked: ' + ISNULL(uw.BlockReason, 'Unknown reason')
            WHEN uw.IsActive = 0 THEN 'Wallet is inactive'
            WHEN uw.Balance < @RequiredAmount THEN 'Insufficient balance'
            ELSE 'Sufficient balance'
        END AS Message
    FROM UserWallets uw
    WHERE uw.UserId = @UserId;
END
GO
