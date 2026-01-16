using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class MotivationMessageRepository : GenericRepository<MotivationMessage>, IMotivationMessageRepository
    {
        public MotivationMessageRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<MotivationMessage?> GetTodayMessageAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Where(m => m.IsActive && 
                           m.Type == MessageType.Daily &&
                           (m.StartDate == null || m.StartDate <= DateTime.UtcNow) &&
                           (m.EndDate == null || m.EndDate >= DateTime.UtcNow))
                .OrderBy(m => m.DisplayOrder)
                .FirstOrDefaultAsync();
        }

        public async Task<MotivationMessage?> GetGreetingMessageAsync()
        {
            return await _dbSet
                .Where(m => m.IsActive && 
                           m.IsGreeting &&
                           (m.StartDate == null || m.StartDate <= DateTime.UtcNow) &&
                           (m.EndDate == null || m.EndDate >= DateTime.UtcNow))
                .OrderBy(m => m.DisplayOrder)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MotivationMessage>> GetActiveMessagesAsync(MessageType? type = null)
        {
            var query = _dbSet.Where(m => m.IsActive &&
                                         (m.StartDate == null || m.StartDate <= DateTime.UtcNow) &&
                                         (m.EndDate == null || m.EndDate >= DateTime.UtcNow));

            if (type.HasValue)
                query = query.Where(m => m.Type == type.Value);

            return await query
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
        }
    }
}
