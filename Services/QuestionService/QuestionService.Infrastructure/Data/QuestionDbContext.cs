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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(q => q.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(q => q.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
