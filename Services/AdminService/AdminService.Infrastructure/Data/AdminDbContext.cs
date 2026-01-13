using AdminService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
        public DbSet<AdminSession> AdminSessions { get; set; }
        public DbSet<AdminActivityLog> AdminActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Admin entity
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasIndex(a => a.UserId);
                entity.Property(a => a.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure Permission entity
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(p => new { p.Name, p.Resource, p.Action }).IsUnique();
                entity.Property(p => p.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure RolePermission join entity
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rp => rp.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(rp => rp.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure AdminRole join entity
            modelBuilder.Entity<AdminRole>(entity =>
            {
                entity.HasKey(ar => new { ar.AdminId, ar.RoleId });

                entity.HasOne(ar => ar.Admin)
                      .WithMany(a => a.AdminRoles)
                      .HasForeignKey(ar => ar.AdminId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ar => ar.Role)
                      .WithMany(r => r.AdminRoles)
                      .HasForeignKey(ar => ar.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ar => ar.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(ar => ar.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure AdminSession entity
            modelBuilder.Entity<AdminSession>(entity =>
            {
                entity.HasOne(s => s.Admin)
                      .WithMany(a => a.AdminSessions)
                      .HasForeignKey(s => s.AdminId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.IsActive).HasDefaultValue(true).ValueGeneratedNever();
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure AdminActivityLog entity
            modelBuilder.Entity<AdminActivityLog>(entity =>
            {
                entity.HasOne(l => l.Admin)
                      .WithMany(a => a.ActivityLogs)
                      .HasForeignKey(l => l.AdminId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(l => new { l.AdminId, l.CreatedAt });
                entity.Property(l => l.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
