

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
        NameEn,
        NameHi,
        [Key],
        Type,
        Description,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE Id = @Id;
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
        NameEn,
        NameHi,
        [Key],
        Type,
        Description,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE IsActive = 1
    ORDER BY DisplayOrder, NameEn;
END
GO

-- Category_GetActiveByType
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_GetActiveByType]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_GetActiveByType]
GO

CREATE PROCEDURE [dbo].[Category_GetActiveByType]
    @Type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        NameEn,
        NameHi,
        [Key],
        Type,
        Description,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE Type = @Type AND IsActive = 1
    ORDER BY DisplayOrder, NameEn;
END
GO

-- Category_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_Create]
GO

CREATE PROCEDURE [dbo].[Category_Create]
    @NameEn NVARCHAR(100),
    @NameHi NVARCHAR(100) = NULL,
    @Key NVARCHAR(50),
    @Type NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @DisplayOrder INT = 0,
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Categories (NameEn, NameHi, [Key], Type, Description, DisplayOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES (@NameEn, @NameHi, @Key, @Type, @Description, @DisplayOrder, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Category_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Category_Update]
GO

CREATE PROCEDURE [dbo].[Category_Update]
    @Id INT,
    @NameEn NVARCHAR(100),
    @NameHi NVARCHAR(100) = NULL,
    @Key NVARCHAR(50),
    @Type NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @DisplayOrder INT = 0,
    @IsActive BIT = 1,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories
    SET 
        NameEn = @NameEn,
        NameHi = @NameHi,
        [Key] = @Key,
        Type = @Type,
        Description = @Description,
        DisplayOrder = @DisplayOrder,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
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
