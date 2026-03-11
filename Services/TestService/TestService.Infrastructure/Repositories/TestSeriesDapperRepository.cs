using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class TestSeriesDapperRepository : BaseDapperRepository, ITestSeriesRepository
    {
        public TestSeriesDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<TestSeries?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestSeries_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<TestSeries>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<TestSeries>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestSeries_GetAll]";
                return await connection.QueryAsync<TestSeries>(sql);
            });
        }

        public async Task<IEnumerable<TestSeries>> FindAsync(System.Linq.Expressions.Expression<Func<TestSeries, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(TestSeries entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[TestSeries_Create] 
                        @Name, @Description, @ExamId, @IsActive, @DisplayOrder, 
                        @CreatedAt, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task UpdateAsync(TestSeries entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[TestSeries_Update] 
                        @Id, @Name, @Description, @ExamId, @IsActive, @DisplayOrder, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task DeleteAsync(TestSeries entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestSeries_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = entity.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }

        public async Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestSeries_GetByExamId] @ExamId";
                return await connection.QueryAsync<TestSeries>(sql, new { ExamId = examId });
            });
        }

        public async Task<TestSeries?> GetByIdWithTestsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestSeries_GetByIdWithTests] @Id";
                return await connection.QueryFirstOrDefaultAsync<TestSeries>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<TestSeries>> GetActiveSeriesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestSeries_GetActive]";
                return await connection.QueryAsync<TestSeries>(sql);
            });
        }
    }
}
