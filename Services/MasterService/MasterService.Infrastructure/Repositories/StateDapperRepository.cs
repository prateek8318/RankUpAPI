using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class StateDapperRepository : IStateRepository
    {
        private readonly MasterDbContext _context;
        private readonly IDbConnection _connection;

        public StateDapperRepository(MasterDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }

        private IDbConnection GetConnection()
        {
            return _connection;
        }


        public async Task<State?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<State>(
                "[dbo].[State_GetById]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<State?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            return await _connection.QueryFirstOrDefaultAsync<State>(
                "[dbo].[State_GetByIdLocalized]",
                new { Id = id, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _connection.QueryAsync<State>(
                "[dbo].[State_GetAll]",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<State>> GetActiveAsync()
        {
            var states = await _connection.QueryAsync<State>(
                "[dbo].[State_GetActive]",
                commandType: CommandType.StoredProcedure);

            // Load StateLanguages for each state
            var stateList = states.ToList();
            foreach (var state in stateList)
            {
                var languages = await _connection.QueryAsync<StateLanguage>(
                    "SELECT sl.*, l.Code as LanguageCode, l.Name as LanguageName FROM StateLanguages sl " +
                    "LEFT JOIN Languages l ON sl.LanguageId = l.Id " +
                    "WHERE sl.StateId = @StateId AND sl.IsActive = 1",
                    new { StateId = state.Id });
                
                state.StateLanguages = languages.ToList();
            }

            return stateList;
        }

        public async Task<IEnumerable<State>> GetActiveLocalizedAsync(string? languageCode)
        {
            using var connection = GetConnection();
            await ((DbConnection)connection).OpenAsync();

            return await connection.QueryAsync<State>(
                "[dbo].[State_GetActiveLocalized]",
                new { LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<State>> GetActiveByCountryCodeAsync(string countryCode)
        {
            return await _connection.QueryAsync<State>(
                "[dbo].[State_GetActiveByCountryCode]",
                new { CountryCode = countryCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<State>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode)
        {
            return await _connection.QueryAsync<State>(
                "[dbo].[State_GetActiveByCountryCodeLocalized]",
                new { CountryCode = countryCode, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<State> AddAsync(State state)
        {
            var namesJson = state.StateLanguages != null && state.StateLanguages.Any()
                ? JsonSerializer.Serialize(state.StateLanguages.Select(x => new { x.LanguageId, x.Name }))
                : null;

            var parameters = new DynamicParameters();
            parameters.Add("@Name", state.Name);
            parameters.Add("@Code", state.Code);
            parameters.Add("@CountryCode", state.CountryCode);
            parameters.Add("@IsActive", state.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(
                "[dbo].[State_Create]",
                parameters,
                commandType: CommandType.StoredProcedure);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                state.Id = parameters.Get<int>("@Id");
            }

            return state;
        }

        public async Task UpdateAsync(State state)
        {
            var namesJson = state.StateLanguages != null && state.StateLanguages.Any()
                ? JsonSerializer.Serialize(state.StateLanguages.Select(x => new { x.LanguageId, x.Name }))
                : null;

            var parameters = new DynamicParameters();
            parameters.Add("@Id", state.Id);
            parameters.Add("@Name", state.Name);
            parameters.Add("@Code", state.Code);
            parameters.Add("@CountryCode", state.CountryCode);
            parameters.Add("@IsActive", state.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await _connection.ExecuteAsync(
                "[dbo].[State_Update]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public Task DeleteAsync(State state)
        {
            _connection.Execute(
                "[dbo].[State_Delete]",
                new { Id = state.Id },
                commandType: CommandType.StoredProcedure);
            return Task.CompletedTask;
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var affected = await _connection.ExecuteAsync(
                "[dbo].[State_SoftDelete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var affected = await _connection.ExecuteAsync(
                "[dbo].[State_SetActive]",
                new { Id = id, IsActive = isActive },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper-based repo executes commands immediately; nothing to commit via EF here.
            return await Task.FromResult(0);
        }

        public async Task<IEnumerable<State>> GetStatesWithEmptyNamesAsync()
        {
            using var connection = GetConnection();
            await ((DbConnection)connection).OpenAsync();
            
            return await connection.QueryAsync<State>(
                "[dbo].[State_GetWithEmptyNames]",
                commandType: CommandType.StoredProcedure);
        }
    }
}
