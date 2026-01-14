using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using ExamService.Application.DTOs;

namespace AdminService.Application.Clients
{
    public class ExamServiceClient : IExamServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExamServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public ExamServiceClient(HttpClient httpClient, ILogger<ExamServiceClient> logger)
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

        public async Task<ExamDto?> GetExamByIdAsync(int examId)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/exams/{examId}"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ExamDto>();
                }

                _logger.LogWarning($"Failed to get exam {examId}: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling ExamService for exam {examId}");
                return null;
            }
        }

        public async Task<IEnumerable<ExamDto>?> GetAllExamsAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/exams"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ExamDto>>();
                }

                _logger.LogWarning($"Failed to get all exams: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ExamService for all exams");
                return null;
            }
        }
    }
}
