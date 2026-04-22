-- Add DurationOptionId column to UserSubscriptions table
-- This column is required by the UserSubscription entity but missing from the database

USE [RankUp_SubscriptionDB]
GO

-- Add DurationOptionId column if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserSubscriptions') AND name = 'DurationOptionId')
BEGIN
    ALTER TABLE [dbo].[UserSubscriptions] 
    ADD [DurationOptionId] INT NULL;
    
    PRINT 'DurationOptionId column added to UserSubscriptions table';
END
ELSE
BEGIN
    PRINT 'DurationOptionId column already exists in UserSubscriptions table';
END
GO

-- Add index for better performance
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('UserSubscriptions') AND name = 'IX_UserSubscriptions_DurationOptionId')
BEGIN
    CREATE INDEX [IX_UserSubscriptions_DurationOptionId] ON [dbo].[UserSubscriptions] ([DurationOptionId]);
    PRINT 'Index IX_UserSubscriptions_DurationOptionId created';
END
ELSE
BEGIN
    PRINT 'Index IX_UserSubscriptions_DurationOptionId already exists';
END
GO

PRINT 'Database schema update completed successfully';
