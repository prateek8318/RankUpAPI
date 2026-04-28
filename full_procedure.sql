Changed database context to 'RankUp_QuestionDB'.
Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Question_AdminCreate]
    @ModuleId INT,
    @ExamId INT,
    @SubjectId INT,
    @TopicId INT = NULL,
    @Marks INT,
    @NegativeMarks DECIMAL(10,2),
    @Difficulty INT,
    @CorrectAnswer NVARCHAR(1),
    @SameExplanationForAl
lLanguages BIT,
    @IsPublished BIT,
    @TranslationsJson NVARCHAR(MAX),
    @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        IF COL_LENGTH('dbo.Subjects', 'ExamId') IS NOT NULL
        BEGIN
            DE
CLARE @SubjectMappedToExam BIT = 0;
            DECLARE @SubjectExamCheckSql NVARCHAR(MAX) = N'
                IF EXISTS (
                    SELECT 1
                    FROM dbo.Subjects s
                    WHERE s.Id = @SubjectIdIn
                
      AND s.ExamId = @ExamIdIn
                )
                    SELECT @IsMappedOut = 1;
                ELSE
                    SELECT @IsMappedOut = 0;';

            EXEC sp_executesql
                @SubjectExamCheckSql,
                N'@Subj
ectIdIn INT, @ExamIdIn INT, @IsMappedOut BIT OUTPUT',
                @SubjectIdIn = @SubjectId,
                @ExamIdIn = @ExamId,
                @IsMappedOut = @SubjectMappedToExam OUTPUT;

            IF @SubjectMappedToExam = 0
            BEGIN
  
              THROW 50020, 'Invalid mapping: SubjectId is not mapped to ExamId.', 1;
            END
        END
        ELSE IF OBJECT_ID('dbo.ExamSubjects', 'U') IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
            
    FROM dbo.ExamSubjects es
                WHERE es.SubjectId = @SubjectId
                  AND es.ExamId = @ExamId
                  AND ISNULL(es.IsActive, 1) = 1
            )
            BEGIN
                THROW 50020, 'Invalid mapping: SubjectI
d is not mapped to ExamId.', 1;
            END
        END
        ELSE IF NOT EXISTS (
            SELECT 1
            FROM dbo.Subjects s
            WHERE s.Id = @SubjectId
        )
        BEGIN
            THROW 50020, 'Invalid mapping: SubjectId 
is not mapped to ExamId.', 1;
        END

        -- Topic is optional for some modules. Treat NULL/0 as "not provided".
        IF @TopicId IS NOT NULL AND @TopicId <= 0
            SET @TopicId = NULL;

        IF @TopicId IS NOT NULL
        BEGIN
   
         IF NOT EXISTS (
                SELECT 1
                FROM dbo.Topics t
                WHERE t.Id = @TopicId
                  AND t.SubjectId = @SubjectId
                  AND t.ExamId = @ExamId
                  AND t.IsActive = 1
        
    )
            BEGIN
                THROW 50021, 'Invalid mapping: TopicId is not mapped to the given ExamId and SubjectId.', 1;
            END
        END

        DECLARE @QuestionText NVARCHAR(MAX) = NULL;
        SELECT @QuestionText = j.Question
Text
        FROM OPENJSON(@TranslationsJson)
        WITH (QuestionText NVARCHAR(MAX) '$.QuestionText') j;

        -- Insert into dbo.Questions with schema-drift tolerance:
        -- some DBs have QuestionText on Questions, some keep it only in transla
tions.
        DECLARE @CreatedByResolved INT = ISNULL(@CreatedBy, 0);

        IF COL_LENGTH('dbo.Questions', 'QuestionText') IS NOT NULL AND COL_LENGTH('dbo.Questions', 'CreatedBy') IS NOT NULL
        BEGIN
            INSERT INTO dbo.Questions
       
     (
                ModuleId, ExamId, SubjectId, TopicId, QuestionText, Marks, NegativeMarks, Difficulty,
                CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt, CreatedBy
            )
            
VALUES
            (
                @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), ISNULL(@QuestionText, N''), @Marks, @NegativeMarks, @Difficulty,
                @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublishe
d = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME(), @CreatedByResolved
            );
        END
        ELSE IF COL_LENGTH('dbo.Questions', 'QuestionText') IS NOT NULL AND COL_LENGTH('dbo.Questions', 'CreatedBy') IS NULL
        BEGIN
      
      INSERT INTO dbo.Questions
            (
                ModuleId, ExamId, SubjectId, TopicId, QuestionText, Marks, NegativeMarks, Difficulty,
                CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedA
t
            )
            VALUES
            (
                @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), ISNULL(@QuestionText, N''), @Marks, @NegativeMarks, @Difficulty,
                @CorrectAnswer, @SameExplanationForAllLanguages, @IsPubl
ished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME()
            );
        END
        ELSE
        BEGIN
            -- If QuestionText column doesn't exist, still try to set CreatedBy when present
            IF CO
L_LENGTH('dbo.Questions', 'CreatedBy') IS NOT NULL
            BEGIN
                INSERT INTO dbo.Questions
                (
                    ModuleId, ExamId, SubjectId, TopicId, Marks, NegativeMarks, Difficulty,
                    CorrectAnswer,
 SameExplanationForAllLanguages, IsPublished, PublishedAt, IsActive, CreatedAt, CreatedBy
                )
                VALUES
                (
                    @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), @Marks, @NegativeMarks, @Difficul
ty,
                    @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME(), @CreatedByResolved
                );
            END
            ELSE
            
BEGIN
                INSERT INTO dbo.Questions
                (
                    ModuleId, ExamId, SubjectId, TopicId, Marks, NegativeMarks, Difficulty,
                    CorrectAnswer, SameExplanationForAllLanguages, IsPublished, PublishedAt, IsAc
tive, CreatedAt
                )
                VALUES
                (
                    @ModuleId, @ExamId, @SubjectId, ISNULL(@TopicId, 0), @Marks, @NegativeMarks, @Difficulty,
                    @CorrectAnswer, @SameExplanationForAllLanguages, @
IsPublished, CASE WHEN @IsPublished = 1 THEN SYSUTCDATETIME() ELSE NULL END, 1, SYSUTCDATETIME()
                );
            END
        END

        DECLARE @QuestionId INT = CAST(SCOPE_IDENTITY() AS INT);

        INSERT INTO dbo.QuestionTranslations

        (
            QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD,
            Explanation, QuestionImageUrl, OptionAImageUrl, OptionBImageUrl, OptionCImageUrl, OptionDImageUrl, CreatedAt
        )
        SELECT
          
  @QuestionId,
            j.LanguageCode,
            j.QuestionText,
            j.OptionA,
            j.OptionB,
            j.OptionC,
            j.OptionD,
            j.Explanation,
            j.QuestionImageUrl,
            j.OptionAImageUrl,
  
          j.OptionBImageUrl,
            j.OptionCImageUrl,
            j.OptionDImageUrl,
            SYSUTCDATETIME()
        FROM OPENJSON(@TranslationsJson)
        WITH
        (
            LanguageCode NVARCHAR(10) '$.LanguageCode',
            Que
stionText NVARCHAR(MAX) '$.QuestionText',
            OptionA NVARCHAR(2000) '$.OptionA',
            OptionB NVARCHAR(2000) '$.OptionB',
            OptionC NVARCHAR(2000) '$.OptionC',
            OptionD NVARCHAR(2000) '$.OptionD',
            Explanati
on NVARCHAR(MAX) '$.Explanation',
            QuestionImageUrl NVARCHAR(500) '$.QuestionImageUrl',
            OptionAImageUrl NVARCHAR(500) '$.OptionAImageUrl',
            OptionBImageUrl NVARCHAR(500) '$.OptionBImageUrl',
            OptionCImageUrl NV
ARCHAR(500) '$.OptionCImageUrl',
            OptionDImageUrl NVARCHAR(500) '$.OptionDImageUrl'
        ) j;

        COMMIT TRANSACTION;
        SELECT @QuestionId AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
       
 THROW;
    END CATCH
END                                                                                                                                                                                                                                      
