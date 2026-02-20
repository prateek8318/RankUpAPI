using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterService.Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly MasterDbContext _context;

        public ExamRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.ExamLanguages)
                .ThenInclude(el => el.Language)
                .Include(e => e.ExamQualifications)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Include(e => e.ExamLanguages)
                .ThenInclude(el => el.Language)
                .Include(e => e.ExamQualifications)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetActiveAsync()
        {
            return await _context.Exams
                .Include(e => e.ExamLanguages)
                .ThenInclude(el => el.Language)
                .Include(e => e.ExamQualifications)
                .Where(e => e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetByFilterAsync(string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge)
        {
            var query = _context.Exams
                .Include(e => e.ExamLanguages)
                .ThenInclude(el => el.Language)
                .Include(e => e.ExamQualifications)
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                query = query.Where(e => e.CountryCode == countryCode);
            }

            if (minAge.HasValue)
            {
                query = query.Where(e => !e.MinAge.HasValue || e.MinAge.Value <= minAge.Value);
            }

            if (maxAge.HasValue)
            {
                query = query.Where(e => !e.MaxAge.HasValue || e.MaxAge.Value >= maxAge.Value);
            }

            if (qualificationId.HasValue)
            {
                query = query.Where(e =>
                    e.ExamQualifications.Any(eq => eq.QualificationId == qualificationId.Value));
            }

            if (streamId.HasValue)
            {
                query = query.Where(e =>
                    e.ExamQualifications.Any(eq => eq.StreamId == streamId.Value));
            }

            return await query.ToListAsync();
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

