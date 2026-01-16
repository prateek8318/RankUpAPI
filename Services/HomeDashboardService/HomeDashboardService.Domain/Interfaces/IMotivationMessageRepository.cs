using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IMotivationMessageRepository : IRepository<MotivationMessage>
    {
        Task<MotivationMessage?> GetTodayMessageAsync();
        Task<MotivationMessage?> GetGreetingMessageAsync();
        Task<IEnumerable<MotivationMessage>> GetActiveMessagesAsync(MessageType? type = null);
    }
}
