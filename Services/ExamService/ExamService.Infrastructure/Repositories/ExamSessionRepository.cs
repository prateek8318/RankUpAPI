using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;
using ExamService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamService.Infrastructure.Repositories
{
    public class ExamSessionRepository : IExamSessionRepository
    {
        private readonly ExamDbContext _context;

        public ExamSessionRepository(ExamDbContext context)
        {
            _context = context;
        }

        public async Task<ExamSession?> GetByIdAsync(int id)
        {
            return await _context.ExamSessions.FindAsync(id);
        }

        public async Task<ExamSession> AddAsync(ExamSession entity)
        {
            await _context.ExamSessions.AddAsync(entity);
            return entity;
        }

        public async Task<ExamSession> UpdateAsync(ExamSession entity)
        {
            _context.ExamSessions.Update(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            
            _context.ExamSessions.Remove(entity);
            return true;
        }

        public async Task<IEnumerable<ExamSession>> GetAllAsync()
        {
            return await _context.ExamSessions.ToListAsync();
        }

        public async Task<ExamSession?> GetActiveSessionByUserIdAsync(int userId)
        {
            return await _context.ExamSessions
                .Where(es => es.UserId == userId && !es.IsCompleted)
                .Include(es => es.Answers)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExamSession>> GetSessionsByUserIdAsync(int userId)
        {
            return await _context.ExamSessions
                .Where(es => es.UserId == userId)
                .OrderByDescending(es => es.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamSession>> GetSessionsByExamIdAsync(int examId)
        {
            return await _context.ExamSessions
                .Where(es => es.ExamId == examId)
                .OrderByDescending(es => es.StartTime)
                .ToListAsync();
        }
    }
}
