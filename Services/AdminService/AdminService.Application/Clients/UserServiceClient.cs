using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using UserService.Application.DTOs;

namespace AdminService.Application.Clients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} after {timespan.TotalSeconds}s");
                    });
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/users/{userId}"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                _logger.LogWarning($"Failed to get user {userId}: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling UserService for user {userId}");
                return null;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/users/by-email/{email}"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling UserService for email {email}");
                return null;
            }
        }
    }
}
