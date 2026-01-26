using Microsoft.EntityFrameworkCore;
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
            return await _dbSet
                .Include(t => t.Exam)
                .Include(t => t.PracticeMode)
                .Include(t => t.Series)
                .Include(t => t.Subject)
                .Where(t => t.ExamId == examId && t.PracticeModeId == practiceModeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Test>> GetByExamAndPracticeModeWithFiltersAsync(
            int examId, 
            int practiceModeId, 
            int? seriesId = null, 
            int? subjectId = null, 
            int? year = null)
        {
            var query = _dbSet
                .Include(t => t.Exam)
                .Include(t => t.PracticeMode)
                .Include(t => t.Series)
                .Include(t => t.Subject)
                .Where(t => t.ExamId == examId && t.PracticeModeId == practiceModeId);

            if (seriesId.HasValue)
                query = query.Where(t => t.SeriesId == seriesId);

            if (subjectId.HasValue)
                query = query.Where(t => t.SubjectId == subjectId);

            if (year.HasValue)
                query = query.Where(t => t.Year == year);

            return await query.OrderBy(t => t.DisplayOrder).ToListAsync();
        }

        public async Task<Test?> GetByIdWithQuestionsAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Exam)
                .Include(t => t.PracticeMode)
                .Include(t => t.Series)
                .Include(t => t.Subject)
                .Include(t => t.TestQuestions)
                    .ThenInclude(tq => tq.Question)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Test>> GetActiveTestsAsync()
        {
            return await _dbSet
                .Include(t => t.Exam)
                .Include(t => t.PracticeMode)
                .Include(t => t.Series)
                .Include(t => t.Subject)
                .Where(t => t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();
        }
    }
}
