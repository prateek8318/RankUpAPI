

USE [RankUp_MasterDB]
GO



-- Language_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Language_GetById]
GO
CREATE PROCEDURE [dbo].[Language_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Languages] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Language_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Language_GetAll]
GO
CREATE PROCEDURE [dbo].[Language_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Languages] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Language_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Language_GetActive]
GO
CREATE PROCEDURE [dbo].[Language_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Languages] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Language_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Language_Create]
GO
CREATE PROCEDURE [dbo].[Language_Create]
    @Name NVARCHAR(100),
    @Code NVARCHAR(10),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Languages] (Name, Code, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Language_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Language_Update]
GO
CREATE PROCEDURE [dbo].[Language_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Code NVARCHAR(10),
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Languages] 
    SET Name = @Name,
        Code = @Code,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Language_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Language_Delete]
GO
CREATE PROCEDURE [dbo].[Language_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Languages] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

-- =====================================================
-- 2. STATE STORED PROCEDURES
-- =====================================================

-- State_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[State_GetById]
GO
CREATE PROCEDURE [dbo].[State_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[States] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- State_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[State_GetAll]
GO
CREATE PROCEDURE [dbo].[State_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[States] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- State_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[State_GetActive]
GO
CREATE PROCEDURE [dbo].[State_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[States] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- State_GetByCountryCode
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetByCountryCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[State_GetByCountryCode]
GO
CREATE PROCEDURE [dbo].[State_GetByCountryCode]
    @CountryCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[States] 
    WHERE CountryCode = @CountryCode AND IsActive = 1
    ORDER BY Name;
END
GO

-- State_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[State_Create]
GO
CREATE PROCEDURE [dbo].[State_Create]
    @Name NVARCHAR(100),
    @Code NVARCHAR(10),
    @CountryCode NVARCHAR(10),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[States] (Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- State_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[State_Update]
GO
CREATE PROCEDURE [dbo].[State_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Code NVARCHAR(10),
    @CountryCode NVARCHAR(10),
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[States] 
    SET Name = @Name,
        Code = @Code,
        CountryCode = @CountryCode,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- State_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[State_Delete]
GO
CREATE PROCEDURE [dbo].[State_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[States] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO


-- 3. COUNTRY STORED PROCEDURES


-- Country_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Country_GetById]
GO
CREATE PROCEDURE [dbo].[Country_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        SubdivisionLabelEn,
        SubdivisionLabelHi,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Countries] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Country_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Country_GetAll]
GO
CREATE PROCEDURE [dbo].[Country_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        SubdivisionLabelEn,
        SubdivisionLabelHi,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Countries] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Country_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Country_GetActive]
GO
CREATE PROCEDURE [dbo].[Country_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Code,
        SubdivisionLabelEn,
        SubdivisionLabelHi,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Countries] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Country_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Country_Create]
GO
CREATE PROCEDURE [dbo].[Country_Create]
    @Name NVARCHAR(100),
    @Code NVARCHAR(10),
    @SubdivisionLabelEn NVARCHAR(50),
    @SubdivisionLabelHi NVARCHAR(50),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Countries] (Name, Code, SubdivisionLabelEn, SubdivisionLabelHi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @SubdivisionLabelEn, @SubdivisionLabelHi, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Country_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Country_Update]
GO
CREATE PROCEDURE [dbo].[Country_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Code NVARCHAR(10),
    @SubdivisionLabelEn NVARCHAR(50),
    @SubdivisionLabelHi NVARCHAR(50),
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Countries] 
    SET Name = @Name,
        Code = @Code,
        SubdivisionLabelEn = @SubdivisionLabelEn,
        SubdivisionLabelHi = @SubdivisionLabelHi,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Country_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Country_Delete]
GO
CREATE PROCEDURE [dbo].[Country_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Countries] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

PRINT 'MasterService Stored Procedures - Part 1 (Language, State, Country) Completed Successfully!';
GO
