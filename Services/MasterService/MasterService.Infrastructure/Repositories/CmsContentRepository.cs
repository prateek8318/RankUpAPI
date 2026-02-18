using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MasterService.Infrastructure.Repositories
{
    public class CmsContentRepository : ICmsContentRepository
    {
        private readonly MasterDbContext _context;

        public CmsContentRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<CmsContent?> GetByIdAsync(int id)
        {
            return await _context.CmsContents
                .Include(c => c.Translations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CmsContent?> GetByKeyAsync(string key)
        {
            return await _context.CmsContents
                .Include(c => c.Translations)
                .FirstOrDefaultAsync(c => c.Key == key);
        }

        public async Task<bool> ExistsByKeyAsync(string key)
        {
            return await _context.CmsContents.AnyAsync(c => c.Key == key);
        }

        public async Task<bool> ExistsByKeyExceptIdAsync(string key, int excludeId)
        {
            return await _context.CmsContents.AnyAsync(c => c.Key == key && c.Id != excludeId);
        }

        public async Task<IEnumerable<CmsContent>> GetAllAsync()
        {
            return await _context.CmsContents
                .Include(c => c.Translations)
                .ToListAsync();
        }

        public async Task<IEnumerable<CmsContent>> GetActiveAsync()
        {
            return await _context.CmsContents
                .Include(c => c.Translations)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<CmsContent> AddAsync(CmsContent content)
        {
            await _context.CmsContents.AddAsync(content);
            return content;
        }

        public Task UpdateAsync(CmsContent content)
        {
            _context.CmsContents.Update(content);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CmsContent content)
        {
            _context.CmsContents.Remove(content);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}

