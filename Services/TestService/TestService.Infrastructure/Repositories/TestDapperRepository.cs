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
    }
}
