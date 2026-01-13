using Microsoft.EntityFrameworkCore;
using QuizService.Application.Interfaces;
using QuizService.Domain.Entities;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly QuizDbContext _context;

        public SubjectRepository(QuizDbContext context)
        {
            _context = context;
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<Subject?> GetByIdWithChaptersAsync(int id)
        {
            return await _context.Subjects
                .Include(s => s.Chapters)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetByExamIdAsync(int examId)
        {
            return await _context.Subjects
                .Where(s => s.ExamId == examId && s.IsActive)
                .ToListAsync();
        }

        public async Task<Subject> AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            return subject;
        }

        public Task UpdateAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Subject subject)
        {
            _context.Subjects.Remove(subject);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
