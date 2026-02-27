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

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectLanguage> SubjectLanguages { get; set; }
        public DbSet<MasterService.Domain.Entities.Stream> Streams { get; set; }
        public DbSet<StreamLanguage> StreamLanguages { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StateLanguage> StateLanguages { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<QualificationLanguage> QualificationLanguages { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamLanguage> ExamLanguages { get; set; }           // ← ADDED
        public DbSet<ExamQualification> ExamQualifications { get; set; } // ← ADDED
        public DbSet<CmsContent> CmsContents { get; set; }
        public DbSet<CmsContentTranslation> CmsContentTranslations { get; set; } // ← ADDED

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EF Core by default singular name use karta hai — DB mein plural hain
            modelBuilder.Entity<ExamLanguage>().ToTable("ExamLanguages");
            modelBuilder.Entity<ExamQualification>().ToTable("ExamQualifications");
            modelBuilder.Entity<CmsContentTranslation>().ToTable("CmsContentTranslations");
        }
    }
}