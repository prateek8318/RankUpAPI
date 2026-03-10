using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Enums;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class UserTestAttemptDapperRepository : IUserTestAttemptRepository
    {
        private readonly TestDbContext _context;
        
        public UserTestAttemptDapperRepository(TestDbContext context)
        {
            _context = context;
        }

        protected SqlConnection GetConnection()
        {
            return (SqlConnection)_context.Database.GetDbConnection();
        }

        public async Task<UserTestAttempt?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserTestAttempt_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<UserTestAttempt>(sql, new { Id = id });
        }

        public async Task<IEnumerable<UserTestAttempt>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserTestAttempt_GetAll]";
            return await connection.QueryAsync<UserTestAttempt>(sql);
        }

        public async Task<IEnumerable<UserTestAttempt>> FindAsync(System.Linq.Expressions.Expression<Func<UserTestAttempt, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(UserTestAttempt entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[UserTestAttempt_Create] 
                    @UserId, @TestId, @Status, @StartedAt, @CompletedAt, 
                    @TimeTaken, @TotalQuestions, @AttemptedQuestions, 
                    @CorrectAnswers, @WrongAnswers, @Marks, @Percentage, 
                    @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateAsync(UserTestAttempt entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[UserTestAttempt_Update] 
                    @Id, @UserId, @TestId, @Status, @StartedAt, @CompletedAt, 
                    @TimeTaken, @TotalQuestions, @AttemptedQuestions, 
                    @CorrectAnswers, @WrongAnswers, @Marks, @Percentage, 
                    @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(UserTestAttempt entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserTestAttempt_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = entity.Id });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserTestAttempt>> GetOngoingByUserIdAsync(int userId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserTestAttempt_GetOngoingByUserId] @UserId";
            return await connection.QueryAsync<UserTestAttempt>(sql, new { UserId = userId });
        }

        public async Task<UserTestAttempt?> GetByIdWithTestAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserTestAttempt_GetByIdWithTest] @Id";
            return await connection.QueryFirstOrDefaultAsync<UserTestAttempt>(sql, new { Id = id });
        }

        public async Task<IEnumerable<UserTestAttempt>> GetByUserIdAsync(int userId, int limit = 10)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[UserTestAttempt_GetByUserId] @UserId, @Limit";
            return await connection.QueryAsync<UserTestAttempt>(sql, new { UserId = userId, Limit = limit });
        }
    }
}
