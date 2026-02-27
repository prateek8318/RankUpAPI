using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class TestRepository : GenericRepository<Test>, ITestRepository
    {
        public TestRepository(TestDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Test>> GetByExamAndPracticeModeAsync(int examId, int practiceModeId)
        {
            var parameters = new[]
            {
                new SqlParameter("@ExamId", examId),
                new SqlParameter("@PracticeModeId", practiceModeId)
            };

            return await _context.Tests
                .FromSqlRaw("EXEC Test_GetByExamAndPracticeMode @ExamId, @PracticeModeId", parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Test>> GetByExamAndPracticeModeWithFiltersAsync(
            int examId, 
            int practiceModeId, 
            int? seriesId = null, 
            int? subjectId = null, 
            int? year = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@ExamId", examId),
                new SqlParameter("@PracticeModeId", practiceModeId),
                new SqlParameter("@SeriesId", (object?)seriesId ?? DBNull.Value),
                new SqlParameter("@SubjectId", (object?)subjectId ?? DBNull.Value),
                new SqlParameter("@Year", (object?)year ?? DBNull.Value)
            };

            return await _context.Tests
                .FromSqlRaw("EXEC Test_GetByExamAndPracticeModeWithFilters @ExamId, @PracticeModeId, @SeriesId, @SubjectId, @Year", parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Test?> GetByIdWithQuestionsAsync(int id)
        {
            var testParam = new SqlParameter("@Id", id);
            
            var test = await _context.Tests
                .FromSqlRaw("EXEC Test_GetByIdWithQuestions @Id", testParam)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if (test != null)
            {
                var questionsParam = new SqlParameter("@Id", id);
                test.TestQuestions = await _context.TestQuestions
                    .FromSqlRaw("EXEC Test_GetByIdWithQuestions @Id", questionsParam)
                    .AsNoTracking()
                    .ToListAsync();
            }
            
            return test;
        }

        public async Task<IEnumerable<Test>> GetActiveTestsAsync()
        {
            return await _context.Tests
                .FromSqlRaw("EXEC Test_GetActiveTests")
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
