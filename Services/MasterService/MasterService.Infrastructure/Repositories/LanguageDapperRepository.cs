using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class LanguageDapperRepository : BaseDapperRepository, ILanguageRepository
    {
        public LanguageDapperRepository(string connectionString) : base(connectionString)
        {
        }


        public async Task<Language?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Language_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<Language>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Language_GetAll]";
                return await connection.QueryAsync<Language>(sql);
            });
        }

        public async Task<IEnumerable<Language>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Language_GetActive]";
                return await connection.QueryAsync<Language>(sql);
            });
        }

        public async Task<Language> AddAsync(Language language)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Language_Create] 
                        @Name, @Code, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@Name", language.Name);
                parameters.Add("@Code", language.Code);
                parameters.Add("@IsActive", language.IsActive);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    language.Id = parameters.Get<int>("@Id");
                }

                return language;
            });
        }

        public async Task UpdateAsync(Language language)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Language_Update] 
                        @Id, @Name, @Code, @IsActive, @UpdatedAt";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", language.Id);
                parameters.Add("@Name", language.Name);
                parameters.Add("@Code", language.Code);
                parameters.Add("@IsActive", language.IsActive);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync(sql, parameters);
            });
        }

        public Task DeleteAsync(Language language)
        {
            return WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Language_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = language.Id });
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper commands are executed immediately; this exists for interface compatibility.
            return await Task.FromResult(1);
        }
    }
}
