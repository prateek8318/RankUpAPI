using Dapper;
using Microsoft.Data.SqlClient;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class ExamDapperRepository : GenericDapperRepository<Exam>, IExamRepository
    {
        public ExamDapperRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Exam>> GetActiveExamsAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetActive]";
            return await connection.QueryAsync<Exam>(sql);
        }

        public async Task<Exam?> GetByIdWithSubjectsAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Exam_GetByIdWithSubjects] @Id";
            return await connection.QueryFirstOrDefaultAsync<Exam>(sql, new { Id = id });
        }
    }
}
