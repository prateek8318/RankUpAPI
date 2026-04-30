using Dapper;
using Microsoft.Data.SqlClient;
using QuestionService.Application.DTOs;
using QuestionService.Application.Interfaces;
using System.Text.Json;
using System.Data;
using Microsoft.Extensions.Logging;

namespace QuestionService.Infrastructure.Repositories
{
    public class MockTestRepository : BaseDapperRepository, IMockTestRepository
    {
        private const int DefaultPerQuestionTimeInSeconds = 45;

        private readonly ILogger<MockTestRepository> _logger;

        public MockTestRepository(string connectionString) : base(connectionString)
        {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MockTestRepository>();
        }

        public MockTestRepository(string connectionString, ILogger<MockTestRepository> logger) : base(connectionString)
        {
            _logger = logger;
        }

        private sealed class MockTestQuestionFlatRow
        {
            public int MockTestQuestionId { get; set; }
            public int MockTestId { get; set; }
            public int QuestionId { get; set; }
            public int QuestionNumber { get; set; }
            public decimal Marks { get; set; }
            public decimal NegativeMarks { get; set; }
            public int ExamId { get; set; }
            public int SubjectId { get; set; }
            public int? TopicId { get; set; }

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

        private sealed class QuestionTranslationFlatRow
        {
            public int QuestionId { get; set; }
            public int Id { get; set; }
            public string LanguageCode { get; set; } = string.Empty;
            public string QuestionText { get; set; } = string.Empty;
            public string? OptionA { get; set; }
            public string? OptionB { get; set; }
            public string? OptionC { get; set; }
            public string? OptionD { get; set; }
            public string? Explanation { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        private static async Task<Dictionary<int, List<QuestionTranslationDto>>> LoadTranslationsByQuestionIdAsync(
            System.Data.IDbConnection connection,
            IEnumerable<int> questionIds)
        {
            var ids = questionIds.Distinct().ToArray();
            if (ids.Length == 0)
            {
                return new Dictionary<int, List<QuestionTranslationDto>>();
            }

            var rows = await connection.QueryAsync<QuestionTranslationFlatRow>(
                @"SELECT
                      qt.QuestionId,
                      qt.Id,
                      qt.LanguageCode,
                      qt.QuestionText,
                      qt.OptionA,
                      qt.OptionB,
                      qt.OptionC,
                      qt.OptionD,
                      qt.Explanation,
                      qt.CreatedAt,
                      qt.UpdatedAt
                  FROM QuestionTranslations qt
                  WHERE qt.QuestionId IN @QuestionIds",
                new { QuestionIds = ids });

            return rows
                .GroupBy(x => x.QuestionId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new QuestionTranslationDto
                    {
                        Id = x.Id,
                        QuestionId = x.QuestionId,
                        LanguageCode = x.LanguageCode,
                        QuestionText = x.QuestionText,
                        OptionA = x.OptionA,
                        OptionB = x.OptionB,
                        OptionC = x.OptionC,
                        OptionD = x.OptionD,
                        Explanation = x.Explanation,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    }).ToList());
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
                        ShowResultType, ImageUrl, CreatedBy, CreatedAt, UpdatedAt
                    )
                    OUTPUT INSERTED.Id, INSERTED.CreatedAt, INSERTED.UpdatedAt
                    VALUES (
                        @Name, @Description, @MockTestType, @ExamId, @SubjectId, @TopicId, @DurationInMinutes, 
                        @TotalQuestions, @TotalMarks, @PassingMarks, @MarksPerQuestion, @HasNegativeMarking, 
                        @NegativeMarkingValue, @SubscriptionPlanId, @AccessType, @AttemptsAllowed, @Status,
                        @Year, @Difficulty, @PaperCode, @ExamDate, @PublishDateTime, @ValidTill, 
                        @ShowResultType, @ImageUrl, @CreatedBy, GETDATE(), GETDATE()
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

                var result = await connection.QuerySingleAsync<(int Id, DateTime CreatedAt, DateTime? UpdatedAt)>(sql, parameters);
                
                // Save languages if provided
                if (dto.Languages != null && dto.Languages.Any())
                {
                    _logger.LogInformation("Saving {Count} languages for MockTest {Id}", dto.Languages.Count, result.Id);
                    await SaveMockTestLanguagesAsync(connection, result.Id, dto.Languages);
                }
                else
                {
                    _logger.LogWarning("No languages provided for MockTest {Id}", result.Id);
                }
                
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
                        q.ExamId,
                        q.SubjectId,
                        q.TopicId,
                        COALESCE(NULLIF(qt.QuestionText, ''), q.QuestionText, '') AS QuestionText,
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
                    OUTER APPLY (
                        SELECT TOP 1 t.QuestionText
                        FROM QuestionTranslations t
                        WHERE t.QuestionId = q.Id
                        ORDER BY CASE WHEN t.LanguageCode = 'en' THEN 0 ELSE 1 END, t.Id
                    ) qt
                    WHERE mtq.MockTestId = @MockTestId AND q.IsActive = 1
                    ORDER BY mtq.QuestionNumber";

                var rows = (await connection.QueryAsync<MockTestQuestionFlatRow>(questionsSql, new { MockTestId = id })).ToList();
                var translationsByQuestionId = await LoadTranslationsByQuestionIdAsync(connection, rows.Select(x => x.QuestionId));

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
                        ExamId = r.ExamId,
                        SubjectId = r.SubjectId,
                        TopicId = r.TopicId,
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
                        ExplanationImageUrl = r.ExplanationImageUrl,
                        Translations = translationsByQuestionId.GetValueOrDefault(r.QuestionId) ?? new List<QuestionTranslationDto>()
                    }
                }).ToList();

