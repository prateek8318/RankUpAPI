

USE [RankUp_AdminDB]
GO

PRINT 'Creating AdminService Stored Procedures...';
PRINT '====================================================';


-- ADMIN PROCEDURES


PRINT 'Creating Admin procedures...';

-- Admin_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Admin_GetAll]
GO

CREATE PROCEDURE [dbo].[Admin_GetAll]
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        a.Id,
        a.UserId,
        a.Role,
        a.IsTwoFactorEnabled,
        a.MobileNumber,
        a.LastLoginAt,
        a.IsActive,
        a.CreatedAt,
        a.UpdatedAt,
        u.Name as UserName,
        u.Email,
        u.PhoneNumber as UserPhoneNumber
    FROM Admins a
    LEFT JOIN Users u ON a.UserId = u.Id
    WHERE a.IsActive = 1
    ORDER BY a.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- Admin_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Admin_GetById]
GO

CREATE PROCEDURE [dbo].[Admin_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.Id,
        a.UserId,
        a.Role,
        a.IsTwoFactorEnabled,
        a.MobileNumber,
        a.LastLoginAt,
        a.IsActive,
        a.CreatedAt,
        a.UpdatedAt,
        u.Name as UserName,
        u.Email,
        u.PhoneNumber as UserPhoneNumber
    FROM Admins a
    LEFT JOIN Users u ON a.UserId = u.Id
    WHERE a.Id = @Id AND a.IsActive = 1;
END
GO

-- Admin_GetByUserId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_GetByUserId]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Admin_GetByUserId]
GO

CREATE PROCEDURE [dbo].[Admin_GetByUserId]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.Id,
        a.UserId,
        a.Role,
        a.IsTwoFactorEnabled,
        a.MobileNumber,
        a.LastLoginAt,
        a.IsActive,
        a.CreatedAt,
        a.UpdatedAt,
        u.Name as UserName,
        u.Email,
        u.PhoneNumber as UserPhoneNumber
    FROM Admins a
    LEFT JOIN Users u ON a.UserId = u.Id
    WHERE a.UserId = @UserId AND a.IsActive = 1;
END
GO

-- Admin_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Admin_Create]
GO

CREATE PROCEDURE [dbo].[Admin_Create]
    @UserId INT,
    @Role NVARCHAR(50) = 'ContentManager',
    @IsTwoFactorEnabled BIT = 0,
    @MobileNumber NVARCHAR(20) = NULL,
    @AdminId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Admins (UserId, Role, IsTwoFactorEnabled, MobileNumber, IsActive, CreatedAt, UpdatedAt)
    VALUES (@UserId, @Role, @IsTwoFactorEnabled, @MobileNumber, 1, GETUTCDATE(), GETUTCDATE());
    
    SET @AdminId = SCOPE_IDENTITY();
END
GO

-- Admin_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Admin_Update]
GO

CREATE PROCEDURE [dbo].[Admin_Update]
    @Id INT,
    @Role NVARCHAR(50),
    @IsTwoFactorEnabled BIT,
    @MobileNumber NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Admins
    SET 
        Role = @Role,
        IsTwoFactorEnabled = @IsTwoFactorEnabled,
        MobileNumber = @MobileNumber,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Admin_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Admin_Delete]
GO

CREATE PROCEDURE [dbo].[Admin_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Admins
    SET 
        IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO


-- ROLE PROCEDURES


PRINT 'Creating Role procedures...';

-- Role_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Role_GetAll]
GO

CREATE PROCEDURE [dbo].[Role_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Roles
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Role_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Role_GetById]
GO

CREATE PROCEDURE [dbo].[Role_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Roles
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Role_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Role_Create]
GO

CREATE PROCEDURE [dbo].[Role_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @RoleId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Roles (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, 1, GETUTCDATE(), GETUTCDATE());
    
    SET @RoleId = SCOPE_IDENTITY();
END
GO

-- Role_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Role_Update]
GO

CREATE PROCEDURE [dbo].[Role_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Roles
    SET 
        Name = @Name,
        Description = @Description,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Role_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Role_Delete]
GO

CREATE PROCEDURE [dbo].[Role_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Roles
    SET 
        IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO


-- PERMISSION PROCEDURES


PRINT 'Creating Permission procedures...';

-- Permission_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Permission_GetAll]
GO

CREATE PROCEDURE [dbo].[Permission_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Resource,
        Action,
        Description,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Permissions
    WHERE IsActive = 1
    ORDER BY Resource, Action;
END
GO

-- Permission_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Permission_GetById]
GO

CREATE PROCEDURE [dbo].[Permission_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Resource,
        Action,
        Description,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Permissions
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Permission_GetByResource
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission_GetByResource]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Permission_GetByResource]
GO

CREATE PROCEDURE [dbo].[Permission_GetByResource]
    @Resource NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Resource,
        Action,
        Description,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Permissions
    WHERE Resource = @Resource AND IsActive = 1
    ORDER BY Action;
END
GO

-- Permission_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Permission_Create]
GO

