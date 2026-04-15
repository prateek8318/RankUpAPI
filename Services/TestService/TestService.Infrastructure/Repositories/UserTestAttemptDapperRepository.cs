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
                var sql = @"SELECT Id, TestId, UserId, StartedAt, CompletedAt, CurrentQuestionIndex, Score, TotalMarks,
                                   Accuracy, Status, AnswersJson, IsActive, CreatedAt, UpdatedAt
                            FROM dbo.UserTestAttempts
                            WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<UserTestAttempt>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<UserTestAttempt>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"SELECT Id, TestId, UserId, StartedAt, CompletedAt, CurrentQuestionIndex, Score, TotalMarks,
                                   Accuracy, Status, AnswersJson, IsActive, CreatedAt, UpdatedAt
                            FROM dbo.UserTestAttempts";
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
                    INSERT INTO dbo.UserTestAttempts
                    (
                        TestId, UserId, StartedAt, CompletedAt, CurrentQuestionIndex, Score, TotalMarks,
                        Accuracy, Status, AnswersJson, IsActive, CreatedAt, UpdatedAt
                    )
                    VALUES
                    (
                        @TestId, @UserId, @StartedAt, @CompletedAt, @CurrentQuestionIndex, @Score, @TotalMarks,
                        @Accuracy, @Status, @AnswersJson, @IsActive, @CreatedAt, @UpdatedAt
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                entity.Id = await connection.ExecuteScalarAsync<int>(sql, new
                {
                    entity.TestId,
                    entity.UserId,
                    entity.StartedAt,
                    entity.CompletedAt,
                    entity.CurrentQuestionIndex,
                    entity.Score,
                    entity.TotalMarks,
                    entity.Accuracy,
                    Status = (int)entity.Status,
                    entity.AnswersJson,
                    entity.IsActive,
                    entity.CreatedAt,
                    entity.UpdatedAt
                });
            });
        }

        public async Task UpdateAsync(UserTestAttempt entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    UPDATE dbo.UserTestAttempts
                    SET TestId = @TestId,
                        UserId = @UserId,
                        StartedAt = @StartedAt,
                        CompletedAt = @CompletedAt,
                        CurrentQuestionIndex = @CurrentQuestionIndex,
                        Score = @Score,
                        TotalMarks = @TotalMarks,
                        Accuracy = @Accuracy,
                        Status = @Status,
                        AnswersJson = @AnswersJson,
                        IsActive = @IsActive,
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, new
                {
                    entity.Id,
                    entity.TestId,
                    entity.UserId,
                    entity.StartedAt,
                    entity.CompletedAt,
                    entity.CurrentQuestionIndex,
                    entity.Score,
                    entity.TotalMarks,
                    entity.Accuracy,
                    Status = (int)entity.Status,
                    entity.AnswersJson,
                    entity.IsActive,
                    entity.UpdatedAt
                });
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
                var sql = @"SELECT Id, TestId, UserId, StartedAt, CompletedAt, CurrentQuestionIndex, Score, TotalMarks,
                                   Accuracy, Status, AnswersJson, IsActive, CreatedAt, UpdatedAt
                            FROM dbo.UserTestAttempts
                            WHERE UserId = @UserId
                              AND IsActive = 1
                              AND Status = @InProgressStatus";
                return await connection.QueryAsync<UserTestAttempt>(sql, new
                {
                    UserId = userId,
                    InProgressStatus = (int)TestAttemptStatus.InProgress
                });
            });
        }

        public async Task<UserTestAttempt?> GetByIdWithTestAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    SELECT a.Id, a.TestId, a.UserId, a.StartedAt, a.CompletedAt, a.CurrentQuestionIndex, a.Score,
                           a.TotalMarks, a.Accuracy, a.Status, a.AnswersJson, a.IsActive, a.CreatedAt, a.UpdatedAt,
                           t.Id, t.ExamId, t.PracticeModeId, t.SeriesId, t.SubjectId, t.Year, t.Title, t.Description,
                           t.DurationInMinutes, t.TotalQuestions, t.TotalMarks, t.PassingMarks, t.InstructionsEnglish,
                           t.InstructionsHindi, t.DisplayOrder, t.IsLocked, t.IsActive, t.CreatedAt, t.UpdatedAt
                    FROM dbo.UserTestAttempts a
                    INNER JOIN dbo.Tests t ON t.Id = a.TestId
                    WHERE a.Id = @Id";

                return (await connection.QueryAsync<UserTestAttempt, Test, UserTestAttempt>(
                    sql,
                    (attempt, test) =>
                    {
                        attempt.Test = test;
                        return attempt;
                    },
                    new { Id = id },
                    splitOn: "Id")).FirstOrDefault();
            });
        }

        public async Task<IEnumerable<UserTestAttempt>> GetByUserIdAsync(int userId, int limit = 10)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"SELECT TOP (@Limit) Id, TestId, UserId, StartedAt, CompletedAt, CurrentQuestionIndex, Score,
                                   TotalMarks, Accuracy, Status, AnswersJson, IsActive, CreatedAt, UpdatedAt
                            FROM dbo.UserTestAttempts
                            WHERE UserId = @UserId
                            ORDER BY CreatedAt DESC";
                return await connection.QueryAsync<UserTestAttempt>(sql, new { UserId = userId, Limit = limit });
            });
        }
    }
}