                // Load languages for this mock test
                var languagesSql = @"
                    SELECT 
                        ml.Id,
                        ml.MockTestId,
                        ml.LanguageId,
                        ml.Name,
                        ml.Description,
                        ml.IsActive,
                        ml.CreatedAt,
                        ml.UpdatedAt
                    FROM MockTestLanguages ml
                    WHERE ml.MockTestId = @MockTestId AND ml.IsActive = 1
                    ORDER BY ml.LanguageId";

                var languageRows = await connection.QueryAsync<MockTestLanguageDto>(languagesSql, new { MockTestId = id });
                mockTest.Languages = languageRows.ToList();
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
                        ExamId = COALESCE(@ExamId, ExamId),
                        SubjectId = COALESCE(@SubjectId, SubjectId),
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
                    ExamId = dto.ExamId,
                    SubjectId = dto.SubjectId,
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
                
                // Update languages if provided
                if (dto.Languages != null && dto.Languages.Any())
                {
                    await SaveMockTestLanguagesAsync(connection, dto.Id, dto.Languages);
                }
                
                return await GetByIdInternalAsync((SqlConnection)connection, dto.Id);
            });
        }

        private async Task SaveMockTestLanguagesAsync(IDbConnection connection, int mockTestId, List<MockTestLanguageDto> languages)
        {
            _logger.LogInformation("SaveMockTestLanguagesAsync called with {Count} languages for MockTest {Id}", languages.Count, mockTestId);
            
            // Delete existing languages for this mock test
            await connection.ExecuteAsync("DELETE FROM MockTestLanguages WHERE MockTestId = @MockTestId", new { MockTestId = mockTestId });
            
            // Insert new languages
            foreach (var language in languages)
            {
                // Validate language name
                if (string.IsNullOrWhiteSpace(language.Name))
                {
                    _logger.LogWarning("Skipping language with empty name: LanguageId={LanguageId}", language.LanguageId);
                    continue;
                }

                var languageSql = @"
                    INSERT INTO MockTestLanguages (MockTestId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
                    VALUES (@MockTestId, @LanguageId, @Name, @Description, @IsActive, GETUTCDATE(), GETUTCDATE())";
                
                await connection.ExecuteAsync(languageSql, new
                {
                    MockTestId = mockTestId,
                    LanguageId = language.LanguageId,
                    Name = language.Name?.Trim(),
                    Description = language.Description,
                    IsActive = language.IsActive
                });
                
                _logger.LogInformation("Saved language: LanguageId={LanguageId}, Name={Name}", language.LanguageId, language.Name);
            }
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
                    "mark" => $"ISNULL(sp.Price, 0) {sortOrder}",
                    "marks" => $"ISNULL(sp.Price, 0) {sortOrder}",
                    "price" => $"ISNULL(sp.Price, 0) {sortOrder}",
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
                    LEFT JOIN [RankUp_SubscriptionDB].[dbo].[SubscriptionPlans] sp ON mt.SubscriptionPlanId = sp.Id
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
                    LEFT JOIN [RankUp_SubscriptionDB].[dbo].[SubscriptionPlans] sp ON mt.SubscriptionPlanId = sp.Id
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
                        mtq.Id AS MockTestQuestionId,
                        mtq.MockTestId,
                        mtq.QuestionId,
                        mtq.QuestionNumber,
                        mtq.Marks,
                        mtq.NegativeMarks,
                        q.ExamId,
                        q.SubjectId,
                        q.TopicId,
                        COALESCE(NULLIF(qt.QuestionText, ''), q.QuestionText, '') AS QuestionText,
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
                    OUTER APPLY (
                        SELECT TOP 1 t.QuestionText
                        FROM QuestionTranslations t
                        WHERE t.QuestionId = q.Id
                        ORDER BY CASE WHEN t.LanguageCode = 'en' THEN 0 ELSE 1 END, t.Id
                    ) qt
                    WHERE mtq.MockTestId = @MockTestId AND q.IsActive = 1
                    ORDER BY mtq.QuestionNumber";

                var rows = (await connection.QueryAsync<MockTestQuestionFlatRow>(sql, new { MockTestId = mockTestId })).ToList();
                var translationsByQuestionId = await LoadTranslationsByQuestionIdAsync(connection, rows.Select(x => x.QuestionId));

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
                        ExamId = r.ExamId,
                        SubjectId = r.SubjectId,
                        TopicId = r.TopicId,
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
                        ExplanationImageUrl = r.ExplanationImageUrl,
                        Translations = translationsByQuestionId.GetValueOrDefault(r.QuestionId) ?? new List<QuestionTranslationDto>()
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
                        q.ExamId,
                        q.SubjectId,
                        q.TopicId,
                        COALESCE(NULLIF(qt.QuestionText, ''), q.QuestionText, '') AS QuestionText,
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
                    OUTER APPLY (
                        SELECT TOP 1 t.QuestionText
                        FROM QuestionTranslations t
                        WHERE t.QuestionId = q.Id
                        ORDER BY CASE WHEN t.LanguageCode = 'en' THEN 0 ELSE 1 END, t.Id
                    ) qt
                    WHERE mtq.MockTestId = @MockTestId AND mtq.QuestionId = @QuestionId AND q.IsActive = 1";

                var row = await connection.QueryFirstOrDefaultAsync<MockTestQuestionFlatRow>(sql, new { MockTestId = mockTestId, QuestionId = questionId });

                if (row == null)
                    return null;

                var translationsByQuestionId = await LoadTranslationsByQuestionIdAsync(connection, new[] { row.QuestionId });

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
                        ExamId = row.ExamId,
                        SubjectId = row.SubjectId,
                        TopicId = row.TopicId,
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
                        ExplanationImageUrl = row.ExplanationImageUrl,
                        Translations = translationsByQuestionId.GetValueOrDefault(row.QuestionId) ?? new List<QuestionTranslationDto>()
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
                    OUTPUT INSERTED.Id, INSERTED.StartedAt
                    VALUES (@MockTestId, @UserId, GETDATE(), 'InProgress', @LanguageCode)";

                var sessionRow = await connection.QuerySingleAsync<(int Id, DateTime StartedAt)>(sessionSql, dto);
                var sessionId = sessionRow.Id;

                // Get questions for the session
                var questions = await GetQuestionsAsync(dto.MockTestId);
                var startedAt = sessionRow.StartedAt;
                var totalDurationInSeconds = GetTotalDurationInSeconds(questions.Count);
                var totalMarks = questions.Sum(q => q.Marks);
                var hasNegativeMarking = questions.Any(q => q.NegativeMarks > 0);
                var negativeMarksPerQuestion = questions.Count > 0 ? questions.Max(q => q.NegativeMarks) : 0;
                
                // Convert to QuizQuestionDto format
                var quizQuestions = questions.Select((q, index) =>
                {
                    var window = GetQuestionWindow(startedAt, index);
                    return new QuizQuestionDto
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
                        ExplanationImageUrl = q.Question?.ExplanationImageUrl,
                        Marks = q.Marks,
                        NegativeMarks = q.NegativeMarks,
                        DifficultyLevel = q.Question?.DifficultyLevel ?? "Medium",
                        QuestionNumber = index + 1,
                        IsMarkedForReview = false,
                        IsAnswered = false,
                        TimeLimitInSeconds = DefaultPerQuestionTimeInSeconds,
                        AvailableFrom = window.Start,
                        AvailableUntil = window.End,
                        CanAnswer = true,
                        Translations = q.Question?.Translations ?? new List<QuestionTranslationDto>()
                    };
                }).ToList();

                return new MockTestSessionDto
                {
                    SessionId = sessionId,
                    MockTestId = dto.MockTestId,
                    MockTestName = mockTest.Name,
                    UserId = dto.UserId,
                    ExamName = mockTest.ExamName,
                    SubjectName = mockTest.SubjectName,
                    StartedAt = startedAt,
                    DurationInMinutes = mockTest.DurationInMinutes,
                    TotalDurationInSeconds = totalDurationInSeconds,
                    PerQuestionTimeInSeconds = DefaultPerQuestionTimeInSeconds,
                    CurrentQuestionNumber = quizQuestions.Count > 0 ? 1 : 0,
                    CurrentQuestionId = quizQuestions.FirstOrDefault()?.Id,
                    RemainingTimeInSeconds = totalDurationInSeconds,
                    TotalQuestions = quizQuestions.Count,
                    AnsweredQuestions = 0,
                    MarkedForReview = 0,
                    TotalMarks = totalMarks,
                    ObtainedMarks = 0,
                    HasNegativeMarking = hasNegativeMarking,
                    NegativeMarksPerQuestion = negativeMarksPerQuestion,
                    Status = "InProgress",
                    Questions = quizQuestions
                };
            });
        }

        public async Task<MockTestSessionDto?> GetSessionAsync(int sessionId, int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sessionAnswerColumns = await GetSessionAnswerFeatureColumnsAsync(connection);
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
                    var now = DateTime.Now;
                    // Get questions and answers for this session
                    var questionsSql = $@"
                        SELECT 
                            mtq.QuestionId, mtq.QuestionNumber, mtq.Marks, mtq.NegativeMarks,
                            q.QuestionText, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.CorrectAnswer,
                            q.Explanation, q.DifficultyLevel, q.QuestionType, q.QuestionImageUrl, q.OptionAImageUrl,
                            q.OptionBImageUrl, q.OptionCImageUrl, q.OptionDImageUrl, q.ExplanationImageUrl,
                            mtsa.SelectedAnswer, mtsa.IsMarkedForReview, mtsa.IsAnswered,
                            {(sessionAnswerColumns.HasIsReported ? "ISNULL(mtsa.IsReported, 0)" : "CAST(0 AS BIT)")} AS IsReported,
                            {(sessionAnswerColumns.HasIsBookmarked ? "ISNULL(mtsa.IsBookmarked, 0)" : "CAST(0 AS BIT)")} AS IsBookmarked
                        FROM MockTestQuestions mtq
                        LEFT JOIN Questions q ON mtq.QuestionId = q.Id
                        LEFT JOIN MockTestSessionAnswers mtsa ON mtq.QuestionId = mtsa.QuestionId AND mtsa.SessionId = @SessionId
                        WHERE mtq.MockTestId = @MockTestId
                        ORDER BY mtq.QuestionNumber";

                    var questionRows = await connection.QueryAsync(questionsSql, new { SessionId = sessionId, MockTestId = session.MockTestId });
                    session.Questions = questionRows.Select(row =>
                    {
                        var question = new QuizQuestionDto
                        {
                            Id = row.QuestionId,
                            QuestionNumber = row.QuestionNumber,
                            Marks = row.Marks,
                            NegativeMarks = row.NegativeMarks,
                            QuestionText = row.QuestionText ?? string.Empty,
                            OptionA = row.OptionA,
                            OptionB = row.OptionB,
                            OptionC = row.OptionC,
                            OptionD = row.OptionD,
                            QuestionImageUrl = row.QuestionImageUrl,
                            OptionAImageUrl = row.OptionAImageUrl,
                            OptionBImageUrl = row.OptionBImageUrl,
                            OptionCImageUrl = row.OptionCImageUrl,
                            OptionDImageUrl = row.OptionDImageUrl,
                            DifficultyLevel = row.DifficultyLevel ?? "Medium",
                            SelectedAnswer = row.SelectedAnswer,
                            IsMarkedForReview = row.IsMarkedForReview ?? false,
                            IsAnswered = row.IsAnswered ?? false,
                            IsReported = row.IsReported ?? false,
                            IsBookmarked = row.IsBookmarked ?? false
                        };

                        var questionIndex = Math.Max(question.QuestionNumber - 1, 0);
                        var window = GetQuestionWindow(session.StartedAt, questionIndex);
                        question.TimeLimitInSeconds = DefaultPerQuestionTimeInSeconds;
                        question.AvailableFrom = window.Start;
                        question.AvailableUntil = window.End;
                        question.CanAnswer = now <= window.End;
                        return question;
                    }).ToList();
                    session.PerQuestionTimeInSeconds = DefaultPerQuestionTimeInSeconds;
                    session.TotalDurationInSeconds = GetTotalDurationInSeconds(session.Questions.Count);
                    session.TotalMarks = session.Questions.Sum(q => q.Marks);
                    session.HasNegativeMarking = session.Questions.Any(q => q.NegativeMarks > 0);
                    session.NegativeMarksPerQuestion = session.Questions.Count > 0 ? session.Questions.Max(q => q.NegativeMarks) : 0;
                    
                    // Calculate answered questions
                    session.AnsweredQuestions = session.Questions.Count(q => q.IsAnswered);
                    session.MarkedForReview = session.Questions.Count(q => q.IsMarkedForReview);
                    var elapsedSeconds = Math.Max(0, (int)(now - session.StartedAt).TotalSeconds);
                    session.RemainingTimeInSeconds = Math.Max(0, session.TotalDurationInSeconds - elapsedSeconds);
                    session.CurrentQuestionNumber = session.RemainingTimeInSeconds == 0 || session.Questions.Count == 0
                        ? 0
                        : Math.Min(session.Questions.Count, (elapsedSeconds / DefaultPerQuestionTimeInSeconds) + 1);
                    session.CurrentQuestionId = session.CurrentQuestionNumber > 0
                        ? session.Questions.FirstOrDefault(q => q.QuestionNumber == session.CurrentQuestionNumber)?.Id
                        : null;
                }

                return session;
            });
        }

        public async Task<MockTestAttemptDto?> GetSessionResultAsync(int sessionId, int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                // First try to find the session info
                var sessionInfo = await connection.QuerySingleOrDefaultAsync(
                    @"SELECT Id, MockTestId, UserId, StartedAt, Status
                      FROM MockTestSessions
                      WHERE Id = @SessionId AND UserId = @UserId",
                    new { SessionId = sessionId, UserId = userId });

                MockTestAttemptDto? attempt = null;

                if (sessionInfo != null)
                {
                    // Try to find attempt by exact StartedAt match first (for backwards compatibility)
                    var exactAttempt = await connection.QuerySingleOrDefaultAsync<dynamic>(
                        @"SELECT TOP 1
                            Id,
                            MockTestId,
                            UserId,
                            StartedAt,
                            CompletedAt,
                            Duration,
                            TotalQuestions,
                            AnsweredQuestions,
                            CorrectAnswers,
                            WrongAnswers,
                            ObtainedMarks,
                            Percentage,
                            Status,
                            Grade
                          FROM MockTestAttempts
                          WHERE MockTestId = @MockTestId
                            AND UserId = @UserId
                            AND StartedAt = @StartedAt
                          ORDER BY Id DESC",
                        new
                        {
                            MockTestId = (int)sessionInfo.MockTestId,
                            UserId = userId,
                            StartedAt = (DateTime)sessionInfo.StartedAt
                        });

                    if (exactAttempt != null)
                    {
                        attempt = await CreateMockTestAttemptDtoAsync(connection, exactAttempt, userId);
                    }
                }

                // If no attempt found by exact match, try to find the most recent attempt for this session
                if (attempt == null)
                {
                    var recentAttempt = await connection.QuerySingleOrDefaultAsync<dynamic>(
                        @"SELECT TOP 1
                            mta.Id,
                            mta.MockTestId,
                            mta.UserId,
                            mta.StartedAt,
                            mta.CompletedAt,
                            mta.Duration,
                            mta.TotalQuestions,
                            mta.AnsweredQuestions,
                            mta.CorrectAnswers,
                            mta.WrongAnswers,
                            mta.ObtainedMarks,
                            mta.Percentage,
                            mta.Status,
                            mta.Grade
                          FROM MockTestAttempts mta
                          INNER JOIN MockTestSessions mts ON mta.MockTestId = mts.MockTestId AND mta.UserId = mts.UserId
                          WHERE mts.Id = @SessionId AND mta.UserId = @UserId
                          ORDER BY mta.Id DESC",
                        new { SessionId = sessionId, UserId = userId });

                    if (recentAttempt != null)
                    {
                        attempt = await CreateMockTestAttemptDtoAsync(connection, recentAttempt, userId);
                    }
                }

                return attempt;
            });
        }

        public async Task<MockTestSolutionDto?> GetSessionSolutionAsync(int sessionId, int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sessionSql = @"
                    SELECT
                        mts.Id AS SessionId,
                        mts.MockTestId,
                        mt.Name AS MockTestName,
                        ISNULL(s.Name, '') AS SubjectName,
                        ISNULL(mts.Status, 'InProgress') AS Status,
                        ISNULL(mts.LanguageCode, 'en') AS LanguageCode
                    FROM MockTestSessions mts
                    INNER JOIN MockTests mt ON mts.MockTestId = mt.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON mt.SubjectId = s.Id
                    WHERE mts.Id = @SessionId AND mts.UserId = @UserId";

                var solution = await connection.QuerySingleOrDefaultAsync<MockTestSolutionDto>(
                    sessionSql,
                    new { SessionId = sessionId, UserId = userId });

                if (solution == null)
                {
                    return null;
                }

                // Get questions with translations based on session language
                var questionsSql = $@"
                    SELECT
                        mtq.QuestionId,
                        mtq.QuestionNumber,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectAnswer,
                        mtsa.SelectedAnswer,
                        q.ExplanationImageUrl,
                        q.QuestionImageUrl,
                        q.OptionAImageUrl,
                        q.OptionBImageUrl,
                        q.OptionCImageUrl,
                        q.OptionDImageUrl,
                        qt.QuestionText,
                        qt.OptionA AS TranslatedOptionA,
                        qt.OptionB AS TranslatedOptionB,
                        qt.OptionC AS TranslatedOptionC,
                        qt.OptionD AS TranslatedOptionD,
                        qt.Explanation
                    FROM MockTestQuestions mtq
                    INNER JOIN Questions q ON mtq.QuestionId = q.Id
                    LEFT JOIN MockTestSessionAnswers mtsa
                        ON mtq.QuestionId = mtsa.QuestionId
                        AND mtsa.SessionId = @SessionId
                    OUTER APPLY (
                        SELECT TOP 1 t.QuestionText, t.OptionA, t.OptionB, t.OptionC, t.OptionD, t.Explanation
                        FROM QuestionTranslations t
                        WHERE t.QuestionId = q.Id
                        ORDER BY CASE WHEN t.LanguageCode = @LanguageCode THEN 0 ELSE 1 END, t.Id
                    ) qt
                    WHERE mtq.MockTestId = @MockTestId
                    ORDER BY mtq.QuestionNumber";

                var questions = (await connection.QueryAsync<MockTestSolutionQuestionDto>(
                    questionsSql,
                    new { SessionId = sessionId, MockTestId = solution.MockTestId, LanguageCode = solution.LanguageCode })).ToList();

                foreach (var question in questions)
                {
                    // Use translated text if available, otherwise fall back to base text
                    question.QuestionText = question.QuestionText ?? string.Empty;
                    question.OptionA = question.TranslatedOptionA ?? question.OptionA;
                    question.OptionB = question.TranslatedOptionB ?? question.OptionB;
                    question.OptionC = question.TranslatedOptionC ?? question.OptionC;
                    question.OptionD = question.TranslatedOptionD ?? question.OptionD;
                    question.Explanation = question.Explanation ?? string.Empty;
                    
                    question.CorrectOptionText = GetOptionText(question.CorrectAnswer, question);
                    question.IsCorrect = !string.IsNullOrWhiteSpace(question.SelectedAnswer) &&
                                         string.Equals(question.SelectedAnswer, question.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
                }

                solution.Questions = questions;
                return solution;
            });
        }

        public async Task<bool> SaveSessionAnswerAsync(int sessionId, int userId, SaveMockTestAnswerDto answer)
        {
            return await WithConnectionAsync(async connection =>
            {
                var session = await GetActiveSessionForActionAsync(connection, sessionId, userId);
                if (session == null)
                {
                    return false;
                }

                var sessionQuestions = (await connection.QueryAsync<(int QuestionId, int QuestionNumber)>(
                    @"SELECT QuestionId, QuestionNumber
                      FROM MockTestQuestions
                      WHERE MockTestId = @MockTestId
                      ORDER BY QuestionNumber",
                    new { MockTestId = (int)session.MockTestId })).ToList();

                var questionEntry = sessionQuestions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
                if (questionEntry == default)
                {
                    throw new InvalidOperationException("Question does not belong to this mock test session.");
                }

                var totalDurationInSeconds = GetTotalDurationInSeconds(sessionQuestions.Count);
                var elapsedSeconds = Math.Max(0, (int)(DateTime.Now - (DateTime)session.StartedAt).TotalSeconds);
                if (elapsedSeconds >= totalDurationInSeconds)
                {
                    throw new InvalidOperationException("Mock test time is over. This session can no longer accept answers.");
                }

                var questionExpiryInSeconds = questionEntry.QuestionNumber * DefaultPerQuestionTimeInSeconds;
                if (elapsedSeconds >= questionExpiryInSeconds)
                {
                    throw new InvalidOperationException("This question's time is over. Please move to the next question.");
                }

                var upsertSql = @"
                    UPDATE MockTestSessionAnswers
                    SET SelectedAnswer = @SelectedAnswer,
                        IsMarkedForReview = @IsMarkedForReview,
                        IsAnswered = @IsAnswered,
                        TimeSpent = @TimeSpent,
                        AnsweredAt = CASE WHEN @IsAnswered = 1 THEN GETDATE() ELSE AnsweredAt END
                    WHERE SessionId = @SessionId AND QuestionId = @QuestionId;

                    IF @@ROWCOUNT = 0
                    BEGIN
                        BEGIN TRY
                            INSERT INTO MockTestSessionAnswers (SessionId, QuestionId, SelectedAnswer, IsMarkedForReview, IsAnswered, TimeSpent, AnsweredAt)
                            VALUES (@SessionId, @QuestionId, @SelectedAnswer, @IsMarkedForReview, @IsAnswered, @TimeSpent,
                                CASE WHEN @IsAnswered = 1 THEN GETDATE() ELSE NULL END);
                        END TRY
                        BEGIN CATCH
                            IF ERROR_NUMBER() IN (2601, 2627)
                            BEGIN
                                UPDATE MockTestSessionAnswers
                                SET SelectedAnswer = @SelectedAnswer,
                                    IsMarkedForReview = @IsMarkedForReview,
                                    IsAnswered = @IsAnswered,
                                    TimeSpent = @TimeSpent,
                                    AnsweredAt = CASE WHEN @IsAnswered = 1 THEN GETDATE() ELSE AnsweredAt END
                                WHERE SessionId = @SessionId AND QuestionId = @QuestionId;
                            END
                            ELSE
                            BEGIN
                                THROW;
                            END
                        END CATCH
                    END";

                var isAnswered = !string.IsNullOrWhiteSpace(answer.SelectedAnswer);
                var timeSpent = Math.Min(
                    DefaultPerQuestionTimeInSeconds,
                    Math.Max(0, answer.TimeTakenInSeconds ?? (elapsedSeconds - ((questionEntry.QuestionNumber - 1) * DefaultPerQuestionTimeInSeconds))));

                await connection.ExecuteAsync(upsertSql, new
                {
                    SessionId = sessionId,
                    QuestionId = answer.QuestionId,
                    SelectedAnswer = answer.SelectedAnswer,
                    IsMarkedForReview = answer.IsMarkedForReview,
                    IsAnswered = isAnswered,
                    TimeSpent = timeSpent
                });

                return true;
            });
        }

        public async Task<MockTestQuestionActionResultDto> ReportQuestionAsync(int sessionId, int userId, ReportMockTestQuestionDto request)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sessionAnswerColumns = await GetSessionAnswerFeatureColumnsAsync(connection);
                if (!sessionAnswerColumns.HasIsReported)
                    throw new InvalidOperationException("Mock test report support is not available in the current database schema. Apply database/Alter_MockTestSessionAnswers_AddReportBookmark.sql.");

                var session = await GetActiveSessionForActionAsync(connection, sessionId, userId);
                if (session == null)
                    throw new KeyNotFoundException("Session not found or already completed");

                await EnsureQuestionBelongsToSessionAsync(connection, (int)session.MockTestId, request.QuestionId);

                var sql = @"
                    IF EXISTS (SELECT 1 FROM MockTestSessionAnswers WHERE SessionId = @SessionId AND QuestionId = @QuestionId)
                        UPDATE MockTestSessionAnswers
                        SET IsReported = 1,
                            ReportReason = @Reason,
                            ReportedAt = GETDATE()
                        WHERE SessionId = @SessionId AND QuestionId = @QuestionId
                    ELSE
                        INSERT INTO MockTestSessionAnswers
                            (SessionId, QuestionId, SelectedAnswer, IsMarkedForReview, IsAnswered, TimeSpent, AnsweredAt, IsReported, ReportReason, ReportedAt, IsBookmarked, BookmarkedAt)
                        VALUES
                            (@SessionId, @QuestionId, NULL, 0, 0, 0, NULL, 1, @Reason, GETDATE(), 0, NULL)";

                await connection.ExecuteAsync(sql, new
                {
                    SessionId = sessionId,
                    QuestionId = request.QuestionId,
                    Reason = string.IsNullOrWhiteSpace(request.Reason) ? "Reported from mock test session" : request.Reason.Trim()
                });

                return await GetQuestionActionSummaryAsync(connection, sessionId, request.QuestionId);
            });
        }

        public async Task<MockTestQuestionActionResultDto> BookmarkQuestionAsync(int sessionId, int userId, BookmarkMockTestQuestionDto request)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sessionAnswerColumns = await GetSessionAnswerFeatureColumnsAsync(connection);
                if (!sessionAnswerColumns.HasIsBookmarked)
                    throw new InvalidOperationException("Mock test bookmark support is not available in the current database schema. Apply database/Alter_MockTestSessionAnswers_AddReportBookmark.sql.");

                var session = await GetActiveSessionForActionAsync(connection, sessionId, userId);
                if (session == null)
                    throw new KeyNotFoundException("Session not found or already completed");

                await EnsureQuestionBelongsToSessionAsync(connection, (int)session.MockTestId, request.QuestionId);

                var sql = @"
                    IF EXISTS (SELECT 1 FROM MockTestSessionAnswers WHERE SessionId = @SessionId AND QuestionId = @QuestionId)
                        UPDATE MockTestSessionAnswers
                        SET IsBookmarked = @IsBookmarked,
                            BookmarkedAt = CASE WHEN @IsBookmarked = 1 THEN GETDATE() ELSE NULL END
                        WHERE SessionId = @SessionId AND QuestionId = @QuestionId
                    ELSE
                        INSERT INTO MockTestSessionAnswers
                            (SessionId, QuestionId, SelectedAnswer, IsMarkedForReview, IsAnswered, TimeSpent, AnsweredAt, IsReported, ReportReason, ReportedAt, IsBookmarked, BookmarkedAt)
                        VALUES
                            (@SessionId, @QuestionId, NULL, 0, 0, 0, NULL, 0, NULL, NULL, @IsBookmarked,
                                CASE WHEN @IsBookmarked = 1 THEN GETDATE() ELSE NULL END)";

                await connection.ExecuteAsync(sql, new
                {
                    SessionId = sessionId,
                    QuestionId = request.QuestionId,
                    IsBookmarked = request.IsBookmarked
                });

                return await GetQuestionActionSummaryAsync(connection, sessionId, request.QuestionId);
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
            var sessionAnswerColumns = await GetSessionAnswerFeatureColumnsAsync(connection, transaction);
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
            decimal totalMarks = 0;
            decimal obtainedMarks = 0;
            decimal negativeMarksDeducted = 0;

            foreach (var qr in questionResults)
            {
                totalMarks += qr.Marks;
                bool isCorrect = qr.SelectedAnswer == qr.CorrectAnswer;
                if (isCorrect)
                {
                    correctAnswers++;
                    obtainedMarks += qr.Marks;
                }
                else if (!string.IsNullOrEmpty(qr.SelectedAnswer))
                {
                    wrongAnswers++;
                    var negativeMarks = qr.NegativeMarks ?? 0;
                    negativeMarksDeducted += negativeMarks;
                    obtainedMarks -= negativeMarks;
                }
            }

            var answeredQuestions = questionResults.Count(q => !string.IsNullOrEmpty((string?)q.SelectedAnswer));
            obtainedMarks = Math.Max(0, obtainedMarks);
            var percentage = totalMarks > 0 ? (obtainedMarks / totalMarks) * 100 : 0;
            var grade = GetGrade(percentage);
            var reportedQuestionIds = sessionAnswerColumns.HasIsReported
                ? (await connection.QueryAsync<int>(
                    @"SELECT QuestionId
                      FROM MockTestSessionAnswers
                      WHERE SessionId = @SessionId AND ISNULL(IsReported, 0) = 1
                      ORDER BY QuestionId",
                    new { SessionId = sessionId }, transaction)).ToList()
                : new List<int>();
            var bookmarkedQuestionIds = sessionAnswerColumns.HasIsBookmarked
                ? (await connection.QueryAsync<int>(
                    @"SELECT QuestionId
                      FROM MockTestSessionAnswers
                      WHERE SessionId = @SessionId AND ISNULL(IsBookmarked, 0) = 1
                      ORDER BY QuestionId",
                    new { SessionId = sessionId }, transaction)).ToList()
                : new List<int>();

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
                NegativeMarksDeducted = negativeMarksDeducted,
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
                NegativeMarksDeducted = negativeMarksDeducted,
                ReportedQuestionsCount = reportedQuestionIds.Count,
                BookmarkedQuestionsCount = bookmarkedQuestionIds.Count,
                ReportedQuestionIds = reportedQuestionIds,
                BookmarkedQuestionIds = bookmarkedQuestionIds,
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
                        SUM(CASE WHEN mt.Status = 'Active' THEN 1 ELSE 0 END) AS ActiveMockTests,
                        SUM(CASE WHEN mt.Status = 'Scheduled' THEN 1 ELSE 0 END) AS ScheduledMockTests,
                        SUM(CASE WHEN mt.Status = 'Draft' THEN 1 ELSE 0 END) AS DraftMockTests,
                        SUM(CASE WHEN mt.AccessType = 'Paid' THEN 1 ELSE 0 END) AS PaidMockTests,
                        COUNT(*) AS MockTestCount,
                        SUM(CASE WHEN mt.MockTestType = 2 THEN 1 ELSE 0 END) AS TestSeriesCount,
                        SUM(CASE WHEN mt.MockTestType = 3 THEN 1 ELSE 0 END) AS DeepPracticeCount,
                        SUM(CASE WHEN mt.MockTestType = 4 THEN 1 ELSE 0 END) AS PreviousYearCount
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

        private static int GetTotalDurationInSeconds(int totalQuestions)
        {
            return Math.Max(0, totalQuestions) * DefaultPerQuestionTimeInSeconds;
        }

        private static async Task<dynamic?> GetActiveSessionForActionAsync(System.Data.IDbConnection connection, int sessionId, int userId)
        {
            const string sql = @"
                SELECT TOP 1
                    Id,
                    StartedAt,
                    MockTestId,
                    Status
                FROM MockTestSessions
                WHERE Id = @SessionId AND UserId = @UserId AND Status = 'InProgress'";

            return await connection.QuerySingleOrDefaultAsync(sql, new { SessionId = sessionId, UserId = userId });
        }

        private static async Task EnsureQuestionBelongsToSessionAsync(System.Data.IDbConnection connection, int mockTestId, int questionId)
        {
            var exists = await connection.QuerySingleAsync<int>(
                @"SELECT COUNT(1)
                  FROM MockTestQuestions
                  WHERE MockTestId = @MockTestId AND QuestionId = @QuestionId",
                new { MockTestId = mockTestId, QuestionId = questionId });

            if (exists == 0)
            {
                throw new InvalidOperationException("Question does not belong to this mock test session.");
            }
        }

        private static async Task<MockTestQuestionActionResultDto> GetQuestionActionSummaryAsync(System.Data.IDbConnection connection, int sessionId, int questionId)
        {
            var sessionAnswerColumns = await GetSessionAnswerFeatureColumnsAsync(connection);
            var summarySql = $@"
                SELECT
                    @SessionId AS SessionId,
                    @QuestionId AS QuestionId,
                    {(sessionAnswerColumns.HasIsReported ? "ISNULL(MAX(CASE WHEN msa.QuestionId = @QuestionId THEN msa.IsReported END), 0)" : "CAST(0 AS BIT)")} AS IsReported,
                    {(sessionAnswerColumns.HasIsBookmarked ? "ISNULL(MAX(CASE WHEN msa.QuestionId = @QuestionId THEN msa.IsBookmarked END), 0)" : "CAST(0 AS BIT)")} AS IsBookmarked,
                    {(sessionAnswerColumns.HasIsReported ? "SUM(CASE WHEN ISNULL(msa.IsReported, 0) = 1 THEN 1 ELSE 0 END)" : "0")} AS ReportedQuestionsCount,
                    {(sessionAnswerColumns.HasIsBookmarked ? "SUM(CASE WHEN ISNULL(msa.IsBookmarked, 0) = 1 THEN 1 ELSE 0 END)" : "0")} AS BookmarkedQuestionsCount,
                    {(sessionAnswerColumns.HasReportReason ? "MAX(CASE WHEN msa.QuestionId = @QuestionId THEN msa.ReportReason END)" : "CAST(NULL AS NVARCHAR(500))")} AS Reason
                FROM MockTestSessionAnswers msa
                WHERE msa.SessionId = @SessionId";

            return await connection.QuerySingleAsync<MockTestQuestionActionResultDto>(summarySql, new
            {
                SessionId = sessionId,
                QuestionId = questionId
            });
        }

        private static (DateTime Start, DateTime End) GetQuestionWindow(DateTime sessionStartedAt, int zeroBasedQuestionIndex)
        {
            var start = sessionStartedAt.AddSeconds(zeroBasedQuestionIndex * DefaultPerQuestionTimeInSeconds);
            var end = start.AddSeconds(DefaultPerQuestionTimeInSeconds);
            return (start, end);
        }

        private sealed class SessionAnswerFeatureColumns
        {
            public bool HasIsReported { get; init; }
            public bool HasIsBookmarked { get; init; }
            public bool HasReportReason { get; init; }
        }

        private static async Task<SessionAnswerFeatureColumns> GetSessionAnswerFeatureColumnsAsync(System.Data.IDbConnection connection, System.Data.IDbTransaction? transaction = null)
        {
            var columns = (await connection.QueryAsync<string>(
                @"SELECT c.name
                  FROM sys.columns c
                  WHERE c.object_id = OBJECT_ID('dbo.MockTestSessionAnswers')
                    AND c.name IN ('IsReported', 'IsBookmarked', 'ReportReason')",
                transaction: transaction)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            return new SessionAnswerFeatureColumns
            {
                HasIsReported = columns.Contains("IsReported"),
                HasIsBookmarked = columns.Contains("IsBookmarked"),
                HasReportReason = columns.Contains("ReportReason")
            };
        }

        private static string GetOptionText(string? answerKey, MockTestSolutionQuestionDto question)
        {
            if (string.IsNullOrWhiteSpace(answerKey))
            {
                return string.Empty;
            }

            return answerKey.Trim().ToUpperInvariant() switch
            {
                "A" => question.OptionA ?? string.Empty,
                "B" => question.OptionB ?? string.Empty,
                "C" => question.OptionC ?? string.Empty,
                "D" => question.OptionD ?? string.Empty,
                _ => string.Empty
            };
        }

        private static async Task<MockTestAttemptDto> CreateMockTestAttemptDtoAsync(System.Data.IDbConnection connection, dynamic attempt, int userId)
        {
            var reportedQuestionIds = (await connection.QueryAsync<int>(
                @"SELECT QuestionId
                  FROM MockTestSessionAnswers
                  WHERE SessionId IN (
                      SELECT Id FROM MockTestSessions 
                      WHERE MockTestId = @MockTestId AND UserId = @UserId AND StartedAt = @StartedAt
                  ) AND ISNULL(IsReported, 0) = 1
                  ORDER BY QuestionId",
                new { MockTestId = attempt.MockTestId, UserId = userId, StartedAt = attempt.StartedAt })).ToList();
            
            var bookmarkedQuestionIds = (await connection.QueryAsync<int>(
                @"SELECT QuestionId
                  FROM MockTestSessionAnswers
                  WHERE SessionId IN (
                      SELECT Id FROM MockTestSessions 
                      WHERE MockTestId = @MockTestId AND UserId = @UserId AND StartedAt = @StartedAt
                  ) AND ISNULL(IsBookmarked, 0) = 1
                  ORDER BY QuestionId",
                new { MockTestId = attempt.MockTestId, UserId = userId, StartedAt = attempt.StartedAt })).ToList();
            
            var negativeMarksDeducted = await connection.QuerySingleAsync<decimal>(
                @"SELECT ISNULL(SUM(ISNULL(mtq.NegativeMarks, 0)), 0)
                  FROM MockTestSessionAnswers mtsa
                  INNER JOIN MockTestSessions mts ON mts.Id = mtsa.SessionId
                  INNER JOIN MockTestQuestions mtq
                      ON mtq.MockTestId = mts.MockTestId
                     AND mtq.QuestionId = mtsa.QuestionId
                  WHERE mts.MockTestId = @MockTestId
                    AND mts.UserId = @UserId
                    AND mts.StartedAt = @StartedAt
                    AND ISNULL(mtsa.IsAnswered, 0) = 1
                    AND ISNULL(mtsa.SelectedAnswer, '') <> ''
                    AND NOT EXISTS (
                        SELECT 1
                        FROM Questions q
                        WHERE q.Id = mtsa.QuestionId
                          AND q.CorrectAnswer = mtsa.SelectedAnswer
                    )",
                new { MockTestId = attempt.MockTestId, UserId = userId, StartedAt = attempt.StartedAt });

            return new MockTestAttemptDto
            {
                Id = attempt.Id,
                MockTestId = attempt.MockTestId,
                UserId = attempt.UserId,
                StartedAt = attempt.StartedAt,
                CompletedAt = attempt.CompletedAt,
                Duration = attempt.Duration != null ? TimeSpan.FromMinutes((int)attempt.Duration) : null,
                TotalQuestions = attempt.TotalQuestions,
                AnsweredQuestions = attempt.AnsweredQuestions,
                CorrectAnswers = attempt.CorrectAnswers,
                WrongAnswers = attempt.WrongAnswers,
                ObtainedMarks = attempt.ObtainedMarks,
                NegativeMarksDeducted = negativeMarksDeducted,
                ReportedQuestionsCount = reportedQuestionIds.Count,
                BookmarkedQuestionsCount = bookmarkedQuestionIds.Count,
                ReportedQuestionIds = reportedQuestionIds,
                BookmarkedQuestionIds = bookmarkedQuestionIds,
                Percentage = attempt.Percentage,
                Status = attempt.Status,
                Grade = attempt.Grade
            };
        }
    }
}
