using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;
using ExamService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamService.Infrastructure.Repositories
{
    public class ExamQualificationRepository : IExamQualificationRepository
    {
        private readonly ExamDbContext _context;

        public ExamQualificationRepository(ExamDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExamQualification examQualification)
        {
            await _context.ExamQualifications.AddAsync(examQualification);
        }

        public async Task AddRangeAsync(IEnumerable<ExamQualification> examQualifications)
        {
            await _context.ExamQualifications.AddRangeAsync(examQualifications);
        }

        public Task DeleteAsync(ExamQualification examQualification)
        {
            _context.ExamQualifications.Remove(examQualification);
            return Task.CompletedTask;
        }

        public async Task DeleteByExamIdAsync(int examId)
        {
            var examQualifications = await _context.ExamQualifications
                .Where(eq => eq.ExamId == examId)
                .ToListAsync();
            
            _context.ExamQualifications.RemoveRange(examQualifications);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
