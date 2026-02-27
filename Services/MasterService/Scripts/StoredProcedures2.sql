

USE [RankUp_MasterDB]
GO



-- Qualification_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Qualification_GetById]
GO
CREATE PROCEDURE [dbo].[Qualification_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Qualifications] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Qualification_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Qualification_GetAll]
GO
CREATE PROCEDURE [dbo].[Qualification_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Qualifications] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Qualification_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Qualification_GetActive]
GO
CREATE PROCEDURE [dbo].[Qualification_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Qualifications] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Qualification_GetByCountryCode
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_GetByCountryCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Qualification_GetByCountryCode]
GO
CREATE PROCEDURE [dbo].[Qualification_GetByCountryCode]
    @CountryCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Qualifications] 
    WHERE CountryCode = @CountryCode AND IsActive = 1
    ORDER BY Name;
END
GO

-- Qualification_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Qualification_Create]
GO
CREATE PROCEDURE [dbo].[Qualification_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @CountryCode NVARCHAR(10),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Qualifications] (Name, Description, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Qualification_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Qualification_Update]
GO
CREATE PROCEDURE [dbo].[Qualification_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @CountryCode NVARCHAR(10),
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Qualifications] 
    SET Name = @Name,
        Description = @Description,
        CountryCode = @CountryCode,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Qualification_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualification_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Qualification_Delete]
GO
CREATE PROCEDURE [dbo].[Qualification_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Qualifications] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

-- =====================================================
-- 5. STREAM STORED PROCEDURES
-- =====================================================

-- Stream_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_GetById]
GO
CREATE PROCEDURE [dbo].[Stream_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        QualificationId,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Streams] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Stream_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_GetAll]
GO
CREATE PROCEDURE [dbo].[Stream_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        QualificationId,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Streams] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Stream_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_GetActive]
GO
CREATE PROCEDURE [dbo].[Stream_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        QualificationId,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Streams] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Stream_GetByQualificationId
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_GetByQualificationId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_GetByQualificationId]
GO
CREATE PROCEDURE [dbo].[Stream_GetByQualificationId]
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        QualificationId,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Streams] 
    WHERE QualificationId = @QualificationId AND IsActive = 1
    ORDER BY Name;
END
GO

-- Stream_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_Create]
GO
CREATE PROCEDURE [dbo].[Stream_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @QualificationId INT,
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Streams] (Name, Description, QualificationId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @QualificationId, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Stream_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_Update]
GO
CREATE PROCEDURE [dbo].[Stream_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @QualificationId INT,
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Streams] 
    SET Name = @Name,
        Description = @Description,
        QualificationId = @QualificationId,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Stream_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stream_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Stream_Delete]
GO
CREATE PROCEDURE [dbo].[Stream_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Streams] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

-- =====================================================
-- 6. SUBJECT STORED PROCEDURES
-- =====================================================

-- Subject_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Subject_GetById]
GO
CREATE PROCEDURE [dbo].[Subject_GetById]
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
    FROM [dbo].[Subjects] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Subject_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Subject_GetAll]
GO
CREATE PROCEDURE [dbo].[Subject_GetAll]
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
    FROM [dbo].[Subjects] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Subject_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Subject_GetActive]
GO
CREATE PROCEDURE [dbo].[Subject_GetActive]
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
    FROM [dbo].[Subjects] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Subject_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Subject_Create]
GO
CREATE PROCEDURE [dbo].[Subject_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Subjects] (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Subject_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Subject_Update]
GO
CREATE PROCEDURE [dbo].[Subject_Update]
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Subjects] 
    SET Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO

-- Subject_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Subject_Delete]
GO
CREATE PROCEDURE [dbo].[Subject_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Subjects] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

-- =====================================================
-- 7. EXAM STORED PROCEDURES
-- =====================================================

-- Exam_GetById
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_GetById]
GO
CREATE PROCEDURE [dbo].[Exam_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        MinAge,
        MaxAge,
        ImageUrl,
        IsInternational,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Exams] 
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Exam_GetAll
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetAll]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_GetAll]
GO
CREATE PROCEDURE [dbo].[Exam_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        MinAge,
        MaxAge,
        ImageUrl,
        IsInternational,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Exams] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Exam_GetActive
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetActive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_GetActive]
GO
CREATE PROCEDURE [dbo].[Exam_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        MinAge,
        MaxAge,
        ImageUrl,
        IsInternational,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Exams] 
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- Exam_GetByCountryCode
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetByCountryCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_GetByCountryCode]
GO
CREATE PROCEDURE [dbo].[Exam_GetByCountryCode]
    @CountryCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        MinAge,
        MaxAge,
        ImageUrl,
        IsInternational,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Exams] 
    WHERE CountryCode = @CountryCode AND IsActive = 1
    ORDER BY Name;
END
GO

-- Exam_GetInternational
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_GetInternational]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_GetInternational]
GO
CREATE PROCEDURE [dbo].[Exam_GetInternational]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        CountryCode,
        MinAge,
        MaxAge,
        ImageUrl,
        IsInternational,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Exams] 
    WHERE IsInternational = 1 AND IsActive = 1
    ORDER BY Name;
END
GO

-- Exam_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_Create]
GO
CREATE PROCEDURE [dbo].[Exam_Create]
    @Name NVARCHAR(150),
    @Description NVARCHAR(1000),
    @CountryCode NVARCHAR(10),
    @MinAge INT,
    @MaxAge INT,
    @ImageUrl NVARCHAR(500),
    @IsInternational BIT = 0,
    @IsActive BIT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Exams] (Name, Description, CountryCode, MinAge, MaxAge, ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @CountryCode, @MinAge, @MaxAge, @ImageUrl, @IsInternational, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO

-- Exam_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_Update]
GO
CREATE PROCEDURE [dbo].[Exam_Update]
    @Id INT,
    @Name NVARCHAR(150),
    @Description NVARCHAR(1000),
    @CountryCode NVARCHAR(10),
    @MinAge INT,
    @MaxAge INT,
    @ImageUrl NVARCHAR(500),
    @IsInternational BIT,
    @IsActive BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Exams] 
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
END
GO

-- Exam_Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Exam_Delete]
GO
CREATE PROCEDURE [dbo].[Exam_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Exams] 
    SET IsActive = 0,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

PRINT 'MasterService Stored Procedures - Part 2 (Qualification, Stream, Subject, Exam) Completed Successfully!';
GO
