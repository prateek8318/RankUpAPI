using AdminService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IExportService
    {
        Task<ExportResultDto> ExportUsersToExcelAsync(ExportFilterDto? filter = null, int adminId = 0);
        Task<ExportResultDto> ExportExamsToExcelAsync(ExportFilterDto? filter = null, int adminId = 0);
        Task<ExportResultDto> ExportSubscriptionsToExcelAsync(ExportFilterDto? filter = null, int adminId = 0);
        Task<ExportResultDto> ExportQuizzesToExcelAsync(ExportFilterDto? filter = null, int adminId = 0);
        Task<ExportLogDto?> GetExportLogByIdAsync(int id);
        Task<IEnumerable<ExportLogDto>> GetExportLogsAsync(int? adminId = null, int page = 1, int pageSize = 50);
    }
}
