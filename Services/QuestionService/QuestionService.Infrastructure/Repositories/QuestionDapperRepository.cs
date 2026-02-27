using Dapper;
using Microsoft.Data.SqlClient;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;
using QuestionService.Infrastructure.Data;
using System.Data;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionDapperRepository : IQuestionRepository
    {
        private readonly QuestionDbContext _context;

        public QuestionDapperRepository(QuestionDbContext context)
        {
            _context = context;
        }

        private SqlConnection GetConnection()
        {
            return (SqlConnection)_context.Database.GetDbConnection();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Question_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<Question>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Question_GetAll]";
            return await connection.QueryAsync<Question>(sql);
        }

        public async Task<IEnumerable<Question>> GetByChapterIdAsync(int chapterId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Question_GetByChapterId] @ChapterId";
            return await connection.QueryAsync<Question>(sql, new { ChapterId = chapterId });
        }

        public async Task<Question> AddAsync(Question question)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Question_Create] 
                    @QuestionText, @QuestionType, @Difficulty, @ChapterId, 
                    @SubjectId, @ExamId, @Explanation, @ImageUrl, @VideoUrl,
                    @Tags, @Points, @TimeLimit, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@QuestionText", question.QuestionText);
            parameters.Add("@QuestionType", question.QuestionType);
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
        }

        public async Task UpdateAsync(Question question)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Question_Update] 
                    @Id, @QuestionText, @QuestionType, @Difficulty, @ChapterId, 
                    @SubjectId, @ExamId, @Explanation, @ImageUrl, @VideoUrl,
                    @Tags, @Points, @TimeLimit, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", question.Id);
            parameters.Add("@QuestionText", question.QuestionText);
            parameters.Add("@QuestionType", question.QuestionType);
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
        }

        public async Task DeleteAsync(Question question)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Question_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = question.Id });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
