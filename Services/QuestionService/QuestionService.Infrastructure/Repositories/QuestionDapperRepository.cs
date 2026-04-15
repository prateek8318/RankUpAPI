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

        public async Task<int> CreateAdminQuestionAsync(CreateQuestionAdminDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_AdminCreate] @ModuleId, @ExamId, @SubjectId, @TopicId, @Marks, @NegativeMarks, @Difficulty, @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, @TranslationsJson";
                return await connection.QueryFirstAsync<int>(sql, new
                {
                    dto.ModuleId,
                    dto.ExamId,
                    dto.SubjectId,
                    dto.TopicId,
                    dto.Marks,
                    dto.NegativeMarks,
                    Difficulty = (int)Enum.Parse(typeof(DifficultyLevel), dto.DifficultyLevel ?? "Medium"),
                    dto.CorrectAnswer,
                    dto.SameExplanationForAllLanguages,
                    dto.IsPublished,
                    TranslationsJson = JsonSerializer.Serialize(dto.Translations)
                });
            });
        }

        public async Task<bool> UpdateAdminQuestionAsync(UpdateQuestionAdminDto dto)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Question_AdminUpdate] @Id, @ModuleId, @ExamId, @SubjectId, @TopicId, @Marks, @NegativeMarks, @Difficulty, @CorrectAnswer, @SameExplanationForAllLanguages, @IsPublished, @IsActive, @TranslationsJson";
                var rows = await connection.ExecuteAsync(sql, new
                {
                    dto.Id,
                    dto.ModuleId,
                    dto.ExamId,
                    dto.SubjectId,
                    dto.TopicId,
                    dto.Marks,
                    dto.NegativeMarks,
                    Difficulty = (int)Enum.Parse(typeof(DifficultyLevel), dto.DifficultyLevel ?? "Medium"),
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
                var sql = "EXEC [dbo].[Question_AdminGetPaged] @PageNumber, @PageSize, @ModuleId, @SubjectId, @ExamId, @TopicId, @Difficulty, @LanguageCode, @IsPublished, @IncludeInactive";
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
    }
}
