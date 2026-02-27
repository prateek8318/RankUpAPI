using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TestService.Infrastructure.Data
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        // No DbSet properties - using Dapper for stored procedures
        // No OnModelCreating - using stored procedures instead
    }
}
