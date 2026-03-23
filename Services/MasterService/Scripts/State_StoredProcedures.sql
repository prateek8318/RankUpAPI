USE [RankUp_MasterDB]
GO

PRINT 'Creating State Stored Procedures...';
PRINT '====================================================';

-- State_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetById]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[State_GetById]
GO

CREATE PROCEDURE [dbo].[State_GetById]
    @Id INT,
    @LanguageId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageId IS NOT NULL
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id AND sl.LanguageId = @LanguageId 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        WHERE s.Id = @Id;
    END
    ELSE
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        WHERE s.Id = @Id;
    END
END
GO

-- State_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetAll]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[State_GetAll]
GO

CREATE PROCEDURE [dbo].[State_GetAll]
    @LanguageId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageId IS NOT NULL
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id AND sl.LanguageId = @LanguageId 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        ORDER BY s.Name;
    END
    ELSE
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        ORDER BY s.Name;
    END
END
GO

-- State_GetByCountryCode
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_GetByCountryCode]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[State_GetByCountryCode]
GO

CREATE PROCEDURE [dbo].[State_GetByCountryCode]
    @CountryCode NVARCHAR(10),
    @LanguageId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageId IS NOT NULL
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id AND sl.LanguageId = @LanguageId 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        WHERE s.CountryCode = @CountryCode
        ORDER BY s.Name;
    END
    ELSE
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        WHERE s.CountryCode = @CountryCode
        ORDER BY s.Name;
    END
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
    @NamesJson NVARCHAR(MAX) = NULL,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if country exists
    IF NOT EXISTS (SELECT 1 FROM Countries WHERE Code = @CountryCode AND IsActive = 1)
    BEGIN
        RAISERROR('Invalid country code', 16, 1);
        RETURN -1;
    END
    
    -- Check for duplicate code within the same country
    IF EXISTS (SELECT 1 FROM States WHERE Code = @Code AND CountryCode = @CountryCode)
    BEGIN
        RAISERROR('State with this code already exists in the country', 16, 1);
        RETURN -1;
    END
    
    INSERT INTO States (Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();

    IF @NamesJson IS NOT NULL AND @Id > 0
    BEGIN
        INSERT INTO dbo.StateLanguages (StateId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StateId,
            LanguageId,
            Name,
            1 as IsActive,
            @CreatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(100) '$.Name'
        );
    END
    RETURN 0;
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
    @IsActive BIT = 1,
    @NamesJson NVARCHAR(MAX) = NULL,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if state exists
    IF NOT EXISTS (SELECT 1 FROM States WHERE Id = @Id)
    BEGIN
        RAISERROR('State not found', 16, 1);
        RETURN -1;
    END
    
    -- Check if country exists
    IF NOT EXISTS (SELECT 1 FROM Countries WHERE Code = @CountryCode AND IsActive = 1)
    BEGIN
        RAISERROR('Invalid country code', 16, 1);
        RETURN -1;
    END
    
    -- Check for duplicate code within the same country (excluding current state)
    IF EXISTS (SELECT 1 FROM States WHERE Code = @Code AND CountryCode = @CountryCode AND Id != @Id)
    BEGIN
        RAISERROR('State with this code already exists in the country', 16, 1);
        RETURN -1;
    END
    
    UPDATE States
    SET 
        Name = @Name,
        Code = @Code,
        CountryCode = @CountryCode,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    IF @NamesJson IS NOT NULL
    BEGIN
        DELETE FROM dbo.StateLanguages WHERE StateId = @Id;

        INSERT INTO dbo.StateLanguages (StateId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StateId,
            LanguageId,
            Name,
            1 as IsActive,
            @UpdatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(100) '$.Name'
        );
    END
    
    RETURN 0;
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
    
    -- Check if state exists
    IF NOT EXISTS (SELECT 1 FROM States WHERE Id = @Id)
    BEGIN
        RAISERROR('State not found', 16, 1);
        RETURN -1;
    END
    
    -- Delete dependent records first
    DELETE FROM StateLanguages WHERE StateId = @Id;
    
    -- Delete the state
    DELETE FROM States WHERE Id = @Id;
    
    RETURN 0;
END
GO

-- State_ToggleStatus
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State_ToggleStatus]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[State_ToggleStatus]
GO

CREATE PROCEDURE [dbo].[State_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if state exists
    IF NOT EXISTS (SELECT 1 FROM States WHERE Id = @Id)
    BEGIN
        RAISERROR('State not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE States
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO

PRINT '====================================================';
PRINT 'STATE STORED PROCEDURES CREATED SUCCESSFULLY!';
PRINT 'Total Procedures: 7';
PRINT '====================================================';
GO
