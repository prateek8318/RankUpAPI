using AdminService.Domain.Entities;

namespace AdminService.Domain.Interfaces
{
    public interface IExportLogRepository
    {
        Task<ExportLog> AddAsync(ExportLog exportLog);
        Task<ExportLog?> GetByIdAsync(int id);
        Task<IEnumerable<ExportLog>> GetExportLogsAsync(int? adminId = null, int page = 1, int pageSize = 50);
        Task<int> SaveChangesAsync();
    }
}
