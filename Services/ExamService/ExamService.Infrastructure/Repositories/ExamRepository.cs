using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;
using ExamService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamService.Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly ExamDbContext _context;

        public ExamRepository(ExamDbContext context)
        {
            _context = context;
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams.FindAsync(id);
        }

        public async Task<Exam?> GetByIdWithQualificationsAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.ExamQualifications)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetActiveAsync()
        {
            return await _context.Exams
                .Where(e => e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetByQualificationIdAsync(int qualificationId)
        {
            return await _context.Exams
                .Include(e => e.ExamQualifications)
                .Where(e => e.ExamQualifications.Any(eq => eq.QualificationId == qualificationId))
                .ToListAsync();
        }

        public async Task<Exam> AddAsync(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            return exam;
        }

        public Task UpdateAsync(Exam exam)
        {
            _context.Exams.Update(exam);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Exam exam)
        {
            _context.Exams.Remove(exam);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
