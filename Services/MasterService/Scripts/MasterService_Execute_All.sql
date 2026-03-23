USE [RankUp_MasterDB];
GO

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
    SELECT Id, Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt
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
    @Code        NVARCHAR(10),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.States (Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt)
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
END
GO

-- State_Update
IF OBJECT_ID('dbo.State_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.State_Update;
GO
CREATE PROCEDURE dbo.State_Update
    @Id          INT,
    @Name        NVARCHAR(100),
    @Code        NVARCHAR(10),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.States
    SET Name = @Name, Code = @Code, CountryCode = @CountryCode, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    -- Replace language rows when provided
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
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
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
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
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
    SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
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
    @Description     NVARCHAR(1000) = NULL,
    @QualificationId INT,
    @IsActive        BIT = 1,
    @NamesJson       NVARCHAR(MAX) = NULL,
    @CreatedAt       DATETIME2,
    @UpdatedAt       DATETIME2,
    @Id              INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Streams (Name, Description, QualificationId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @QualificationId, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();

    IF @NamesJson IS NOT NULL AND @Id > 0
    BEGIN
        INSERT INTO dbo.StreamLanguages (StreamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StreamId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @CreatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
END
GO

-- Stream_Update
IF OBJECT_ID('dbo.Stream_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_Update;
GO
CREATE PROCEDURE dbo.Stream_Update
    @Id              INT,
    @Name            NVARCHAR(200),
    @Description     NVARCHAR(1000) = NULL,
    @QualificationId INT,
    @IsActive        BIT,
    @NamesJson       NVARCHAR(MAX) = NULL,
    @UpdatedAt       DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Streams
    SET Name = @Name, Description = @Description, QualificationId = @QualificationId, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    IF @NamesJson IS NOT NULL
    BEGIN
        DELETE FROM dbo.StreamLanguages WHERE StreamId = @Id;

        INSERT INTO dbo.StreamLanguages (StreamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StreamId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @UpdatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
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

-- Stream_GetActiveLocalized
IF OBJECT_ID('dbo.Stream_GetActiveLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_GetActiveLocalized;
GO
CREATE PROCEDURE dbo.Stream_GetActiveLocalized
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1 AND NameHi IS NOT NULL AND NameHi != '';
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1;
    END
END
GO

-- Stream_GetByIdLocalized
IF OBJECT_ID('dbo.Stream_GetByIdLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_GetByIdLocalized;
GO
CREATE PROCEDURE dbo.Stream_GetByIdLocalized
    @Id INT,
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE Id = @Id AND IsActive = 1 AND NameHi IS NOT NULL AND NameHi != '';
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE Id = @Id AND IsActive = 1;
    END
END
GO

-- Stream_GetActiveByQualificationIdLocalized
IF OBJECT_ID('dbo.Stream_GetActiveByQualificationIdLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Stream_GetActiveByQualificationIdLocalized;
GO
CREATE PROCEDURE dbo.Stream_GetActiveByQualificationIdLocalized
    @QualificationId INT,
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1 AND QualificationId = @QualificationId AND NameHi IS NOT NULL AND NameHi != '';
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1 AND QualificationId = @QualificationId;
    END
END
GO

-- Exam_GetAll
IF OBJECT_ID('dbo.Exam_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetAll;
GO
CREATE PROCEDURE dbo.Exam_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    ORDER BY Name;
END
GO

-- Exam_GetActive
IF OBJECT_ID('dbo.Exam_GetActive', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetActive;
GO
CREATE PROCEDURE dbo.Exam_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Exam_GetById
IF OBJECT_ID('dbo.Exam_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetById;
GO
CREATE PROCEDURE dbo.Exam_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    WHERE Id = @Id;
END
GO

-- Exam_GetByFilter
IF OBJECT_ID('dbo.Exam_GetByFilter', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_GetByFilter;
GO
CREATE PROCEDURE dbo.Exam_GetByFilter
    @CountryCode     NVARCHAR(10) = NULL,
    @QualificationId INT = NULL,
    @StreamId        INT = NULL,
    @MinAge          INT = NULL,
    @MaxAge          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT
        e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
        e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY e.Name;
END
GO

-- Exam_Create
IF OBJECT_ID('dbo.Exam_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_Create;
GO
CREATE PROCEDURE dbo.Exam_Create
    @Name        NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10) = NULL,
    @MinAge      INT = NULL,
    @MaxAge      INT = NULL,
    @ImageUrl    NVARCHAR(500) = NULL,
    @IsInternational BIT = 0,
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @RelationsJson NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.Exams (Name, Description, CountryCode, MinAge, MaxAge, ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt)
        VALUES (@Name, @Description, @CountryCode, @MinAge, @MaxAge, @ImageUrl, @IsInternational, @IsActive, @CreatedAt, @UpdatedAt);
        SET @Id = SCOPE_IDENTITY();

        IF @NamesJson IS NOT NULL AND @Id > 0
        BEGIN
            INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                LanguageId,
                Name,
                Description,
                1 as IsActive,
                @CreatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@NamesJson) 
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(150) '$.Name',
                Description NVARCHAR(1000) '$.Description'
            );
        END

        IF @RelationsJson IS NOT NULL AND @Id > 0
        BEGIN
            INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                QualificationId,
                StreamId,
                1 as IsActive,
                @CreatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@RelationsJson) 
            WITH (
                QualificationId INT '$.QualificationId',
                StreamId INT '$.StreamId'
            );
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Exam_Update
IF OBJECT_ID('dbo.Exam_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_Update;
GO
CREATE PROCEDURE dbo.Exam_Update
    @Id          INT,
    @Name        NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10) = NULL,
    @MinAge      INT = NULL,
    @MaxAge      INT = NULL,
    @ImageUrl    NVARCHAR(500) = NULL,
    @IsInternational BIT = 0,
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @RelationsJson NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.Exams
        SET Name = @Name,
            Description = @Description,
            CountryCode = @CountryCode,
            MinAge = @MinAge,
            MaxAge = @MaxAge,
            ImageUrl = @ImageUrl,
            IsInternational = @IsInternational,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id;

        DELETE FROM dbo.ExamLanguages WHERE ExamId = @Id;
        IF @NamesJson IS NOT NULL
        BEGIN
            INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                LanguageId,
                Name,
                Description,
                1 as IsActive,
                @UpdatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@NamesJson) 
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(150) '$.Name',
                Description NVARCHAR(1000) '$.Description'
            );
        END

        DELETE FROM dbo.ExamQualifications WHERE ExamId = @Id;
        IF @RelationsJson IS NOT NULL
        BEGIN
            INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                QualificationId,
                StreamId,
                1 as IsActive,
                @UpdatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@RelationsJson) 
            WITH (
                QualificationId INT '$.QualificationId',
                StreamId INT '$.StreamId'
            );
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Exam_Delete
IF OBJECT_ID('dbo.Exam_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.Exam_Delete;
GO
CREATE PROCEDURE dbo.Exam_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Exams WHERE Id = @Id;
END
GO

-- =====================================================
-- SUBJECT STORED PROCEDURES
-- =====================================================

-- Subject_GetAll
IF OBJECT_ID('dbo.Subject_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.Subject_GetAll;
GO
CREATE PROCEDURE dbo.Subject_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Subjects
    ORDER BY Name;
END
GO

-- Subject_GetById
IF OBJECT_ID('dbo.Subject_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.Subject_GetById;
GO
CREATE PROCEDURE dbo.Subject_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Subjects
    WHERE Id = @Id;
END
GO

-- Subject_GetActive
IF OBJECT_ID('dbo.Subject_GetActive', 'P') IS NOT NULL DROP PROCEDURE dbo.Subject_GetActive;
GO
CREATE PROCEDURE dbo.Subject_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Subjects
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Subject_Create
IF OBJECT_ID('dbo.Subject_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.Subject_Create;
GO
CREATE PROCEDURE dbo.Subject_Create
    @Name        NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @IsActive    BIT = 1,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Subjects (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Subject_Update
IF OBJECT_ID('dbo.Subject_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.Subject_Update;
GO
CREATE PROCEDURE dbo.Subject_Update
    @Id          INT,
    @Name        NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @IsActive    BIT,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Subjects
    SET Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Subject_Delete
IF OBJECT_ID('dbo.Subject_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.Subject_Delete;
GO
CREATE PROCEDURE dbo.Subject_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Subjects WHERE Id = @Id;
END
GO

-- Subject_Exists
IF OBJECT_ID('dbo.Subject_Exists', 'P') IS NOT NULL DROP PROCEDURE dbo.Subject_Exists;
GO
CREATE PROCEDURE dbo.Subject_Exists
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.Subjects
    WHERE Id = @Id;
END
GO

-- Qualification_GetActiveLocalized
IF OBJECT_ID('dbo.Qualification_GetActiveLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_GetActiveLocalized;
GO
CREATE PROCEDURE dbo.Qualification_GetActiveLocalized
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive
        FROM dbo.Qualifications
        WHERE IsActive = 1 AND NameHi IS NOT NULL AND NameHi != ''
        ORDER BY NameHi;
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive
        FROM dbo.Qualifications
        WHERE IsActive = 1
        ORDER BY Name;
    END
END
GO

-- Qualification_GetActiveByCountryCodeLocalized
IF OBJECT_ID('dbo.Qualification_GetActiveByCountryCodeLocalized', 'P') IS NOT NULL DROP PROCEDURE dbo.Qualification_GetActiveByCountryCodeLocalized;
GO
CREATE PROCEDURE dbo.Qualification_GetActiveByCountryCodeLocalized
    @CountryCode NVARCHAR(10),
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive
        FROM dbo.Qualifications
        WHERE IsActive = 1 AND CountryCode = @CountryCode AND NameHi IS NOT NULL AND NameHi != ''
        ORDER BY NameHi;
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive
        FROM dbo.Qualifications
        WHERE IsActive = 1 AND CountryCode = @CountryCode
        ORDER BY Name;
    END
END
GO