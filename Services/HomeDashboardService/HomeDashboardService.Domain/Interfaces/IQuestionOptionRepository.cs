using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IQuestionOptionRepository : IRepository<QuestionOption>
    {
        Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(int questionId);
    }
}
