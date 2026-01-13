using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Question?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(q => q.Chapter)
                    .ThenInclude(c => c.Subject)
                        .ThenInclude(s => s.Exam)
                .Include(q => q.TestSeriesQuestions)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Question>> GetActiveAsync()
        {
            return await _dbSet
                .Include(q => q.Chapter)
                    .ThenInclude(c => c.Subject)
                        .ThenInclude(s => s.Exam)
                .Where(q => q.IsActive)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByChapterIdAsync(int chapterId)
        {
            return await _dbSet
                .Include(q => q.Chapter)
                    .ThenInclude(c => c.Subject)
                        .ThenInclude(s => s.Exam)
                .Where(q => q.ChapterId == chapterId && q.IsActive)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetBySubjectIdAsync(int subjectId)
        {
            return await _dbSet
                .Include(q => q.Chapter)
                    .ThenInclude(c => c.Subject)
                        .ThenInclude(s => s.Exam)
                .Where(q => q.Chapter.SubjectId == subjectId && q.IsActive)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByExamIdAsync(int examId)
        {
            return await _dbSet
                .Include(q => q.Chapter)
                    .ThenInclude(c => c.Subject)
                        .ThenInclude(s => s.Exam)
                .Where(q => q.Chapter.Subject.ExamId == examId && q.IsActive)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync();
        }
    }
}
