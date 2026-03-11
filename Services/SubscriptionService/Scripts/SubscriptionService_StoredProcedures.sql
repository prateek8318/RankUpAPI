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
