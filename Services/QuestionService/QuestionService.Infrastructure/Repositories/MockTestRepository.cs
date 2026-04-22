using Dapper;
using Microsoft.Data.SqlClient;
using QuestionService.Application.DTOs;
using QuestionService.Application.Interfaces;
using System.Text.Json;

namespace QuestionService.Infrastructure.Repositories
{
    public class MockTestRepository : BaseDapperRepository, IMockTestRepository
    {
        public MockTestRepository(string connectionString) : base(connectionString)
        {
        }

        // Mock Test CRUD Operations
        public async Task<MockTestDto> CreateAsync(CreateMockTestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    INSERT INTO MockTests (
                        Name, Description, ExamId, DurationInMinutes, TotalQuestions, TotalMarks,
                        PassingMarks, SubscriptionPlanId, AccessType, AttemptsAllowed, CreatedBy, CreatedAt, IsActive
                    )
                    OUTPUT INSERTED.Id, INSERTED.CreatedAt
                    VALUES (
                        @Name, @Description, @ExamId, @DurationInMinutes, @TotalQuestions, @TotalMarks,
                        @PassingMarks, @SubscriptionPlanId, @AccessType, @AttemptsAllowed, @CreatedBy, GETDATE(), 1
                    )";

                var result = await connection.QuerySingleAsync<(int Id, DateTime CreatedAt)>(sql, dto);
                
                // Get the created mock test with full details
                return await GetByIdInternalAsync((SqlConnection)connection, result.Id);
            });
        }

        public async Task<MockTestDto?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                return await GetByIdInternalAsync((SqlConnection)connection, id);
            });
        }

        private async Task<MockTestDto> GetByIdInternalAsync(SqlConnection connection, int id)
        {
            var sql = @"
                SELECT 
                    mt.Id, mt.Name, mt.Description, mt.ExamId, mt.DurationInMinutes, 
                    mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
                    mt.AccessType, mt.AttemptsAllowed, mt.IsActive, mt.CreatedAt, mt.UpdatedAt, mt.CreatedBy,
                    e.Name AS ExamName, e.ExamType, e.SubjectId, e.HasNegativeMarking, e.NegativeMarkingValue,
                    s.Name AS SubjectName,
                    sp.Name AS SubscriptionPlanName, sp.Description AS SubscriptionPlanDescription,
                    sp.Price AS SubscriptionPlanPrice, sp.DurationInDays AS SubscriptionPlanDuration
                FROM MockTests mt
                LEFT JOIN Exams e ON mt.ExamId = e.Id
                LEFT JOIN Subjects s ON e.SubjectId = s.Id
                LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
                WHERE mt.Id = @Id";

            var mockTest = await connection.QuerySingleOrDefaultAsync<MockTestDto>(sql, new { Id = id });
            
            if (mockTest != null)
            {
                // Get questions for this mock test
                var questionsSql = @"
                    SELECT 
                        mtq.Id, mtq.MockTestId, mtq.QuestionId, mtq.QuestionNumber, mtq.Marks, mtq.NegativeMarks,
                        q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectAnswer,
                        q.Explanation, q.DifficultyLevel, q.QuestionType, q.QuestionImageUrl, q.OptionAImageUrl,
                        q.OptionBImageUrl, q.OptionCImageUrl, q.OptionDImageUrl, q.ExplanationImageUrl
                    FROM MockTestQuestions mtq
                    LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                    WHERE mtq.MockTestId = @MockTestId AND q.IsActive = 1
                    ORDER BY mtq.QuestionNumber";

                var questions = await connection.QueryAsync<MockTestQuestionDto, QuestionDto, MockTestQuestionDto>(
                    questionsSql,
                    (mockTestQuestion, question) =>
                    {
                        mockTestQuestion.Question = question;
                        return mockTestQuestion;
                    },
                    new { MockTestId = id },
                    splitOn: "QuestionId"
                );

                mockTest.Questions = questions.ToList();
            }

            return mockTest;
        }

        public async Task<MockTestDto> UpdateAsync(UpdateMockTestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE MockTests SET
                        Name = @Name,
                        Description = @Description,
                        DurationInMinutes = @DurationInMinutes,
                        TotalQuestions = @TotalQuestions,
                        TotalMarks = @TotalMarks,
                        PassingMarks = @PassingMarks,
                        SubscriptionPlanId = @SubscriptionPlanId,
                        AccessType = @AccessType,
                        AttemptsAllowed = @AttemptsAllowed,
                        IsActive = @IsActive,
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, dto);
                return await GetByIdInternalAsync((SqlConnection)connection, dto.Id);
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "UPDATE MockTests SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            });
        }

        public async Task<(List<MockTestDto> MockTests, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? examId = null, int? subjectId = null, bool? isActive = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mt.Id, mt.Name, mt.Description, mt.ExamId, mt.DurationInMinutes, 
                        mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
                        mt.AccessType, mt.AttemptsAllowed, mt.IsActive, mt.CreatedAt, mt.UpdatedAt, mt.CreatedBy,
                        e.Name AS ExamName, e.ExamType, e.SubjectId, e.HasNegativeMarking, e.NegativeMarkingValue,
                        s.Name AS SubjectName,
                        sp.Name AS SubscriptionPlanName
                    FROM MockTests mt
                    LEFT JOIN Exams e ON mt.ExamId = e.Id
                    LEFT JOIN Subjects s ON e.SubjectId = s.Id
                    LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
                    WHERE (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId)
                    AND (@IsActive IS NULL OR mt.IsActive = @IsActive)
                    ORDER BY mt.CreatedAt DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var countSql = @"
                    SELECT COUNT(*)
                    FROM MockTests mt
                    LEFT JOIN Exams e ON mt.ExamId = e.Id
                    WHERE (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId)
                    AND (@IsActive IS NULL OR mt.IsActive = @IsActive)";

                var parameters = new
                {
                    ExamId = examId,
                    SubjectId = subjectId,
                    IsActive = isActive,
                    Offset = (pageNumber - 1) * pageSize,
                    PageSize = pageSize
                };

                var mockTests = await connection.QueryAsync<MockTestDto>(sql, parameters);
                var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

                return (mockTests.ToList(), totalCount);
            });
        }

        // Question Management
        public async Task<bool> AddQuestionAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    INSERT INTO MockTestQuestions (MockTestId, QuestionId, QuestionNumber, Marks, NegativeMarks)
                    VALUES (@MockTestId, @QuestionId, @QuestionNumber, @Marks, @NegativeMarks)";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    MockTestId = mockTestId,
                    QuestionId = questionId,
                    QuestionNumber = questionNumber,
                    Marks = marks,
                    NegativeMarks = negativeMarks
                });

                return rowsAffected > 0;
            });
        }

        public async Task<bool> RemoveQuestionAsync(int mockTestId, int questionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "DELETE FROM MockTestQuestions WHERE MockTestId = @MockTestId AND QuestionId = @QuestionId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { MockTestId = mockTestId, QuestionId = questionId });
                return rowsAffected > 0;
            });
        }

        public async Task<bool> UpdateQuestionAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE MockTestQuestions SET
                        QuestionNumber = @QuestionNumber,
                        Marks = @Marks,
                        NegativeMarks = @NegativeMarks
                    WHERE MockTestId = @MockTestId AND QuestionId = @QuestionId";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    MockTestId = mockTestId,
                    QuestionId = questionId,
                    QuestionNumber = questionNumber,
                    Marks = marks,
                    NegativeMarks = negativeMarks
                });

                return rowsAffected > 0;
            });
        }

        public async Task<List<MockTestQuestionDto>> GetQuestionsAsync(int mockTestId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mtq.Id, mtq.MockTestId, mtq.QuestionId, mtq.QuestionNumber, mtq.Marks, mtq.NegativeMarks,
                        q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectAnswer,
                        q.Explanation, q.DifficultyLevel, q.QuestionType, q.QuestionImageUrl, q.OptionAImageUrl,
                        q.OptionBImageUrl, q.OptionCImageUrl, q.OptionDImageUrl, q.ExplanationImageUrl
                    FROM MockTestQuestions mtq
                    LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                    WHERE mtq.MockTestId = @MockTestId AND q.IsActive = 1
                    ORDER BY mtq.QuestionNumber";

                var questions = await connection.QueryAsync<MockTestQuestionDto, QuestionDto, MockTestQuestionDto>(
                    sql,
                    (mockTestQuestion, question) =>
                    {
                        mockTestQuestion.Question = question;
                        return mockTestQuestion;
                    },
                    new { MockTestId = mockTestId },
                    splitOn: "QuestionId"
                );

                return questions.ToList();
            });
        }

        // User Specific Operations
        public async Task<List<MockTestListDto>> GetForUserAsync(int userId, int pageNumber, int pageSize, int? examId = null, int? subjectId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mt.Id, mt.Name, mt.Description, mt.ExamId, mt.DurationInMinutes, 
                        mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
                        mt.AccessType, mt.AttemptsAllowed, mt.CreatedAt,
                        e.Name AS ExamName, e.ExamType, e.SubjectId, e.HasNegativeMarking, e.NegativeMarkingValue,
                        s.Name AS SubjectName,
                        sp.Name AS SubscriptionPlanName,
                        CASE 
                            WHEN mt.AccessType = 'Free' THEN 1
                            WHEN EXISTS (
                                SELECT 1 FROM UserSubscriptions us 
                                WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ExpiresAt > GETDATE()
                                AND (mt.SubscriptionPlanId IS NULL OR us.SubscriptionPlanId = mt.SubscriptionPlanId)
                            ) THEN 1
                            ELSE 0
                        END AS IsUnlocked,
                        ISNULL((SELECT COUNT(*) FROM MockTestAttempts mta WHERE mta.MockTestId = mt.Id AND mta.UserId = @UserId), 0) AS AttemptsUsed
                    FROM MockTests mt
                    LEFT JOIN Exams e ON mt.ExamId = e.Id
                    LEFT JOIN Subjects s ON e.SubjectId = s.Id
                    LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
                    WHERE mt.IsActive = 1
                    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId)
                    ORDER BY e.ExamType, mt.Name
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var parameters = new
                {
                    UserId = userId,
                    ExamId = examId,
                    SubjectId = subjectId,
                    Offset = (pageNumber - 1) * pageSize,
                    PageSize = pageSize
                };

                return (await connection.QueryAsync<MockTestListDto>(sql, parameters)).ToList();
            });
        }

        public async Task<MockTestDetailDto?> GetDetailForUserAsync(int userId, int mockTestId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mt.Id, mt.Name, mt.Description, mt.ExamId, mt.DurationInMinutes, 
                        mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, mt.SubscriptionPlanId,
                        mt.AccessType, mt.AttemptsAllowed, mt.IsActive, mt.CreatedAt, mt.UpdatedAt, mt.CreatedBy,
                        e.Name AS ExamName, e.ExamType, e.SubjectId, e.HasNegativeMarking, e.NegativeMarkingValue,
                        s.Name AS SubjectName,
                        sp.Name AS SubscriptionPlanName,
                        CASE 
                            WHEN mt.AccessType = 'Free' THEN 1
                            WHEN EXISTS (
                                SELECT 1 FROM UserSubscriptions us 
                                WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ExpiresAt > GETDATE()
                                AND (mt.SubscriptionPlanId IS NULL OR us.SubscriptionPlanId = mt.SubscriptionPlanId)
                            ) THEN 1
                            ELSE 0
                        END AS IsUnlocked,
                        ISNULL((SELECT COUNT(*) FROM MockTestAttempts mta WHERE mta.MockTestId = mt.Id AND mta.UserId = @UserId), 0) AS AttemptsUsed
                    FROM MockTests mt
                    LEFT JOIN Exams e ON mt.ExamId = e.Id
                    LEFT JOIN Subjects s ON e.SubjectId = s.Id
                    LEFT JOIN SubscriptionPlans sp ON mt.SubscriptionPlanId = sp.Id
                    WHERE mt.Id = @MockTestId AND mt.IsActive = 1";

                var mockTest = await connection.QuerySingleOrDefaultAsync<MockTestDetailDto>(sql, new { UserId = userId, MockTestId = mockTestId });
                
                if (mockTest != null)
                {
                    // Get attempts for this user
                    var attemptsSql = @"
                        SELECT 
                            Id, MockTestId, UserId, StartedAt, CompletedAt, Duration,
                            TotalQuestions, AnsweredQuestions, CorrectAnswers, WrongAnswers,
                            ObtainedMarks, Percentage, Status, Grade
                        FROM MockTestAttempts
                        WHERE MockTestId = @MockTestId AND UserId = @UserId
                        ORDER BY StartedAt DESC";

                    mockTest.Attempts = (await connection.QueryAsync<MockTestAttemptDto>(attemptsSql, new { UserId = userId, MockTestId = mockTestId })).ToList();
                }

                return mockTest;
            });
        }

        public async Task<UserSubscriptionDto?> GetUserSubscriptionAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        us.Id, us.UserId, us.SubscriptionPlanId, sp.Name AS PlanName,
                        us.StartedAt, us.ExpiresAt, us.IsActive
                    FROM UserSubscriptions us
                    LEFT JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
                    WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ExpiresAt > GETDATE()
                    ORDER BY us.StartedAt DESC";

                return await connection.QuerySingleOrDefaultAsync<UserSubscriptionDto>(sql, new { UserId = userId });
            });
        }

        // Session Management
        public async Task<MockTestSessionDto> CreateSessionAsync(StartMockTestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                // Get mock test details
                var mockTest = await GetByIdInternalAsync((SqlConnection)connection, dto.MockTestId);
                if (mockTest == null)
                {
                    throw new KeyNotFoundException($"Mock test with ID {dto.MockTestId} not found");
                }

                // Create session
                var sessionSql = @"
                    INSERT INTO MockTestSessions (MockTestId, UserId, StartedAt, Status, LanguageCode)
                    OUTPUT INSERTED.Id
                    VALUES (@MockTestId, @UserId, GETDATE(), 'InProgress', @LanguageCode)";

                var sessionId = await connection.QuerySingleAsync<int>(sessionSql, dto);

                // Get questions for the session
                var questions = await GetQuestionsAsync(dto.MockTestId);
                
                // Convert to QuizQuestionDto format
                var quizQuestions = questions.Select((q, index) => new QuizQuestionDto
                {
                    Id = q.QuestionId,
                    QuestionText = q.Question?.QuestionText ?? string.Empty,
                    OptionA = q.Question?.OptionA,
                    OptionB = q.Question?.OptionB,
                    OptionC = q.Question?.OptionC,
                    OptionD = q.Question?.OptionD,
                    QuestionImageUrl = q.Question?.QuestionImageUrl,
                    OptionAImageUrl = q.Question?.OptionAImageUrl,
                    OptionBImageUrl = q.Question?.OptionBImageUrl,
                    OptionCImageUrl = q.Question?.OptionCImageUrl,
                    OptionDImageUrl = q.Question?.OptionDImageUrl,
                    Marks = q.Marks,
                    NegativeMarks = q.NegativeMarks,
                    DifficultyLevel = q.Question?.DifficultyLevel ?? "Medium",
                    QuestionNumber = index + 1,
                    IsMarkedForReview = false,
                    IsAnswered = false
                }).ToList();

                return new MockTestSessionDto
                {
                    SessionId = sessionId,
                    MockTestId = dto.MockTestId,
                    MockTestName = mockTest.Name,
                    UserId = dto.UserId,
                    ExamName = mockTest.ExamName,
                    SubjectName = mockTest.SubjectName,
                    StartedAt = DateTime.UtcNow,
                    DurationInMinutes = mockTest.DurationInMinutes,
                    TotalQuestions = mockTest.TotalQuestions,
                    AnsweredQuestions = 0,
                    MarkedForReview = 0,
                    TotalMarks = mockTest.TotalMarks,
                    ObtainedMarks = 0,
                    HasNegativeMarking = mockTest.HasNegativeMarking,
                    NegativeMarksPerQuestion = mockTest.NegativeMarkingValue ?? 0,
                    Status = "InProgress",
                    Questions = quizQuestions
                };
            });
        }

        public async Task<MockTestSessionDto?> GetSessionAsync(int sessionId, int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mts.Id AS SessionId, mts.MockTestId, mts.UserId, mts.StartedAt, mts.CompletedAt,
                        mts.Status, mts.LanguageCode,
                        mt.Name AS MockTestName, mt.DurationInMinutes, mt.TotalQuestions, mt.TotalMarks,
                        e.Name AS ExamName, s.Name AS SubjectName, e.HasNegativeMarking, e.NegativeMarkingValue
                    FROM MockTestSessions mts
                    LEFT JOIN MockTests mt ON mts.MockTestId = mt.Id
                    LEFT JOIN Exams e ON mt.ExamId = e.Id
                    LEFT JOIN Subjects s ON e.SubjectId = s.Id
                    WHERE mts.Id = @SessionId AND mts.UserId = @UserId";

                var session = await connection.QuerySingleOrDefaultAsync<MockTestSessionDto>(sql, new { SessionId = sessionId, UserId = userId });
                
                if (session != null)
                {
                    // Get questions and answers for this session
                    var questionsSql = @"
                        SELECT 
                            mtq.QuestionId, mtq.QuestionNumber, mtq.Marks, mtq.NegativeMarks,
                            q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectAnswer,
                            q.Explanation, q.DifficultyLevel, q.QuestionType, q.QuestionImageUrl, q.OptionAImageUrl,
                            q.OptionBImageUrl, q.OptionCImageUrl, q.OptionDImageUrl, q.ExplanationImageUrl,
                            mtsa.SelectedAnswer, mtsa.IsMarkedForReview, mtsa.IsAnswered
                        FROM MockTestQuestions mtq
                        LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                        LEFT JOIN MockTestSessionAnswers mtsa ON mtq.QuestionId = mtsa.QuestionId AND mtsa.SessionId = @SessionId
                        WHERE mtq.MockTestId = @MockTestId
                        ORDER BY mtq.QuestionNumber";

                    var questions = await connection.QueryAsync<QuizQuestionDto, string?, bool?, bool?, QuizQuestionDto>(
                        questionsSql,
                        (question, selectedAnswer, isMarkedForReview, isAnswered) =>
                        {
                            question.SelectedAnswer = selectedAnswer;
                            question.IsMarkedForReview = isMarkedForReview ?? false;
                            question.IsAnswered = isAnswered ?? false;
                            return question;
                        },
                        new { SessionId = sessionId, MockTestId = session.MockTestId },
                        splitOn: "SelectedAnswer"
                    );

                    session.Questions = questions.ToList();
                    
                    // Calculate answered questions
                    session.AnsweredQuestions = questions.Count(q => q.IsAnswered);
                    session.MarkedForReview = questions.Count(q => q.IsMarkedForReview);
                }

                return session;
            });
        }

        public async Task<MockTestAttemptDto> SubmitSessionAsync(int sessionId, int userId, List<QuizAnswerRequestDto> answers)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var transaction = connection.BeginTransaction();
                
                try
                {
                    // Get session details
                    var sessionSql = @"
                        SELECT mts.MockTestId, mts.StartedAt, mt.DurationInMinutes, mt.TotalMarks, mt.PassingMarks,
                               e.HasNegativeMarking, e.NegativeMarkingValue
                        FROM MockTestSessions mts
                        LEFT JOIN MockTests mt ON mts.MockTestId = mt.Id
                        LEFT JOIN Exams e ON mt.ExamId = e.Id
                        WHERE mts.Id = @SessionId AND mts.UserId = @UserId";

                    var session = await connection.QuerySingleOrDefaultAsync(sessionSql, new { SessionId = sessionId, UserId = userId });
                    if (session == null)
                    {
                        throw new KeyNotFoundException("Session not found");
                    }

                    // Save answers
                    foreach (var answer in answers)
                    {
                        var answerSql = @"
                            IF EXISTS (SELECT 1 FROM MockTestSessionAnswers WHERE SessionId = @SessionId AND QuestionId = @QuestionId)
                                UPDATE MockTestSessionAnswers SET SelectedAnswer = @Answer, IsMarkedForReview = @MarkForReview, IsAnswered = 1
                            ELSE
                                INSERT INTO MockTestSessionAnswers (SessionId, QuestionId, SelectedAnswer, IsMarkedForReview, IsAnswered)
                                VALUES (@SessionId, @QuestionId, @Answer, @MarkForReview, 1)";

                        await connection.ExecuteAsync(answerSql, new
                        {
                            SessionId = sessionId,
                            QuestionId = answer.QuestionId,
                            Answer = answer.Answer,
                            MarkForReview = answer.MarkForReview
                        }, transaction);
                    }

                    // Calculate results
                    var resultSql = @"
                        SELECT 
                            mtq.QuestionId, mtq.Marks, mtq.NegativeMarks, q.CorrectAnswer,
                            mtsa.SelectedAnswer
                        FROM MockTestQuestions mtq
                        LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                        LEFT JOIN MockTestSessionAnswers mtsa ON mtq.QuestionId = mtsa.QuestionId AND mtsa.SessionId = @SessionId
                        WHERE mtq.MockTestId = @MockTestId";

                    var questionResults = await connection.QueryAsync(resultSql, new { SessionId = sessionId, MockTestId = session.MockTestId });

                    int correctAnswers = 0, wrongAnswers = 0;
                    decimal obtainedMarks = 0;

                    foreach (var qr in questionResults)
                    {
                        bool isCorrect = qr.SelectedAnswer == qr.CorrectAnswer;
                        if (isCorrect)
                        {
                            correctAnswers++;
                            obtainedMarks += qr.Marks;
                        }
                        else if (!string.IsNullOrEmpty(qr.SelectedAnswer))
                        {
                            wrongAnswers++;
                            if (session.HasNegativeMarking)
                                obtainedMarks -= session.NegativeMarkingValue ?? 0;
                        }
                    }

                    var percentage = session.TotalMarks > 0 ? (obtainedMarks / session.TotalMarks) * 100 : 0;
                    var grade = GetGrade(percentage);

                    // Update session
                    var updateSessionSql = @"
                        UPDATE MockTestSessions SET 
                            CompletedAt = GETDATE(), 
                            Status = 'Completed'
                        WHERE Id = @SessionId";

                    await connection.ExecuteAsync(updateSessionSql, new { SessionId = sessionId }, transaction);

                    // Create attempt record
                    var attemptSql = @"
                        INSERT INTO MockTestAttempts (
                            MockTestId, UserId, StartedAt, CompletedAt, Duration,
                            TotalQuestions, AnsweredQuestions, CorrectAnswers, WrongAnswers,
                            ObtainedMarks, Percentage, Status, Grade
                        )
                        VALUES (
                            @MockTestId, @UserId, @StartedAt, GETDATE(), 
                            DATEDIFF(MINUTE, @StartedAt, GETDATE()),
                            @TotalQuestions, @AnsweredQuestions, @CorrectAnswers, @WrongAnswers,
                            @ObtainedMarks, @Percentage, 'Completed', @Grade
                        )
                        OUTPUT INSERTED.Id, INSERTED.StartedAt, INSERTED.CompletedAt, INSERTED.Duration,
                               INSERTED.TotalQuestions, INSERTED.AnsweredQuestions, INSERTED.CorrectAnswers,
                               INSERTED.WrongAnswers, INSERTED.ObtainedMarks, INSERTED.Percentage,
                               INSERTED.Status, INSERTED.Grade";

                    var attempt = await connection.QuerySingleAsync<MockTestAttemptDto>(attemptSql, new
                    {
                        MockTestId = session.MockTestId,
                        UserId = userId,
                        StartedAt = session.StartedAt,
                        TotalQuestions = questionResults.Count(),
                        AnsweredQuestions = answers.Count,
                        CorrectAnswers = correctAnswers,
                        WrongAnswers = wrongAnswers,
                        ObtainedMarks = obtainedMarks,
                        Percentage = percentage,
                        Grade = grade
                    }, transaction);

                    transaction.Commit();
                    return attempt;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
        }

        // Statistics
        public async Task<object> GetStatisticsAsync(int? examId = null, int? subjectId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        COUNT(*) AS TotalMockTests,
                        SUM(CASE WHEN mt.AccessType = 'Free' THEN 1 ELSE 0 END) AS FreeMockTests,
                        SUM(CASE WHEN mt.AccessType = 'Paid' THEN 1 ELSE 0 END) AS PaidMockTests,
                        COUNT(DISTINCT mt.ExamId) AS ExamsCovered,
                        COUNT(DISTINCT e.SubjectId) AS SubjectsCovered
                    FROM MockTests mt
                    LEFT JOIN Exams e ON mt.ExamId = e.Id
                    WHERE mt.IsActive = 1
                    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR e.SubjectId = @SubjectId)";

                return await connection.QuerySingleAsync<object>(sql, new { ExamId = examId, SubjectId = subjectId });
            });
        }

        public async Task<List<object>> GetUserPerformanceAsync(int userId, int? examId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mt.Id AS MockTestId,
                        mt.Name AS MockTestName,
                        e.Name AS ExamName,
                        e.ExamType,
                        mta.StartedAt,
                        mta.Percentage,
                        mta.Grade,
                        mta.Status
                    FROM MockTestAttempts mta
                    LEFT JOIN MockTests mt ON mta.MockTestId = mt.Id
                    LEFT JOIN Exams e ON mt.ExamId = e.Id
                    WHERE mta.UserId = @UserId
                    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    ORDER BY mta.StartedAt DESC";

                var results = await connection.QueryAsync(sql, new { UserId = userId, ExamId = examId });
                return results.Select(r => (object)r).ToList();
            });
        }

        private static string GetGrade(decimal percentage)
        {
            return percentage switch
            {
                >= 90 => "A+",
                >= 80 => "A",
                >= 70 => "B+",
                >= 60 => "B",
                >= 50 => "C",
                >= 40 => "D",
                _ => "F"
            };
        }
    }
}
