using Microsoft.EntityFrameworkCore;
using QuizService.Application.Interfaces;
using QuizService.Domain.Entities;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly QuizDbContext _context;

        public ChapterRepository(QuizDbContext context)
        {
            _context = context;
        }

        public async Task<Chapter?> GetByIdAsync(int id)
        {
            return await _context.Chapters.FindAsync(id);
        }

        public async Task<IEnumerable<Chapter>> GetAllAsync()
        {
            return await _context.Chapters.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Chapter>> GetBySubjectIdAsync(int subjectId)
        {
            return await _context.Chapters
                .Where(c => c.SubjectId == subjectId && c.IsActive)
                .ToListAsync();
        }

        public async Task<Chapter> AddAsync(Chapter chapter)
        {
            await _context.Chapters.AddAsync(chapter);
            return chapter;
        }

        public Task UpdateAsync(Chapter chapter)
        {
            _context.Chapters.Update(chapter);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Chapter chapter)
        {
            _context.Chapters.Remove(chapter);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
