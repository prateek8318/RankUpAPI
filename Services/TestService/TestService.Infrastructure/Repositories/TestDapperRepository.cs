using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class TestDapperRepository : BaseDapperRepository, ITestRepository
    {
        public TestDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<Test?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<Test>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<Test>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetAll]";
                return await connection.QueryAsync<Test>(sql);
            });
        }

        public async Task<IEnumerable<Test>> FindAsync(System.Linq.Expressions.Expression<Func<Test, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(Test entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Test_Create] 
                        @Name, @Description, @ExamId, @PracticeModeId, @SeriesId, 
                        @SubjectId, @Year, @Duration, @TotalQuestions, @TotalMarks, 
                        @PassingMarks, @IsActive, @DisplayOrder, @CreatedAt, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task UpdateAsync(Test entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Test_Update] 
                        @Id, @Name, @Description, @ExamId, @PracticeModeId, @SeriesId, 
                        @SubjectId, @Year, @Duration, @TotalQuestions, @TotalMarks, 
                        @PassingMarks, @IsActive, @DisplayOrder, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task DeleteAsync(Test entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = entity.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }

        public async Task<IEnumerable<Test>> GetByExamAndPracticeModeAsync(int examId, int practiceModeId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetByExamAndPracticeMode] @ExamId, @PracticeModeId";
                return await connection.QueryAsync<Test>(sql, new { ExamId = examId, PracticeModeId = practiceModeId });
            });
        }

        public async Task<IEnumerable<Test>> GetByExamAndPracticeModeWithFiltersAsync(
            int examId, 
            int practiceModeId, 
            int? seriesId = null, 
            int? subjectId = null, 
            int? year = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetByExamAndPracticeModeWithFilters] @ExamId, @PracticeModeId, @SeriesId, @SubjectId, @Year";
                return await connection.QueryAsync<Test>(sql, new { 
                    ExamId = examId, 
                    PracticeModeId = practiceModeId, 
                    SeriesId = seriesId, 
                    SubjectId = subjectId, 
                    Year = year 
                });
            });
        }

        public async Task<Test?> GetByIdWithQuestionsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetByIdWithQuestions] @Id";
                var test = await connection.QueryFirstOrDefaultAsync<Test>(sql, new { Id = id });
                
                if (test != null)
                {
                    var questionsSql = "EXEC [dbo].[Test_GetQuestionsByTestId] @Id";
                    test.TestQuestions = (await connection.QueryAsync<TestQuestion>(questionsSql, new { Id = id })).ToList();
                }
                
                return test;
            });
        }

        public async Task<IEnumerable<Test>> GetActiveTestsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetActiveTests]";
                return await connection.QueryAsync<Test>(sql);
            });
        }

        public async Task<(IEnumerable<UserAvailableTest> Items, int TotalCount)> GetAvailableTestsForUserAsync(int userId, int examId, int? practiceModeId, int? subjectId, int? seriesId, int? year, int pageNumber, int pageSize)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetAvailableForUserPaged] @UserId, @ExamId, @PracticeModeId, @SubjectId, @SeriesId, @Year, @PageNumber, @PageSize";
                using var multi = await connection.QueryMultipleAsync(sql, new
                {
                    UserId = userId,
                    ExamId = examId,
                    PracticeModeId = practiceModeId,
                    SubjectId = subjectId,
                    SeriesId = seriesId,
                    Year = year,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

                var items = await multi.ReadAsync<UserAvailableTest>();
                var totalCount = await multi.ReadFirstOrDefaultAsync<int>();
                return (items, totalCount);
            });
        }

        public async Task<bool> MapTestToPlanAsync(int testId, int subscriptionPlanId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[TestPlanMapping_CreateOrUpdate] @TestId, @SubscriptionPlanId";
                var affected = await connection.ExecuteAsync(sql, new
                {
                    TestId = testId,
                    SubscriptionPlanId = subscriptionPlanId
                });
                return affected > 0;
            });
        }

        public async Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int testId, int top = 20)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Test_GetLeaderboard] @TestId, @Top";
                var rows = await connection.QueryAsync<LeaderboardEntry>(sql, new { TestId = testId, Top = top });
                return rows.ToList();
            });
        }
    }
}
