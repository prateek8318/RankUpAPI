using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterService.Infrastructure.Repositories
{
    public class QualificationRepository : IQualificationRepository
    {
        private readonly MasterDbContext _context;

        public QualificationRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<Qualification?> GetByIdAsync(int id)
        {
            return await _context.Qualifications
                .Include(q => q.QualificationLanguages)
                .ThenInclude(ql => ql.Language)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            return await _context.Qualifications
                .Include(q => q.QualificationLanguages)
                .ThenInclude(ql => ql.Language)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualification>> GetActiveAsync()
        {
            return await _context.Qualifications
                .Include(q => q.QualificationLanguages)
                .ThenInclude(ql => ql.Language)
                .Where(q => q.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualification>> GetActiveByCountryCodeAsync(string countryCode)
        {
            return await _context.Qualifications
                .Include(q => q.QualificationLanguages)
                .ThenInclude(ql => ql.Language)
                .Where(q => q.IsActive && q.CountryCode == countryCode)
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
