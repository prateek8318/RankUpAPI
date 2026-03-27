USE [RankUp_MasterDB]
GO

PRINT 'Creating Countries Table...';
PRINT '====================================================';

-- Check if table exists and drop it
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries]') AND type in (N'U'))
    DROP TABLE [dbo].[Countries]
GO

-- Create Countries table
CREATE TABLE [dbo].[Countries](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Iso2] [nvarchar](2) NOT NULL,
    [CountryCode] [nvarchar](5) NOT NULL,
    [PhoneLength] [int] NOT NULL DEFAULT 10,
    [CurrencyCode] [nvarchar](3) NOT NULL,
    [Image] [nvarchar](255) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- Add indexes for better performance
CREATE INDEX [IX_Countries_Iso2] ON [dbo].[Countries] ([Iso2])
GO

CREATE INDEX [IX_Countries_CountryCode] ON [dbo].[Countries] ([CountryCode])
GO

CREATE INDEX [IX_Countries_IsActive] ON [dbo].[Countries] ([IsActive])
GO

-- Insert sample data for India
INSERT INTO [dbo].[Countries] ([Name], [Iso2], [CountryCode], [PhoneLength], [CurrencyCode], [Image], [IsActive], [CreatedAt])
VALUES ('India', 'IN', '+91', 10, 'INR', '/uploads/flags/india.png', 1, GETUTCDATE())
GO

PRINT '====================================================';
PRINT 'COUNTRIES TABLE CREATED SUCCESSFULLY!';
PRINT 'Sample data inserted for India';
PRINT '====================================================';
GO
