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
        }
    }
}
