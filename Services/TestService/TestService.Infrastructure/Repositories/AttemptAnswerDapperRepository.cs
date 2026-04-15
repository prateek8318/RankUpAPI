using Dapper;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class AttemptAnswerDapperRepository : BaseDapperRepository, IAttemptAnswerRepository
    {
        public AttemptAnswerDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task SaveAsync(int attemptId, int questionId, string? answer, bool isMarkedForReview, bool isAnswered)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[AttemptAnswer_Save] @AttemptId, @QuestionId, @Answer, @IsMarkedForReview, @IsAnswered";
                await connection.ExecuteAsync(sql, new
                {
                    AttemptId = attemptId,
                    QuestionId = questionId,
                    Answer = answer,
                    IsMarkedForReview = isMarkedForReview,
                    IsAnswered = isAnswered
                });
            });
        }

        public async Task<IReadOnlyList<AttemptAnswer>> GetByAttemptIdAsync(int attemptId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[AttemptAnswer_GetByAttemptId] @AttemptId";
                var rows = await connection.QueryAsync<AttemptAnswer>(sql, new { AttemptId = attemptId });
                return rows.ToList();
            });
        }
    }
}

