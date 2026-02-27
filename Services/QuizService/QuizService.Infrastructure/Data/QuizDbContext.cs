using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuizService.Domain.Entities;

namespace QuizService.Infrastructure.Data
{
    public class QuizDbContext : DbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestSeries> TestSeries { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Chapter> Chapters { get; set; }

        // No OnModelCreating - using stored procedures instead
    }
}
