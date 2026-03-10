using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class TestDapperRepository : ITestRepository
    {
        private readonly TestDbContext _context;
        
        public TestDapperRepository(TestDbContext context)
        {
            _context = context;
        }

        protected SqlConnection GetConnection()
        {
            return (SqlConnection)_context.Database.GetDbConnection();
        }

        public async Task<Test?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Test_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<Test>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Test>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Test_GetAll]";
            return await connection.QueryAsync<Test>(sql);
        }

        public async Task<IEnumerable<Test>> FindAsync(System.Linq.Expressions.Expression<Func<Test, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(Test entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Test_Create] 
                    @Name, @Description, @ExamId, @PracticeModeId, @SeriesId, 
                    @SubjectId, @Year, @Duration, @TotalQuestions, @TotalMarks, 
                    @PassingMarks, @IsActive, @DisplayOrder, @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateAsync(Test entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Test_Update] 
                    @Id, @Name, @Description, @ExamId, @PracticeModeId, @SeriesId, 
                    @SubjectId, @Year, @Duration, @TotalQuestions, @TotalMarks, 
                    @PassingMarks, @IsActive, @DisplayOrder, @UpdatedAt";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(Test entity)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Test_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = entity.Id });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Test>> GetByExamAndPracticeModeAsync(int examId, int practiceModeId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Test_GetByExamAndPracticeMode] @ExamId, @PracticeModeId";
            return await connection.QueryAsync<Test>(sql, new { ExamId = examId, PracticeModeId = practiceModeId });
        }

        public async Task<IEnumerable<Test>> GetByExamAndPracticeModeWithFiltersAsync(
            int examId, 
            int practiceModeId, 
            int? seriesId = null, 
            int? subjectId = null, 
            int? year = null)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Test_GetByExamAndPracticeModeWithFilters] @ExamId, @PracticeModeId, @SeriesId, @SubjectId, @Year";
            return await connection.QueryAsync<Test>(sql, new { 
                ExamId = examId, 
                PracticeModeId = practiceModeId, 
                SeriesId = seriesId, 
                SubjectId = subjectId, 
                Year = year 
            });
        }

        public async Task<Test?> GetByIdWithQuestionsAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Test_GetByIdWithQuestions] @Id";
            var test = await connection.QueryFirstOrDefaultAsync<Test>(sql, new { Id = id });
            
            if (test != null)
            {
                var questionsSql = "EXEC [dbo].[Test_GetQuestionsByTestId] @Id";
                test.TestQuestions = (await connection.QueryAsync<TestQuestion>(questionsSql, new { Id = id })).ToList();
            }
            
            return test;
        }

        public async Task<IEnumerable<Test>> GetActiveTestsAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Test_GetActiveTests]";
            return await connection.QueryAsync<Test>(sql);
        }
    }
}
