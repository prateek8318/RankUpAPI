using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AdminDbContext _context;

        public AuditLogRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            return auditLog;
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? adminId = null, string? serviceName = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            var query = _context.AuditLogs
                .Include(a => a.Admin)
                .AsQueryable();

            if (adminId.HasValue)
                query = query.Where(a => a.AdminId == adminId.Value);

            if (!string.IsNullOrEmpty(serviceName))
                query = query.Where(a => a.ServiceName == serviceName);

            if (startDate.HasValue)
                query = query.Where(a => a.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.CreatedAt <= endDate.Value);

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
