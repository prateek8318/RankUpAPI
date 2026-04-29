-- Get Mock Test ID 12 details
SELECT 
    Id,
    Name as Title,
    Description,
    ExamId,
    DurationInMinutes as Duration,
    TotalQuestions,
    TotalMarks,
    PassingMarks,
    MarksPerQuestion,
    HasNegativeMarking,
    NegativeMarkingValue,
    AccessType,
    AttemptsAllowed,
    IsActive,
    Status,
    Difficulty,
    PaperCode,
    CreatedAt,
    UpdatedAt
FROM MockTests
WHERE Id = 12;

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
    Id,
    MockTestId,
    UserId,
    StartedAt,
    CompletedAt,
    Status,
    LanguageCode,
    CreatedAt
FROM MockTestSessions
WHERE MockTestId = 12
ORDER BY CreatedAt DESC;
