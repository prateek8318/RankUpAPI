using MasterService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.State)
                      .WithMany()
                      .HasForeignKey(e => e.StateId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(e => e.Language)
                      .WithMany()
                      .HasForeignKey(e => e.LanguageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
