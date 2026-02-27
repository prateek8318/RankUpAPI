using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuizService.Application.Interfaces;
using QuizService.Domain.Entities;
using QuizService.Infrastructure.Data;
using System.Data;

namespace QuizService.Infrastructure.Repositories
{
    public class TestSeriesDapperRepository : ITestSeriesRepository
    {
        private readonly QuizDbContext _context;

        public TestSeriesDapperRepository(QuizDbContext context)
        {
            _context = context;
        }

        private SqlConnection GetConnection()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection is SqlConnection sqlConnection)
                return sqlConnection;
            throw new InvalidOperationException("Database connection is not a SqlConnection");
        }

        public async Task<TestSeries?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<TestSeries>(sql, new { Id = id });
        }

        public async Task<IEnumerable<TestSeries>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetAll]";
            return await connection.QueryAsync<TestSeries>(sql);
        }

        public async Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_GetByExamId] @ExamId";
            return await connection.QueryAsync<TestSeries>(sql, new { ExamId = examId });
        }

        public async Task<TestSeries> AddAsync(TestSeries testSeries)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[TestSeries_Create] 
                    @Title, @Description, @ExamId, @Duration, @TotalQuestions,
                    @TotalMarks, @PassingMarks, @Price, @DiscountPrice, @ImageUrl,
                    @ThumbnailUrl, @Difficulty, @Tags, @DisplayOrder, @IsActive,
                    @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Title", testSeries.Title);
            parameters.Add("@Description", testSeries.Description);
            parameters.Add("@ExamId", testSeries.ExamId);
            parameters.Add("@Duration", testSeries.Duration);
            parameters.Add("@TotalQuestions", testSeries.TotalQuestions);
            parameters.Add("@TotalMarks", testSeries.TotalMarks);
            parameters.Add("@PassingMarks", testSeries.PassingMarks);
            parameters.Add("@Price", testSeries.Price);
            parameters.Add("@DiscountPrice", testSeries.DiscountPrice);
            parameters.Add("@ImageUrl", testSeries.ImageUrl);
            parameters.Add("@ThumbnailUrl", testSeries.ThumbnailUrl);
            parameters.Add("@Difficulty", testSeries.Difficulty);
            parameters.Add("@Tags", testSeries.Tags);
            parameters.Add("@DisplayOrder", testSeries.DisplayOrder);
            parameters.Add("@IsActive", testSeries.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                testSeries.Id = parameters.Get<int>("@Id");
            }

            return testSeries;
        }

        public async Task UpdateAsync(TestSeries testSeries)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[TestSeries_Update] 
                    @Id, @Title, @Description, @ExamId, @Duration, @TotalQuestions,
                    @TotalMarks, @PassingMarks, @Price, @DiscountPrice, @ImageUrl,
                    @ThumbnailUrl, @Difficulty, @Tags, @DisplayOrder, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", testSeries.Id);
            parameters.Add("@Title", testSeries.Title);
            parameters.Add("@Description", testSeries.Description);
            parameters.Add("@ExamId", testSeries.ExamId);
            parameters.Add("@Duration", testSeries.Duration);
            parameters.Add("@TotalQuestions", testSeries.TotalQuestions);
            parameters.Add("@TotalMarks", testSeries.TotalMarks);
            parameters.Add("@PassingMarks", testSeries.PassingMarks);
            parameters.Add("@Price", testSeries.Price);
            parameters.Add("@DiscountPrice", testSeries.DiscountPrice);
            parameters.Add("@ImageUrl", testSeries.ImageUrl);
            parameters.Add("@ThumbnailUrl", testSeries.ThumbnailUrl);
            parameters.Add("@Difficulty", testSeries.Difficulty);
            parameters.Add("@Tags", testSeries.Tags);
            parameters.Add("@DisplayOrder", testSeries.DisplayOrder);
            parameters.Add("@IsActive", testSeries.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await connection.ExecuteAsync(sql, parameters);
        }

        public async Task DeleteAsync(TestSeries testSeries)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[TestSeries_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = testSeries.Id });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
