USE [RankUp_MasterDB];
GO

IF OBJECT_ID('dbo.ExamSubjects', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExamSubjects
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ExamId INT NOT NULL,
        SubjectId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX UX_ExamSubjects_ExamId_SubjectId
        ON dbo.ExamSubjects(ExamId, SubjectId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_ExamSubjects_Exams'
)
BEGIN
    ALTER TABLE dbo.ExamSubjects
    ADD CONSTRAINT FK_ExamSubjects_Exams
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_ExamSubjects_Subjects'
)
BEGIN
    ALTER TABLE dbo.ExamSubjects
    ADD CONSTRAINT FK_ExamSubjects_Subjects
    FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(Id);
END
GO

IF OBJECT_ID('dbo.ExamSubject_ReplaceMappings', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ExamSubject_ReplaceMappings;
GO
CREATE PROCEDURE dbo.ExamSubject_ReplaceMappings
    @ExamId INT,
    @SubjectIds NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DELETE FROM dbo.ExamSubjects WHERE ExamId = @ExamId;

        IF @SubjectIds IS NOT NULL AND LTRIM(RTRIM(@SubjectIds)) <> ''
        BEGIN
            DECLARE @RequestedSubjects TABLE (SubjectId INT PRIMARY KEY);
            INSERT INTO @RequestedSubjects (SubjectId)
            SELECT DISTINCT TRY_CAST(value AS INT)
            FROM STRING_SPLIT(@SubjectIds, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;

            IF EXISTS (
                SELECT 1
                FROM @RequestedSubjects rs
                LEFT JOIN dbo.Subjects s ON s.Id = rs.SubjectId
                WHERE s.Id IS NULL
            )
            BEGIN
                THROW 51001, 'One or more SubjectIds are invalid for exam mapping.', 1;
            END

            INSERT INTO dbo.ExamSubjects (ExamId, SubjectId, IsActive, CreatedAt, UpdatedAt)
            SELECT
                @ExamId,
                rs.SubjectId,
                1,
                GETUTCDATE(),
                GETUTCDATE()
            FROM @RequestedSubjects rs;
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

IF OBJECT_ID('dbo.ExamSubject_GetByExamIds', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ExamSubject_GetByExamIds;
GO
CREATE PROCEDURE dbo.ExamSubject_GetByExamIds
    @ExamIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        es.ExamId,
        es.SubjectId
    FROM dbo.ExamSubjects es
    INNER JOIN STRING_SPLIT(@ExamIds, ',') ids ON es.ExamId = TRY_CAST(ids.value AS INT)
    WHERE es.IsActive = 1
    ORDER BY es.ExamId, es.SubjectId;
END
GO
