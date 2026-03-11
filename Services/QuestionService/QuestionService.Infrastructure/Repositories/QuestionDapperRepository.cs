using Dapper;
using Microsoft.Data.SqlClient;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;
using System.Data;

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
                parameters.Add("@QuestionText", question.QuestionTextEnglish);
                parameters.Add("@QuestionType", question.Type);
                parameters.Add("@Difficulty", question.Difficulty);
                parameters.Add("@ChapterId", question.ChapterId);
                parameters.Add("@SubjectId", question.SubjectId);
                parameters.Add("@ExamId", question.ExamId);
                parameters.Add("@Explanation", question.Explanation);
                parameters.Add("@ImageUrl", question.ImageUrl);
                parameters.Add("@VideoUrl", question.VideoUrl);
                parameters.Add("@Tags", question.Tags);
                parameters.Add("@Points", question.Points);
                parameters.Add("@TimeLimit", question.TimeLimit);
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
                parameters.Add("@QuestionText", question.QuestionTextEnglish);
                parameters.Add("@QuestionType", question.Type);
                parameters.Add("@Difficulty", question.Difficulty);
                parameters.Add("@ChapterId", question.ChapterId);
                parameters.Add("@SubjectId", question.SubjectId);
                parameters.Add("@ExamId", question.ExamId);
                parameters.Add("@Explanation", question.Explanation);
                parameters.Add("@ImageUrl", question.ImageUrl);
                parameters.Add("@VideoUrl", question.VideoUrl);
                parameters.Add("@Tags", question.Tags);
                parameters.Add("@Points", question.Points);
                parameters.Add("@TimeLimit", question.TimeLimit);
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
    }
}
