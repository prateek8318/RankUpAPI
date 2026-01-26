using ExamService.Domain.Entities;

namespace ExamService.Application.Interfaces
{
    public interface IExamRepository
    {
        Task<Exam?> GetByIdAsync(int id);
        Task<Exam?> GetByIdWithQualificationsAsync(int id);
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<IEnumerable<Exam>> GetActiveAsync();
        Task<IEnumerable<Exam>> GetByQualificationIdAsync(int qualificationId);
        Task<IEnumerable<Exam>> GetByQualificationAndStreamAsync(int qualificationId, int? streamId);
        Task<Exam> AddAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(Exam exam);
        Task<int> SaveChangesAsync();
    }
}
