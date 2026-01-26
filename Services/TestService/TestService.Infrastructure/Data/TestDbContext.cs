using Microsoft.EntityFrameworkCore;
using TestService.Domain.Entities;

namespace TestService.Infrastructure.Data
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<ExamMaster> Exams { get; set; }
        public DbSet<SubjectMaster> Subjects { get; set; }
        public DbSet<PracticeMode> PracticeModes { get; set; }
        public DbSet<TestSeries> TestSeries { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserTestAttempt> UserTestAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ExamMaster
            modelBuilder.Entity<ExamMaster>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IconUrl).HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.IsActive);
            });

            // Configure SubjectMaster
            modelBuilder.Entity<SubjectMaster>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IconUrl).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.ExamId);
                entity.HasIndex(e => e.IsActive);
                entity.HasOne(e => e.Exam).WithMany(ex => ex.Subjects).HasForeignKey(e => e.ExamId);
            });

            // Configure PracticeMode
            modelBuilder.Entity<PracticeMode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IconUrl).HasMaxLength(500);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.IsActive);
                entity.HasData(
                    new PracticeMode { Id = PracticeModeIds.MockTest, Name = "Mock Test", Description = "Full-length mock tests", DisplayOrder = 1, IsFeatured = true, IsActive = true },
                    new PracticeMode { Id = PracticeModeIds.TestSeries, Name = "Test Series", Description = "Series of practice tests", DisplayOrder = 2, IsFeatured = true, IsActive = true },
                    new PracticeMode { Id = PracticeModeIds.DeepPractice, Name = "Deep Practice", Description = "Subject-wise focused practice", DisplayOrder = 3, IsFeatured = true, IsActive = true },
                    new PracticeMode { Id = PracticeModeIds.PreviousYear, Name = "Previous Year", Description = "Previous year question papers", DisplayOrder = 4, IsFeatured = true, IsActive = true }
                );
            });

            // Configure TestSeries
            modelBuilder.Entity<TestSeries>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.InstructionsEnglish).HasMaxLength(2000);
                entity.Property(e => e.InstructionsHindi).HasMaxLength(2000);
                entity.Property(e => e.IsLocked).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.ExamId);
                entity.HasIndex(e => e.IsActive);
                entity.HasOne(e => e.Exam).WithMany(ex => ex.TestSeries).HasForeignKey(e => e.ExamId);
            });

            // Configure Test
            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.InstructionsEnglish).HasMaxLength(2000);
                entity.Property(e => e.InstructionsHindi).HasMaxLength(2000);
                entity.Property(e => e.IsLocked).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.ExamId);
                entity.HasIndex(e => e.PracticeModeId);
                entity.HasIndex(e => e.SeriesId);
                entity.HasIndex(e => e.SubjectId);
                entity.HasIndex(e => e.Year);
                entity.HasIndex(e => e.IsActive);
                entity.HasOne(e => e.Exam).WithMany(ex => ex.Tests).HasForeignKey(e => e.ExamId);
                entity.HasOne(e => e.PracticeMode).WithMany().HasForeignKey(e => e.PracticeModeId);
                entity.HasOne(e => e.Series).WithMany(ts => ts.Tests).HasForeignKey(e => e.SeriesId);
                entity.HasOne(e => e.Subject).WithMany(sub => sub.Tests).HasForeignKey(e => e.SubjectId);
            });

            // Configure Question
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuestionText).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.VideoUrl).HasMaxLength(500);
                entity.Property(e => e.Explanation).HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.IsActive);
            });

            // Configure TestQuestion
            modelBuilder.Entity<TestQuestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.TestId);
                entity.HasIndex(e => e.QuestionId);
                entity.HasIndex(e => e.IsActive);
                entity.HasOne(e => e.Test).WithMany(t => t.TestQuestions).HasForeignKey(e => e.TestId);
                entity.HasOne(e => e.Question).WithMany(q => q.TestQuestions).HasForeignKey(e => e.QuestionId);
            });

            // Configure UserTestAttempt
            modelBuilder.Entity<UserTestAttempt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AnswersJson).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.TestId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsActive);
                entity.HasOne(e => e.Test).WithMany(t => t.UserTestAttempts).HasForeignKey(e => e.TestId);
            });

            // Apply soft delete filter
            ApplySoftDeleteFilter(modelBuilder);
        }

        private void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamMaster>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<SubjectMaster>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<PracticeMode>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<TestSeries>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Test>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Question>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<TestQuestion>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<UserTestAttempt>().HasQueryFilter(e => e.IsActive);
        }
    }
}
