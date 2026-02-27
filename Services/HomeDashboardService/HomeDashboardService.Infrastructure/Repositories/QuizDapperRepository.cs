using Dapper;
using Microsoft.Data.SqlClient;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class QuizDapperRepository : GenericDapperRepository<Quiz>, IQuizRepository
    {
        public QuizDapperRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Quiz>> GetByChapterIdAsync(int chapterId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Quiz_GetByChapterId] @ChapterId";
            return await connection.QueryAsync<Quiz>(sql, new { ChapterId = chapterId });
        }

        public async Task<Quiz?> GetByIdWithQuestionsAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Quiz_GetByIdWithQuestions] @Id";
            return await connection.QueryFirstOrDefaultAsync<Quiz>(sql, new { Id = id });
        }
    }
}
