-- =============================================
-- Update MockTest Stored Procedures to support UpdatedAt field
-- =============================================

-- 1. Update Question_CreateForMockTest procedure to handle UpdatedAt in Questions table
-- This procedure creates questions and should set UpdatedAt = CreatedAt for new questions

-- Drop existing procedure if it exists
DROP PROCEDURE IF EXISTS [dbo].[Question_CreateForMockTest];

-- Recreate procedure with UpdatedAt support
CREATE PROCEDURE [dbo].[Question_CreateForMockTest]
    @MockTestId INT,
    @QuestionText NVARCHAR(MAX),
    @OptionA NVARCHAR(500),
    @OptionB NVARCHAR(500),
    @OptionC NVARCHAR(500),
    @OptionD NVARCHAR(500),
    @CorrectAnswer CHAR(1),
    @Explanation NVARCHAR(MAX),
    @Marks DECIMAL(5,2),
    @NegativeMarks DECIMAL(5,2),
    @DifficultyLevel NVARCHAR(20),
    @QuestionType NVARCHAR(20),
    @QuestionImageUrl NVARCHAR(500),
    @OptionAImageUrl NVARCHAR(500),
    @OptionBImageUrl NVARCHAR(500),
    @OptionCImageUrl NVARCHAR(500),
    @OptionDImageUrl NVARCHAR(500),
    @ExplanationImageUrl NVARCHAR(500),
    @SameExplanationForAllLanguages BIT,
    @Reference NVARCHAR(500),
    @Tags NVARCHAR(MAX),
    @CreatedBy INT,
    @TranslationsJson NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TransactionCount INT = @@TRANCOUNT;
    DECLARE @QuestionId INT;
    DECLARE @ModuleId INT;
    DECLARE @ExamId INT;
    DECLARE @SubjectId INT;
    DECLARE @TopicId INT;

    -- Only start transaction if not already in one
    IF @TransactionCount = 0
        BEGIN TRANSACTION;
    ELSE
        SAVE TRANSACTION QuestionCreateForMockTestProc;

    BEGIN TRY
        -- Get MockTest details to extract context
        SELECT 
            @ModuleId = MockTestType,
            @ExamId = ExamId,
            @SubjectId = SubjectId,
            @TopicId = TopicId
        FROM MockTests
        WHERE Id = @MockTestId AND IsActive = 1;

        -- Validate MockTest exists
        IF @ModuleId IS NULL
        BEGIN
            RAISERROR('MockTest with ID %d does not exist or is not active', 16, 1, @MockTestId);
            RETURN -1;
        END

        -- Validate Subject exists
        IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = @SubjectId AND IsActive = 1)
        BEGIN
            RAISERROR('Subject with ID %d does not exist or is not active', 16, 1, @SubjectId);
            RETURN -1;
        END

        -- Validate Exam exists (if MasterDB is accessible)
        IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'RankUp_MasterDB')
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM [RankUp_MasterDB].[dbo].[Exams] WHERE Id = @ExamId AND IsActive = 1)
            BEGIN
                RAISERROR('Exam with ID %d does not exist or is not active', 16, 1, @ExamId);
                RETURN -1;
            END
        END

        -- Insert the question with MockTest context and UpdatedAt support
        INSERT INTO Questions (
            ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD,
            CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType,
            QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl,
            ExplanationImageUrl, SameExplanationForAllLanguages, Reference, Tags, CreatedBy, CreatedAt, UpdatedAt
        )
        VALUES (
            @ModuleId, @ExamId, @SubjectId, @TopicId, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD,
            @CorrectAnswer, @Explanation, @Marks, @NegativeMarks, @DifficultyLevel, @QuestionType,
            @QuestionImageUrl, @OptionAImageUrl, @OptionBImageUrl, @OptionCImageUrl, @OptionDImageUrl,
            @ExplanationImageUrl, @SameExplanationForAllLanguages, @Reference, @Tags, @CreatedBy, GETUTCDATE(), GETUTCDATE()
        );

        SET @QuestionId = SCOPE_IDENTITY();

        -- Insert translations if provided
        IF @TranslationsJson IS NOT NULL AND LEN(@TranslationsJson) > 0
        BEGIN
            DECLARE @Translations TABLE (
                LanguageCode NVARCHAR(10),
                QuestionText NVARCHAR(MAX),
                OptionA NVARCHAR(500),
                OptionB NVARCHAR(500),
                OptionC NVARCHAR(500),
                OptionD NVARCHAR(500),
                Explanation NVARCHAR(MAX)
            );

            INSERT INTO @Translations (LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation)
            SELECT
                JSON_VALUE(value, '$.LanguageCode'),
                JSON_VALUE(value, '$.QuestionText'),
                JSON_VALUE(value, '$.OptionA'),
                JSON_VALUE(value, '$.OptionB'),
                JSON_VALUE(value, '$.OptionC'),
                JSON_VALUE(value, '$.OptionD'),
                JSON_VALUE(value, '$.Explanation')
            FROM OPENJSON(@TranslationsJson);

            INSERT INTO QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt, UpdatedAt)      
            SELECT
                @QuestionId,
                LanguageCode,
                QuestionText,
                OptionA,
                OptionB,
                OptionC,
                OptionD,
                Explanation,
                GETUTCDATE(),
                GETUTCDATE()
            FROM @Translations
            WHERE LanguageCode != 'en'; -- Don't duplicate English if it's the primary
        END

        -- Automatically add question to MockTest
        DECLARE @QuestionNumber INT = (
            SELECT ISNULL(MAX(QuestionNumber), 0) + 1
            FROM MockTestQuestions
            WHERE MockTestId = @MockTestId
        );

        INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
        VALUES (@MockTestId, @QuestionId, @QuestionNumber, @Marks, @NegativeMarks);

        -- Commit only if we started the transaction
        IF @TransactionCount = 0
            COMMIT TRANSACTION;

        SELECT @QuestionId AS QuestionId, @QuestionNumber AS QuestionNumber;
    END TRY

    BEGIN CATCH
        -- Rollback only if we started the transaction
        IF @TransactionCount = 0
            ROLLBACK TRANSACTION;
        ELSE
            ROLLBACK TRANSACTION QuestionCreateForMockTestProc;

        -- Re-throw the error
        THROW;
    END CATCH
