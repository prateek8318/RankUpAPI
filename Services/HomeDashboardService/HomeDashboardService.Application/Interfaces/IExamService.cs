using HomeDashboardService.Application.DTOs;

namespace HomeDashboardService.Application.Interfaces
{
    public interface IExamService
    {
        Task<ExamDto> CreateExamAsync(CreateExamDto createExamDto);
        Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateExamDto);
        Task<bool> DeleteExamAsync(int id);
        Task<bool> EnableDisableExamAsync(int id, bool isActive);
        Task<ExamDto?> GetExamByIdAsync(int id);
        Task<IEnumerable<ExamDto>> GetAllExamsAsync();
        Task<IEnumerable<ExamDto>> GetActiveExamsAsync();
    }
}
