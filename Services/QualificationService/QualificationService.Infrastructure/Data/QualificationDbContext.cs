using QualificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace QualificationService.Infrastructure.Data
{
    public class QualificationDbContext : DbContext
    {
        public QualificationDbContext(DbContextOptions<QualificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Qualification> Qualifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
