-- =====================================================
-- SUBJECT LANGUAGE STORED PROCEDURES (OPTIMIZED)
-- =====================================================

-- ✅ OPTIMIZED: Get by Subject ID (No SELECT *, Specific Columns)
CREATE OR ALTER PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId]
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.SubjectId = @SubjectId AND sl.IsActive = 1
    ORDER BY l.Name;
END

-- ✅ OPTIMIZED: Get by ID (Specific Columns + JOIN)
CREATE OR ALTER PROCEDURE [dbo].[SubjectLanguage_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.Id = @Id AND sl.IsActive = 1;
END

-- ✅ OPTIMIZED: Create (OUTPUT instead of SCOPE_IDENTITY)
CREATE OR ALTER PROCEDURE [dbo].[SubjectLanguage_Create]
    @SubjectId INT,
    @LanguageId INT,
    @Name NVARCHAR(200),
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO SubjectLanguages (SubjectId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id
        VALUES (@SubjectId, @LanguageId, @Name, @IsActive, @CreatedAt, @UpdatedAt);
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END

-- ✅ OPTIMIZED: Update (TRY-CATCH + Error Handling)
CREATE OR ALTER PROCEDURE [dbo].[SubjectLanguage_Update]
    @Id INT,
    @SubjectId INT,
    @LanguageId INT,
    @Name NVARCHAR(200),
    @IsActive BIT = 1,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages 
        SET SubjectId = @SubjectId, LanguageId = @LanguageId, Name = @Name, 
            IsActive = @IsActive, UpdatedAt = @UpdatedAt
        WHERE Id = @Id;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END

-- ✅ OPTIMIZED: Delete (Soft Delete)
CREATE OR ALTER PROCEDURE [dbo].[SubjectLanguage_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages 
        SET IsActive = 0, UpdatedAt = GETUTCDATE()
        WHERE Id = @Id;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END

-- =====================================================
-- PERFORMANCE INDEXES FOR SUBJECT LANGUAGE
-- =====================================================

-- Create indexes for better performance
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SubjectLanguages_SubjectId')
BEGIN
    CREATE INDEX IX_SubjectLanguages_SubjectId ON SubjectLanguages(SubjectId);
    CREATE INDEX IX_SubjectLanguages_LanguageId ON SubjectLanguages(LanguageId);
    CREATE INDEX IX_SubjectLanguages_SubjectId_LanguageId ON SubjectLanguages(SubjectId, LanguageId);
END

-- =====================================================
-- USAGE EXAMPLES:
-- =====================================================
/*
-- Get all languages for a subject
EXEC [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId = 1;

-- Get specific subject language
EXEC [dbo].[SubjectLanguage_GetById] @Id = 1;

-- Create new subject language
EXEC [dbo].[SubjectLanguage_Create] 
    @SubjectId = 1, 
    @LanguageId = 2, 
    @Name = 'Mathematics',
    @CreatedAt = GETUTCDATE(),
    @UpdatedAt = GETUTCDATE();

-- Update subject language
EXEC [dbo].[SubjectLanguage_Update] 
    @Id = 1, 
    @SubjectId = 1, 
    @LanguageId = 2, 
    @Name = 'Advanced Mathematics',
    @UpdatedAt = GETUTCDATE();

-- Delete subject language
EXEC [dbo].[SubjectLanguage_Delete] @Id = 1;
*/
