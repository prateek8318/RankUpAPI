using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanDto createPlanDto);
        Task<SubscriptionPlanDto> UpdatePlanAsync(int id, UpdateSubscriptionPlanDto updatePlanDto);
        Task<bool> DeletePlanAsync(int id);
        Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id);
        Task<IEnumerable<SubscriptionPlanListDto>> GetAllPlansAsync();
        Task<IEnumerable<SubscriptionPlanListDto>> GetPlansByExamCategoryAsync(string examCategory);
        Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync();
    }
}
