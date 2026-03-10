using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class LanguageDapperRepository : ILanguageRepository
    {
        private readonly MasterDbContext _context;
        private readonly IDbConnection _connection;

        public LanguageDapperRepository(MasterDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }


        public async Task<Language?> GetByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[Language_GetById] @Id";
            return await _connection.QueryFirstOrDefaultAsync<Language>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            var sql = "EXEC [dbo].[Language_GetAll]";
            return await _connection.QueryAsync<Language>(sql);
        }

        public async Task<IEnumerable<Language>> GetActiveAsync()
        {
            var sql = "EXEC [dbo].[Language_GetActive]";
            return await _connection.QueryAsync<Language>(sql);
        }

        public async Task<Language> AddAsync(Language language)
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

            await _connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                language.Id = parameters.Get<int>("@Id");
            }

            return language;
        }

        public Task UpdateAsync(Language language)
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

            _connection.Execute(sql, parameters);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Language language)
        {
            var sql = "EXEC [dbo].[Language_Delete] @Id";
            _connection.Execute(sql, new { Id = language.Id });
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
