using MasterService.Domain.Entities;
using Common.DTOs;
using System.Data;

namespace MasterService.Application.Interfaces
{
    public interface IExamRepository
    {
        Task<Exam?> GetByIdAsync(int id);
        Task<Exam?> GetByIdLocalizedAsync(int id, string? languageCode);
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<PaginatedResponse<Exam>> GetAllAsync(PaginationRequest pagination);
        Task<IEnumerable<Exam>> GetActiveAsync();
        Task<PaginatedResponse<Exam>> GetActiveAsync(PaginationRequest pagination);
        Task<IEnumerable<Exam>> GetAllIncludingInactiveAsync();
        Task<IEnumerable<Exam>> GetActiveLocalizedAsync(string? languageCode);
        Task<IEnumerable<Exam>> GetAllIncludingInactiveLocalizedAsync(string? languageCode);
        Task<IEnumerable<Exam>> GetByFilterAsync(string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge);
        Task<IEnumerable<Exam>> GetByFilterLocalizedAsync(string? languageCode, string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge);
        Task<Exam> AddAsync(Exam exam, string? namesJson = null, string? relationsJson = null);
        Task<Exam> AddAsync(Exam exam, string? namesJson = null, string? relationsJson = null, IDbTransaction? transaction = null);
        Task UpdateAsync(Exam exam, string? namesJson = null, string? relationsJson = null);
        Task UpdateAsync(Exam exam, string? namesJson = null, string? relationsJson = null, IDbTransaction? transaction = null);
        Task ReplaceExamSubjectsAsync(int examId, IEnumerable<int> subjectIds, IDbTransaction? transaction = null);
        Task<Dictionary<int, List<int>>> GetSubjectMappingsByExamIdsAsync(IEnumerable<int> examIds);
        Task DeleteAsync(Exam exam);
        Task DeleteAsync(Exam exam, IDbTransaction? transaction = null);
        Task<bool> SoftDeleteByIdAsync(int id);
        Task<bool> SetActiveAsync(int id, bool isActive);
        Task<int> SaveChangesAsync();
    }
}

