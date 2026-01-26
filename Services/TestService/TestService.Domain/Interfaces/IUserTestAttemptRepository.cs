using TestService.Domain.Entities;

namespace TestService.Domain.Interfaces
{
    public interface IUserTestAttemptRepository : IRepository<UserTestAttempt>
    {
        Task<IEnumerable<UserTestAttempt>> GetOngoingByUserIdAsync(int userId);
        Task<UserTestAttempt?> GetByIdWithTestAsync(int id);
        Task<IEnumerable<UserTestAttempt>> GetByUserIdAsync(int userId, int limit = 10);
    }
}
