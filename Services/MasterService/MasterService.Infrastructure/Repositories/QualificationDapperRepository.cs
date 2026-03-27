using Dapper;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class QualificationDapperRepository : BaseDapperRepository, IQualificationRepository
    {
        public QualificationDapperRepository(string connectionString, ILogger<QualificationDapperRepository> logger) : base(connectionString, logger)
        {
        }

        private const string QualificationLanguageSql = @"
SELECT
    ql.Id,
    ql.QualificationId,
    ql.LanguageId,
    ql.Name,
    ql.Description,
    ql.IsActive,
    ql.CreatedAt,
    ql.UpdatedAt,
    l.Id,
    l.Code,
    l.Name
FROM dbo.QualificationLanguages ql
LEFT JOIN dbo.Languages l ON l.Id = ql.LanguageId
WHERE ql.IsActive = 1
  AND ql.QualificationId IN @QualificationIds;";


        public async Task<Qualification?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var qualification = await connection.QueryFirstOrDefaultAsync<Qualification>(
                    "[dbo].[Qualification_GetById]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure);

                if (qualification != null)
                {
                    var languages = await GetQualificationLanguagesAsync(connection, new[] { qualification.Id });
                    RepositoryEntityMapper.AttachQualificationLanguages(new[] { qualification }, languages);
                }

                return qualification;
            });
        }

        public async Task<Qualification?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
                await connection.QueryFirstOrDefaultAsync<Qualification>(
                    "[dbo].[Qualification_GetByIdLocalized]",
                    new { Id = id, LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var qualifications = (await connection.QueryAsync<Qualification>(
                    "[dbo].[Qualification_GetAll]",
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateQualificationLanguagesAsync(connection, qualifications);
                return qualifications;
            });
        }

        public async Task<IEnumerable<Qualification>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var qualifications = (await connection.QueryAsync<Qualification>(
                    "[dbo].[Qualification_GetActive]",
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateQualificationLanguagesAsync(connection, qualifications);
                return qualifications;
            });
        }

        public async Task<IEnumerable<Qualification>> GetActiveLocalizedAsync(string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
                await connection.QueryAsync<Qualification>(
                    "[dbo].[Qualification_GetActiveLocalized]",
                    new { LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<IEnumerable<Qualification>> GetActiveByCountryCodeAsync(string countryCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var qualifications = (await connection.QueryAsync<Qualification>(
                    "[dbo].[Qualification_GetActiveByCountryCode]",
                    new { CountryCode = countryCode },
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateQualificationLanguagesAsync(connection, qualifications);
                return qualifications;
            });
        }

        public async Task<IEnumerable<Qualification>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
                await connection.QueryAsync<Qualification>(
                    "[dbo].[Qualification_GetActiveByCountryCodeLocalized]",
                    new { CountryCode = countryCode, LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<Qualification> AddAsync(Qualification qualification, string? namesJson = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", qualification.Name);
            parameters.Add("@NameHi", qualification.NameHi);
            parameters.Add("@Description", qualification.Description);
            parameters.Add("@CountryCode", qualification.CountryCode);
            parameters.Add("@IsActive", qualification.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await WithConnectionAsync(async connection =>
            {
                await connection.ExecuteAsync(
                    "[dbo].[Qualification_Create]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            });
            
            if (parameters.Get<int>("@Id") > 0)
            {
                qualification.Id = parameters.Get<int>("@Id");
            }

            return qualification;
        }

        public async Task UpdateAsync(Qualification qualification, string? namesJson = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", qualification.Id);
            parameters.Add("@Name", qualification.Name);
            parameters.Add("@NameHi", qualification.NameHi);
            parameters.Add("@Description", qualification.Description);
            parameters.Add("@CountryCode", qualification.CountryCode);
            parameters.Add("@IsActive", qualification.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Qualification_Update]",
                    parameters,
                    commandType: CommandType.StoredProcedure));
        }

        public Task DeleteAsync(Qualification qualification)
        {
            return WithConnectionAsync(async connection =>
            {
                await connection.ExecuteAsync(
                    "[dbo].[Qualification_Delete]",
                    new { Id = qualification.Id },
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var affected = await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Qualification_SoftDelete]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure));

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var affected = await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Qualification_SetActive]",
                    new { Id = id, IsActive = isActive },
                    commandType: CommandType.StoredProcedure));

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogDebug("SaveChangesAsync invoked on {Repository}. Dapper calls are already committed.", nameof(QualificationDapperRepository));
            return await Task.FromResult(1);
        }

        public async Task<bool> HasRelatedStreamsAsync(int qualificationId)
        {
            var result = await WithConnectionAsync(async connection =>
                await connection.QueryFirstOrDefaultAsync<int>(
                    "[dbo].[Qualification_HasRelatedStreams]",
                    new { QualificationId = qualificationId },
                    commandType: CommandType.StoredProcedure));
            return result > 0;
        }

        public async Task<bool> HardDeleteByIdAsync(int id)
        {
            var affected = await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Qualification_HardDelete]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure));

            return affected > 0;
        }

        private async Task PopulateQualificationLanguagesAsync(IDbConnection connection, IList<Qualification> qualifications)
        {
            if (qualifications.Count == 0)
            {
                return;
            }

            var languages = await GetQualificationLanguagesAsync(connection, qualifications.Select(qualification => qualification.Id));
            RepositoryEntityMapper.AttachQualificationLanguages(qualifications, languages);
        }

        private async Task<IReadOnlyCollection<QualificationLanguage>> GetQualificationLanguagesAsync(IDbConnection connection, IEnumerable<int> qualificationIds)
        {
            var ids = qualificationIds.Distinct().ToArray();
            if (ids.Length == 0)
            {
                return Array.Empty<QualificationLanguage>();
            }

            var languages = await connection.QueryAsync<QualificationLanguage>(QualificationLanguageSql, new { QualificationIds = ids });
            return languages.ToList();
        }
    }
}
