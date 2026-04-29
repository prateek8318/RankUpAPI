-- Get Mock Test ID 12 details directly from database
SELECT 
    mt.Id,
    mt.Title,
    mt.Description,
    mt.ExamId,
    e.ExamName,
    mt.SubjectId,
    s.SubjectName,
    mt.TotalQuestions,
    mt.Duration,
    mt.MarkPerQuestion,
    mt.NegativeMarking,
    mt.IsFree,
    mt.IsActive,
    mt.CreatedAt,
    mt.UpdatedAt
FROM MockTests mt
JOIN Exams e ON mt.ExamId = e.Id
JOIN Subjects s ON mt.SubjectId = s.Id
WHERE mt.Id = 12;

-- Get questions for Mock Test ID 12
SELECT 
    q.Id,
    q.QuestionText,
    q.OptionA,
    q.OptionB,
    q.OptionC,
    q.OptionD,
    q.CorrectAnswer,
    q.Explanation,
    q.QuestionType,
    q.DifficultyLevel,
    q.TopicId,
    t.TopicName
FROM Questions q
LEFT JOIN Topics t ON q.TopicId = t.Id
WHERE q.MockTestId = 12
ORDER BY q.Id;

-- Get mock test attempts/sessions
SELECT 
    mts.Id,
    mts.UserId,
    mts.StartTime,
    mts.EndTime,
    mts.TotalQuestions,
    mts.AnsweredQuestions,
    mts.CorrectAnswers,
    mts.WrongAnswers,
    mts.TotalMarks,
    mts.Status,
    mts.CreatedAt
FROM MockTestSessions mts
WHERE mts.MockTestId = 12
ORDER BY mts.CreatedAt DESC;
