using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AdminService.Application.Clients
{
    public class QuestionServiceClient : IQuestionServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuestionServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public QuestionServiceClient(HttpClient httpClient, ILogger<QuestionServiceClient> logger)
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

        public async Task<object?> GetQuestionsByQuizIdAsync(int quizId)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/questions/quiz/{quizId}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuestionService for quiz {quizId}");
                return null;
            }
        }

        public async Task<object?> GetQuestionByIdAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/questions/{id}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuestionService for question {id}");
                return null;
            }
        }

        public async Task<object?> CreateQuestionAsync(object createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/admin/questions", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling QuestionService to create question");
                return null;
            }
        }

        public async Task<object?> UpdateQuestionAsync(int id, object updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PutAsync($"/api/admin/questions/{id}", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuestionService to update question {id}");
                return null;
            }
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.DeleteAsync($"/api/admin/questions/{id}"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuestionService to delete question {id}");
                return false;
            }
        }

        public async Task<object?> BulkUploadQuestionsAsync(int quizId, Stream fileStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                content.Add(streamContent, "file", fileName);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync($"/api/admin/questions/bulk-upload/{quizId}", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling QuestionService for bulk upload to quiz {quizId}");
                return null;
            }
        }
    }
}
