-- =====================================================
-- FIX FOR QUOTED_IDENTIFIER ERROR IN EXAM_UPDATE
-- =====================================================

-- Drop existing procedure
IF OBJECT_ID('dbo.Exam_Update', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.Exam_Update;
GO

-- Set QUOTED_IDENTIFIER ON at COMPILE TIME (this is the key fix!)
SET QUOTED_IDENTIFIER ON;
GO

-- Create procedure with proper QUOTED_IDENTIFIER setting
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
        -- Update exam
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
        
        -- Delete existing exam languages for this exam
        DELETE FROM dbo.ExamLanguages WHERE ExamId = @Id;
        
        -- Insert new exam languages if NamesJson is provided
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
        
        -- Delete existing exam qualifications for this exam
        DELETE FROM dbo.ExamQualifications WHERE ExamId = @Id;
        
        -- Insert new exam qualifications if RelationsJson is provided
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
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Verify procedure was created with correct QUOTED_IDENTIFIER setting
SELECT 
    name,
    uses_quoted_identifier,
    uses_ansi_nulls,
    create_date
FROM sys.procedures 
WHERE name = 'Exam_Update';
GO

PRINT 'Exam_Update procedure recreated with QUOTED_IDENTIFIER ON at compile time!';
PRINT 'This should fix the Error 1934: QUOTED_IDENTIFIER issue.';
