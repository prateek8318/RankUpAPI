using Microsoft.EntityFrameworkCore;
using RankUpAPI.Models;

namespace RankUpAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQualification> ExamQualifications { get; set; }
        public DbSet<HomeSectionItem> HomeSectionItems { get; set; }
        
        // Test Series Hierarchy
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<TestSeries> TestSeries { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestSeriesQuestion> TestSeriesQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                // Use SQL Server compatible default datetime
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });

            // Configure Admin entity
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithOne(u => u.Admin)
                      .HasForeignKey<Admin>(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Qualification entity
            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.HasIndex(q => q.Name).IsUnique();
                // Ensure IsActive is treated as a client-set value so EF includes it in INSERTs
                entity.Property(q => q.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(q => q.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Exam entity
            modelBuilder.Entity<Exam>(entity =>
            {
                // Ensure IsActive is treated as a client-set value so EF includes it in INSERTs
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure ExamQualification join entity for many-to-many relationship
            modelBuilder.Entity<ExamQualification>(entity =>
            {
                entity.HasKey(eq => new { eq.ExamId, eq.QualificationId });

                entity.HasOne(eq => eq.Exam)
                      .WithMany(e => e.ExamQualifications)
                      .HasForeignKey(eq => eq.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(eq => eq.Qualification)
                      .WithMany(q => q.ExamQualifications)
                      .HasForeignKey(eq => eq.QualificationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure HomeSectionItem entity
            modelBuilder.Entity<HomeSectionItem>(entity =>
            {
                entity.HasIndex(h => new { h.SectionType, h.ExamId, h.DisplayOrder });
                entity.Property(h => h.IsVisible)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
            });

            // Configure Subject entity
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasOne(s => s.Exam)
                      .WithMany()
                      .HasForeignKey(s => s.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(s => s.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Chapter entity
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasOne(c => c.Subject)
                      .WithMany(s => s.Chapters)
                      .HasForeignKey(c => c.SubjectId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(c => c.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure TestSeries entity
            modelBuilder.Entity<TestSeries>(entity =>
            {
                entity.HasOne(ts => ts.Exam)
                      .WithMany()
                      .HasForeignKey(ts => ts.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(ts => ts.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(ts => ts.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Question entity
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne(q => q.Chapter)
                      .WithMany(c => c.Questions)
                      .HasForeignKey(q => q.ChapterId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(q => q.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(q => q.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure TestSeriesQuestion join entity
            modelBuilder.Entity<TestSeriesQuestion>(entity =>
            {
                entity.HasKey(tsq => new { tsq.TestSeriesId, tsq.QuestionId });

                entity.HasOne(tsq => tsq.TestSeries)
                      .WithMany(ts => ts.TestSeriesQuestions)
                      .HasForeignKey(tsq => tsq.TestSeriesId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tsq => tsq.Question)
                      .WithMany(q => q.TestSeriesQuestions)
                      .HasForeignKey(tsq => tsq.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(tsq => tsq.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(tsq => tsq.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
