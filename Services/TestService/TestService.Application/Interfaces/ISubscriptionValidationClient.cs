using SubscriptionValidationResponse = TestService.Application.Services.SubscriptionValidationResponse;

namespace TestService.Application.Interfaces
{
    public interface ISubscriptionValidationClient
    {
        Task<SubscriptionValidationResponse?> ValidateAsync(int userId);
    }
}
