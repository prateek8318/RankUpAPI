using ExamService.Application.DTOs;

namespace ExamService.Application.Interfaces
{
    public interface IExamService
    {
        Task<ExamDto> CreateExamAsync(CreateExamDto createDto);
        Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto);
        Task<bool> DeleteExamAsync(int id);
        Task<ExamDto?> GetExamByIdAsync(int id);
        Task<ExamDto?> GetExamByIdAsync(int id, string? language);
        Task<IEnumerable<ExamDto>> GetAllExamsAsync(bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetAllExamsIncludingInactiveAsync(bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetAllExamsIncludingInactiveAsync(string? language, bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetExamsByQualificationAsync(int qualificationId, int? streamId = null);
        Task<IEnumerable<ExamDto>> GetExamsByQualificationAsync(int qualificationId, string? language, int? streamId = null);
        Task<IEnumerable<ExamDto>> GetExamsForUserAsync(int userId);
        Task<bool> ToggleExamStatusAsync(int id, bool isActive);
        Task<int> SeedInternationalExamsAsync();

        Task<IEnumerable<ExamCategoryDto>> GetActiveCategoriesAsync();
        Task<IEnumerable<ExamTypeDto>> GetTypesByCategoryIdAsync(int categoryId);
        
        // Admin specific methods
        Task<ExamStatsDto> GetExamStatsAsync();
        Task<IEnumerable<ExamCategoryDto>> GetExamCategoriesAsync();
        Task<IEnumerable<ExamTypeDto>> GetExamTypesByCategoryAsync(int categoryId);
        Task<IEnumerable<ExamDto>> GetFilteredExamsAsync(int? categoryId, int? typeId, string? status);
        Task<bool> UpdateExamStatusAsync(int id, string status);
        Task<ExamDashboardDto> GetExamDashboardAsync();
    }
}
