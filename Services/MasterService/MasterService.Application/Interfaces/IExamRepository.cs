using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface IExamRepository
    {
        Task<Exam?> GetByIdAsync(int id);
        Task<Exam?> GetByIdLocalizedAsync(int id, string? languageCode);
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<IEnumerable<Exam>> GetActiveAsync();
        Task<IEnumerable<Exam>> GetActiveLocalizedAsync(string? languageCode);
        Task<IEnumerable<Exam>> GetByFilterAsync(string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge);
        Task<IEnumerable<Exam>> GetByFilterLocalizedAsync(string? languageCode, string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge);
        Task<Exam> AddAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(Exam exam);
        Task<bool> SoftDeleteByIdAsync(int id);
        Task<bool> SetActiveAsync(int id, bool isActive);
        Task<int> SaveChangesAsync();
    }
}

