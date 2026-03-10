using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class TestSeriesDapperRepository : ITestSeriesRepository
    {
        private readonly TestDbContext _context;
        
        public TestSeriesDapperRepository(TestDbContext context)
        {
            _context = context;
        }

        protected SqlConnection GetConnection()
        {
            return (SqlConnection)_context.Database.GetDbConnection();
        }

        public async Task<TestSeries?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<TestSeries>(sql, new { Id = id });
        }

        public async Task<IEnumerable<TestSeries>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetAll]";
            return await connection.QueryAsync<TestSeries>(sql);
        }

        public async Task<IEnumerable<TestSeries>> FindAsync(System.Linq.Expressions.Expression<Func<TestSeries, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(TestSeries entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[TestSeries_Create] 
                    @Name, @Description, @ExamId, @IsActive, @DisplayOrder, 
                    @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateAsync(TestSeries entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[TestSeries_Update] 
                    @Id, @Name, @Description, @ExamId, @IsActive, @DisplayOrder, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(TestSeries entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = entity.Id });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetByExamId] @ExamId";
            return await connection.QueryAsync<TestSeries>(sql, new { ExamId = examId });
        }

        public async Task<TestSeries?> GetByIdWithTestsAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetByIdWithTests] @Id";
            return await connection.QueryFirstOrDefaultAsync<TestSeries>(sql, new { Id = id });
        }

        public async Task<IEnumerable<TestSeries>> GetActiveSeriesAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetActive]";
            return await connection.QueryAsync<TestSeries>(sql);
        }
    }
}
