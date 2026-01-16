using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Infrastructure.Data
{
    public class HomeDashboardDbContext : DbContext
    {
        public HomeDashboardDbContext(DbContextOptions<HomeDashboardDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<HomeBanner> HomeBanners { get; set; }
        public DbSet<OfferBanner> OfferBanners { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<DailyVideo> DailyVideos { get; set; }
        public DbSet<BulkUploadLog> BulkUploadLogs { get; set; }
        public DbSet<PracticeMode> PracticeModes { get; set; }
        public DbSet<DailyTarget> DailyTargets { get; set; }
        public DbSet<RapidFireTest> RapidFireTests { get; set; }
        public DbSet<FreeTest> FreeTests { get; set; }
        public DbSet<MotivationMessage> MotivationMessages { get; set; }
        public DbSet<SubscriptionBanner> SubscriptionBanners { get; set; }
        public DbSet<ContinuePracticeItem> ContinuePracticeItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Exam
            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // Configure Subject
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => e.ExamId);
                entity.HasIndex(e => e.IsActive);
                
                entity.HasOne(e => e.Exam)
                    .WithMany(e => e.Subjects)
                    .HasForeignKey(e => e.ExamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Chapter
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => e.SubjectId);
                entity.HasIndex(e => e.IsActive);
                
                entity.HasOne(e => e.Subject)
                    .WithMany(e => e.Chapters)
                    .HasForeignKey(e => e.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Quiz
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.HasIndex(e => e.ChapterId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsActive);
                
                entity.HasOne(e => e.Chapter)
                    .WithMany(e => e.Quizzes)
                    .HasForeignKey(e => e.ChapterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Question
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuestionText).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.VideoUrl).HasMaxLength(500);
                entity.Property(e => e.Explanation).HasMaxLength(2000);
                entity.HasIndex(e => e.QuizId);
                entity.HasIndex(e => e.IsActive);
                
                entity.HasOne(e => e.Quiz)
                    .WithMany(e => e.Questions)
                    .HasForeignKey(e => e.QuizId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure QuestionOption
            modelBuilder.Entity<QuestionOption>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OptionText).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.HasIndex(e => e.QuestionId);
                entity.HasIndex(e => e.IsActive);
                
                entity.HasOne(e => e.Question)
                    .WithMany(e => e.Options)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure HomeBanner (keeping table name DashboardBanners for backward compatibility)
            modelBuilder.Entity<HomeBanner>(entity =>
            {
                entity.ToTable("DashboardBanners"); // Keep existing table name
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // Configure OfferBanner
            modelBuilder.Entity<OfferBanner>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.Property(e => e.DiscountCode).HasMaxLength(100);
                entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // Configure Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).HasMaxLength(1000);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure LeaderboardEntry
            modelBuilder.Entity<LeaderboardEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Accuracy).HasPrecision(5, 2);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.QuizId);
                entity.HasIndex(e => e.Rank);
                
                entity.HasOne(e => e.Quiz)
                    .WithMany()
                    .HasForeignKey(e => e.QuizId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure QuizAttempt
            modelBuilder.Entity<QuizAttempt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Accuracy).HasPrecision(5, 2);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.QuizId);
                entity.HasIndex(e => e.Status);
                
                entity.HasOne(e => e.Quiz)
                    .WithMany(e => e.QuizAttempts)
                    .HasForeignKey(e => e.QuizId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure DailyVideo
            modelBuilder.Entity<DailyVideo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.VideoUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.VideoId).HasMaxLength(100);
                entity.Property(e => e.PlaylistId).HasMaxLength(100);
                entity.HasIndex(e => e.VideoDate);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure BulkUploadLog
            modelBuilder.Entity<BulkUploadLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FilePath).HasMaxLength(500);
                entity.Property(e => e.ErrorReportPath).HasMaxLength(500);
                entity.Property(e => e.ErrorSummary).HasMaxLength(2000);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ProcessedByUserId);
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
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.Type);
            });

            // Configure DailyTarget
            modelBuilder.Entity<DailyTarget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.TargetDate);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure RapidFireTest
            modelBuilder.Entity<RapidFireTest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.QuizId);
            });

            // Configure FreeTest
            modelBuilder.Entity<FreeTest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.QuizId);
                entity.HasIndex(e => e.ExamId);
            });

            // Configure MotivationMessage
            modelBuilder.Entity<MotivationMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Author).HasMaxLength(200);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsGreeting);
            });

            // Configure SubscriptionBanner
            modelBuilder.Entity<SubscriptionBanner>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.Property(e => e.CtaText).HasMaxLength(100);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
            });

            // Configure ContinuePracticeItem
            modelBuilder.Entity<ContinuePracticeItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuizTitle).HasMaxLength(200);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.QuizId);
                entity.HasIndex(e => e.QuizAttemptId);
                entity.HasIndex(e => e.IsActive);
            });

            // Configure global query filters for soft delete
            modelBuilder.Entity<Exam>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Subject>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Chapter>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Quiz>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Question>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<QuestionOption>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<HomeBanner>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<OfferBanner>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Notification>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<LeaderboardEntry>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<QuizAttempt>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<DailyVideo>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<BulkUploadLog>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<PracticeMode>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<DailyTarget>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<RapidFireTest>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<FreeTest>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<MotivationMessage>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<SubscriptionBanner>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<ContinuePracticeItem>().HasQueryFilter(e => e.IsActive);
        }
    }
}
