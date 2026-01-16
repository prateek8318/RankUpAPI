using QualificationService.Application.Interfaces;
using QualificationService.Domain.Entities;
using QualificationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace QualificationService.Infrastructure.Repositories
{
    public class QualificationRepository : IQualificationRepository
    {
        private readonly QualificationDbContext _context;

        public QualificationRepository(QualificationDbContext context)
        {
            _context = context;
        }

        public async Task<Qualification?> GetByIdAsync(int id)
        {
            return await _context.Qualifications.FindAsync(id);
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            return await _context.Qualifications.ToListAsync();
        }

        public async Task<IEnumerable<Qualification>> GetActiveAsync()
        {
            return await _context.Qualifications
                .Where(q => q.IsActive)
                .ToListAsync();
        }

        public async Task<Qualification> AddAsync(Qualification qualification)
        {
            await _context.Qualifications.AddAsync(qualification);
            return qualification;
        }

        public Task UpdateAsync(Qualification qualification)
        {
            _context.Qualifications.Update(qualification);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Qualification qualification)
        {
            _context.Qualifications.Remove(qualification);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
