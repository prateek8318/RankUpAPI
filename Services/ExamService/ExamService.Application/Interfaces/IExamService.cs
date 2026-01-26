using ExamService.Application.DTOs;

namespace ExamService.Application.Interfaces
{
    public interface IExamService
    {
        Task<ExamDto> CreateExamAsync(CreateExamDto createDto);
        Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto);
        Task<bool> DeleteExamAsync(int id);
        Task<ExamDto?> GetExamByIdAsync(int id);
        Task<IEnumerable<ExamDto>> GetAllExamsAsync(bool? isInternational = null);
        Task<IEnumerable<ExamDto>> GetExamsByQualificationAsync(int qualificationId, int? streamId = null);
        Task<IEnumerable<ExamDto>> GetExamsForUserAsync(int userId);
        Task<bool> ToggleExamStatusAsync(int id, bool isActive);
        Task<int> SeedInternationalExamsAsync();
    }
}
