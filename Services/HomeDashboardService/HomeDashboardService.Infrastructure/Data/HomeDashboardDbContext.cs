using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HomeDashboardService.Infrastructure.Data
{
    public class HomeDashboardDbContext : DbContext
    {
        public HomeDashboardDbContext(DbContextOptions<HomeDashboardDbContext> options)
            : base(options)
        {
        }

        // No DbSet properties - using Dapper for stored procedures
        // No OnModelCreating - using stored procedures instead
    }
}
