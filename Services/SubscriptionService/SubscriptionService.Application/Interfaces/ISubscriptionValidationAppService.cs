using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface ISubscriptionValidationAppService
    {
        Task<SubscriptionValidationResponseDto> ValidateSubscriptionAsync(SubscriptionValidationRequestDto request);
        Task<DemoEligibilityResponseDto> CheckDemoEligibilityAsync(DemoEligibilityRequestDto request);
        Task<bool> IsSubscriptionActiveAsync(int userId);
        Task<bool> LogDemoAccessAsync(LogDemoAccessDto logDemoAccessDto);
        Task<bool> ValidateSubscriptionForServiceAsync(int userId, string examCategory = null);
    }
}
