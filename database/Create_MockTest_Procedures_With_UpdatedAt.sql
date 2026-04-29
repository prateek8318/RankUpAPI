-- =============================================
-- Create MockTest procedures with UpdatedAt support
-- =============================================

-- 1. Create procedure for updating MockTests with UpdatedAt support
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
GO

-- 2. Create procedure for creating MockTests with UpdatedAt support
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
GO

PRINT 'MockTest procedures created successfully with UpdatedAt support!';
PRINT 'Created procedures:';
PRINT '1. MockTest_UpdateWithUpdatedAt - For updating MockTests with automatic UpdatedAt';
PRINT '2. MockTest_CreateWithUpdatedAt - For creating MockTests with UpdatedAt = CreatedAt';
