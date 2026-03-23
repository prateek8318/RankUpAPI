using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Infrastructure.Repositories
{
    public class StateDapperRepository : BaseDapperRepository, IStateRepository
    {
        public StateDapperRepository(string connectionString) : base(connectionString)
        {
        }


        public async Task<State?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[State_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<State>(sql, new { Id = id });
            });
        }

        public async Task<State?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[State_GetByIdLocalized] @Id, @LanguageCode";
                return await connection.QueryFirstOrDefaultAsync<State>(sql, new { Id = id, LanguageCode = languageCode });
            });
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[State_GetAll]";
                return await connection.QueryAsync<State>(sql);
            });
        }

        public async Task<IEnumerable<State>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[State_GetActive]";
                var states = await connection.QueryAsync<State>(sql);

                // Load StateLanguages for each state
                var stateList = states.ToList();
                foreach (var state in stateList)
                {
                    var languages = await connection.QueryAsync<StateLanguage>(
                        "SELECT sl.*, l.Code as LanguageCode, l.Name as LanguageName FROM StateLanguages sl " +
                        "LEFT JOIN Languages l ON sl.LanguageId = l.Id " +
                        "WHERE sl.StateId = @StateId AND sl.IsActive = 1",
                        new { StateId = state.Id });
                    
                    state.StateLanguages = languages.ToList();
                }

                return stateList;
            });
        }

        public async Task<IEnumerable<State>> GetActiveLocalizedAsync(string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[State_GetActiveLocalized] @LanguageCode";
                return await connection.QueryAsync<State>(sql, new { LanguageCode = languageCode });
            });
        }

        public async Task<IEnumerable<State>> GetActiveByCountryCodeAsync(string countryCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[State_GetActiveByCountryCode] @CountryCode";
                return await connection.QueryAsync<State>(sql, new { CountryCode = countryCode });
            });
        }

        public async Task<IEnumerable<State>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[State_GetActiveByCountryCodeLocalized] @CountryCode, @LanguageCode";
                return await connection.QueryAsync<State>(sql, new { CountryCode = countryCode, LanguageCode = languageCode });
            });
        }

        public async Task<State> AddAsync(State state)
        {
            return await WithConnectionAsync(async connection =>
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

                await connection.ExecuteAsync(
                    "[dbo].[State_Create]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    state.Id = parameters.Get<int>("@Id");
                }

                return state;
            });
        }

        public async Task UpdateAsync(State state)
        {
            await WithConnectionAsync(async connection =>
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

                await connection.ExecuteAsync(
                    "[dbo].[State_Update]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task DeleteAsync(State state)
        {
            await WithConnectionAsync(async connection =>
            {
                await connection.ExecuteAsync(
                    "[dbo].[State_Delete]",
                    new { Id = state.Id },
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var affected = await WithConnectionAsync(async connection =>
            {
                return await connection.ExecuteAsync(
                    "[dbo].[State_SoftDelete]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure);
            });

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var affected = await WithConnectionAsync(async connection =>
            {
                return await connection.ExecuteAsync(
                    "[dbo].[State_SetActive]",
                    new { Id = id, IsActive = isActive },
                    commandType: CommandType.StoredProcedure);
            });

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper commands are executed immediately; this exists for interface compatibility.
            return await Task.FromResult(1);
        }

        public async Task<IEnumerable<State>> GetStatesWithEmptyNamesAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<State>(
                    "[dbo].[State_GetWithEmptyNames]",
                    commandType: CommandType.StoredProcedure);
            });
        }
    }
}
