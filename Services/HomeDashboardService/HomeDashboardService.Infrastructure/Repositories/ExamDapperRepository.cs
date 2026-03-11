using Dapper;
using Microsoft.Data.SqlClient;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class ExamDapperRepository : GenericDapperRepository<Exam>, IExamRepository
    {
        public ExamDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<IEnumerable<Exam>> GetActiveExamsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetActive]";
                return await connection.QueryAsync<Exam>(sql);
            });
        }

        public async Task<Exam?> GetByIdWithSubjectsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Exam_GetByIdWithSubjects] @Id";
                return await connection.QueryFirstOrDefaultAsync<Exam>(sql, new { Id = id });
            });
        }
    }
}
