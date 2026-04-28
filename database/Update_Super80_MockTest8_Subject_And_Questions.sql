USE [RankUp_QuestionDB]
GO

/*
    Super 80 (MockTestId = 8) alignment script
    - Updates selected subject on mock test
    - Maps questions of same exam + selected subject into this mock test
    - Recomputes total question count and marks

    Default subject in this script: English (SubjectId = 7)
    If you want GK instead, change @TargetSubjectId accordingly.
*/

DECLARE @MockTestId INT = 8;
DECLARE @TargetSubjectId INT = 7; -- English
DECLARE @CurrentExamId INT;
DECLARE @MaxQuestionNumber INT;
DECLARE @TargetCount INT;

SELECT
    @CurrentExamId = mt.ExamId,
    @TargetCount = ISNULL(mt.TotalQuestions, 0)
FROM dbo.MockTests mt
WHERE mt.Id = @MockTestId;

IF @CurrentExamId IS NULL
BEGIN
    RAISERROR('Mock test not found for id = %d', 16, 1, @MockTestId);
    RETURN;
END

-- 1) Update subject on mock test
UPDATE dbo.MockTests
SET SubjectId = @TargetSubjectId,
    UpdatedAt = GETDATE()
WHERE Id = @MockTestId;

-- 2) Fill mock test with matching questions (exam + subject), avoiding duplicates
SELECT @MaxQuestionNumber = ISNULL(MAX(mtq.QuestionNumber), 0)
FROM dbo.MockTestQuestions mtq
WHERE mtq.MockTestId = @MockTestId;

;WITH ExistingCount AS
(
    SELECT COUNT(1) AS Cnt
    FROM dbo.MockTestQuestions
    WHERE MockTestId = @MockTestId
),
Candidates AS
(
    SELECT
        q.Id AS QuestionId,
        q.Marks,
        q.NegativeMarks,
        ROW_NUMBER() OVER (ORDER BY q.CreatedAt DESC, q.Id DESC) AS rn
    FROM dbo.Questions q
    WHERE q.IsActive = 1
      AND q.ExamId = @CurrentExamId
      AND q.SubjectId = @TargetSubjectId
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.MockTestQuestions mtq
          WHERE mtq.MockTestId = @MockTestId
            AND mtq.QuestionId = q.Id
      )
)
INSERT INTO dbo.MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
SELECT
    @MockTestId,
    c.QuestionId,
    @MaxQuestionNumber + c.rn,
    ISNULL(NULLIF(c.Marks, 0), 1.00),
    ISNULL(c.NegativeMarks, 0.00)
FROM Candidates c
CROSS JOIN ExistingCount ec
WHERE c.rn <= CASE
                WHEN @TargetCount > 0 AND @TargetCount > ec.Cnt THEN @TargetCount - ec.Cnt
                ELSE 1000 -- If target not set, load all matching candidates
              END;

-- 3) Recompute totals from actual mapped questions
UPDATE mt
SET
    mt.TotalQuestions = agg.QuestionCount,
    mt.TotalMarks = agg.TotalMarks,
    mt.UpdatedAt = GETDATE()
FROM dbo.MockTests mt
INNER JOIN
(
    SELECT
        mtq.MockTestId,
        COUNT(1) AS QuestionCount,
        CAST(ISNULL(SUM(mtq.Marks), 0.00) AS DECIMAL(10,2)) AS TotalMarks
    FROM dbo.MockTestQuestions mtq
    WHERE mtq.MockTestId = @MockTestId
    GROUP BY mtq.MockTestId
) agg
    ON agg.MockTestId = mt.Id
WHERE mt.Id = @MockTestId;

-- Output validation
SELECT
    mt.Id,
    mt.Name,
    mt.ExamId,
    mt.SubjectId,
    mt.TotalQuestions,
    mt.TotalMarks,
    mt.UpdatedAt
FROM dbo.MockTests mt
WHERE mt.Id = @MockTestId;

SELECT
    mtq.MockTestId,
    mtq.QuestionId,
    mtq.QuestionNumber,
    mtq.Marks,
    mtq.NegativeMarks,
    q.SubjectId,
    q.ExamId,
    q.QuestionText
FROM dbo.MockTestQuestions mtq
INNER JOIN dbo.Questions q ON q.Id = mtq.QuestionId
WHERE mtq.MockTestId = @MockTestId
ORDER BY mtq.QuestionNumber;
GO