END

-- 2. Create a new procedure for updating MockTests with UpdatedAt support
CREATE OR ALTER PROCEDURE [dbo].[MockTest_UpdateWithUpdatedAt]
    @Id INT,
    @Name NVARCHAR(200) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @DurationInMinutes INT = NULL,
    @TotalQuestions INT = NULL,
    @TotalMarks DECIMAL(10,2) = NULL,
    @PassingMarks DECIMAL(10,2) = NULL,
    @MarksPerQuestion DECIMAL(10,2) = NULL,
    @HasNegativeMarking BIT = NULL,
    @NegativeMarkingValue DECIMAL(10,2) = NULL,
    @SubscriptionPlanId INT = NULL,
    @AccessType NVARCHAR(20) = NULL,
    @AttemptsAllowed INT = NULL,
    @Status NVARCHAR(20) = NULL,
    @Year INT = NULL,
    @Difficulty NVARCHAR(20) = NULL,
    @PaperCode NVARCHAR(50) = NULL,
    @ExamDate DATETIME2 = NULL,
    @PublishDateTime DATETIME2 = NULL,
    @ValidTill DATETIME2 = NULL,
    @ShowResultType NVARCHAR(20) = NULL,
    @ImageUrl NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionCount INT = @@TRANCOUNT;
    
    -- Only start transaction if not already in one
    IF @TransactionCount = 0
        BEGIN TRANSACTION;
    
    BEGIN TRY
        UPDATE MockTests SET
            Name = COALESCE(@Name, Name),
            Description = COALESCE(@Description, Description),
            DurationInMinutes = COALESCE(@DurationInMinutes, DurationInMinutes),
            TotalQuestions = COALESCE(@TotalQuestions, TotalQuestions),
            TotalMarks = COALESCE(@TotalMarks, TotalMarks),
            PassingMarks = COALESCE(@PassingMarks, PassingMarks),
            MarksPerQuestion = COALESCE(@MarksPerQuestion, MarksPerQuestion),
            HasNegativeMarking = COALESCE(@HasNegativeMarking, HasNegativeMarking),
            NegativeMarkingValue = COALESCE(@NegativeMarkingValue, NegativeMarkingValue),
            SubscriptionPlanId = COALESCE(@SubscriptionPlanId, SubscriptionPlanId),
            AccessType = COALESCE(@AccessType, AccessType),
            AttemptsAllowed = COALESCE(@AttemptsAllowed, AttemptsAllowed),
            Status = COALESCE(@Status, Status),
            [Year] = COALESCE(@Year, [Year]),
            Difficulty = COALESCE(@Difficulty, Difficulty),
            PaperCode = COALESCE(@PaperCode, PaperCode),
            ExamDate = COALESCE(@ExamDate, ExamDate),
            PublishDateTime = COALESCE(@PublishDateTime, PublishDateTime),
            ValidTill = COALESCE(@ValidTill, ValidTill),
            ShowResultType = COALESCE(@ShowResultType, ShowResultType),
            ImageUrl = COALESCE(@ImageUrl, ImageUrl),
            UpdatedAt = GETDATE()
        WHERE Id = @Id;
        
        -- Commit only if we started the transaction
        IF @TransactionCount = 0
            COMMIT TRANSACTION;
            
        SELECT @@ROWCOUNT AS RowsAffected;
    END TRY
    BEGIN CATCH
        -- Rollback only if we started the transaction
        IF @TransactionCount = 0
            ROLLBACK TRANSACTION;
            
        -- Re-throw the error
        THROW;
    END CATCH
