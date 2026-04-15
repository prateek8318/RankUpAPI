using ExamService.Domain.Entities;

namespace ExamService.Application.Interfaces
{
    public interface IExamRepository
    {
        Task<Exam?> GetByIdAsync(int id);
        Task<Exam?> GetByIdWithQualificationsAsync(int id);
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<IEnumerable<Exam>> GetActiveAsync();
        Task<IEnumerable<Exam>> GetAllIncludingInactiveAsync();
        Task<IEnumerable<Exam>> GetByQualificationIdAsync(int qualificationId);
        Task<IEnumerable<Exam>> GetByQualificationAndStreamAsync(int qualificationId, int? streamId);
        Task<Exam> AddAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(Exam exam);
        Task<bool> HardDeleteByIdAsync(int id);
        Task<int> SaveChangesAsync();

        Task<IEnumerable<ExamCategory>> GetActiveCategoriesAsync();
        Task<IEnumerable<ExamType>> GetTypesByCategoryIdAsync(int categoryId);
        
        // Admin specific methods
        Task<ExamStatsDto> GetExamStatsAsync();
        Task<IEnumerable<ExamCategory>> GetExamCategoriesAsync();
        Task<IEnumerable<ExamType>> GetExamTypesByCategoryAsync(int categoryId);
        Task<IEnumerable<Exam>> GetFilteredExamsAsync(int? categoryId, int? typeId, string? status);
        Task<bool> UpdateExamStatusAsync(int id, string status);
        Task<ExamDashboardDto> GetExamDashboardAsync();
    }
}
