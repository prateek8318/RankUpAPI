using System.Collections.Generic;
using System.Threading.Tasks;
using MasterService.Application.DTOs;
using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();
        Task<SubjectDto> GetSubjectByIdAsync(int id);
        Task<IEnumerable<SubjectListDto>> GetActiveSubjectsAsync();
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto);
        Task<SubjectDto> UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto);
        Task<bool> DeleteSubjectAsync(int id);
        Task<bool> SubjectExistsAsync(int id);
    }

    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject> GetByIdAsync(int id);
        Task<IEnumerable<Subject>> GetActiveSubjectsAsync();
        Task<Subject> AddAsync(Subject subject);
        Task<Subject> UpdateAsync(Subject subject);
        Task DeleteAsync(Subject subject);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }

    public interface ISubjectLanguageRepository
    {
        Task<IEnumerable<SubjectLanguage>> GetBySubjectIdAsync(int subjectId);
        Task<SubjectLanguage> GetByIdAsync(int id);
        Task<SubjectLanguage> AddAsync(SubjectLanguage subjectLanguage);
        Task<SubjectLanguage> UpdateAsync(SubjectLanguage subjectLanguage);
        Task DeleteAsync(SubjectLanguage subjectLanguage);
        Task SaveChangesAsync();
    }
}
