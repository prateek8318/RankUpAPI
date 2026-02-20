using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanDto createPlanDto);
        Task<SubscriptionPlanDto> UpdatePlanAsync(int id, UpdateSubscriptionPlanDto updatePlanDto);
        Task<bool> DeletePlanAsync(int id);
        Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id, string? language = null);
        Task<IEnumerable<SubscriptionPlanListDto>> GetAllPlansAsync(string? language = null);
        Task<IEnumerable<SubscriptionPlanListDto>> GetPlansByExamCategoryAsync(string examCategory, string? language = null);
        Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync(string? language = null);
    }
}
