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

        private sealed class MockTestQuestionFlatRow
        {
            public int MockTestQuestionId { get; set; }
            public int MockTestId { get; set; }
            public int QuestionId { get; set; }
            public int QuestionNumber { get; set; }
            public decimal Marks { get; set; }
            public decimal NegativeMarks { get; set; }

            public string? QuestionText { get; set; }
            public string? OptionA { get; set; }
            public string? OptionB { get; set; }
            public string? OptionC { get; set; }
            public string? OptionD { get; set; }
            public string? CorrectAnswer { get; set; }
            public string? Explanation { get; set; }
            public string? DifficultyLevel { get; set; }
            public string? QuestionType { get; set; }
            public string? QuestionImageUrl { get; set; }
            public string? OptionAImageUrl { get; set; }
            public string? OptionBImageUrl { get; set; }
            public string? OptionCImageUrl { get; set; }
            public string? OptionDImageUrl { get; set; }
            public string? ExplanationImageUrl { get; set; }
        }

        // Mock Test CRUD Operations
        public async Task<MockTestDto> CreateAsync(CreateMockTestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    INSERT INTO MockTests (
                        Name, Description, MockTestType, ExamId, SubjectId, TopicId, DurationInMinutes, 
                        TotalQuestions, TotalMarks, PassingMarks, MarksPerQuestion, HasNegativeMarking, 
                        NegativeMarkingValue, SubscriptionPlanId, AccessType, AttemptsAllowed, Status,
                        Year, Difficulty, PaperCode, ExamDate, PublishDateTime, ValidTill, 
                        ShowResultType, ImageUrl, CreatedBy, CreatedAt
                    )
                    OUTPUT INSERTED.Id, INSERTED.CreatedAt
                    VALUES (
                        @Name, @Description, @MockTestType, @ExamId, @SubjectId, @TopicId, @DurationInMinutes, 
                        @TotalQuestions, @TotalMarks, @PassingMarks, @MarksPerQuestion, @HasNegativeMarking, 
                        @NegativeMarkingValue, @SubscriptionPlanId, @AccessType, @AttemptsAllowed, @Status,
                        @Year, @Difficulty, @PaperCode, @ExamDate, @PublishDateTime, @ValidTill, 
                        @ShowResultType, @ImageUrl, @CreatedBy, GETDATE()
                    )";

                var parameters = new
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    MockTestType = (int)dto.MockTestType,
                    ExamId = dto.ExamId,
                    SubjectId = dto.SubjectId,
                    TopicId = dto.TopicId,
                    DurationInMinutes = dto.DurationInMinutes,
                    TotalQuestions = dto.TotalQuestions,
                    TotalMarks = dto.TotalMarks,
                    PassingMarks = dto.PassingMarks,
                    MarksPerQuestion = dto.MarksPerQuestion,
                    HasNegativeMarking = dto.HasNegativeMarking,
                    NegativeMarkingValue = dto.NegativeMarkingValue,
                    SubscriptionPlanId = dto.SubscriptionPlanId,
                    AccessType = dto.AccessType,
                    AttemptsAllowed = dto.AttemptsAllowed,
                    Status = dto.Status.ToString(),
                    Year = dto.Year,
                    Difficulty = dto.Difficulty,
                    PaperCode = dto.PaperCode,
                    ExamDate = dto.ExamDate,
                    PublishDateTime = dto.PublishDateTime,
                    ValidTill = dto.ValidTill,
                    ShowResultType = ((int)dto.ShowResultType).ToString(),
                    ImageUrl = dto.ImageUrl,
                    CreatedBy = dto.CreatedBy
                };

                var result = await connection.QuerySingleAsync<(int Id, DateTime CreatedAt)>(sql, parameters);
                
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
                    mt.Id, mt.Name, mt.Description, ISNULL(mt.MockTestType, 1) AS MockTestType, mt.ExamId, mt.SubjectId, CAST(NULL AS INT) AS TopicId,
                    mt.DurationInMinutes, mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, 
                    ISNULL(mt.MarksPerQuestion, 0) AS MarksPerQuestion, ISNULL(mt.HasNegativeMarking, 0) AS HasNegativeMarking, mt.NegativeMarkingValue,
                    mt.SubscriptionPlanId, mt.AccessType, mt.AttemptsAllowed, ISNULL(mt.Status, 'Active') AS Status,
                    mt.CreatedAt, mt.UpdatedAt, mt.CreatedBy,
                    mt.[Year], mt.Difficulty, mt.PaperCode, mt.ExamDate, mt.PublishDateTime, 
                    mt.ValidTill, ISNULL(mt.ShowResultType, '1') AS ShowResultType, mt.ImageUrl,
                    e.Name AS ExamName,
                    CAST('' AS NVARCHAR(100)) AS ExamType,
                    mt.SubjectId AS SubjectId,
                    s.Name AS SubjectName,
                    CAST(NULL AS NVARCHAR(200)) AS SubscriptionPlanName, CAST(NULL AS NVARCHAR(MAX)) AS SubscriptionPlanDescription,
                    CAST(NULL AS DECIMAL(10,2)) AS SubscriptionPlanPrice, CAST(NULL AS INT) AS SubscriptionPlanDuration
                FROM MockTests mt
                LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
                LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
                WHERE mt.Id = @Id";

            var mockTest = await connection.QuerySingleOrDefaultAsync<MockTestDto>(sql, new { Id = id });
            
            if (mockTest != null)
            {
                // Convert string values to enums
                if (Enum.TryParse<MockTestType>(mockTest.MockTestType.ToString(), out var mockTestType))
                    mockTest.MockTestType = mockTestType;
                    
                // Status is now stored as string directly, no enum conversion needed
                    
                if (int.TryParse(mockTest.ShowResultType.ToString(), out var showResultTypeInt) && 
                    Enum.TryParse<ShowResultType>(showResultTypeInt.ToString(), out var showResultType))
                    mockTest.ShowResultType = showResultType;

                // Get questions for this mock test
                var questionsSql = @"
                    SELECT 
                        mtq.Id AS MockTestQuestionId,
                        mtq.MockTestId,
                        mtq.QuestionId,
                        mtq.QuestionNumber,
                        mtq.Marks,
                        mtq.NegativeMarks,
                        q.QuestionText,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectAnswer,
                        q.Explanation,
                        q.DifficultyLevel,
                        q.QuestionType,
                        q.QuestionImageUrl,
                        q.OptionAImageUrl,
                        q.OptionBImageUrl,
                        q.OptionCImageUrl,
                        q.OptionDImageUrl,
                        q.ExplanationImageUrl
                    FROM MockTestQuestions mtq
                    LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                    WHERE mtq.MockTestId = @MockTestId AND q.IsActive = 1
                    ORDER BY mtq.QuestionNumber";

                var rows = (await connection.QueryAsync<MockTestQuestionFlatRow>(questionsSql, new { MockTestId = id })).ToList();

                mockTest.Questions = rows.Select(r => new MockTestQuestionDto
                {
                    Id = r.MockTestQuestionId,
                    MockTestId = r.MockTestId,
                    QuestionId = r.QuestionId,
                    QuestionNumber = r.QuestionNumber,
                    Marks = r.Marks,
                    NegativeMarks = r.NegativeMarks,
                    Question = new QuestionDto
                    {
                        Id = r.QuestionId,
                        QuestionText = r.QuestionText ?? string.Empty,
                        OptionA = r.OptionA,
                        OptionB = r.OptionB,
                        OptionC = r.OptionC,
                        OptionD = r.OptionD,
                        CorrectAnswer = r.CorrectAnswer ?? string.Empty,
                        Explanation = r.Explanation,
                        DifficultyLevel = r.DifficultyLevel ?? "Medium",
                        QuestionType = r.QuestionType ?? "MCQ",
                        QuestionImageUrl = r.QuestionImageUrl,
                        OptionAImageUrl = r.OptionAImageUrl,
                        OptionBImageUrl = r.OptionBImageUrl,
                        OptionCImageUrl = r.OptionCImageUrl,
                        OptionDImageUrl = r.OptionDImageUrl,
                        ExplanationImageUrl = r.ExplanationImageUrl
                    }
                }).ToList();
            }

            return mockTest;
        }

        public async Task<MockTestDto> UpdateAsync(UpdateMockTestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
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
                        ShowResultType = COALESCE(CAST(@ShowResultType AS NVARCHAR(20)), ShowResultType),
                        ImageUrl = COALESCE(@ImageUrl, ImageUrl),
                        UpdatedAt = GETDATE()
                    WHERE Id = @Id";

                var parameters = new
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    DurationInMinutes = dto.DurationInMinutes,
                    TotalQuestions = dto.TotalQuestions,
                    TotalMarks = dto.TotalMarks,
                    PassingMarks = dto.PassingMarks,
                    MarksPerQuestion = dto.MarksPerQuestion,
                    HasNegativeMarking = dto.HasNegativeMarking,
                    NegativeMarkingValue = dto.NegativeMarkingValue,
                    SubscriptionPlanId = dto.SubscriptionPlanId,
                    AccessType = dto.AccessType,
                    AttemptsAllowed = dto.AttemptsAllowed,
                    Status = dto.Status?.ToString(),
                    Year = dto.Year,
                    Difficulty = dto.Difficulty,
                    PaperCode = dto.PaperCode,
                    ExamDate = dto.ExamDate,
                    PublishDateTime = dto.PublishDateTime,
                    ValidTill = dto.ValidTill,
                    ShowResultType = dto.ShowResultType?.ToString(),
                    ImageUrl = dto.ImageUrl
                };

                await connection.ExecuteAsync(sql, parameters);
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

        public async Task<(List<MockTestDto> MockTests, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? examId = null, int? subjectId = null, bool? isActive = null, MockTestListRequestDto? request = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sortBy = request?.SortBy?.Trim().ToLowerInvariant();
                var sortOrder = request?.SortOrder?.Trim().ToLowerInvariant() == "asc" ? "ASC" : "DESC";
                var orderByClause = sortBy switch
                {
                    "name" => $"mt.Name {sortOrder}",
                    "date" => $"mt.CreatedAt {sortOrder}",
                    "mark" => $"mt.TotalMarks {sortOrder}",
                    "marks" => $"mt.TotalMarks {sortOrder}",
                    "difficulty" => $"mt.Difficulty {sortOrder}",
                    "year" => $"mt.[Year] {sortOrder}",
                    "attempts" => $"mt.AttemptsAllowed {sortOrder}",
                    "questions" => $"mt.TotalQuestions {sortOrder}",
                    "duration" => $"mt.DurationInMinutes {sortOrder}",
                    "examdate" => $"mt.ExamDate {sortOrder}",
                    "publishdate" => $"mt.PublishDateTime {sortOrder}",
                    "status" => $"mt.Status {sortOrder}",
                    _ => $"mt.CreatedAt {sortOrder}"
                };

                var sql = @"
                    SELECT 
                        mt.Id, mt.Name, mt.Description, ISNULL(mt.MockTestType, 1) AS MockTestType, mt.ExamId, mt.SubjectId, CAST(NULL AS INT) AS TopicId,
                        mt.DurationInMinutes, mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks, 
                        ISNULL(mt.MarksPerQuestion, 0) AS MarksPerQuestion, ISNULL(mt.HasNegativeMarking, 0) AS HasNegativeMarking, mt.NegativeMarkingValue,
                        mt.SubscriptionPlanId, mt.AccessType, mt.AttemptsAllowed, ISNULL(mt.Status, 'Active') AS Status,
                        mt.CreatedAt, mt.UpdatedAt, mt.CreatedBy,
                        mt.[Year], mt.Difficulty, mt.PaperCode, mt.ExamDate, mt.PublishDateTime, 
                        mt.ValidTill, ISNULL(mt.ShowResultType, '1') AS ShowResultType, mt.ImageUrl,
                        e.Name AS ExamName,
                        CAST('' AS NVARCHAR(100)) AS ExamType,
                        mt.SubjectId AS SubjectId,
                        s.Name AS SubjectName,
                        CAST('' AS NVARCHAR(100)) AS TopicName,
                        CAST('' AS NVARCHAR(50)) AS MockTestTypeDisplay,
                        CAST(0 AS INT) AS AttemptsUsed,
                        CAST(0 AS BIT) AS IsUnlocked,
                        CAST(NULL AS NVARCHAR(200)) AS SubscriptionPlanName,
                        ISNULL(qc.QuestionCount, 0) AS QuestionsAdded
                    FROM MockTests mt
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
                    LEFT JOIN (
                        SELECT mtq.MockTestId, COUNT(*) AS QuestionCount
                        FROM dbo.MockTestQuestions mtq
                        INNER JOIN dbo.Questions q ON q.Id = mtq.QuestionId
                        WHERE q.IsActive = 1
                        GROUP BY mtq.MockTestId
                    ) qc ON qc.MockTestId = mt.Id
                    WHERE (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR mt.SubjectId = @SubjectId)
                    AND (@IsActive IS NULL OR mt.IsActive = @IsActive)
                    AND (@SearchTerm IS NULL OR mt.Name LIKE '%' + @SearchTerm + '%' OR mt.Description LIKE '%' + @SearchTerm + '%')
                    AND (@MockTestType IS NULL OR mt.MockTestType = @MockTestType)
                    AND (@AccessType IS NULL OR mt.AccessType = @AccessType)
                    AND (@Status IS NULL OR mt.Status = @Status)
                    AND (@AttemptsAllowed IS NULL OR mt.AttemptsAllowed = @AttemptsAllowed)
                    AND (@CreatedFrom IS NULL OR CAST(mt.CreatedAt AS DATE) >= CAST(@CreatedFrom AS DATE))
                    AND (@CreatedTo IS NULL OR CAST(mt.CreatedAt AS DATE) <= CAST(@CreatedTo AS DATE))
                    AND (@Difficulty IS NULL OR mt.Difficulty = @Difficulty)
                    AND (@Year IS NULL OR mt.[Year] = @Year)
                    AND (@ExamDateFrom IS NULL OR CAST(mt.ExamDate AS DATE) >= CAST(@ExamDateFrom AS DATE))
                    AND (@ExamDateTo IS NULL OR CAST(mt.ExamDate AS DATE) <= CAST(@ExamDateTo AS DATE))
                    AND (@PublishDateFrom IS NULL OR CAST(mt.PublishDateTime AS DATE) >= CAST(@PublishDateFrom AS DATE))
                    AND (@PublishDateTo IS NULL OR CAST(mt.PublishDateTime AS DATE) <= CAST(@PublishDateTo AS DATE))
                    AND (@ValidTillFrom IS NULL OR CAST(mt.ValidTill AS DATE) >= CAST(@ValidTillFrom AS DATE))
                    AND (@ValidTillTo IS NULL OR CAST(mt.ValidTill AS DATE) <= CAST(@ValidTillTo AS DATE))
                    AND (@HasNegativeMarking IS NULL OR mt.HasNegativeMarking = @HasNegativeMarking)
                    AND (@MinTotalMarks IS NULL OR mt.TotalMarks >= @MinTotalMarks)
                    AND (@MaxTotalMarks IS NULL OR mt.TotalMarks <= @MaxTotalMarks)
                    AND (@MinDuration IS NULL OR mt.DurationInMinutes >= @MinDuration)
                    AND (@MaxDuration IS NULL OR mt.DurationInMinutes <= @MaxDuration)
                    ORDER BY 
                        " + orderByClause + @"
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var countSql = @"
                    SELECT COUNT(*)
                    FROM MockTests mt
                    WHERE (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR mt.SubjectId = @SubjectId)
                    AND (@IsActive IS NULL OR mt.IsActive = @IsActive)
                    AND (@SearchTerm IS NULL OR mt.Name LIKE '%' + @SearchTerm + '%' OR mt.Description LIKE '%' + @SearchTerm + '%')
                    AND (@MockTestType IS NULL OR mt.MockTestType = @MockTestType)
                    AND (@AccessType IS NULL OR mt.AccessType = @AccessType)
                    AND (@Status IS NULL OR mt.Status = @Status)
                    AND (@AttemptsAllowed IS NULL OR mt.AttemptsAllowed = @AttemptsAllowed)
                    AND (@CreatedFrom IS NULL OR CAST(mt.CreatedAt AS DATE) >= CAST(@CreatedFrom AS DATE))
                    AND (@CreatedTo IS NULL OR CAST(mt.CreatedAt AS DATE) <= CAST(@CreatedTo AS DATE))
                    AND (@Difficulty IS NULL OR mt.Difficulty = @Difficulty)
                    AND (@Year IS NULL OR mt.[Year] = @Year)
                    AND (@ExamDateFrom IS NULL OR CAST(mt.ExamDate AS DATE) >= CAST(@ExamDateFrom AS DATE))
                    AND (@ExamDateTo IS NULL OR CAST(mt.ExamDate AS DATE) <= CAST(@ExamDateTo AS DATE))
                    AND (@PublishDateFrom IS NULL OR CAST(mt.PublishDateTime AS DATE) >= CAST(@PublishDateFrom AS DATE))
                    AND (@PublishDateTo IS NULL OR CAST(mt.PublishDateTime AS DATE) <= CAST(@PublishDateTo AS DATE))
                    AND (@ValidTillFrom IS NULL OR CAST(mt.ValidTill AS DATE) >= CAST(@ValidTillFrom AS DATE))
                    AND (@ValidTillTo IS NULL OR CAST(mt.ValidTill AS DATE) <= CAST(@ValidTillTo AS DATE))
                    AND (@HasNegativeMarking IS NULL OR mt.HasNegativeMarking = @HasNegativeMarking)
                    AND (@MinTotalMarks IS NULL OR mt.TotalMarks >= @MinTotalMarks)
                    AND (@MaxTotalMarks IS NULL OR mt.TotalMarks <= @MaxTotalMarks)
                    AND (@MinDuration IS NULL OR mt.DurationInMinutes >= @MinDuration)
                    AND (@MaxDuration IS NULL OR mt.DurationInMinutes <= @MaxDuration)";

                var parameters = new
                {
                    ExamId = examId,
                    SubjectId = subjectId,
                    IsActive = isActive,
                    SearchTerm = request?.SearchTerm,
                    MockTestType = request?.MockTestType.HasValue == true ? (int?)request.MockTestType.Value : null,
                    AccessType = request?.AccessType,
                    Status = request?.Status?.ToString(),
                    AttemptsAllowed = request?.AttemptsAllowed,
                    CreatedFrom = request?.CreatedFrom,
                    CreatedTo = request?.CreatedTo,
                    // New filters
                    Difficulty = request?.Difficulty,
                    Year = request?.Year,
                    ExamDateFrom = request?.ExamDateFrom,
                    ExamDateTo = request?.ExamDateTo,
                    PublishDateFrom = request?.PublishDateFrom,
                    PublishDateTo = request?.PublishDateTo,
                    ValidTillFrom = request?.ValidTillFrom,
                    ValidTillTo = request?.ValidTillTo,
                    HasNegativeMarking = request?.HasNegativeMarking,
                    MinTotalMarks = request?.MinTotalMarks,
                    MaxTotalMarks = request?.MaxTotalMarks,
                    MinDuration = request?.MinDuration,
                    MaxDuration = request?.MaxDuration,
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
                // First check if the record exists
                var checkSql = "SELECT COUNT(1) FROM MockTestQuestions WHERE MockTestId = @MockTestId AND QuestionId = @QuestionId";
                var exists = await connection.QuerySingleAsync<int>(checkSql, new { MockTestId = mockTestId, QuestionId = questionId });
                
                if (exists == 0)
                    return false;

                var sql = @"
                    UPDATE MockTestQuestions SET
                        QuestionNumber = @QuestionNumber,
                        Marks = @Marks,
                        NegativeMarks = @NegativeMarks,
                        UpdatedAt = GETDATE()
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
                        mtq.Id AS MockTestQuestionId,
                        mtq.MockTestId,
                        mtq.QuestionId,
                        mtq.QuestionNumber,
                        mtq.Marks,
                        mtq.NegativeMarks,
                        q.QuestionText,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectAnswer,
                        q.Explanation,
                        q.DifficultyLevel,
                        q.QuestionType,
                        q.QuestionImageUrl,
                        q.OptionAImageUrl,
                        q.OptionBImageUrl,
                        q.OptionCImageUrl,
                        q.OptionDImageUrl,
                        q.ExplanationImageUrl
                    FROM MockTestQuestions mtq
                    LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                    WHERE mtq.MockTestId = @MockTestId AND q.IsActive = 1
                    ORDER BY mtq.QuestionNumber";

                var rows = (await connection.QueryAsync<MockTestQuestionFlatRow>(sql, new { MockTestId = mockTestId })).ToList();

                return rows.Select(r => new MockTestQuestionDto
                {
                    Id = r.MockTestQuestionId,
                    MockTestId = r.MockTestId,
                    QuestionId = r.QuestionId,
                    QuestionNumber = r.QuestionNumber,
                    Marks = r.Marks,
                    NegativeMarks = r.NegativeMarks,
                    Question = new QuestionDto
                    {
                        Id = r.QuestionId,
                        QuestionText = r.QuestionText ?? string.Empty,
                        OptionA = r.OptionA,
                        OptionB = r.OptionB,
                        OptionC = r.OptionC,
                        OptionD = r.OptionD,
                        CorrectAnswer = r.CorrectAnswer ?? string.Empty,
                        Explanation = r.Explanation,
                        DifficultyLevel = r.DifficultyLevel ?? "Medium",
                        QuestionType = r.QuestionType ?? "MCQ",
                        QuestionImageUrl = r.QuestionImageUrl,
                        OptionAImageUrl = r.OptionAImageUrl,
                        OptionBImageUrl = r.OptionBImageUrl,
                        OptionCImageUrl = r.OptionCImageUrl,
                        OptionDImageUrl = r.OptionDImageUrl,
                        ExplanationImageUrl = r.ExplanationImageUrl
                    }
                }).ToList();
            });
        }

        public async Task<MockTestQuestionDto?> GetQuestionByIdAsync(int mockTestId, int questionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mtq.Id AS MockTestQuestionId,
                        mtq.MockTestId,
                        mtq.QuestionId,
                        mtq.QuestionNumber,
                        mtq.Marks,
                        mtq.NegativeMarks,
                        q.QuestionText,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectAnswer,
                        q.Explanation,
                        q.DifficultyLevel,
                        q.QuestionType,
                        q.QuestionImageUrl,
                        q.OptionAImageUrl,
                        q.OptionBImageUrl,
                        q.OptionCImageUrl,
                        q.OptionDImageUrl,
                        q.ExplanationImageUrl
                    FROM MockTestQuestions mtq
                    LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                    WHERE mtq.MockTestId = @MockTestId AND mtq.QuestionId = @QuestionId AND q.IsActive = 1";

                var row = await connection.QueryFirstOrDefaultAsync<MockTestQuestionFlatRow>(sql, new { MockTestId = mockTestId, QuestionId = questionId });

                if (row == null)
                    return null;

                return new MockTestQuestionDto
                {
                    Id = row.MockTestQuestionId,
                    MockTestId = row.MockTestId,
                    QuestionId = row.QuestionId,
                    QuestionNumber = row.QuestionNumber,
                    Marks = row.Marks,
                    NegativeMarks = row.NegativeMarks,
                    Question = new QuestionDto
                    {
                        Id = row.QuestionId,
                        QuestionText = row.QuestionText ?? string.Empty,
                        OptionA = row.OptionA,
                        OptionB = row.OptionB,
                        OptionC = row.OptionC,
                        OptionD = row.OptionD,
                        CorrectAnswer = row.CorrectAnswer ?? string.Empty,
                        Explanation = row.Explanation,
                        DifficultyLevel = row.DifficultyLevel ?? "Medium",
                        QuestionType = row.QuestionType ?? "MCQ",
                        QuestionImageUrl = row.QuestionImageUrl,
                        OptionAImageUrl = row.OptionAImageUrl,
                        OptionBImageUrl = row.OptionBImageUrl,
                        OptionCImageUrl = row.OptionCImageUrl,
                        OptionDImageUrl = row.OptionDImageUrl,
                        ExplanationImageUrl = row.ExplanationImageUrl
                    }
                };
            });
        }

        // User Specific Operations
        public async Task<List<MockTestListDto>> GetForUserAsync(int userId, int pageNumber, int pageSize, int? examId = null, int? subjectId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mt.Id, mt.Name, mt.Description, ISNULL(mt.MockTestType, 1) AS MockTestType, mt.ExamId, mt.SubjectId, CAST(NULL AS INT) AS TopicId,
                        mt.DurationInMinutes, mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks,
                        ISNULL(mt.MarksPerQuestion, 0) AS MarksPerQuestion, ISNULL(mt.HasNegativeMarking, 0) AS HasNegativeMarking, mt.NegativeMarkingValue,
                        mt.SubscriptionPlanId, mt.AccessType, mt.AttemptsAllowed, ISNULL(mt.Status, 'Active') AS Status,
                        mt.CreatedAt, mt.UpdatedAt, mt.CreatedBy,
                        mt.[Year], mt.Difficulty, mt.PaperCode, mt.ExamDate, mt.PublishDateTime,
                        mt.ValidTill, ISNULL(mt.ShowResultType, '1') AS ShowResultType, mt.ImageUrl,
                        e.Name AS ExamName,
                        CAST('' AS NVARCHAR(100)) AS ExamType,
                        mt.SubjectId AS SubjectId,
                        s.Name AS SubjectName,
                        CAST(NULL AS NVARCHAR(200)) AS SubscriptionPlanName,
                        CASE 
                            WHEN mt.AccessType = 'Free' THEN 1
                            WHEN EXISTS (
                                SELECT 1 FROM [RankUp_SubscriptionDB].[dbo].[UserSubscriptions] us 
                                WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ValidTill > GETDATE()
                                AND (mt.SubscriptionPlanId IS NULL OR us.SubscriptionPlanId = mt.SubscriptionPlanId)
                            ) THEN 1
                            ELSE 0
                        END AS IsUnlocked,
                        ISNULL((SELECT COUNT(*) FROM MockTestAttempts mta WHERE mta.MockTestId = mt.Id AND mta.UserId = @UserId), 0) AS AttemptsUsed,
                        mt.AttemptsAllowed AS AttemptsAllowed
                    FROM MockTests mt
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
                    WHERE mt.IsActive = 1
                    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR mt.SubjectId = @SubjectId)
                    ORDER BY 
                        CASE WHEN mt.AccessType = 'Free' THEN 0 ELSE 1 END,
                        mt.CreatedAt ASC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var parameters = new
                {
                    UserId = userId,
                    ExamId = examId,
                    SubjectId = subjectId,
                    Offset = (pageNumber - 1) * pageSize,
                    PageSize = pageSize
                };

                var mockTests = await connection.QueryAsync<MockTestListDto>(sql, parameters);
                
                // Convert string values to enums for each mock test
                var result = mockTests.Select(mt => {
                    if (Enum.TryParse<MockTestType>(mt.MockTestType.ToString(), out var mockTestType))
                        mt.MockTestType = mockTestType;
                        
                    // Status is now stored as string directly, no enum conversion needed
                        
                    if (int.TryParse(mt.ShowResultType.ToString(), out var showResultTypeInt) && 
                        Enum.TryParse<ShowResultType>(showResultTypeInt.ToString(), out var showResultType))
                        mt.ShowResultType = showResultType;
                        
                    return mt;
                }).ToList();

                return result;
            });
        }

        public async Task<MockTestDetailDto?> GetDetailForUserAsync(int userId, int mockTestId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        mt.Id, mt.Name, mt.Description, ISNULL(mt.MockTestType, 1) AS MockTestType, mt.ExamId, mt.SubjectId, CAST(NULL AS INT) AS TopicId,
                        mt.DurationInMinutes, mt.TotalQuestions, mt.TotalMarks, mt.PassingMarks,
                        ISNULL(mt.MarksPerQuestion, 0) AS MarksPerQuestion, ISNULL(mt.HasNegativeMarking, 0) AS HasNegativeMarking, mt.NegativeMarkingValue,
                        mt.SubscriptionPlanId, mt.AccessType, mt.AttemptsAllowed, ISNULL(mt.Status, 'Active') AS Status,
                        mt.CreatedAt, mt.UpdatedAt, mt.CreatedBy,
                        mt.[Year], mt.Difficulty, mt.PaperCode, mt.ExamDate, mt.PublishDateTime,
                        mt.ValidTill, ISNULL(mt.ShowResultType, '1') AS ShowResultType, mt.ImageUrl,
                        e.Name AS ExamName,
                        CAST('' AS NVARCHAR(100)) AS ExamType,
                        mt.SubjectId AS SubjectId,
                        s.Name AS SubjectName,
                        CAST(NULL AS NVARCHAR(200)) AS SubscriptionPlanName,
                        CASE 
                            WHEN mt.AccessType = 'Free' THEN 1
                            WHEN EXISTS (
                                SELECT 1 FROM [RankUp_SubscriptionDB].[dbo].[UserSubscriptions] us 
                                WHERE us.UserId = @UserId AND us.IsActive = 1 AND us.ValidTill > GETDATE()
                                AND (mt.SubscriptionPlanId IS NULL OR us.SubscriptionPlanId = mt.SubscriptionPlanId)
                            ) THEN 1
                            ELSE 0
                        END AS IsUnlocked,
                        ISNULL((SELECT COUNT(*) FROM MockTestAttempts mta WHERE mta.MockTestId = mt.Id AND mta.UserId = @UserId), 0) AS AttemptsUsed,
                        mt.AttemptsAllowed AS AttemptsAllowed
                    FROM MockTests mt
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
                    WHERE mt.Id = @MockTestId AND mt.IsActive = 1";

                var mockTest = await connection.QuerySingleOrDefaultAsync<MockTestDetailDto>(sql, new { UserId = userId, MockTestId = mockTestId });
                
                if (mockTest != null)
                {
                    // Convert string values to enums
                    if (Enum.TryParse<MockTestType>(mockTest.MockTestType.ToString(), out var mockTestType))
                        mockTest.MockTestType = mockTestType;
                        
                    // Status is now stored as string directly, no enum conversion needed
                        
                    if (int.TryParse(mockTest.ShowResultType.ToString(), out var showResultTypeInt) && 
                        Enum.TryParse<ShowResultType>(showResultTypeInt.ToString(), out var showResultType))
                        mockTest.ShowResultType = showResultType;

                    // Get attempts for this user
                    var attemptsSql = @"
                        SELECT 
                            Id, MockTestId, UserId, StartedAt, CompletedAt, Duration,
                            TotalQuestions, AnsweredQuestions, CorrectAnswers, WrongAnswers,
                            ObtainedMarks, Percentage, Status, Grade
                        FROM MockTestAttempts
                        WHERE MockTestId = @MockTestId AND UserId = @UserId
                        ORDER BY StartedAt DESC";

                    var attemptResults = await connection.QueryAsync<dynamic>(attemptsSql, new { UserId = userId, MockTestId = mockTestId });
                    
                    mockTest.Attempts = attemptResults.Select(ar => new MockTestAttemptDto
                    {
                        Id = ar.Id,
                        MockTestId = ar.MockTestId,
                        UserId = ar.UserId,
                        StartedAt = ar.StartedAt,
                        CompletedAt = ar.CompletedAt,
                        Duration = TimeSpan.FromMinutes((int)ar.Duration),
                        TotalQuestions = ar.TotalQuestions,
                        AnsweredQuestions = ar.AnsweredQuestions,
                        CorrectAnswers = ar.CorrectAnswers,
                        WrongAnswers = ar.WrongAnswers,
                        ObtainedMarks = ar.ObtainedMarks,
                        Percentage = ar.Percentage,
                        Status = ar.Status,
                        Grade = ar.Grade
                    }).ToList();
                }

                return mockTest;
            });
        }

        public async Task<UserSubscriptionDto?> GetUserSubscriptionAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    DECLARE @startedExpr NVARCHAR(128) = CASE
                        WHEN COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'StartedAt') IS NOT NULL THEN 'us.StartedAt'
                        WHEN COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'PurchasedDate') IS NOT NULL THEN 'us.PurchasedDate'
                        ELSE 'us.CreatedAt'
                    END;

                    DECLARE @expiresExpr NVARCHAR(128) = CASE
                        WHEN COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'ExpiresAt') IS NOT NULL THEN 'us.ValidTill'
                        WHEN COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'ValidTill') IS NOT NULL THEN 'us.ValidTill'
                        ELSE 'NULL'
                    END;

                    DECLARE @testsUsedExpr NVARCHAR(128) = CASE
                        WHEN COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'TestsUsed') IS NOT NULL THEN 'ISNULL(us.TestsUsed, 0)'
                        ELSE '0'
                    END;

                    DECLARE @testsTotalExpr NVARCHAR(128) = CASE
                        WHEN COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'TestsTotal') IS NOT NULL THEN 'ISNULL(us.TestsTotal, 0)'
                        ELSE '0'
                    END;

                    DECLARE @sql NVARCHAR(MAX) = N'
                        SELECT TOP 1
                            us.Id,
                            us.UserId,
                            us.SubscriptionPlanId,
                            sp.Name AS PlanName,
                            ' + @startedExpr + N' AS StartedAt,
                            ' + @expiresExpr + N' AS ExpiresAt,
                            ' + @testsUsedExpr + N' AS TestsUsed,
                            ' + @testsTotalExpr + N' AS TestsTotal,
                            us.IsActive
                        FROM [RankUp_SubscriptionDB].[dbo].[UserSubscriptions] us
                        LEFT JOIN [RankUp_SubscriptionDB].[dbo].[SubscriptionPlans] sp ON us.SubscriptionPlanId = sp.Id
                        WHERE us.UserId = @UserId
                          AND us.IsActive = 1';

                    IF COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'Status') IS NOT NULL
                        SET @sql = @sql + N' AND us.Status = ''Active''';

                    IF COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'ExpiresAt') IS NOT NULL
                        SET @sql = @sql + N' AND us.ValidTill > GETDATE()';
                    ELSE IF COL_LENGTH('[RankUp_SubscriptionDB].[dbo].[UserSubscriptions]', 'ValidTill') IS NOT NULL
                        SET @sql = @sql + N' AND us.ValidTill > GETDATE()';

                    SET @sql = @sql + N' ORDER BY ' + @startedExpr + N' DESC';

                    EXEC sp_executesql @sql, N'@UserId INT', @UserId;";

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
                        e.Name AS ExamName, s.Name AS SubjectName,
                        CAST(0 AS BIT) AS HasNegativeMarking,
                        CAST(0 AS DECIMAL(10,2)) AS NegativeMarkingValue
                    FROM MockTestSessions mts
                    LEFT JOIN MockTests mt ON mts.MockTestId = mt.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
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

        public async Task<bool> SaveSessionAnswerAsync(int sessionId, int userId, SaveMockTestAnswerDto answer)
        {
            return await WithConnectionAsync(async connection =>
            {
                var validateSql = @"
                    SELECT COUNT(1)
                    FROM MockTestSessions
                    WHERE Id = @SessionId AND UserId = @UserId AND Status = 'InProgress'";

                var isValidSession = await connection.QuerySingleAsync<int>(validateSql, new { SessionId = sessionId, UserId = userId });
                if (isValidSession == 0)
                {
                    return false;
                }

                var upsertSql = @"
                    IF EXISTS (SELECT 1 FROM MockTestSessionAnswers WHERE SessionId = @SessionId AND QuestionId = @QuestionId)
                        UPDATE MockTestSessionAnswers
                        SET SelectedAnswer = @SelectedAnswer,
                            IsMarkedForReview = @IsMarkedForReview,
                            IsAnswered = @IsAnswered
                        WHERE SessionId = @SessionId AND QuestionId = @QuestionId
                    ELSE
                        INSERT INTO MockTestSessionAnswers (SessionId, QuestionId, SelectedAnswer, IsMarkedForReview, IsAnswered)
                        VALUES (@SessionId, @QuestionId, @SelectedAnswer, @IsMarkedForReview, @IsAnswered)";

                var isAnswered = !string.IsNullOrWhiteSpace(answer.SelectedAnswer);

                await connection.ExecuteAsync(upsertSql, new
                {
                    SessionId = sessionId,
                    QuestionId = answer.QuestionId,
                    SelectedAnswer = answer.SelectedAnswer,
                    IsMarkedForReview = answer.IsMarkedForReview,
                    IsAnswered = isAnswered
                });

                return true;
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
                               CAST(0 AS BIT) AS HasNegativeMarking,
                               CAST(0 AS DECIMAL(10,2)) AS NegativeMarkingValue
                        FROM MockTestSessions mts
                        LEFT JOIN MockTests mt ON mts.MockTestId = mt.Id
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
                    var attempt = await FinalizeSessionAsync(connection, transaction, sessionId, userId, session);
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

        public async Task<MockTestAttemptDto> SubmitSessionAsync(int sessionId, int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var transaction = connection.BeginTransaction();
                try
                {
                    var sessionSql = @"
                        SELECT mts.MockTestId, mts.StartedAt, mt.DurationInMinutes, mt.TotalMarks, mt.PassingMarks,
                               CAST(0 AS BIT) AS HasNegativeMarking,
                               CAST(0 AS DECIMAL(10,2)) AS NegativeMarkingValue
                        FROM MockTestSessions mts
                        LEFT JOIN MockTests mt ON mts.MockTestId = mt.Id
                        WHERE mts.Id = @SessionId AND mts.UserId = @UserId";

                    var session = await connection.QuerySingleOrDefaultAsync(sessionSql, new { SessionId = sessionId, UserId = userId }, transaction);
                    if (session == null)
                    {
                        throw new KeyNotFoundException("Session not found");
                    }

                    var attempt = await FinalizeSessionAsync(connection, transaction, sessionId, userId, session);
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

        private static async Task<MockTestAttemptDto> FinalizeSessionAsync(System.Data.IDbConnection connection, System.Data.IDbTransaction transaction, int sessionId, int userId, dynamic session)
        {
            var resultSql = @"
                SELECT 
                    mtq.QuestionId, mtq.Marks, mtq.NegativeMarks, q.CorrectAnswer,
                    mtsa.SelectedAnswer
                FROM MockTestQuestions mtq
                LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                LEFT JOIN MockTestSessionAnswers mtsa ON mtq.QuestionId = mtsa.QuestionId AND mtsa.SessionId = @SessionId
                WHERE mtq.MockTestId = @MockTestId";

            var questionResults = (await connection.QueryAsync(resultSql, new { SessionId = sessionId, MockTestId = session.MockTestId }, transaction)).ToList();

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

            var answeredQuestions = questionResults.Count(q => !string.IsNullOrEmpty((string?)q.SelectedAnswer));
            var percentage = session.TotalMarks > 0 ? (obtainedMarks / session.TotalMarks) * 100 : 0;
            var grade = GetGrade(percentage);

            var updateSessionSql = @"
                UPDATE MockTestSessions SET 
                    CompletedAt = GETDATE(), 
                    Status = 'Completed'
                WHERE Id = @SessionId";

            await connection.ExecuteAsync(updateSessionSql, new { SessionId = sessionId }, transaction);

            var attemptSql = @"
                INSERT INTO MockTestAttempts (
                    MockTestId, UserId, StartedAt, CompletedAt, Duration,
                    TotalQuestions, AnsweredQuestions, CorrectAnswers, WrongAnswers,
                    ObtainedMarks, Percentage, Status, Grade
                )
                OUTPUT INSERTED.Id, INSERTED.StartedAt, INSERTED.CompletedAt,
                       INSERTED.TotalQuestions, INSERTED.AnsweredQuestions, INSERTED.CorrectAnswers,
                       INSERTED.WrongAnswers, INSERTED.ObtainedMarks, INSERTED.Percentage,
                       INSERTED.Status, INSERTED.Grade
                VALUES (
                    @MockTestId, @UserId, @StartedAt, GETDATE(), 
                    DATEDIFF(MINUTE, @StartedAt, GETDATE()),
                    @TotalQuestions, @AnsweredQuestions, @CorrectAnswers, @WrongAnswers,
                    @ObtainedMarks, @Percentage, 'Completed', @Grade
                )";

            var result = await connection.QuerySingleAsync<dynamic>(attemptSql, new
            {
                MockTestId = session.MockTestId,
                UserId = userId,
                StartedAt = session.StartedAt,
                TotalQuestions = questionResults.Count,
                AnsweredQuestions = answeredQuestions,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                ObtainedMarks = obtainedMarks,
                Percentage = percentage,
                Grade = grade
            }, transaction);

            // Calculate duration in minutes and convert to TimeSpan
            var durationInMinutes = (DateTime)result.CompletedAt - session.StartedAt;
            
            return new MockTestAttemptDto
            {
                Id = result.Id,
                MockTestId = session.MockTestId,
                UserId = userId,
                StartedAt = result.StartedAt,
                CompletedAt = result.CompletedAt,
                Duration = durationInMinutes,
                TotalQuestions = result.TotalQuestions,
                AnsweredQuestions = result.AnsweredQuestions,
                CorrectAnswers = result.CorrectAnswers,
                WrongAnswers = result.WrongAnswers,
                ObtainedMarks = result.ObtainedMarks,
                Percentage = result.Percentage,
                Status = result.Status,
                Grade = result.Grade
            };
        }

        // Statistics
        public async Task<MockTestStatisticsDto> GetMockTestStatisticsAsync(int? examId = null, int? subjectId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        COUNT(*) AS TotalMockTests,
                        SUM(CASE WHEN mt.IsActive = 1 THEN 1 ELSE 0 END) AS ActiveMockTests,
                        CAST(0 AS INT) AS ScheduledMockTests,
                        CAST(0 AS INT) AS DraftMockTests,
                        SUM(CASE WHEN mt.AccessType = 'Paid' THEN 1 ELSE 0 END) AS PaidMockTests,
                        COUNT(*) AS MockTestCount,
                        CAST(0 AS INT) AS TestSeriesCount,
                        CAST(0 AS INT) AS DeepPracticeCount,
                        CAST(0 AS INT) AS PreviousYearCount
                    FROM MockTests mt
                    WHERE (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR EXISTS (
                        SELECT 1
                        FROM [RankUp_MasterDB].[dbo].[ExamSubjects] esf
                        WHERE esf.ExamId = mt.ExamId
                          AND esf.SubjectId = @SubjectId
                          AND ISNULL(esf.IsActive, 1) = 1
                    ))";

                return await connection.QuerySingleAsync<MockTestStatisticsDto>(sql, new { ExamId = examId, SubjectId = subjectId });
            });
        }

        public async Task<object> GetStatisticsLegacyAsync(int? examId = null, int? subjectId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        COUNT(*) AS TotalMockTests,
                        SUM(CASE WHEN mt.AccessType = 'Free' THEN 1 ELSE 0 END) AS FreeMockTests,
                        SUM(CASE WHEN mt.AccessType = 'Paid' THEN 1 ELSE 0 END) AS PaidMockTests,
                        COUNT(DISTINCT mt.ExamId) AS ExamsCovered,
                        COUNT(DISTINCT es.SubjectId) AS SubjectsCovered
                    FROM MockTests mt
                    LEFT JOIN [RankUp_MasterDB].[dbo].[ExamSubjects] es ON es.ExamId = mt.ExamId AND ISNULL(es.IsActive, 1) = 1
                    WHERE mt.IsActive = 1
                    AND (@ExamId IS NULL OR mt.ExamId = @ExamId)
                    AND (@SubjectId IS NULL OR es.SubjectId = @SubjectId)";

                return await connection.QuerySingleAsync<object>(sql, new { ExamId = examId, SubjectId = subjectId });
            });
        }

        public async Task<IEnumerable<SubjectListDto>> GetSubjectsForExamAsync(int examId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT 
                        s.Id,
                        s.Name,
                        s.Description,
                        ISNULL(COUNT(q.Id), 0) AS QuestionCount,
                        CAST(ISNULL(s.IsActive, 1) AS BIT) AS IsActive
                    FROM [RankUp_MasterDB].[dbo].[ExamSubjects] es
                    INNER JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON es.SubjectId = s.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Questions] q ON s.Id = q.SubjectId AND q.IsActive = 1
                    WHERE es.ExamId = @ExamId 
                      AND ISNULL(es.IsActive, 1) = 1
                      AND ISNULL(s.IsActive, 1) = 1
                    GROUP BY s.Id, s.Name, s.Description, s.IsActive
                    ORDER BY s.Name";

                return await connection.QueryAsync<SubjectListDto>(sql, new { ExamId = examId });
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
                        CAST('' AS NVARCHAR(100)) AS ExamType,
                        mta.StartedAt,
                        mta.Percentage,
                        mta.Grade,
                        mta.Status
                    FROM MockTestAttempts mta
                    LEFT JOIN MockTests mt ON mta.MockTestId = mt.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON mt.ExamId = e.Id
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
