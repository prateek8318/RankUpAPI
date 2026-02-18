using ExamService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamService.Infrastructure.Data
{
    public class ExamDbContext : DbContext
    {
        public ExamDbContext(DbContextOptions<ExamDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQualification> ExamQualifications { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<ExamAnswer> ExamAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Exam entity
            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure ExamQualification join entity
            modelBuilder.Entity<ExamQualification>(entity =>
            {
                entity.HasKey(eq => new { eq.ExamId, eq.QualificationId });

                entity.HasOne(eq => eq.Exam)
                      .WithMany(e => e.ExamQualifications)
                      .HasForeignKey(eq => eq.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(eq => eq.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(eq => eq.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
