using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace QuestionService.Infrastructure.Data
{
    public class QuestionDbContext : DbContext
    {
        public QuestionDbContext(DbContextOptions<QuestionDbContext> options)
            : base(options)
        {
        }

        // No DbSet properties - using Dapper for stored procedures
        // No OnModelCreating - using stored procedures instead
    }
}
