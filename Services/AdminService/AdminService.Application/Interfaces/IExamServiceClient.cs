using ExamService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IExamServiceClient
    {
        Task<ExamDto?> GetExamByIdAsync(int examId);
        Task<IEnumerable<ExamDto>?> GetAllExamsAsync();
        Task<ExamDto?> CreateExamAsync(object createDto);
        Task<ExamDto?> UpdateExamAsync(int id, object updateDto);
        Task<bool> DeleteExamAsync(int id);
        Task<bool> EnableDisableExamAsync(int id, bool isActive);
    }
}
