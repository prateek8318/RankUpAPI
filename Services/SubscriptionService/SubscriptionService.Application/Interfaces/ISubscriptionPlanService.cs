using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanDto createPlanDto);
        Task<SubscriptionPlanDto> UpdatePlanAsync(int id, UpdateSubscriptionPlanDto updatePlanDto);
        Task<bool> DeletePlanAsync(int id);
        Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id, string? language = null);
        Task<SubscriptionPlanPagedResponseDto> GetPlansPagedAsync(SubscriptionPlanPagedRequestDto request);
        Task<IEnumerable<SubscriptionPlanListDto>> GetAllPlansAsync(string? language = null);
        Task<IEnumerable<SubscriptionPlanListDto>> GetPlansByExamCategoryAsync(string examCategory, string? language = null);
        Task<IEnumerable<SubscriptionPlanListDto>> GetPlansByExamIdAsync(int examId, string? language = null);
        Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync(string? language = null);
        Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansByExamIdsAsync(string? language, List<int> examIds);
        Task<SubscriptionPlanStatsDto> GetStatsAsync();
        
        // New methods for duration options support
        Task<PlanWithDurationOptionsDto> CreatePlanWithDurationsAsync(CreateSubscriptionPlanWithDurationDto createPlanDto);
        Task<PlanWithDurationOptionsDto> UpsertPlanWithDurationsAsync(CreateSubscriptionPlanWithDurationDto dto);
        Task<PlanWithDurationOptionsDto?> GetPlanWithDurationsAsync(int id, string? language = null, int? userId = null);
        Task<IEnumerable<PlanWithDurationOptionsDto>> GetActivePlansWithDurationsAsync(string? language = null, int? examId = null, int? userId = null);
        Task<bool> AddDurationOptionsAsync(int planId, List<CreatePlanDurationOptionDto> durationOptions);
        Task<PlanDurationOptionDto> UpdateDurationOptionAsync(int durationOptionId, UpdatePlanDurationOptionDto updateDto);
        Task<bool> DeleteDurationOptionAsync(int durationOptionId);
        Task<PurchasePlanResponseDto> PurchasePlanAsync(int userId, PurchasePlanRequestDto purchaseRequest);
        
        // Updated existing methods to support filtering
        Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync(string? language = null, int? examId = null, int? userId = null);
        Task<IEnumerable<PlanWithDurationOptionsDto>> GetAllPlansWithDurationsAsync(string? language = null, bool includeInactive = false);
    }
}
