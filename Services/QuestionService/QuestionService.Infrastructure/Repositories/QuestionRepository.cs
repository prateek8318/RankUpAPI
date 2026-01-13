using Microsoft.EntityFrameworkCore;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;
using QuestionService.Infrastructure.Data;

namespace QuestionService.Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QuestionDbContext _context;

        public QuestionRepository(QuestionDbContext context)
        {
            _context = context;
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions.Where(q => q.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByChapterIdAsync(int chapterId)
        {
            return await _context.Questions
                .Where(q => q.ChapterId == chapterId && q.IsActive)
                .ToListAsync();
        }

        public async Task<Question> AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            return question;
        }

        public Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Question question)
        {
            _context.Questions.Remove(question);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
