using RankUpAPI.DTOs;

namespace RankUpAPI.Services.Interfaces
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
