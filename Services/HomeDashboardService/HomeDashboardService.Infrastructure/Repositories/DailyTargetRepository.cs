using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class DailyTargetRepository : GenericRepository<DailyTarget>, IDailyTargetRepository
    {
        public DailyTargetRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<DailyTarget?> GetUserTargetForDateAsync(int userId, DateTime targetDate)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@TargetDate", targetDate)
            };

            return await _context.DailyTargets
                .FromSqlRaw("EXEC DailyTarget_GetUserTargetForDate @UserId, @TargetDate", parameters)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DailyTarget>> GetUserTargetsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@StartDate", (object?)startDate ?? DBNull.Value),
                new SqlParameter("@EndDate", (object?)endDate ?? DBNull.Value)
            };

            return await _context.DailyTargets
                .FromSqlRaw("EXEC DailyTarget_GetUserTargets @UserId, @StartDate, @EndDate", parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<DailyTarget>> GetTargetsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@StartDate", (object?)startDate ?? DBNull.Value),
                new SqlParameter("@EndDate", (object?)endDate ?? DBNull.Value)
            };

            return await _context.DailyTargets
                .FromSqlRaw("EXEC DailyTarget_GetTargets @StartDate, @EndDate", parameters)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
