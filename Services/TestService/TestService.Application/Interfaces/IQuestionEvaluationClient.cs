using QuestionEvaluationMetadata = TestService.Application.Services.QuestionEvaluationMetadata;

namespace TestService.Application.Interfaces
{
    public interface IQuestionEvaluationClient
    {
        Task<IReadOnlyDictionary<int, QuestionEvaluationMetadata>> GetMetadataByIdsAsync(IEnumerable<int> questionIds);
    }
}
