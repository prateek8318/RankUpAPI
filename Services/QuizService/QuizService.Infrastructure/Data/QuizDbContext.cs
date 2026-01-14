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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TestSeries
            modelBuilder.Entity<TestSeries>(entity =>
            {
                entity.Property(ts => ts.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(ts => ts.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Subject
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasMany(s => s.Chapters)
                      .WithOne(c => c.Subject)
                      .HasForeignKey(c => c.SubjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Chapter
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasOne(c => c.Subject)
                      .WithMany(s => s.Chapters)
                      .HasForeignKey(c => c.SubjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
