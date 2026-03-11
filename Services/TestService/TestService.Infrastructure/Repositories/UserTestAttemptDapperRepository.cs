using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Enums;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class UserTestAttemptDapperRepository : BaseDapperRepository, IUserTestAttemptRepository
    {
        public UserTestAttemptDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<UserTestAttempt?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[UserTestAttempt_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<UserTestAttempt>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<UserTestAttempt>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[UserTestAttempt_GetAll]";
                return await connection.QueryAsync<UserTestAttempt>(sql);
            });
        }

        public async Task<IEnumerable<UserTestAttempt>> FindAsync(System.Linq.Expressions.Expression<Func<UserTestAttempt, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(UserTestAttempt entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[UserTestAttempt_Create] 
                        @UserId, @TestId, @Status, @StartedAt, @CompletedAt, 
                        @TimeTaken, @TotalQuestions, @AttemptedQuestions, 
                        @CorrectAnswers, @WrongAnswers, @Marks, @Percentage, 
                        @CreatedAt, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task UpdateAsync(UserTestAttempt entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[UserTestAttempt_Update] 
                        @Id, @UserId, @TestId, @Status, @StartedAt, @CompletedAt, 
                        @TimeTaken, @TotalQuestions, @AttemptedQuestions, 
                        @CorrectAnswers, @WrongAnswers, @Marks, @Percentage, 
                        @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task DeleteAsync(UserTestAttempt entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[UserTestAttempt_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = entity.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }

        public async Task<IEnumerable<UserTestAttempt>> GetOngoingByUserIdAsync(int userId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[UserTestAttempt_GetOngoingByUserId] @UserId";
                return await connection.QueryAsync<UserTestAttempt>(sql, new { UserId = userId });
            });
        }

        public async Task<UserTestAttempt?> GetByIdWithTestAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[UserTestAttempt_GetByIdWithTest] @Id";
                return await connection.QueryFirstOrDefaultAsync<UserTestAttempt>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<UserTestAttempt>> GetByUserIdAsync(int userId, int limit = 10)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[UserTestAttempt_GetByUserId] @UserId, @Limit";
                return await connection.QueryAsync<UserTestAttempt>(sql, new { UserId = userId, Limit = limit });
            });
        }
    }
}
