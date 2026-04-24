USE [RankUp_MasterDB];
GO

-- =====================================================
-- EXAM SUBJECT RELATIONSHIP TABLE AND PROCEDURES
-- =====================================================

-- Create ExamSubjects table
IF OBJECT_ID('dbo.ExamSubjects', 'U') IS NOT NULL DROP TABLE dbo.ExamSubjects;
GO
CREATE TABLE dbo.ExamSubjects (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ExamId INT NOT NULL,
    SubjectId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_ExamSubjects_Exams FOREIGN KEY (ExamId) REFERENCES dbo.Exams(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ExamSubjects_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_ExamSubjects_ExamSubject UNIQUE (ExamId, SubjectId)
);
GO

-- Create index for performance
CREATE INDEX IX_ExamSubjects_ExamId ON dbo.ExamSubjects(ExamId);
CREATE INDEX IX_ExamSubjects_SubjectId ON dbo.ExamSubjects(SubjectId);
GO

-- =====================================================
-- EXAM SUBJECT STORED PROCEDURES
-- =====================================================

-- ExamSubject_ReplaceMappings
IF OBJECT_ID('dbo.ExamSubject_ReplaceMappings', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamSubject_ReplaceMappings;
GO
CREATE PROCEDURE dbo.ExamSubject_ReplaceMappings
    @ExamId INT,
    @SubjectIds NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Delete existing mappings for this exam
        DELETE FROM dbo.ExamSubjects WHERE ExamId = @ExamId;
        
        -- Insert new mappings if SubjectIds is provided
        IF @SubjectIds IS NOT NULL AND LEN(@SubjectIds) > 0
        BEGIN
            INSERT INTO dbo.ExamSubjects (ExamId, SubjectId, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @ExamId as ExamId,
                value as SubjectId,
                1 as IsActive,
                GETUTCDATE() as CreatedAt,
                GETUTCDATE() as UpdatedAt
            FROM STRING_SPLIT(@SubjectIds, ',')
            WHERE TRIM(value) != '' AND TRY_CAST(TRIM(value) AS INT) IS NOT NULL;
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ExamSubject_GetByExamId
IF OBJECT_ID('dbo.ExamSubject_GetByExamId', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamSubject_GetByExamId;
GO
CREATE PROCEDURE dbo.ExamSubject_GetByExamId
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT es.Id, es.ExamId, es.SubjectId, es.IsActive, es.CreatedAt, es.UpdatedAt,
           s.Name as SubjectName, s.Description as SubjectDescription
    FROM dbo.ExamSubjects es
    INNER JOIN dbo.Subjects s ON es.SubjectId = s.Id
    WHERE es.ExamId = @ExamId AND es.IsActive = 1;
END
GO

-- ExamSubject_GetBySubjectId
IF OBJECT_ID('dbo.ExamSubject_GetBySubjectId', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamSubject_GetBySubjectId;
GO
CREATE PROCEDURE dbo.ExamSubject_GetBySubjectId
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT es.Id, es.ExamId, es.SubjectId, es.IsActive, es.CreatedAt, es.UpdatedAt,
           e.Name as ExamName, e.Description as ExamDescription
    FROM dbo.ExamSubjects es
    INNER JOIN dbo.Exams e ON es.ExamId = e.Id
    WHERE es.SubjectId = @SubjectId AND es.IsActive = 1;
END
GO

-- ExamSubject_Create
IF OBJECT_ID('dbo.ExamSubject_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamSubject_Create;
GO
CREATE PROCEDURE dbo.ExamSubject_Create
    @ExamId INT,
    @SubjectId INT,
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ExamSubjects (ExamId, SubjectId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ExamId, @SubjectId, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO

-- ExamSubject_Delete
IF OBJECT_ID('dbo.ExamSubject_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamSubject_Delete;
GO
CREATE PROCEDURE dbo.ExamSubject_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ExamSubjects WHERE Id = @Id;
END
GO

-- ExamSubject_SoftDeleteByExamId
IF OBJECT_ID('dbo.ExamSubject_SoftDeleteByExamId', 'P') IS NOT NULL DROP PROCEDURE dbo.ExamSubject_SoftDeleteByExamId;
GO
CREATE PROCEDURE dbo.ExamSubject_SoftDeleteByExamId
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.ExamSubjects
    SET IsActive = 0, UpdatedAt = GETUTCDATE()
    WHERE ExamId = @ExamId;
END
GO

PRINT 'ExamSubject table and procedures created successfully!';
GO
