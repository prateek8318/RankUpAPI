USE [RankUp_SubscriptionDB]
GO

-- Add missing columns to UserSubscriptions table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'OriginalAmount')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [OriginalAmount] [decimal](18,2) NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'FinalAmount')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [FinalAmount] [decimal](18,2) NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'StartDate')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [StartDate] [datetime2](7) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'EndDate')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [EndDate] [datetime2](7) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'AutoRenew')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [AutoRenew] [bit] NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'RazorpaySubscriptionId')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [RazorpaySubscriptionId] [nvarchar](100) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'LastRenewalDate')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [LastRenewalDate] [datetime2](7) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'CancelledDate')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [CancelledDate] [datetime2](7) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserSubscriptions' AND COLUMN_NAME = 'CancellationReason')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] ADD [CancellationReason] [nvarchar](500) NULL;
END
GO

PRINT 'UserSubscriptions table columns updated successfully'