CREATE PROCEDURE [dbo].[Permission_Create]
    @Name NVARCHAR(100),
    @Resource NVARCHAR(200) = NULL,
    @Action NVARCHAR(50) = NULL,
    @Description NVARCHAR(500) = NULL,
    @PermissionId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Permissions (Name, Resource, Action, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Resource, @Action, @Description, 1, GETUTCDATE(), GETUTCDATE());
    
    SET @PermissionId = SCOPE_IDENTITY();
END
GO

-- =====================================================
-- SUBSCRIPTION PLAN PROCEDURES
-- =====================================================

PRINT 'Creating SubscriptionPlan procedures...';

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
        PlanName,
        ExamType,
        Price,
        Duration,
        ColorCode,
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
        PlanName,
        ExamType,
        Price,
        Duration,
        ColorCode,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- SubscriptionPlan_GetByExamType
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_GetByExamType]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_GetByExamType]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_GetByExamType]
    @ExamType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        PlanName,
        ExamType,
        Price,
        Duration,
        ColorCode,
        IsPopular,
        IsRecommended,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM SubscriptionPlans
    WHERE ExamType = @ExamType AND IsActive = 1
    ORDER BY Price;
END
GO

-- SubscriptionPlan_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_Create]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_Create]
    @PlanName NVARCHAR(100),
    @ExamType NVARCHAR(50),
    @Price DECIMAL(18,2),
    @Duration NVARCHAR(50),
    @ColorCode NVARCHAR(20) = NULL,
    @IsPopular BIT = 0,
    @IsRecommended BIT = 0,
    @PlanId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO SubscriptionPlans (PlanName, ExamType, Price, Duration, ColorCode, IsPopular, IsRecommended, IsActive, CreatedAt, UpdatedAt)
    VALUES (@PlanName, @ExamType, @Price, @Duration, @ColorCode, @IsPopular, @IsRecommended, 1, GETUTCDATE(), GETUTCDATE());
    
    SET @PlanId = SCOPE_IDENTITY();
END
GO

-- SubscriptionPlan_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscriptionPlan_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[SubscriptionPlan_Update]
GO

CREATE PROCEDURE [dbo].[SubscriptionPlan_Update]
    @Id INT,
    @PlanName NVARCHAR(100),
    @ExamType NVARCHAR(50),
    @Price DECIMAL(18,2),
    @Duration NVARCHAR(50),
    @ColorCode NVARCHAR(20) = NULL,
    @IsPopular BIT = 0,
    @IsRecommended BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE SubscriptionPlans
    SET 
        PlanName = @PlanName,
        ExamType = @ExamType,
        Price = @Price,
        Duration = @Duration,
        ColorCode = @ColorCode,
        IsPopular = @IsPopular,
        IsRecommended = @IsRecommended,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- =====================================================
-- AUDIT LOG PROCEDURES
-- =====================================================

PRINT 'Creating AuditLog procedures...';

-- AuditLog_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLog_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[AuditLog_GetAll]
GO

CREATE PROCEDURE [dbo].[AuditLog_GetAll]
    @PageNumber INT = 1,
    @PageSize INT = 50,
    @AdminId INT = NULL,
    @Action NVARCHAR(100) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        al.Id,
        al.AdminId,
        al.Action,
        al.EntityType,
        al.EntityId,
        al.OldValues,
        al.NewValues,
        al.IpAddress,
        al.UserAgent,
        al.CreatedAt,
        a.Name as AdminName
    FROM AuditLogs al
    LEFT JOIN Admins a ON al.AdminId = a.Id
    WHERE (@AdminId IS NULL OR al.AdminId = @AdminId)
    AND (@Action IS NULL OR al.Action LIKE '%' + @Action + '%')
    AND (@StartDate IS NULL OR al.CreatedAt >= @StartDate)
    AND (@EndDate IS NULL OR al.CreatedAt <= @EndDate)
    ORDER BY al.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- AuditLog_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLog_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[AuditLog_Create]
GO

CREATE PROCEDURE [dbo].[AuditLog_Create]
    @AdminId INT,
    @Action NVARCHAR(100),
    @EntityType NVARCHAR(100) = NULL,
    @EntityId INT = NULL,
    @OldValues NVARCHAR(MAX) = NULL,
    @NewValues NVARCHAR(MAX) = NULL,
    @IpAddress NVARCHAR(45) = NULL,
    @UserAgent NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO AuditLogs (AdminId, Action, EntityType, EntityId, OldValues, NewValues, IpAddress, UserAgent, CreatedAt)
    VALUES (@AdminId, @Action, @EntityType, @EntityId, @OldValues, @NewValues, @IpAddress, @UserAgent, GETUTCDATE());
    
    SELECT SCOPE_IDENTITY() as Id;
END
GO

-- PRINT '====================================================';
-- PRINT 'ADMINSERVICE STORED PROCEDURES CREATED SUCCESSFULLY!';
-- PRINT 'Total Procedures: 25+';
-- PRINT '====================================================';
GO