END

-- 3. Create a procedure for creating MockTests with UpdatedAt support
CREATE OR ALTER PROCEDURE [dbo].[MockTest_CreateWithUpdatedAt]
    @Name NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @MockTestType INT = 1,
    @ExamId INT,
    @SubjectId INT = NULL,
    @TopicId INT = NULL,
    @DurationInMinutes INT = 30,
    @TotalQuestions INT = 20,
    @TotalMarks DECIMAL(10,2) = 100.00,
    @PassingMarks DECIMAL(10,2) = 35.00,
    @MarksPerQuestion DECIMAL(10,2) = 5.00,
    @HasNegativeMarking BIT = 0,
    @NegativeMarkingValue DECIMAL(10,2) = 0.00,
    @SubscriptionPlanId INT = NULL,
    @AccessType NVARCHAR(20) = 'Free',
    @AttemptsAllowed INT = 1,
    @Status NVARCHAR(20) = 'Active',
    @Year INT = NULL,
    @Difficulty NVARCHAR(20) = NULL,
    @PaperCode NVARCHAR(50) = NULL,
    @ExamDate DATETIME2 = NULL,
    @PublishDateTime DATETIME2 = NULL,
    @ValidTill DATETIME2 = NULL,
    @ShowResultType NVARCHAR(20) = '1',
    @ImageUrl NVARCHAR(500) = NULL,
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TransactionCount INT = @@TRANCOUNT;
    DECLARE @NewId INT;
    
    -- Only start transaction if not already in one
    IF @TransactionCount = 0
        BEGIN TRANSACTION;
    
    BEGIN TRY
        INSERT INTO MockTests (
            Name, Description, MockTestType, ExamId, SubjectId, TopicId, DurationInMinutes,
            TotalQuestions, TotalMarks, PassingMarks, MarksPerQuestion, HasNegativeMarking,
            NegativeMarkingValue, SubscriptionPlanId, AccessType, AttemptsAllowed, Status,
            Year, Difficulty, PaperCode, ExamDate, PublishDateTime, ValidTill,
            ShowResultType, ImageUrl, CreatedBy, CreatedAt, UpdatedAt
        )
        VALUES (
            @Name, @Description, @MockTestType, @ExamId, @SubjectId, @TopicId, @DurationInMinutes,
            @TotalQuestions, @TotalMarks, @PassingMarks, @MarksPerQuestion, @HasNegativeMarking,
            @NegativeMarkingValue, @SubscriptionPlanId, @AccessType, @AttemptsAllowed, @Status,
            @Year, @Difficulty, @PaperCode, @ExamDate, @PublishDateTime, @ValidTill,
            @ShowResultType, @ImageUrl, @CreatedBy, GETDATE(), GETDATE()
        );
        
        SET @NewId = SCOPE_IDENTITY();
        
        -- Commit only if we started the transaction
        IF @TransactionCount = 0
            COMMIT TRANSACTION;
            
        SELECT @NewId AS MockTestId;
    END TRY
    BEGIN CATCH
        -- Rollback only if we started the transaction
        IF @TransactionCount = 0
            ROLLBACK TRANSACTION;
            
        -- Re-throw the error
        THROW;
    END CATCH
END

PRINT 'MockTest Stored Procedures updated successfully with UpdatedAt support!';
PRINT 'Updated procedures:';
PRINT '1. Question_CreateForMockTest - Now handles UpdatedAt for Questions and QuestionTranslations';
PRINT '2. MockTest_UpdateWithUpdatedAt - New procedure for updating MockTests with UpdatedAt';
PRINT '3. MockTest_CreateWithUpdatedAt - New procedure for creating MockTests with UpdatedAt';
