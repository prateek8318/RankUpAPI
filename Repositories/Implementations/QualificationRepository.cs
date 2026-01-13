using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class QualificationRepository : Repository<Qualification>, IQualificationRepository
    {
        public QualificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Qualification?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(q => q.Name == name);
        }

        public async Task<IEnumerable<Qualification>> GetActiveAsync()
        {
            return await _dbSet.Where(q => q.IsActive).ToListAsync();
        }
    }
}
