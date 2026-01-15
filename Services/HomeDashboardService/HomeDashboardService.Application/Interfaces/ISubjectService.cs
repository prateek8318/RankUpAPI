using HomeDashboardService.Application.DTOs;

namespace HomeDashboardService.Application.Interfaces
{
    public interface ISubjectService
    {
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
        Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto);
        Task<bool> DeleteSubjectAsync(int id);
        Task<bool> EnableDisableSubjectAsync(int id, bool isActive);
        Task<SubjectDto?> GetSubjectByIdAsync(int id);
        Task<IEnumerable<SubjectDto>> GetSubjectsByExamIdAsync(int examId);
    }
}
