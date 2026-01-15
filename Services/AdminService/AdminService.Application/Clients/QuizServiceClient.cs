using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AdminService.Application.Clients
{
    public class QuizServiceClient : IQuizServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuizServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public QuizServiceClient(HttpClient httpClient, ILogger<QuizServiceClient> logger)
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

        public async Task<object?> GetAllQuizzesAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/quizzes"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling QuizService for all quizzes");
                return null;
            }
        }

        public async Task<object?> GetQuizByIdAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/quizzes/{id}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuizService for quiz {id}");
                return null;
            }
        }

        public async Task<object?> CreateQuizAsync(object createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/admin/quizzes", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling QuizService to create quiz");
                return null;
            }
        }

        public async Task<object?> UpdateQuizAsync(int id, object updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PutAsync($"/api/admin/quizzes/{id}", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuizService to update quiz {id}");
                return null;
            }
        }

        public async Task<bool> DeleteQuizAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.DeleteAsync($"/api/admin/quizzes/{id}"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuizService to delete quiz {id}");
                return false;
            }
        }
    }
}
