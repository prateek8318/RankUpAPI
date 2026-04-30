USE [RankUp_QuestionDB]
GO

PRINT 'Creating MockTestLanguages Table...';
PRINT '====================================================';

-- Check if table exists and drop it
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MockTestLanguages]') AND type in (N'U'))
    DROP TABLE [dbo].[MockTestLanguages]
GO

-- Create MockTestLanguages table
CREATE TABLE [dbo].[MockTestLanguages](
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [MockTestId] INT NOT NULL,
    [LanguageId] INT NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    
    -- Foreign Key Constraints
    CONSTRAINT [FK_MockTestLanguages_MockTests] FOREIGN KEY ([MockTestId]) REFERENCES [dbo].[MockTests]([Id]) ON DELETE CASCADE,
    
    -- Unique Constraints
    CONSTRAINT [UK_MockTestLanguages_MockTestLanguage] UNIQUE ([MockTestId], [LanguageId]),
    
    -- Check Constraints
    CONSTRAINT [CK_MockTestLanguages_Name] CHECK ([Name] <> '')
)
GO

-- Add indexes for better performance
CREATE INDEX [IX_MockTestLanguages_MockTestId] ON [dbo].[MockTestLanguages] ([MockTestId])
GO

CREATE INDEX [IX_MockTestLanguages_LanguageId] ON [dbo].[MockTestLanguages] ([LanguageId])
GO

CREATE INDEX [IX_MockTestLanguages_IsActive] ON [dbo].[MockTestLanguages] ([IsActive])
GO

PRINT '====================================================';
PRINT 'MOCKTESTLANGUAGES TABLE CREATED SUCCESSFULLY!';
PRINT '====================================================';
GO
