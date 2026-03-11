using Dapper;
using Microsoft.Data.SqlClient;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class QuizDapperRepository : GenericDapperRepository<Quiz>, IQuizRepository
    {
        public QuizDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<IEnumerable<Quiz>> GetByChapterIdAsync(int chapterId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Quiz_GetByChapterId] @ChapterId";
                return await connection.QueryAsync<Quiz>(sql, new { ChapterId = chapterId });
            });
        }

        public async Task<Quiz?> GetByIdWithQuestionsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Quiz_GetByIdWithQuestions] @Id";
                return await connection.QueryFirstOrDefaultAsync<Quiz>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<Quiz>> GetActiveQuizzesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Quiz_GetActive]";
                return await connection.QueryAsync<Quiz>(sql);
            });
        }

        public async Task<IEnumerable<Quiz>> GetByTypeAsync(QuizType type)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Quiz_GetByType] @Type";
                return await connection.QueryAsync<Quiz>(sql, new { Type = type });
            });
        }

        public async Task<IEnumerable<Quiz>> GetTrendingQuizzesAsync(int limit = 10)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Quiz_GetTrending] @Limit";
                return await connection.QueryAsync<Quiz>(sql, new { Limit = limit });
            });
        }
    }
}
