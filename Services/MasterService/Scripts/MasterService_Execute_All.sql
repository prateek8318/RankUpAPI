

-- Language_GetAll
IF OBJECT_ID('dbo.Language_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.Language_GetAll;
GO
CREATE PROCEDURE dbo.Language_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Languages;
END
GO

-- Language_GetActive
IF OBJECT_ID('dbo.Language_GetActive', 'P') IS NOT NULL DROP PROCEDURE dbo.Language_GetActive;
GO
CREATE PROCEDURE dbo.Language_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Languages
    WHERE IsActive = 1;
END
GO

-- Language_GetById
IF OBJECT_ID('dbo.Language_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.Language_GetById;
GO
CREATE PROCEDURE dbo.Language_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Languages
    WHERE Id = @Id;
END
GO

-- Language_Create
IF OBJECT_ID('dbo.Language_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.Language_Create;
GO
CREATE PROCEDURE dbo.Language_Create
    @Name       NVARCHAR(100),
    @Code       NVARCHAR(10),
    @IsActive   BIT,
    @CreatedAt  DATETIME2,
    @UpdatedAt  DATETIME2,
    @Id         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Languages (Name, Code, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Language_Update
IF OBJECT_ID('dbo.Language_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Language_Update;
GO
CREATE PROCEDURE dbo.Language_Update
    @Id         INT,
    @Name       NVARCHAR(100),
    @Code       NVARCHAR(10),
    @IsActive   BIT,
    @UpdatedAt  DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Languages
    SET Name = @Name, Code = @Code, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Language_Delete
IF OBJECT_ID('dbo.Language_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.Language_Delete;
GO
CREATE PROCEDURE dbo.Language_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Languages WHERE Id = @Id;
END
GO



-- COUNTRY STORED PROCEDURES


-- Country_GetAll
IF OBJECT_ID('dbo.Country_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.Country_GetAll;
GO
CREATE PROCEDURE dbo.Country_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, SubdivisionLabelEn, SubdivisionLabelHi, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries;
END
GO

-- Country_GetById
IF OBJECT_ID('dbo.Country_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.Country_GetById;
GO
CREATE PROCEDURE dbo.Country_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, SubdivisionLabelEn, SubdivisionLabelHi, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries
    WHERE Id = @Id;
END
GO

-- Country_GetByCode
IF OBJECT_ID('dbo.Country_GetByCode', 'P') IS NOT NULL DROP PROCEDURE dbo.Country_GetByCode;
GO
CREATE PROCEDURE dbo.Country_GetByCode
    @Code NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, SubdivisionLabelEn, SubdivisionLabelHi, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries
    WHERE Code = @Code;
END
GO

-- Country_Create
IF OBJECT_ID('dbo.Country_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.Country_Create;
GO
CREATE PROCEDURE dbo.Country_Create
    @Name                   NVARCHAR(100),
    @Code                   NVARCHAR(10),
    @SubdivisionLabelEn     NVARCHAR(100),
    @SubdivisionLabelHi     NVARCHAR(100),
    @IsActive               BIT,
    @CreatedAt              DATETIME2,
    @UpdatedAt              DATETIME2,
    @Id                     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Countries (Name, Code, SubdivisionLabelEn, SubdivisionLabelHi, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @SubdivisionLabelEn, @SubdivisionLabelHi, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Country_Update
IF OBJECT_ID('dbo.Country_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Country_Update;
GO
CREATE PROCEDURE dbo.Country_Update
    @Id                     INT,
    @Name                   NVARCHAR(100),
    @Code                   NVARCHAR(10),
    @SubdivisionLabelEn     NVARCHAR(100),
    @SubdivisionLabelHi     NVARCHAR(100),
    @IsActive               BIT,
    @UpdatedAt              DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Countries
    SET Name = @Name, Code = @Code,
        SubdivisionLabelEn = @SubdivisionLabelEn,
        SubdivisionLabelHi = @SubdivisionLabelHi,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Country_Delete
IF OBJECT_ID('dbo.Country_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.Country_Delete;
GO
CREATE PROCEDURE dbo.Country_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Countries WHERE Id = @Id;
END
GO



-- STATE STORED PROCEDURES


-- State_GetAll
IF OBJECT_ID('dbo.State_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.State_GetAll;
GO
CREATE PROCEDURE dbo.State_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States;
END
GO

-- State_GetActive
IF OBJECT_ID('dbo.State_GetActive', 'P') IS NOT NULL DROP PROCEDURE dbo.State_GetActive;
GO
CREATE PROCEDURE dbo.State_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE IsActive = 1;
END
GO

-- State_GetById
IF OBJECT_ID('dbo.State_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.State_GetById;
GO
CREATE PROCEDURE dbo.State_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE Id = @Id;
END
GO

-- State_GetActiveByCountryCode
IF OBJECT_ID('dbo.State_GetActiveByCountryCode', 'P') IS NOT NULL DROP PROCEDURE dbo.State_GetActiveByCountryCode;
GO
CREATE PROCEDURE dbo.State_GetActiveByCountryCode
    @CountryCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE IsActive = 1 AND CountryCode = @CountryCode;
END
GO

-- State_Create
IF OBJECT_ID('dbo.State_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.State_Create;
GO
CREATE PROCEDURE dbo.State_Create
    @Name        NVARCHAR(100),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.States (Name, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- State_Update
IF OBJECT_ID('dbo.State_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.State_Update;
GO
CREATE PROCEDURE dbo.State_Update
    @Id          INT,
    @Name        NVARCHAR(100),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.States
    SET Name = @Name, CountryCode = @CountryCode, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- State_Delete
IF OBJECT_ID('dbo.State_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.State_Delete;
GO
CREATE PROCEDURE dbo.State_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.States WHERE Id = @Id;
END
GO

-- State_GetWithEmptyNames
IF OBJECT_ID('dbo.State_GetWithEmptyNames', 'P') IS NOT NULL DROP PROCEDURE dbo.State_GetWithEmptyNames;
GO
CREATE PROCEDURE dbo.State_GetWithEmptyNames
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE Name IS NULL OR Name = '';
END
GO



-- QUALIFICATION STORED PROCEDURES


-- Qualification_GetAll
IF OBJECT_ID('dbo.Qualification_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_GetAll;
GO
CREATE PROCEDURE dbo.Qualification_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications;
END
GO

-- Qualification_GetActive
IF OBJECT_ID('dbo.Qualification_GetActive', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_GetActive;
GO
CREATE PROCEDURE dbo.Qualification_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE IsActive = 1;
END
GO

-- Qualification_GetById
IF OBJECT_ID('dbo.Qualification_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_GetById;
GO
CREATE PROCEDURE dbo.Qualification_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE Id = @Id;
END
GO

-- Qualification_GetActiveByCountryCode
IF OBJECT_ID('dbo.Qualification_GetActiveByCountryCode', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_GetActiveByCountryCode;
GO
CREATE PROCEDURE dbo.Qualification_GetActiveByCountryCode
    @CountryCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE IsActive = 1 AND CountryCode = @CountryCode;
END
GO

-- Qualification_Create
IF OBJECT_ID('dbo.Qualification_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_Create;
GO
CREATE PROCEDURE dbo.Qualification_Create
    @Name        NVARCHAR(200),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Qualifications (Name, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Qualification_Update
IF OBJECT_ID('dbo.Qualification_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_Update;
GO
CREATE PROCEDURE dbo.Qualification_Update
    @Id          INT,
    @Name        NVARCHAR(200),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Qualifications
    SET Name = @Name, CountryCode = @CountryCode, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Qualification_Delete
IF OBJECT_ID('dbo.Qualification_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_Delete;
GO
CREATE PROCEDURE dbo.Qualification_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Qualifications WHERE Id = @Id;
END
GO

-- Qualification_HasRelatedStreams
IF OBJECT_ID('dbo.Qualification_HasRelatedStreams', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_HasRelatedStreams;
GO
CREATE PROCEDURE dbo.Qualification_HasRelatedStreams
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.Streams
    WHERE QualificationId = @QualificationId;
END
GO



-- STREAM STORED PROCEDURES


-- Stream_GetAll
IF OBJECT_ID('dbo.Stream_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_GetAll;
GO
CREATE PROCEDURE dbo.Stream_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams;
END
GO

-- Stream_GetActive
IF OBJECT_ID('dbo.Stream_GetActive', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_GetActive;
GO
CREATE PROCEDURE dbo.Stream_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams
    WHERE IsActive = 1;
END
GO

-- Stream_GetById
IF OBJECT_ID('dbo.Stream_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_GetById;
GO
CREATE PROCEDURE dbo.Stream_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams
    WHERE Id = @Id;
END
GO

-- Stream_GetActiveByQualificationId
IF OBJECT_ID('dbo.Stream_GetActiveByQualificationId', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_GetActiveByQualificationId;
GO
CREATE PROCEDURE dbo.Stream_GetActiveByQualificationId
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams
    WHERE IsActive = 1 AND QualificationId = @QualificationId;
END
GO

-- Stream_Create
IF OBJECT_ID('dbo.Stream_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_Create;
GO
CREATE PROCEDURE dbo.Stream_Create
    @Name            NVARCHAR(200),
    @QualificationId INT,
    @IsActive        BIT,
    @CreatedAt       DATETIME2,
    @UpdatedAt       DATETIME2,
    @Id              INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Streams (Name, QualificationId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @QualificationId, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Stream_Update
IF OBJECT_ID('dbo.Stream_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_Update;
GO
CREATE PROCEDURE dbo.Stream_Update
    @Id              INT,
    @Name            NVARCHAR(200),
    @QualificationId INT,
    @IsActive        BIT,
    @UpdatedAt       DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Streams
    SET Name = @Name, QualificationId = @QualificationId, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Stream_Delete
IF OBJECT_ID('dbo.Stream_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_Delete;
GO
CREATE PROCEDURE dbo.Stream_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Streams WHERE Id = @Id;
END
GO

