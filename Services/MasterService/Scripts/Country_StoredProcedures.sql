USE [RankUp_MasterDB]
GO

PRINT 'Creating Country Stored Procedures...';
PRINT '====================================================';

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
        Iso2,
        CountryCode,
        PhoneLength,
        CurrencyCode,
        Image,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Countries] 
    WHERE Id = @Id;
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
        Iso2,
        CountryCode,
        PhoneLength,
        CurrencyCode,
        Image,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Countries] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Country_GetByCode
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_GetByCode]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Country_GetByCode]
GO

CREATE PROCEDURE [dbo].[Country_GetByCode]
    @Iso2 NVARCHAR(2)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Iso2,
        CountryCode,
        PhoneLength,
        CurrencyCode,
        Image,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Countries] 
    WHERE Iso2 = @Iso2 AND IsActive = 1;
END
GO

-- Country_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Country_Create]
GO

CREATE PROCEDURE [dbo].[Country_Create]
    @Name NVARCHAR(100),
    @Iso2 NVARCHAR(2),
    @CountryCode NVARCHAR(5),
    @PhoneLength INT = 10,
    @CurrencyCode NVARCHAR(3),
    @Image NVARCHAR(255) = NULL,
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if Iso2 already exists
    IF EXISTS (SELECT 1 FROM Countries WHERE Iso2 = @Iso2)
    BEGIN
        RAISERROR('Country with this ISO2 code already exists', 16, 1);
        RETURN -1;
    END
    
    INSERT INTO Countries (Name, Iso2, CountryCode, PhoneLength, CurrencyCode, Image, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Iso2, @CountryCode, @PhoneLength, @CurrencyCode, @Image, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
    RETURN 0;
END
GO

-- Country_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Country_Update]
GO

CREATE PROCEDURE [dbo].[Country_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Iso2 NVARCHAR(2),
    @CountryCode NVARCHAR(5),
    @PhoneLength INT = 10,
    @CurrencyCode NVARCHAR(3),
    @Image NVARCHAR(255) = NULL,
    @IsActive BIT = 1,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if country exists
    IF NOT EXISTS (SELECT 1 FROM Countries WHERE Id = @Id)
    BEGIN
        RAISERROR('Country not found', 16, 1);
        RETURN -1;
    END
    
    -- Check if Iso2 already exists for another country
    IF EXISTS (SELECT 1 FROM Countries WHERE Iso2 = @Iso2 AND Id != @Id)
    BEGIN
        RAISERROR('Country with this ISO2 code already exists', 16, 1);
        RETURN -1;
    END
    
    UPDATE Countries
    SET 
        Name = @Name,
        Iso2 = @Iso2,
        CountryCode = @CountryCode,
        PhoneLength = @PhoneLength,
        CurrencyCode = @CurrencyCode,
        Image = @Image,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
    
    RETURN 0;
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
    
    -- Check if country exists
    IF NOT EXISTS (SELECT 1 FROM Countries WHERE Id = @Id)
    BEGIN
        RAISERROR('Country not found', 16, 1);
        RETURN -1;
    END
    
    -- Check if country has states (assuming States table has CountryCode or CountryId)
    IF EXISTS (SELECT 1 FROM States WHERE CountryCode = (SELECT Iso2 FROM Countries WHERE Id = @Id))
    BEGIN
        -- Soft delete - set IsActive to 0 instead of hard delete
        UPDATE Countries
        SET 
            IsActive = 0,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @Id;
    END
    ELSE
    BEGIN
        -- Hard delete if no dependent records
        DELETE FROM Countries WHERE Id = @Id;
    END
    
    RETURN 0;
END
GO

-- Country_ToggleStatus
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country_ToggleStatus]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Country_ToggleStatus]
GO

CREATE PROCEDURE [dbo].[Country_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if country exists
    IF NOT EXISTS (SELECT 1 FROM Countries WHERE Id = @Id)
    BEGIN
        RAISERROR('Country not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE Countries
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO

PRINT '====================================================';
PRINT 'COUNTRY STORED PROCEDURES CREATED SUCCESSFULLY!';
PRINT 'Total Procedures: 7';
PRINT '====================================================';
GO
