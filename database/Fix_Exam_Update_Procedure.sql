-- Fix for Exam_Update stored procedure to handle nested transactions
-- This script will modify the existing procedure to avoid nested transaction conflicts

-- First drop the existing procedure
DROP PROCEDURE [dbo].[Exam_Update];
GO

-- Create the updated procedure with transaction handling fix
CREATE PROCEDURE [dbo].[Exam_Update]
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
    
    -- Check if we're already in a transaction
    -- If @@TRANCOUNT > 0, we're in a nested transaction scenario
    DECLARE @IsInTransaction BIT = CASE WHEN @@TRANCOUNT > 0 THEN 1 ELSE 0 END;
    
    IF @IsInTransaction = 0
    BEGIN
        -- Only create transaction if we're not already in one
        BEGIN TRANSACTION;
        BEGIN TRY
            -- Update the main exam record
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

            -- Handle exam languages
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

            -- Handle exam qualifications
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
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            THROW;
        END CATCH
    END
    ELSE
    BEGIN
        -- We're already in a transaction, so just execute the operations
        -- without creating a new transaction
        BEGIN TRY
            -- Update the main exam record
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

            -- Handle exam languages
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

            -- Handle exam qualifications
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
        END TRY
        BEGIN CATCH
            -- Don't rollback here, let the outer transaction handle it
            -- Just re-throw the error
            THROW;
        END CATCH
    END
END
GO

-- Print confirmation
PRINT 'Exam_Update procedure has been updated to handle nested transactions correctly.';
GO
