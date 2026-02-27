

USE [RankUp_MasterDB]
GO

PRINT 'Creating Category Stored Procedures...';
PRINT '====================================================';

-- Category_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_GetById]
GO

CREATE PROCEDURE [dbo].[Category_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        Icon,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Category_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_GetAll]
GO

CREATE PROCEDURE [dbo].[Category_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        Icon,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Category_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_Create]
GO

CREATE PROCEDURE [dbo].[Category_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Icon NVARCHAR(200) = NULL,
    @CategoryId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Categories (Name, Description, Icon, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @Icon, 1, GETUTCDATE(), GETUTCDATE());
    
    SET @CategoryId = SCOPE_IDENTITY();
END
GO

-- Category_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_Update]
GO

CREATE PROCEDURE [dbo].[Category_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Icon NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories
    SET 
        Name = @Name,
        Description = @Description,
        Icon = @Icon,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Category_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_Delete]
GO

CREATE PROCEDURE [dbo].[Category_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories
    SET 
        IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO

-- PRINT '====================================================';
-- PRINT 'CATEGORY STORED PROCEDURES CREATED SUCCESSFULLY!';
-- PRINT 'Total Procedures: 5';
-- PRINT '====================================================';
GO
