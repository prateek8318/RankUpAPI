using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AdminService.Application.Clients
{
    public class HomeDashboardServiceClient : IHomeDashboardServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeDashboardServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public HomeDashboardServiceClient(HttpClient httpClient, ILogger<HomeDashboardServiceClient> logger)
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

        public async Task<object?> GetDailyVideoAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/user/dashboard/daily-video"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling HomeDashboardService for daily video");
                return null;
            }
        }

        public async Task<object?> CreateDailyVideoAsync(object createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/admin/daily-videos", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling HomeDashboardService to create daily video");
                return null;
            }
        }

        public async Task<object?> UpdateDailyVideoAsync(int id, object updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PutAsync($"/api/admin/daily-videos/{id}", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling HomeDashboardService to update daily video {id}");
                return null;
            }
        }

        public async Task<bool> DeleteDailyVideoAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.DeleteAsync($"/api/admin/daily-videos/{id}"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling HomeDashboardService to delete daily video {id}");
                return false;
            }
        }

        public async Task<object?> GetAllDailyVideosAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/daily-videos"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling HomeDashboardService for all daily videos");
                return null;
            }
        }

        public async Task<object?> GetBannersAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/user/dashboard/banners"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling HomeDashboardService for banners");
                return null;
            }
        }

        public async Task<object?> CreateBannerAsync(object createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/admin/banners", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling HomeDashboardService to create banner");
                return null;
            }
        }

        public async Task<object?> UpdateBannerAsync(int id, object updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PutAsync($"/api/admin/banners/{id}", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling HomeDashboardService to update banner {id}");
                return null;
            }
        }

        public async Task<bool> DeleteBannerAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.DeleteAsync($"/api/admin/banners/{id}"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling HomeDashboardService to delete banner {id}");
                return false;
            }
        }
    }
}
