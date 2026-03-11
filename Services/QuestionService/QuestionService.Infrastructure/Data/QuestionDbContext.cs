using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;

namespace QuestionService.Infrastructure.Data
{
    public class QuestionDbContext : DbContext
    {
        public QuestionDbContext(DbContextOptions<QuestionDbContext> options)
            : base(options)
        {
        }

        public DbSet<Question> Questions { get; set; }

        // No OnModelCreating - using stored procedures instead
    }
}
