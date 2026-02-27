using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class StateDapperRepository : IStateRepository
    {
        private readonly MasterDbContext _context;

        public StateDapperRepository(MasterDbContext context)
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

        public async Task<State?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[State_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<State>(sql, new { Id = id });
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[State_GetAll]";
            return await connection.QueryAsync<State>(sql);
        }

        public async Task<IEnumerable<State>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[State_GetActive]";
            return await connection.QueryAsync<State>(sql);
        }

        public async Task<IEnumerable<State>> GetActiveByCountryCodeAsync(string countryCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[State_GetActiveByCountryCode] @CountryCode";
            return await connection.QueryAsync<State>(sql, new { CountryCode = countryCode });
        }

        public async Task<State> AddAsync(State state)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[State_Create] 
                    @Name, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", state.Name);
            parameters.Add("@CountryCode", state.CountryCode);
            parameters.Add("@IsActive", state.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                state.Id = parameters.Get<int>("@Id");
            }

            return state;
        }

        public Task UpdateAsync(State state)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = @"
                EXEC [dbo].[State_Update] 
                    @Id, @Name, @CountryCode, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", state.Id);
            parameters.Add("@Name", state.Name);
            parameters.Add("@CountryCode", state.CountryCode);
            parameters.Add("@IsActive", state.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            connection.Execute(sql, parameters);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(State state)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = "EXEC [dbo].[State_Delete] @Id";
            connection.Execute(sql, new { Id = state.Id });
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<State>> GetStatesWithEmptyNamesAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[State_GetWithEmptyNames]";
            return await connection.QueryAsync<State>(sql);
        }
    }
}
