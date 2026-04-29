using Dapper;
using Microsoft.Data.SqlClient;
using QuestionService.Application.Interfaces;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Entities;
using System.Data;
using System.Text.Json;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionDapperRepository : BaseDapperRepository, IQuestionRepository
    {
        public QuestionDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<Question>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_GetAll]";
                return await connection.QueryAsync<Question>(sql);
            });
        }

        public async Task<IEnumerable<Question>> GetByChapterIdAsync(int chapterId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_GetByChapterId] @ChapterId";
                return await connection.QueryAsync<Question>(sql, new { ChapterId = chapterId });
            });
        }

        public async Task<Question> AddAsync(Question question)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Question_Create] 
                        @QuestionText, @QuestionType, @Difficulty, @ChapterId, 
                        @SubjectId, @ExamId, @Explanation, @ImageUrl, @VideoUrl,
                        @Tags, @Points, @TimeLimit, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@QuestionText", question.QuestionText);
                parameters.Add("@QuestionType", question.QuestionType);
                parameters.Add("@Difficulty", question.DifficultyLevel);
                parameters.Add("@ChapterId", question.TopicId);
                parameters.Add("@SubjectId", question.SubjectId);
                parameters.Add("@ExamId", question.ExamId);
                parameters.Add("@Explanation", question.Explanation);
                parameters.Add("@ImageUrl", question.QuestionImageUrl);
                parameters.Add("@VideoUrl", null);
                parameters.Add("@Tags", question.Tags);
                parameters.Add("@Points", question.Marks);
                parameters.Add("@TimeLimit", null);
                parameters.Add("@IsActive", question.IsActive);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    question.Id = parameters.Get<int>("@Id");
                }

                return question;
            });
        }

        public async Task UpdateAsync(Question question)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Question_Update] 
                        @Id, @QuestionText, @QuestionType, @Difficulty, @ChapterId, 
                        @SubjectId, @ExamId, @Explanation, @ImageUrl, @VideoUrl,
                        @Tags, @Points, @TimeLimit, @IsActive, @UpdatedAt";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", question.Id);
                parameters.Add("@QuestionText", question.QuestionText);
                parameters.Add("@QuestionType", question.QuestionType);
                parameters.Add("@Difficulty", question.DifficultyLevel);
                parameters.Add("@ChapterId", question.TopicId);
                parameters.Add("@SubjectId", question.SubjectId);
                parameters.Add("@ExamId", question.ExamId);
                parameters.Add("@Explanation", question.Explanation);
                parameters.Add("@ImageUrl", question.QuestionImageUrl);
                parameters.Add("@VideoUrl", null);
                parameters.Add("@Tags", question.Tags);
                parameters.Add("@Points", question.Marks);
                parameters.Add("@TimeLimit", null);
                parameters.Add("@IsActive", question.IsActive);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync(sql, parameters);
            });
        }

        public async Task DeleteAsync(Question question)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = question.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }

        public async Task<int> CreateTopicAsync(CreateTopicDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Topic_Create] @Name, @SubjectId, @ExamId";
                return await connection.QueryFirstAsync<int>(sql, new
                {
                    dto.Name,
                    SubjectId = dto.SubjectId,
                    ExamId = 0 // Not available in CreateTopicDto
                });
            });
        }

        public async Task<IEnumerable<TopicDto>> GetTopicsAsync(int? subjectId, int? examId, bool includeInactive)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Topic_GetAll] @SubjectId, @ExamId, @IncludeInactive";
                return await connection.QueryAsync<TopicDto>(sql, new
                {
                    SubjectId = subjectId,
                    ExamId = examId,
                    IncludeInactive = includeInactive
                });
            });
        }

        public async Task<int> CreateAdminQuestionAsync(CreateQuestionRequestDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var translations = dto.Translations?.Any() == true
                    ? dto.Translations
                    : new List<QuestionTranslationUpsertDto>
                    {
                        new()
                        {
                            LanguageCode = "en",
                            QuestionText = dto.QuestionText,
                            OptionA = dto.OptionA,
                            OptionB = dto.OptionB,
                            OptionC = dto.OptionC,
                            OptionD = dto.OptionD,
                            Explanation = dto.Explanation,
                            QuestionImageUrl = dto.QuestionImageUrl,
                            OptionAImageUrl = dto.OptionAImageUrl,
                            OptionBImageUrl = dto.OptionBImageUrl,
                            OptionCImageUrl = dto.OptionCImageUrl,
                            OptionDImageUrl = dto.OptionDImageUrl
                        }
                    };
                var translationsJson = JsonSerializer.Serialize(translations);

                var sql = "EXEC [dbo].[Question_AdminCreate] @ModuleId, @ExamId, @SubjectId, @TopicId, @Marks, @NegativeMarks, @Difficulty, @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, @TranslationsJson, @CreatedBy";
                
                var parameters = new
                {
                    dto.ModuleId,
                    dto.ExamId,
                    dto.SubjectId,
                    dto.TopicId,
                    dto.Marks,
                    dto.NegativeMarks,
                    Difficulty = (int)Enum.Parse(typeof(DifficultyLevel), dto.DifficultyLevel ?? "Medium", ignoreCase: true),
                    dto.CorrectAnswer,
                    dto.SameExplanationForAllLanguages,
                    IsPublished = true, // Default to published for admin questions
                    TranslationsJson = translationsJson,
                    dto.CreatedBy
                };
                
                return await connection.QueryFirstAsync<int>(sql, parameters);
            });
        }

        public async Task<bool> UpdateAdminQuestionAsync(UpdateQuestionAdminDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_AdminUpdate] @Id, @ModuleId, @ExamId, @SubjectId, @TopicId, @Marks, @NegativeMarks, @Difficulty, @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, @IsActive, @TranslationsJson";
                var normalizedTopicId = dto.TopicId <= 0 ? (int?)null : dto.TopicId;
                var rows = await connection.QueryFirstAsync<int>(sql, new
                {
                    dto.Id,
                    dto.ModuleId,
                    dto.ExamId,
                    dto.SubjectId,
                    TopicId = normalizedTopicId,
                    dto.Marks,
                    dto.NegativeMarks,
                    Difficulty = (int)Enum.Parse(typeof(DifficultyLevel), dto.DifficultyLevel ?? "Medium", ignoreCase: true),
                    dto.CorrectAnswer,
                    dto.SameExplanationForAllLanguages,
                    dto.IsPublished,
                    dto.IsActive,
                    TranslationsJson = JsonSerializer.Serialize(dto.Translations)
                });

                return rows > 0;
            });
        }

        public async Task<QuestionAdminDetailDto?> GetAdminQuestionByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_AdminGetById] @Id";
                using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });
                var question = await multi.ReadFirstOrDefaultAsync<QuestionAdminDetailDto>();
                if (question == null)
                {
                    return null;
                }

                var translations = await multi.ReadAsync<QuestionTranslationDto>();
                question.Translations = translations.ToList();
                return question;
            });
        }

        public async Task<(IEnumerable<QuestionAdminListItemDto> Items, int TotalCount)> GetAdminQuestionsPagedAsync(QuestionFilterRequestDto filter)
        {
            return await WithConnectionAsync(async connection =>
            {
                var hasModulesTable = await connection.ExecuteScalarAsync<int>(@"
                    SELECT CASE
                        WHEN DB_ID(N'RankUp_MasterDB') IS NOT NULL
                         AND EXISTS
                         (
                             SELECT 1
                             FROM [RankUp_MasterDB].sys.tables t
                             INNER JOIN [RankUp_MasterDB].sys.schemas s ON s.schema_id = t.schema_id
                             WHERE s.name = N'dbo' AND t.name = N'Modules'
                         )
                        THEN 1 ELSE 0 END;");

                var moduleNameSelect = hasModulesTable == 1
                    ? "mod.Name AS ModuleName,"
                    : "CAST(NULL AS NVARCHAR(200)) AS ModuleName,";

                var moduleJoin = hasModulesTable == 1
                    ? @"OUTER APPLY
                        (
                            SELECT TOP 1 m.Name
                            FROM [RankUp_MasterDB].[dbo].[Modules] m
                            WHERE m.Id = f.ModuleId
                        ) mod"
                    : string.Empty;

                // Add MockTestName lookup
                var mockTestNameSelect = "COALESCE(mt.Name, CAST(NULL AS NVARCHAR(200))) AS MockTestName,";

                var mockTestJoin = @"LEFT JOIN dbo.MockTestQuestions mtq ON mtq.QuestionId = f.Id
                    LEFT JOIN dbo.MockTests mt ON mt.Id = mtq.MockTestId AND mt.IsActive = 1";

                // If MockTestId is specified, use custom query to filter by MockTestQuestions
                if (filter.MockTestId.HasValue)
                {
                    var sql = $@"
                        WITH Filtered AS
                        (
                            SELECT q.Id, q.ModuleId, q.ExamId, q.SubjectId, q.TopicId, q.Difficulty, q.Marks, q.NegativeMarks, q.IsPublished, q.IsActive, q.CreatedAt
                            FROM dbo.Questions q
                            INNER JOIN dbo.MockTestQuestions mtq ON q.Id = mtq.QuestionId
                            WHERE mtq.MockTestId = @MockTestId
                              AND (@IncludeInactive = 1 OR q.IsActive = 1)
                              AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
                              AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
                              AND (@ExamId IS NULL OR q.ExamId = @ExamId)
                              AND (@TopicId IS NULL OR q.TopicId = @TopicId)
                              AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
                              AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished)
                        )
                        SELECT
                            f.Id,
                            f.ModuleId,
                            {moduleNameSelect}
                            {mockTestNameSelect}
                            f.ExamId,
                            e.Name AS ExamName,
                            f.SubjectId,
                            s.Name AS SubjectName,
                            f.TopicId,
                            tp.Name AS TopicName,
                            CASE f.Difficulty WHEN 1 THEN 'Easy' WHEN 2 THEN 'Medium' WHEN 3 THEN 'Hard' ELSE 'Medium' END AS DifficultyLevel,
                            f.Marks,
                            f.NegativeMarks,
                            f.IsPublished,
                            f.IsActive,
                            f.CreatedAt,
                            mtq.MockTestId,
                            COALESCE(NULLIF(t.QuestionText, ''), q.QuestionText, '') AS QuestionText,
                            COALESCE(NULLIF(t.OptionA, ''), q.OptionA) AS OptionA,
                            COALESCE(NULLIF(t.OptionB, ''), q.OptionB) AS OptionB,
                            COALESCE(NULLIF(t.OptionC, ''), q.OptionC) AS OptionC,
                            COALESCE(NULLIF(t.OptionD, ''), q.OptionD) AS OptionD,
                            q.CorrectAnswer,
                            COALESCE(NULLIF(t.Explanation, ''), q.Explanation) AS Explanation,
                            q.QuestionType,
                            q.QuestionImageUrl,
                            q.OptionAImageUrl,
                            q.OptionBImageUrl,
                            q.OptionCImageUrl,
                            q.OptionDImageUrl,
                            q.ExplanationImageUrl,
                            q.SameExplanationForAllLanguages,
                            q.Reference,
                            q.Tags,
                            q.CreatedBy,
                            q.ReviewedBy,
                            q.UpdatedAt,
                            q.PublishDate,
                            COALESCE(NULLIF(t.QuestionText, ''), q.QuestionText, '') AS DisplayQuestionText,
                            ISNULL(t.LanguageCode, 'en') AS LanguageCode,
                            t.OptionA AS TranslatedOptionA,
                            t.OptionB AS TranslatedOptionB,
                            t.OptionC AS TranslatedOptionC,
                            t.OptionD AS TranslatedOptionD,
                            t.Explanation AS TranslatedExplanation
                        FROM Filtered f
                        INNER JOIN dbo.Questions q ON q.Id = f.Id
                        INNER JOIN dbo.MockTestQuestions mtq ON mtq.QuestionId = f.Id AND mtq.MockTestId = @MockTestId
                        {moduleJoin}
                        {mockTestJoin}
                        LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON e.Id = f.ExamId
                        LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON s.Id = f.SubjectId
                        LEFT JOIN dbo.Topics tp ON tp.Id = f.TopicId
                        OUTER APPLY
                        (
                            SELECT TOP 1 qt.QuestionText, qt.LanguageCode, qt.OptionA, qt.OptionB, qt.OptionC, qt.OptionD, qt.Explanation
                            FROM dbo.QuestionTranslations qt
                            WHERE qt.QuestionId = f.Id
                              AND (@LanguageCode IS NULL OR qt.LanguageCode = @LanguageCode)
                            ORDER BY CASE WHEN qt.LanguageCode = ISNULL(@LanguageCode, 'en') THEN 0 ELSE 1 END
                        ) t
                        ORDER BY f.CreatedAt DESC
                        OFFSET (@PageNumber - 1) * @PageSize ROWS
                        FETCH NEXT @PageSize ROWS ONLY;

                        SELECT COUNT(1)
                        FROM dbo.Questions q
                        INNER JOIN dbo.MockTestQuestions mtq ON q.Id = mtq.QuestionId
                        WHERE mtq.MockTestId = @MockTestId
                          AND (@IncludeInactive = 1 OR q.IsActive = 1)
                          AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
                          AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
                          AND (@ExamId IS NULL OR q.ExamId = @ExamId)
                          AND (@TopicId IS NULL OR q.TopicId = @TopicId)
                          AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
                          AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished)";

                    using var multi = await connection.QueryMultipleAsync(sql, new
                    {
                        filter.MockTestId,
                        filter.PageNumber,
                        filter.PageSize,
                        filter.ModuleId,
                        filter.SubjectId,
                        filter.ExamId,
                        filter.TopicId,
                        Difficulty = !string.IsNullOrEmpty(filter.DifficultyLevel) ? (int?)Enum.Parse(typeof(DifficultyLevel), filter.DifficultyLevel) : null,
                        filter.LanguageCode,
                        filter.IsPublished,
                        filter.IncludeInactive
                    });

                    var items = await multi.ReadAsync<QuestionAdminListItemDto>();
                    var total = await multi.ReadFirstOrDefaultAsync<int>();
                    return (items, total);
                }
                else
                {
                    var sql = $@"
                        WITH FilteredQuestions AS
                        (
                            SELECT
                                q.Id,
                                q.ModuleId,
                                q.ExamId,
                                q.SubjectId,
                                q.TopicId,
                                q.Difficulty,
                                q.Marks,
                                q.NegativeMarks,
                                q.IsPublished,
                                q.IsActive,
                                q.CreatedAt,
                                ROW_NUMBER() OVER (ORDER BY q.CreatedAt DESC) AS RowNum
                            FROM dbo.Questions q
                            WHERE (@IncludeInactive = 1 OR q.IsActive = 1)
                              AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
                              AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
                              AND (@ExamId IS NULL OR q.ExamId = @ExamId)
                              AND (@TopicId IS NULL OR q.TopicId = @TopicId)
                              AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
                              AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished)
                        )
                        SELECT
                            f.Id,
                            f.ModuleId,
                            {moduleNameSelect}
                            {mockTestNameSelect}
                            f.ExamId,
                            e.Name AS ExamName,
                            f.SubjectId,
                            s.Name AS SubjectName,
                            f.TopicId,
                            tp.Name AS TopicName,
                            CASE f.Difficulty WHEN 1 THEN 'Easy' WHEN 2 THEN 'Medium' WHEN 3 THEN 'Hard' ELSE 'Medium' END AS DifficultyLevel,
                            f.Marks,
                            f.NegativeMarks,
                            f.IsPublished,
                            f.IsActive,
                            f.CreatedAt,
                            mtq.MockTestId,
                            COALESCE(NULLIF(qt.QuestionText, ''), q.QuestionText, '') AS QuestionText,
                            COALESCE(NULLIF(qt.OptionA, ''), q.OptionA) AS OptionA,
                            COALESCE(NULLIF(qt.OptionB, ''), q.OptionB) AS OptionB,
                            COALESCE(NULLIF(qt.OptionC, ''), q.OptionC) AS OptionC,
                            COALESCE(NULLIF(qt.OptionD, ''), q.OptionD) AS OptionD,
                            q.CorrectAnswer,
                            COALESCE(NULLIF(qt.Explanation, ''), q.Explanation) AS Explanation,
                            q.QuestionType,
                            q.QuestionImageUrl,
                            q.OptionAImageUrl,
                            q.OptionBImageUrl,
                            q.OptionCImageUrl,
                            q.OptionDImageUrl,
                            q.ExplanationImageUrl,
                            q.SameExplanationForAllLanguages,
                            q.Reference,
                            q.Tags,
                            q.CreatedBy,
                            q.ReviewedBy,
                            q.UpdatedAt,
                            q.PublishDate,
                            COALESCE(NULLIF(qt.QuestionText, ''), q.QuestionText, '') AS DisplayQuestionText,
                            ISNULL(qt.LanguageCode, 'en') AS LanguageCode,
                            qt.OptionA AS TranslatedOptionA,
                            qt.OptionB AS TranslatedOptionB,
                            qt.OptionC AS TranslatedOptionC,
                            qt.OptionD AS TranslatedOptionD,
                            qt.Explanation AS TranslatedExplanation
                        FROM FilteredQuestions f
                        INNER JOIN dbo.Questions q ON f.Id = q.Id
                        {moduleJoin}
                        {mockTestJoin}
                        LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON e.Id = f.ExamId
                        LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON s.Id = f.SubjectId
                        LEFT JOIN dbo.Topics tp ON tp.Id = f.TopicId
                        OUTER APPLY
                        (
                            SELECT TOP 1 qt.QuestionText, qt.LanguageCode, qt.OptionA, qt.OptionB, qt.OptionC, qt.OptionD, qt.Explanation
                            FROM dbo.QuestionTranslations qt
                            WHERE qt.QuestionId = f.Id
                              AND (@LanguageCode IS NULL OR qt.LanguageCode = @LanguageCode)
                            ORDER BY CASE WHEN qt.LanguageCode = ISNULL(@LanguageCode, 'en') THEN 0 ELSE 1 END
                        ) qt
                        WHERE f.RowNum > (@PageNumber - 1) * @PageSize
                          AND f.RowNum <= @PageNumber * @PageSize
                        ORDER BY f.CreatedAt DESC;

                        SELECT COUNT(1)
                        FROM dbo.Questions q
                        WHERE (@IncludeInactive = 1 OR q.IsActive = 1)
                          AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
                          AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
                          AND (@ExamId IS NULL OR q.ExamId = @ExamId)
                          AND (@TopicId IS NULL OR q.TopicId = @TopicId)
                          AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
                          AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished);";
                    using var multi = await connection.QueryMultipleAsync(sql, new
                    {
                        filter.PageNumber,
                        filter.PageSize,
                        filter.ModuleId,
                        filter.SubjectId,
                        filter.ExamId,
                        filter.TopicId,
                        Difficulty = !string.IsNullOrEmpty(filter.DifficultyLevel) ? (int?)Enum.Parse(typeof(DifficultyLevel), filter.DifficultyLevel) : null,
                        filter.LanguageCode,
                        filter.IsPublished,
                        filter.IncludeInactive
                    });

                    var items = await multi.ReadAsync<QuestionAdminListItemDto>();
                    var total = await multi.ReadFirstOrDefaultAsync<int>();
                    return (items, total);
                }
            });
        }

        public async Task<QuestionDashboardStatsDto> GetDashboardStatsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_AdminDashboardStats]";
                var stats = await connection.QueryFirstOrDefaultAsync<QuestionDashboardStatsDto>(sql);
                return stats ?? new QuestionDashboardStatsDto();
            });
        }

        public async Task<bool> SetPublishStatusAsync(int id, bool isPublished)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_SetPublishStatus] @Id, @IsPublished";
                var rows = await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    IsPublished = isPublished
                });

                return rows > 0;
            });
        }

        public async Task<int> BulkCreateQuestionsAsync(IEnumerable<CreateQuestionAdminDto> questions)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_BulkCreate] @QuestionsJson";
                return await connection.QueryFirstAsync<int>(sql, new
                {
                    QuestionsJson = JsonSerializer.Serialize(questions)
                });
            });
        }

        public async Task<bool> UpdateQuestionImageUrlsAsync(int questionId, string? questionImageUrl, string? optionAImageUrl, string? optionBImageUrl, string? optionCImageUrl, string? optionDImageUrl, string? explanationImageUrl)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE Questions SET
                        QuestionImageUrl = @QuestionImageUrl,
                        OptionAImageUrl = @OptionAImageUrl,
                        OptionBImageUrl = @OptionBImageUrl,
                        OptionCImageUrl = @OptionCImageUrl,
                        OptionDImageUrl = @OptionDImageUrl,
                        ExplanationImageUrl = @ExplanationImageUrl,
                        UpdatedAt = GETDATE()
                    WHERE Id = @QuestionId AND IsActive = 1";

                var rows = await connection.ExecuteAsync(sql, new
                {
                    QuestionId = questionId,
                    QuestionImageUrl = questionImageUrl,
                    OptionAImageUrl = optionAImageUrl,
                    OptionBImageUrl = optionBImageUrl,
                    OptionCImageUrl = optionCImageUrl,
                    OptionDImageUrl = optionDImageUrl,
                    ExplanationImageUrl = explanationImageUrl
                });

                return rows > 0;
            });
        }

        public async Task<(IEnumerable<QuestionAdminListItemDto> Items, int TotalCount)> GetAllAdminQuestionsAsync(QuestionFilterRequestDto filter)
        {
            return await WithConnectionAsync(async connection =>
            {
                var hasModulesTable = await connection.ExecuteScalarAsync<int>(@"
                    SELECT CASE
                        WHEN DB_ID(N'RankUp_MasterDB') IS NOT NULL
                         AND EXISTS
                         (
                             SELECT 1
                             FROM [RankUp_MasterDB].sys.tables t
                             INNER JOIN [RankUp_MasterDB].sys.schemas s ON s.schema_id = t.schema_id
                             WHERE s.name = N'dbo' AND t.name = N'Modules'
                         )
                        THEN 1 ELSE 0 END;");

                var moduleNameSelect = hasModulesTable == 1
                    ? "mod.Name AS ModuleName,"
                    : "CAST(NULL AS NVARCHAR(200)) AS ModuleName,";

                var moduleJoin = hasModulesTable == 1
                    ? @"OUTER APPLY
                        (
                            SELECT TOP 1 m.Name
                            FROM [RankUp_MasterDB].[dbo].[Modules] m
                            WHERE m.Id = f.ModuleId
                        ) mod"
                    : string.Empty;

                // IMPORTANT:
                // This method is used by the "grouped" admin listing endpoint (questions grouped by module/subject).
                // When we are NOT filtering by MockTestId, joining MockTestQuestions causes duplicate rows for the same
                // question (a question can be mapped to multiple mock tests). That confuses the UI and pagination.
                //
                // So:
                // - If MockTestId is provided, include the join and return MockTestId/MockTestName
                // - Otherwise, do NOT join and return NULL for mock test fields

                var includeMockTestJoin = filter.MockTestId.HasValue && filter.MockTestId.Value > 0;

                var mockTestNameSelect = includeMockTestJoin
                    ? "COALESCE(mt.Name, CAST(NULL AS NVARCHAR(200))) AS MockTestName,"
                    : "CAST(NULL AS NVARCHAR(200)) AS MockTestName,";

                var mockTestIdSelect = includeMockTestJoin
                    ? "mtq.MockTestId,"
                    : "CAST(NULL AS INT) AS MockTestId,";

                var mockTestJoin = includeMockTestJoin
                    ? @"INNER JOIN dbo.MockTestQuestions mtq ON mtq.QuestionId = f.Id AND mtq.MockTestId = @MockTestId
                        LEFT JOIN dbo.MockTests mt ON mt.Id = mtq.MockTestId AND mt.IsActive = 1"
                    : string.Empty;

                var sql = $@"
                    SELECT 
                        f.Id,
                        f.ModuleId,
                        {moduleNameSelect}
                        f.ExamId,
                        e.Name AS ExamName,
                        f.SubjectId,
                        s.Name AS SubjectName,
                        f.TopicId,
                        NULL AS TopicName,
                        CASE f.Difficulty WHEN 1 THEN 'Easy' WHEN 2 THEN 'Medium' WHEN 3 THEN 'Hard' ELSE 'Medium' END AS DifficultyLevel,
                        f.Marks,
                        f.NegativeMarks,
                        f.IsPublished,
                        f.IsActive,
                        f.CreatedAt,
                        {mockTestIdSelect}
                        {mockTestNameSelect}
                        f.QuestionText,
                        f.OptionA,
                        f.OptionB,
                        f.OptionC,
                        f.OptionD,
                        f.CorrectAnswer,
                        f.Explanation,
                        f.QuestionType,
                        f.QuestionImageUrl,
                        f.OptionAImageUrl,
                        f.OptionBImageUrl,
                        f.OptionCImageUrl,
                        f.OptionDImageUrl,
                        f.ExplanationImageUrl,
                        f.SameExplanationForAllLanguages,
                        f.Reference,
                        f.Tags,
                        f.CreatedBy,
                        f.ReviewedBy,
                        f.PublishDate,
                        f.PublishedAt,
                        f.UpdatedAt
                    FROM dbo.Questions f
                    LEFT JOIN dbo.QuestionTranslations qt ON f.Id = qt.QuestionId AND qt.LanguageCode = @LanguageCode
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON f.ExamId = e.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON f.SubjectId = s.Id
                    -- LEFT JOIN [RankUp_MasterDB].[dbo].[Topics] tp ON f.TopicId = tp.Id -- Topics table doesn't exist
                    {mockTestJoin}
                    {moduleJoin}
                    WHERE 1=1 -- Always true since we're filtering by QuestionTranslations
                      AND (@IncludeInactive = 1 OR f.IsActive = 1)
                      AND (@ExamId IS NULL OR f.ExamId = @ExamId)
                      AND (@SubjectId IS NULL OR f.SubjectId = @SubjectId)
                      AND (@ModuleId IS NULL OR f.ModuleId = @ModuleId)
                      AND (@IsPublished IS NULL OR f.IsPublished = @IsPublished)
                    ORDER BY f.CreatedAt DESC";

                var questions = await connection.QueryAsync<QuestionAdminListItemDto>(sql, new
                {
                    filter.LanguageCode,
                    filter.IncludeInactive,
                    filter.ExamId,
                    filter.SubjectId,
                    filter.ModuleId,
                    filter.IsPublished,
                    filter.MockTestId
                });

                // Get total count
                var countSql = $@"
                    SELECT COUNT(DISTINCT f.Id)
                    FROM dbo.Questions f
                    LEFT JOIN dbo.QuestionTranslations qt ON f.Id = qt.QuestionId AND qt.LanguageCode = @LanguageCode
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON f.ExamId = e.Id
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON f.SubjectId = s.Id
                    -- LEFT JOIN [RankUp_MasterDB].[dbo].[Topics] tp ON f.TopicId = tp.Id -- Topics table doesn't exist
                    {mockTestJoin}
                    {moduleJoin}
                    WHERE 1=1 -- Always true since we're filtering by QuestionTranslations
                      AND (@IncludeInactive = 1 OR f.IsActive = 1)
                      AND (@ExamId IS NULL OR f.ExamId = @ExamId)
                      AND (@SubjectId IS NULL OR f.SubjectId = @SubjectId)
                      AND (@ModuleId IS NULL OR f.ModuleId = @ModuleId)
                      AND (@IsPublished IS NULL OR f.IsPublished = @IsPublished)";

                var totalCount = await connection.QueryFirstAsync<int>(countSql, new
                {
                    filter.LanguageCode,
                    filter.IncludeInactive,
                    filter.ExamId,
                    filter.SubjectId,
                    filter.ModuleId,
                    filter.IsPublished,
                    filter.MockTestId
                });

                return (questions, totalCount);
            });
        }

        public async Task<IEnumerable<QuestionAdminListItemDto>> GetAllAdminQuestionsGroupedByMockTestAsync(QuestionFilterRequestDto filter)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT
                        q.Id,
                        q.ModuleId,
                        CAST(NULL AS NVARCHAR(200)) AS ModuleName,
                        mtq.MockTestId,
                        mt.Name AS MockTestName,
                        mt.MockTestType AS MockTestTypeId,
                        mt.ExamId,
                        e.Name AS ExamName,
                        mt.SubjectId,
                        s.Name AS SubjectName,
                        q.TopicId,
                        CAST(NULL AS NVARCHAR(200)) AS TopicName,
                        CASE q.Difficulty WHEN 1 THEN 'Easy' WHEN 2 THEN 'Medium' WHEN 3 THEN 'Hard' ELSE 'Medium' END AS DifficultyLevel,
                        q.Marks,
                        q.NegativeMarks,
                        q.IsPublished,
                        q.IsActive,
                        q.CreatedAt,
                        COALESCE(NULLIF(t.QuestionText, ''), q.QuestionText, '') AS QuestionText,
                        COALESCE(NULLIF(t.OptionA, ''), q.OptionA) AS OptionA,
                        COALESCE(NULLIF(t.OptionB, ''), q.OptionB) AS OptionB,
                        COALESCE(NULLIF(t.OptionC, ''), q.OptionC) AS OptionC,
                        COALESCE(NULLIF(t.OptionD, ''), q.OptionD) AS OptionD,
                        q.CorrectAnswer,
                        COALESCE(NULLIF(t.Explanation, ''), q.Explanation) AS Explanation,
                        q.QuestionType,
                        q.QuestionImageUrl,
                        q.OptionAImageUrl,
                        q.OptionBImageUrl,
                        q.OptionCImageUrl,
                        q.OptionDImageUrl,
                        q.ExplanationImageUrl,
                        q.SameExplanationForAllLanguages,
                        q.Reference,
                        q.Tags,
                        q.CreatedBy,
                        q.ReviewedBy,
                        q.UpdatedAt,
                        q.PublishDate,
                        COALESCE(NULLIF(t.QuestionText, ''), q.QuestionText, '') AS DisplayQuestionText,
                        ISNULL(t.LanguageCode, 'en') AS LanguageCode,
                        t.OptionA AS TranslatedOptionA,
                        t.OptionB AS TranslatedOptionB,
                        t.OptionC AS TranslatedOptionC,
                        t.OptionD AS TranslatedOptionD,
                        t.Explanation AS TranslatedExplanation
                    FROM dbo.MockTestQuestions mtq
                    INNER JOIN dbo.MockTests mt ON mt.Id = mtq.MockTestId AND mt.IsActive = 1
                    INNER JOIN dbo.Questions q ON q.Id = mtq.QuestionId
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Exams] e ON e.Id = mt.ExamId
                    LEFT JOIN [RankUp_MasterDB].[dbo].[Subjects] s ON s.Id = mt.SubjectId
                    OUTER APPLY
                    (
                        SELECT TOP 1 qt.QuestionText, qt.LanguageCode, qt.OptionA, qt.OptionB, qt.OptionC, qt.OptionD, qt.Explanation
                        FROM dbo.QuestionTranslations qt
                        WHERE qt.QuestionId = q.Id
                          AND (@LanguageCode IS NULL OR qt.LanguageCode = @LanguageCode)
                        ORDER BY CASE WHEN qt.LanguageCode = ISNULL(@LanguageCode, 'en') THEN 0 ELSE 1 END
                    ) t
                    WHERE (@IncludeInactive = 1 OR q.IsActive = 1)
                      AND (@MockTestId IS NULL OR mtq.MockTestId = @MockTestId)
                      AND (@ModuleId IS NULL OR q.ModuleId = @ModuleId)
                      AND (@SubjectId IS NULL OR q.SubjectId = @SubjectId)
                      AND (@ExamId IS NULL OR q.ExamId = @ExamId)
                      AND (@TopicId IS NULL OR q.TopicId = @TopicId)
                      AND (@Difficulty IS NULL OR q.Difficulty = @Difficulty)
                      AND (@IsPublished IS NULL OR q.IsPublished = @IsPublished)
                    ORDER BY mtq.MockTestId, mtq.QuestionNumber;";

                return await connection.QueryAsync<QuestionAdminListItemDto>(sql, new
                {
                    filter.LanguageCode,
                    filter.IncludeInactive,
                    filter.MockTestId,
                    filter.ModuleId,
                    filter.SubjectId,
                    filter.ExamId,
                    filter.TopicId,
                    Difficulty = !string.IsNullOrEmpty(filter.DifficultyLevel) ? filter.DifficultyLevel : null,
                    filter.IsPublished
                });
            });
        }
    }
}
