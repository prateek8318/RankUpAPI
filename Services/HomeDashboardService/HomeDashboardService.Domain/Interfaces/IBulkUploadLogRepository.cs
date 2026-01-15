using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IBulkUploadLogRepository : IRepository<BulkUploadLog>
    {
        Task<IEnumerable<BulkUploadLog>> GetByStatusAsync(BulkUploadStatus status);
        Task<BulkUploadLog?> GetByIdWithErrorsAsync(int id);
    }
}
