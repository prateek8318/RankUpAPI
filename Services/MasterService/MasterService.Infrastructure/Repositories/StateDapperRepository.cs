using Dapper;
using System.Data;
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
                var state = await connection.QueryFirstOrDefaultAsync<State>(
                    "[dbo].[State_GetById]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure);

                if (state != null)
                {
                    await PopulateStateLanguagesAsync(connection, new[] { state });
                }

                return state;
            });
        }

        public async Task<State?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var state = await connection.QueryFirstOrDefaultAsync<State>(
                    "[dbo].[State_GetByIdLocalized]",
                    new { Id = id, LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure);

                if (state != null)
                {
                    await PopulateStateLanguagesAsync(connection, new[] { state });
                }

                return state;
            });
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var stateList = (await connection.QueryAsync<State>(
                    "[dbo].[State_GetAll]",
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStateLanguagesAsync(connection, stateList);
                return stateList;
            });
        }

        public async Task<IEnumerable<State>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var stateList = (await connection.QueryAsync<State>(
                    "[dbo].[State_GetActive]",
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStateLanguagesAsync(connection, stateList);
                return stateList;
            });
        }

        public async Task<IEnumerable<State>> GetActiveLocalizedAsync(string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var stateList = (await connection.QueryAsync<State>(
                    "[dbo].[State_GetActiveLocalized]",
                    new { LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStateLanguagesAsync(connection, stateList);
                return stateList;
            });
        }

        public async Task<IEnumerable<State>> GetActiveByCountryCodeAsync(string countryCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var stateList = (await connection.QueryAsync<State>(
                    "[dbo].[State_GetActiveByCountryCode]",
                    new { CountryCode = countryCode },
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStateLanguagesAsync(connection, stateList);
                return stateList;
            });
        }

        public async Task<IEnumerable<State>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var stateList = (await connection.QueryAsync<State>(
                    "[dbo].[State_GetActiveByCountryCodeLocalized]",
                    new { CountryCode = countryCode, LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStateLanguagesAsync(connection, stateList);
                return stateList;
            });
        }

        public async Task<State> AddAsync(State state, string? namesJson = null)
        {
            return await WithConnectionAsync(async connection =>
            {
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

        public async Task UpdateAsync(State state, string? namesJson = null)
        {
            await WithConnectionAsync(async connection =>
            {
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
            return await Task.FromResult(0);
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

        private static async Task PopulateStateLanguagesAsync(IDbConnection connection, IList<State> states)
        {
            if (states.Count == 0)
            {
                return;
            }

            var languages = await connection.QueryAsync<StateLanguage>(
                "[dbo].[StateLanguage_GetByStateIds]",
                new { StateIds = string.Join(",", states.Select(state => state.Id).Distinct()) },
                commandType: CommandType.StoredProcedure);

            var languageLookup = languages
                .GroupBy(language => language.StateId)
                .ToDictionary(group => group.Key, group => (ICollection<StateLanguage>)group.ToList());

            foreach (var state in states)
            {
                state.StateLanguages = languageLookup.TryGetValue(state.Id, out var items)
                    ? items
                    : new List<StateLanguage>();
            }
        }
    }
}
