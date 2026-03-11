using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class TestQuestionDapperRepository : BaseDapperRepository, ITestQuestionRepository
    {
        public TestQuestionDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<TestQuestion?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestQuestion_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<TestQuestion>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<TestQuestion>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestQuestion_GetAll]";
                return await connection.QueryAsync<TestQuestion>(sql);
            });
        }

        public async Task<IEnumerable<TestQuestion>> GetByTestIdAsync(int testId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestQuestion_GetByTestId] @TestId";
                return await connection.QueryAsync<TestQuestion>(sql, new { TestId = testId });
            });
        }

        public async Task<IEnumerable<TestQuestion>> GetByQuestionIdAsync(int questionId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestQuestion_GetByQuestionId] @QuestionId";
                return await connection.QueryAsync<TestQuestion>(sql, new { QuestionId = questionId });
            });
        }

        public async Task<IEnumerable<TestQuestion>> FindAsync(System.Linq.Expressions.Expression<Func<TestQuestion, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(TestQuestion entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[TestQuestion_Create] 
                        @TestId, @QuestionId, @QuestionNumber, @Marks, @IsOptional, @CreatedAt, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task UpdateAsync(TestQuestion entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[TestQuestion_Update] 
                        @Id, @TestId, @QuestionId, @QuestionNumber, @Marks, @IsOptional, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task DeleteAsync(TestQuestion entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestQuestion_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = entity.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }
    }
}
