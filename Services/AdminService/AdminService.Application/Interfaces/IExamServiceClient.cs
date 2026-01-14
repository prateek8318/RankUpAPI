using ExamService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IExamServiceClient
    {
        Task<ExamDto?> GetExamByIdAsync(int examId);
        Task<IEnumerable<ExamDto>?> GetAllExamsAsync();
    }
}
