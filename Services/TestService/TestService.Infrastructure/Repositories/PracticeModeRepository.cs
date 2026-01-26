using Microsoft.EntityFrameworkCore;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class PracticeModeRepository : GenericRepository<PracticeMode>, IPracticeModeRepository
    {
        public PracticeModeRepository(TestDbContext context) : base(context)
        {
        }

        public async Task<PracticeMode?> GetByIdAsync(int id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pm => pm.Id == id);
        }

        public async Task<IEnumerable<PracticeMode>> GetActiveModesAsync()
        {
            return await _dbSet
                .Where(pm => pm.IsActive)
                .OrderBy(pm => pm.DisplayOrder)
                .ToListAsync();
        }
    }
}
