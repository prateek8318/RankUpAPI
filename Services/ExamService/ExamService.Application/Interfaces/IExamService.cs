using ExamService.Application.DTOs;

namespace ExamService.Application.Interfaces
{
    public interface IExamService
    {
        Task<ExamDto> CreateExamAsync(CreateExamDto createDto, string? imageUrl = null);
        Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto);
        Task<bool> DeleteExamAsync(int id);
        Task<ExamDto?> GetExamByIdAsync(int id);
        Task<ExamDto?> GetExamByIdAsync(int id, string? language);
        Task<IEnumerable<ExamDto>> GetAllExamsAsync(bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetActiveExamsByFiltersAsync(int? examCategoryId = null, int? subjectId = null, int? examTypeId = null, bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetAllExamsIncludingInactiveAsync(bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetAllExamsIncludingInactiveAsync(string? language, bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetExamsForUserAsync(int userId);
        Task<bool> ToggleExamStatusAsync(int id, bool isActive);
        Task<int> SeedInternationalExamsAsync();

        // Admin specific methods
        Task<ExamStatsDto> GetExamStatsAsync();
        Task<IEnumerable<ExamDto>> GetAllExamsForAdminAsync(bool? isInternational = null);
        Task<bool> UpdateExamStatusAsync(int id, string status);
        Task<ExamDashboardDto> GetExamDashboardAsync();
        Task<int> BulkHardDeleteExamsAsync(int[] excludedIds);
    }
}
