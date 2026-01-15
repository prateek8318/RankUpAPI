using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class BulkUploadLogRepository : GenericRepository<BulkUploadLog>, IBulkUploadLogRepository
    {
        public BulkUploadLogRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BulkUploadLog>> GetByStatusAsync(BulkUploadStatus status)
        {
            return await _dbSet
                .Where(l => l.Status == status && l.IsActive)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<BulkUploadLog?> GetByIdWithErrorsAsync(int id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(l => l.Id == id && l.IsActive);
        }
    }
}
