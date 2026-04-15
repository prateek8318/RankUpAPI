using System.Collections.Generic;
using System.Threading.Tasks;
using MasterService.Application.DTOs;
using MasterService.Domain.Entities;
using Common.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync(int? languageId = null);
        Task<IEnumerable<SubjectDto>> GetAllSubjectsIncludingInactiveAsync(int? languageId = null);
        Task<SubjectDto?> GetSubjectByIdAsync(int id, int? languageId = null);
        Task<IEnumerable<SubjectListDto>> GetActiveSubjectsAsync(int? languageId = null);
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
        Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto);
        Task<bool> DeleteSubjectAsync(int id);
        Task<bool> ToggleSubjectStatusAsync(int id, bool isActive);
        Task<bool> SubjectExistsAsync(int id);
        Task<PaginatedResponse<SubjectDto>> GetAllSubjectsPaginatedAsync(PaginationRequest pagination);
        Task<PaginatedResponse<SubjectListDto>> GetActiveSubjectsPaginatedAsync(PaginationRequest pagination);
    }

    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync(int? languageId = null);
        Task<PaginatedResponse<Subject>> GetAllAsync(PaginationRequest pagination);
        Task<Subject?> GetByIdAsync(int id, int? languageId = null);
        Task<IEnumerable<Subject>> GetActiveSubjectsAsync(int? languageId = null);
        Task<PaginatedResponse<Subject>> GetActiveSubjectsAsync(PaginationRequest pagination);
        Task<Subject> AddAsync(Subject subject, string? namesJson = null);
        Task<Subject> UpdateAsync(Subject subject, string? namesJson = null);
        Task DeleteAsync(Subject subject);
        Task<bool> ToggleSubjectStatusAsync(int id, bool isActive);
        Task<bool> ExistsAsync(int id);
        Task<int> SaveChangesAsync();
    }

    public interface ISubjectLanguageRepository
    {
        Task<IEnumerable<SubjectLanguage>> GetBySubjectIdAsync(int subjectId);
        Task<SubjectLanguage?> GetByIdAsync(int id);
        Task<SubjectLanguage> AddAsync(SubjectLanguage subjectLanguage);
        Task<SubjectLanguage> UpdateAsync(SubjectLanguage subjectLanguage);
        Task DeleteAsync(SubjectLanguage subjectLanguage);
        Task<int> SaveChangesAsync();
    }
}
