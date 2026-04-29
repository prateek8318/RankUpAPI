-- Get Mock Test ID 12 details from QuestionDB
SELECT 
    mt.Id,
    mt.Title,
    mt.Description,
    mt.ExamId,
    mt.SubjectId,
    mt.TotalQuestions,
    mt.Duration,
    mt.MarkPerQuestion,
    mt.NegativeMarking,
    mt.IsFree,
    mt.IsActive,
    mt.CreatedAt,
    mt.UpdatedAt
FROM MockTests mt
WHERE mt.Id = 12;

-- Get questions for Mock Test ID 12
SELECT 
    mtq.MockTestId,
    mtq.QuestionId,
    q.QuestionText,
    q.OptionA,
    q.OptionB,
    q.OptionC,
    q.OptionD,
    q.CorrectAnswer,
    q.Explanation,
    q.QuestionType,
    q.Difficulty as DifficultyLevel,
    q.TopicId,
    t.Name as TopicName
FROM MockTestQuestions mtq
JOIN Questions q ON mtq.QuestionId = q.Id
LEFT JOIN Topics t ON q.TopicId = t.Id
WHERE mtq.MockTestId = 12
ORDER BY mtq.Id;

-- Get mock test sessions
SELECT 
    mts.Id,
    mts.MockTestId,
    mts.UserId,
    mts.StartedAt,
    mts.CompletedAt,
    mts.Status,
    mts.LanguageCode,
    mts.CreatedAt
FROM MockTestSessions mts
WHERE mts.MockTestId = 12
ORDER BY mts.CreatedAt DESC;
