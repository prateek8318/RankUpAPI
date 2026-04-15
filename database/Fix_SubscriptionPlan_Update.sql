-- Fix for SubscriptionPlan_Update stored procedure
-- This script updates the stored procedure to include missing fields: IsPopular, IsRecommended, CardColorTheme

USE [RankUp_SubscriptionDB]
GO

-- Drop existing procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_Update]
GO

-- Create updated procedure with missing fields
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

PRINT 'SubscriptionPlan_Update stored procedure fixed successfully.'
