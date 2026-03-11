using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class PracticeModeDapperRepository : BaseDapperRepository, IPracticeModeRepository
    {
        public PracticeModeDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<PracticeMode?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[PracticeMode_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<PracticeMode>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<PracticeMode>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[PracticeMode_GetAll]";
                return await connection.QueryAsync<PracticeMode>(sql);
            });
        }

        public async Task<IEnumerable<PracticeMode>> GetActiveModesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[PracticeMode_GetActive]";
                return await connection.QueryAsync<PracticeMode>(sql);
            });
        }

        public async Task<IEnumerable<PracticeMode>> FindAsync(System.Linq.Expressions.Expression<Func<PracticeMode, bool>> predicate)
        {
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public async Task AddAsync(PracticeMode entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[PracticeMode_Create] 
                        @Name, @Description, @IsActive, @DisplayOrder, @CreatedAt, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task UpdateAsync(PracticeMode entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[PracticeMode_Update] 
                        @Id, @Name, @Description, @IsActive, @DisplayOrder, @UpdatedAt";

                await connection.ExecuteAsync(sql, entity);
            });
        }

        public async Task DeleteAsync(PracticeMode entity)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[PracticeMode_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = entity.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }
    }
}
