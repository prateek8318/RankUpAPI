CREATE PROCEDURE Test_GetByIdWithQuestions
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.Id,
        t.ExamId,
        t.PracticeModeId,
        t.SeriesId,
        t.SubjectId,
        t.Year,
        t.Name,
        t.Description,
        t.DurationMinutes,
        t.TotalQuestions,
        t.MaxMarks,
        t.DisplayOrder,
        t.IsActive,
        t.CreatedAt,
        t.UpdatedAt,
        e.Id AS Exam_Id,
        e.Name AS Exam_Name,
        e.DisplayOrder AS Exam_DisplayOrder,
        e.IsActive AS Exam_IsActive,
        pm.Id AS PracticeMode_Id,
        pm.Name AS PracticeMode_Name,
        pm.DisplayOrder AS PracticeMode_DisplayOrder,
        pm.IsActive AS PracticeMode_IsActive,
        s.Id AS Series_Id,
        s.Name AS Series_Name,
        s.DisplayOrder AS Series_DisplayOrder,
        s.IsActive AS Series_IsActive,
        sub.Id AS Subject_Id,
        sub.Name AS Subject_Name,
        sub.DisplayOrder AS Subject_DisplayOrder,
        sub.IsActive AS Subject_IsActive
    FROM Tests t
    LEFT JOIN Exams e ON t.ExamId = e.Id
    LEFT JOIN PracticeModes pm ON t.PracticeModeId = pm.Id
    LEFT JOIN TestSeries s ON t.SeriesId = s.Id
    LEFT JOIN Subjects sub ON t.SubjectId = sub.Id
    WHERE t.Id = @Id;
    
    SELECT 
        tq.Id,
        tq.TestId,
        tq.QuestionId,
        tq.DisplayOrder,
        q.Id AS Question_Id,
        q.QuestionText,
        q.QuestionType,
        q.DifficultyLevel,
        q.SubjectId,
        q.Explanation,
        q.IsActive,
        q.CreatedAt,
        q.UpdatedAt
    FROM TestQuestions tq
    LEFT JOIN Questions q ON tq.QuestionId = q.Id
    WHERE tq.TestId = @Id
    ORDER BY tq.DisplayOrder;
END
