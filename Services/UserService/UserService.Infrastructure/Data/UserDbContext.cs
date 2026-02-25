using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserSocialLogin> UserSocialLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(u => u.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
                entity.HasIndex(u => u.PhoneNumber).IsUnique();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<UserSocialLogin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Provider).IsRequired(false);
                entity.Property(e => e.GoogleId).IsRequired(false);
                entity.Property(e => e.AvatarUrl).IsRequired(false);
                entity.Property(e => e.AccessToken).IsRequired(false);
                entity.Property(e => e.RefreshToken).IsRequired(false);
                entity.Property(e => e.ExpiresAt).IsRequired(false);
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });
        }
    }
}
