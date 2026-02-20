using MasterService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Infrastructure.Data
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }

        public DbSet<Language> Languages { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<StateLanguage> StateLanguages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CmsContent> CmsContents { get; set; }
        public DbSet<CmsContentTranslation> CmsContentTranslations { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<QualificationLanguage> QualificationLanguages { get; set; }
        public DbSet<StreamEntity> Streams { get; set; }
        public DbSet<StreamLanguage> StreamLanguages { get; set; }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamLanguage> ExamLanguages { get; set; }
        public DbSet<ExamQualification> ExamQualifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne<Country>()
                      .WithMany()
                      .HasForeignKey(e => e.CountryCode)
                      .HasPrincipalKey(c => c.Code)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<StateLanguage>(entity =>
            {
                entity.HasIndex(e => new { e.StateId, e.LanguageId }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.State)
                      .WithMany(s => s.StateLanguages)
                      .HasForeignKey(e => e.StateId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(e => e.Language)
                      .WithMany()
                      .HasForeignKey(e => e.LanguageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Key).IsUnique();
                entity.Property(e => e.NameEn)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.NameHi)
                      .HasMaxLength(100);
                entity.Property(e => e.Key)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(e => e.Type)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<CmsContent>(entity =>
            {
                entity.HasIndex(e => e.Key).IsUnique();
                entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.Translations)
                      .WithOne(t => t.CmsContent)
                      .HasForeignKey(t => t.CmsContentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CmsContentTranslation>(entity =>
            {
                entity.HasIndex(e => new { e.CmsContentId, e.LanguageCode }).IsUnique();
                entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Content).IsRequired();
            });

            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne<Country>()
                    .WithMany()
                    .HasForeignKey(e => e.CountryCode)
                    .HasPrincipalKey(c => c.Code)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Streams)
                    .WithOne(s => s.Qualification)
                    .HasForeignKey(s => s.QualificationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QualificationLanguage>(entity =>
            {
                entity.HasIndex(e => new { e.QualificationId, e.LanguageId }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.Qualification)
                    .WithMany(q => q.QualificationLanguages)
                    .HasForeignKey(e => e.QualificationId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Language)
                    .WithMany()
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StreamEntity>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne(s => s.Qualification)
                    .WithMany(q => q.Streams)
                    .HasForeignKey(s => s.QualificationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StreamLanguage>(entity =>
            {
                entity.HasIndex(e => new { e.StreamId, e.LanguageId }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.Stream)
                    .WithMany(s => s.StreamLanguages)
                    .HasForeignKey(e => e.StreamId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Language)
                    .WithMany()
                    .HasForeignKey(e => e.LanguageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne<Country>()
                      .WithMany()
                      .HasForeignKey(e => e.CountryCode)
                      .HasPrincipalKey(c => c.Code)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExamLanguage>(entity =>
            {
                entity.HasIndex(e => new { e.ExamId, e.LanguageId }).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Exam)
                      .WithMany(q => q.ExamLanguages)
                      .HasForeignKey(e => e.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Language)
                      .WithMany()
                      .HasForeignKey(e => e.LanguageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExamQualification>(entity =>
            {
                entity.HasIndex(e => new { e.ExamId, e.QualificationId, e.StreamId }).IsUnique();
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Exam)
                      .WithMany(e => e.ExamQualifications)
                      .HasForeignKey(e => e.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Qualification)
                      .WithMany()
                      .HasForeignKey(e => e.QualificationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Stream)
                      .WithMany()
                      .HasForeignKey(e => e.StreamId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
