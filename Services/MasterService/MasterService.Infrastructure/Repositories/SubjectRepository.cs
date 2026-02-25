using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly MasterDbContext _context;

        public SubjectRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .Include(s => s.SubjectLanguages)
                .ThenInclude(sl => sl.Language)
                .ToListAsync();
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.Subjects
                .Include(s => s.SubjectLanguages)
                .ThenInclude(sl => sl.Language)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync()
        {
            return await _context.Subjects
                .Include(s => s.SubjectLanguages)
                .ThenInclude(sl => sl.Language)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<Subject> AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            return subject;
        }

        public Task<Subject> UpdateAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            return Task.FromResult(subject);
        }

        public Task DeleteAsync(Subject subject)
        {
            _context.Subjects.Remove(subject);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subjects.AnyAsync(s => s.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

