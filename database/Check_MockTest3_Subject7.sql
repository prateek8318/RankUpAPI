-- Check Mock Test ID 3 and Subject ID 7 details
-- =============================================

-- Check Mock Test 3 details
SELECT 
    mt.Id, mt.Name, mt.Description, mt.ExamId, mt.DurationInMinutes, 
    mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.AccessType,
    e.Name AS ExamName, e.SubjectId,
    s.Name AS SubjectName
FROM MockTests mt
LEFT JOIN Exams e ON mt.ExamId = e.Id
LEFT JOIN Subjects s ON e.SubjectId = s.Id
WHERE mt.Id = 3;

-- Check existing questions in Mock Test 3
SELECT 
    mtq.MockTestId, mtq.QuestionId, mtq.QuestionNumber, mtq.Marks,
    q.QuestionText, q.SubjectId, q.ExamId,
    s.Name AS SubjectName,
    qt.LanguageCode, qt.QuestionText AS TranslatedQuestionText
FROM MockTestQuestions mtq
LEFT JOIN Questions q ON mtq.QuestionId = q.Id
LEFT JOIN Subjects s ON q.SubjectId = s.Id
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE mtq.MockTestId = 3
ORDER BY mtq.QuestionNumber;

-- Check Subject ID 7 details
SELECT 
    s.Id, s.Name, s.Description, s.IsActive,
    e.Name AS ExamName, e.ExamType
FROM Subjects s
LEFT JOIN Exams e ON s.Id = e.SubjectId
WHERE s.Id = 7;

-- Check existing questions for Subject ID 7
SELECT 
    q.Id, q.QuestionText, q.SubjectId, q.ExamId, q.IsPublished,
    s.Name AS SubjectName,
    qt.LanguageCode, qt.QuestionText AS TranslatedQuestionText,
    COUNT(qt.QuestionId) OVER (PARTITION BY q.Id) AS TranslationCount
FROM Questions q
LEFT JOIN Subjects s ON q.SubjectId = s.Id
LEFT JOIN QuestionTranslations qt ON q.Id = qt.QuestionId
WHERE q.SubjectId = 7
ORDER BY q.Id;

-- Check if there are any questions for Subject 7 that can be added to Mock Test 3
SELECT 
    q.Id, q.QuestionText, q.SubjectId, q.ExamId, q.IsPublished,
    s.Name AS SubjectName,
    CASE WHEN mtq.QuestionId IS NULL THEN 'Available' ELSE 'Already in Mock Test' END AS Status
FROM Questions q
LEFT JOIN Subjects s ON q.SubjectId = s.Id
LEFT JOIN MockTestQuestions mtq ON q.Id = mtq.QuestionId AND mtq.MockTestId = 3
WHERE q.SubjectId = 7 AND q.IsPublished = 1
ORDER BY q.Id;
