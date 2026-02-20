using MasterService.Application.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface IExamService
    {
        Task<ExamDto> CreateExamAsync(CreateExamDto createDto);
        Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto);
        Task<bool> DeleteExamAsync(int id);
        Task<ExamDto?> GetExamByIdAsync(int id, int? languageId = null);
        Task<IEnumerable<ExamDto>> GetAllExamsAsync(int? languageId = null);
        Task<IEnumerable<ExamDto>> GetExamsByFilterAsync(string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge, int? languageId = null);
        Task<bool> ToggleExamStatusAsync(int id, bool isActive);
    }
}

