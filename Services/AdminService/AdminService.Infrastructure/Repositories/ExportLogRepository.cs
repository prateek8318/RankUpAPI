using AdminService.Domain.Entities;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminService.Infrastructure.Repositories
{
    public class ExportLogRepository : IExportLogRepository
    {
        private readonly AdminDbContext _context;

        public ExportLogRepository(AdminDbContext context)
        {
            _context = context;
        }

        public async Task<ExportLog> AddAsync(ExportLog exportLog)
        {
            await _context.ExportLogs.AddAsync(exportLog);
            return exportLog;
        }

        public async Task<ExportLog?> GetByIdAsync(int id)
        {
            return await _context.ExportLogs
                .Include(e => e.Admin)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ExportLog>> GetExportLogsAsync(int? adminId = null, int page = 1, int pageSize = 50)
        {
            var query = _context.ExportLogs
                .Include(e => e.Admin)
                .AsQueryable();

            if (adminId.HasValue)
                query = query.Where(e => e.AdminId == adminId.Value);

            return await query
                .OrderByDescending(e => e.CreatedAt)
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
