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
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ExportLog> ExportLogs { get; set; }
        public DbSet<DashboardCache> DashboardCaches { get; set; }

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

            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(200);
                entity.Property(e => e.HttpMethod).HasMaxLength(50);
                entity.Property(e => e.RequestPayload).HasMaxLength(2000);
                entity.Property(e => e.ResponsePayload).HasMaxLength(2000);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);

                entity.HasOne(e => e.Admin)
                      .WithMany()
                      .HasForeignKey(e => e.AdminId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.AdminId, e.CreatedAt });
                entity.HasIndex(e => e.ServiceName);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure ExportLog entity
            modelBuilder.Entity<ExportLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExportType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FilePath).HasMaxLength(500);
                entity.Property(e => e.FileName).HasMaxLength(200);
                entity.Property(e => e.Format).HasMaxLength(50);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.FilterCriteria).HasMaxLength(2000);

                entity.HasOne(e => e.Admin)
                      .WithMany()
                      .HasForeignKey(e => e.AdminId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AdminId);
                entity.HasIndex(e => e.ExportType);
                entity.HasIndex(e => e.Status);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure DashboardCache entity
            modelBuilder.Entity<DashboardCache>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CacheKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CacheData).IsRequired();
                entity.Property(e => e.CacheType).HasMaxLength(50);

                entity.HasIndex(e => e.CacheKey).IsUnique();
                entity.HasIndex(e => e.CacheType);
                entity.HasIndex(e => e.ExpiresAt);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
