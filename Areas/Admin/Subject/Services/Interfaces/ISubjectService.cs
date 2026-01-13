using RankUpAPI.Areas.Admin.Subject.DTOs;

namespace RankUpAPI.Areas.Admin.Subject.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createDto);
        Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto updateDto);
        Task<bool> DeleteSubjectAsync(int id);
        Task<SubjectDto?> GetSubjectByIdAsync(int id);
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();
        Task<IEnumerable<SubjectDto>> GetSubjectsByExamIdAsync(int examId);
        Task<bool> ToggleSubjectStatusAsync(int id, bool isActive);
    }
}
