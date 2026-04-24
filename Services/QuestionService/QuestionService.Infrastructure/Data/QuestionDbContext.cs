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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Even though most writes happen via stored procedures + Dapper, EF still validates
            // model metadata at startup; be explicit about decimal precision to avoid truncation.
            modelBuilder.Entity<Question>(b =>
            {
                b.Property(x => x.Marks).HasPrecision(10, 2);
                b.Property(x => x.NegativeMarks).HasPrecision(10, 2);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
