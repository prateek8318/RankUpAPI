using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectLanguageRepository : ISubjectLanguageRepository
    {
        private readonly MasterDbContext _context;

        public SubjectLanguageRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectLanguage>> GetBySubjectIdAsync(int subjectId)
        {
            return await _context.SubjectLanguages
                .Include(sl => sl.Language)
                .Where(sl => sl.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<SubjectLanguage?> GetByIdAsync(int id)
        {
            return await _context.SubjectLanguages
                .Include(sl => sl.Language)
                .FirstOrDefaultAsync(sl => sl.Id == id);
        }

        public async Task<SubjectLanguage> AddAsync(SubjectLanguage subjectLanguage)
        {
            await _context.SubjectLanguages.AddAsync(subjectLanguage);
            return subjectLanguage;
        }

        public Task<SubjectLanguage> UpdateAsync(SubjectLanguage subjectLanguage)
        {
            _context.SubjectLanguages.Update(subjectLanguage);
            return Task.FromResult(subjectLanguage);
        }

        public Task DeleteAsync(SubjectLanguage subjectLanguage)
        {
            _context.SubjectLanguages.Remove(subjectLanguage);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

