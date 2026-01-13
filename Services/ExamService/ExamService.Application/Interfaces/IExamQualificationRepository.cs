using ExamService.Domain.Entities;

namespace ExamService.Application.Interfaces
{
    public interface IExamQualificationRepository
    {
        Task AddAsync(ExamQualification examQualification);
        Task AddRangeAsync(IEnumerable<ExamQualification> examQualifications);
        Task DeleteAsync(ExamQualification examQualification);
        Task DeleteByExamIdAsync(int examId);
        Task<int> SaveChangesAsync();
    }
}
