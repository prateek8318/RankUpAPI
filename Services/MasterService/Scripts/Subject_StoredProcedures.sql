USE [RankUp_MasterDB]
GO

PRINT 'Creating Subject Stored Procedures...';
PRINT '====================================================';

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
        s.Id,
        s.Name,
        s.Description,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Subjects] s
    WHERE s.Id = @Id;
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
        s.Id,
        s.Name,
        s.Description,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Subjects] s
    ORDER BY s.Name;
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
        s.Id,
        s.Name,
        s.Description,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Subjects] s
    WHERE s.IsActive = 1
    ORDER BY s.Name;
END
GO

-- Subject_Create
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Create]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Subject_Create]
GO

CREATE PROCEDURE [dbo].[Subject_Create]
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check for duplicate name
    IF EXISTS (SELECT 1 FROM Subjects WHERE Name = @Name AND IsActive = 1)
    BEGIN
        RAISERROR('Subject with this name already exists', 16, 1);
        RETURN -1;
    END
    
    INSERT INTO Subjects (Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
    RETURN 0;
END
GO

-- Subject_Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Subject_Update]
GO

CREATE PROCEDURE [dbo].[Subject_Update]
    @Id INT,
    @Name NVARCHAR(100) = NULL,
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT = NULL,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if subject exists
    IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = @Id)
    BEGIN
        RAISERROR('Subject not found', 16, 1);
        RETURN -1;
    END
    
    -- Check for duplicate name (if name is being updated)
    IF @Name IS NOT NULL AND EXISTS (SELECT 1 FROM Subjects WHERE Name = @Name AND Id != @Id AND IsActive = 1)
    BEGIN
        RAISERROR('Subject with this name already exists', 16, 1);
        RETURN -1;
    END
    
    UPDATE Subjects
    SET 
        Name = ISNULL(@Name, Name),
        Description = ISNULL(@Description, Description),
        IsActive = ISNULL(@IsActive, IsActive),
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
    
    RETURN 0;
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
    
    -- Check if subject exists
    IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = @Id)
    BEGIN
        RAISERROR('Subject not found', 16, 1);
        RETURN -1;
    END
    
    -- Check if subject has dependent records (e.g., in SubjectLanguages)
    IF EXISTS (SELECT 1 FROM SubjectLanguages WHERE SubjectId = @Id)
    BEGIN
        -- Soft delete - set IsActive to 0 and delete dependent records
        DELETE FROM SubjectLanguages WHERE SubjectId = @Id;
        UPDATE Subjects
        SET 
            IsActive = 0,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @Id;
    END
    ELSE
    BEGIN
        -- Hard delete if no dependent records
        DELETE FROM Subjects WHERE Id = @Id;
    END
    
    RETURN 0;
END
GO

-- Subject_ToggleStatus
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_ToggleStatus]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Subject_ToggleStatus]
GO

CREATE PROCEDURE [dbo].[Subject_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if subject exists
    IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = @Id)
    BEGIN
        RAISERROR('Subject not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE Subjects
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO

-- Subject_Exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject_Exists]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[Subject_Exists]
GO

CREATE PROCEDURE [dbo].[Subject_Exists]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT CASE WHEN EXISTS (SELECT 1 FROM Subjects WHERE Id = @Id AND IsActive = 1) 
           THEN CAST(1 AS INT) ELSE CAST(0 AS INT) END AS [Exists];
END
GO

PRINT '====================================================';
PRINT 'SUBJECT STORED PROCEDURES CREATED SUCCESSFULLY!';
PRINT 'Total Procedures: 8';
PRINT '====================================================';
GO
