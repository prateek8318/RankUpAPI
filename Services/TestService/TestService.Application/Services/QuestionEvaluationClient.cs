using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestService.Application.Interfaces;

namespace TestService.Application.Services
{
    public class QuestionEvaluationClient : IQuestionEvaluationClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuestionEvaluationClient> _logger;

        public QuestionEvaluationClient(HttpClient httpClient, ILogger<QuestionEvaluationClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IReadOnlyDictionary<int, QuestionEvaluationMetadata>> GetMetadataByIdsAsync(IEnumerable<int> questionIds)
        {
            var distinctIds = questionIds.Distinct().ToList();
            var results = new Dictionary<int, QuestionEvaluationMetadata>();

            var tasks = distinctIds.Select(async questionId =>
            {
                try
                {
                    var metadata = await _httpClient.GetFromJsonAsync<QuestionEvaluationMetadata>($"/api/questions/{questionId}");
                    if (metadata != null)
                    {
                        lock (results)
                        {
                            results[questionId] = metadata;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to load evaluation metadata for question {QuestionId}", questionId);
                }
            });

            await Task.WhenAll(tasks);
            return results;
        }
    }

    public class QuestionEvaluationMetadata
    {
        public int Id { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? ExplanationEnglish { get; set; }
        public int Marks { get; set; }
    }
}
