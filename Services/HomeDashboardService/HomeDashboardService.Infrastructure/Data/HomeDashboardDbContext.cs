using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Infrastructure.Data
{
    public class HomeDashboardDbContext : DbContext
    {
        public HomeDashboardDbContext(DbContextOptions<HomeDashboardDbContext> options)
            : base(options)
        {
        }

        public DbSet<DailyTarget> DailyTargets { get; set; }

        // No OnModelCreating - using stored procedures instead
    }
}
