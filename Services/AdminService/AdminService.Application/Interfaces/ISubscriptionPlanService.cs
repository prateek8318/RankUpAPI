using AdminService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanRequest request);
        Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id);
        Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync(int page = 1, int pageSize = 50);
        Task<(IEnumerable<SubscriptionPlanDto> Plans, int TotalCount)> GetFilteredPlansAsync(SubscriptionPlanFilterRequest filter);
        Task<SubscriptionPlanDto> UpdatePlanAsync(int id, UpdateSubscriptionPlanRequest request);
        Task<bool> DeletePlanAsync(int id);
        Task<SubscriptionPlanStatsDto> GetStatsAsync();
    }
}
