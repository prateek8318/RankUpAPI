using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterService.Infrastructure.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly MasterDbContext _context;

        public LanguageRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<Language?> GetByIdAsync(int id)
        {
            return await _context.Languages.FindAsync(id);
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            return await _context.Languages.ToListAsync();
        }

        public async Task<IEnumerable<Language>> GetActiveAsync()
        {
            return await _context.Languages
                .Where(l => l.IsActive)
                .ToListAsync();
        }

        public async Task<Language> AddAsync(Language language)
        {
            await _context.Languages.AddAsync(language);
            return language;
        }

        public Task UpdateAsync(Language language)
        {
            _context.Languages.Update(language);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Language language)
        {
            _context.Languages.Remove(language);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
