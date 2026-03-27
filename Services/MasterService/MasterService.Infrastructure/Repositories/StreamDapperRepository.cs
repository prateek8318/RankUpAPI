using Dapper;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;
using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Infrastructure.Repositories
{
    public class StreamDapperRepository : BaseDapperRepository, IStreamRepository
    {
        public StreamDapperRepository(string connectionString, ILogger<StreamDapperRepository> logger) : base(connectionString, logger)
        {
        }

        private const string StreamLanguageSql = @"
SELECT
    sl.Id,
    sl.StreamId,
    sl.LanguageId,
    sl.Name,
    sl.Description,
    sl.IsActive,
    sl.CreatedAt,
    sl.UpdatedAt,
    l.Id,
    l.Code,
    l.Name
FROM dbo.StreamLanguages sl
LEFT JOIN dbo.Languages l ON l.Id = sl.LanguageId
WHERE sl.IsActive = 1
  AND sl.StreamId IN @StreamIds;";

        private const string QualificationSql = @"
SELECT
    Id,
    Name,
    NameHi,
    Description,
    CountryCode,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM dbo.Qualifications
WHERE IsActive = 1
  AND Id IN @Ids;";


        public async Task<StreamEntity?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var stream = await connection.QueryFirstOrDefaultAsync<StreamEntity>(
                    "EXEC [dbo].[Stream_GetById] @Id", new { Id = id });
                
                if (stream != null)
                {
                    await PopulateStreamDetailsAsync(connection, new[] { stream });
                }
                
                return stream;
            });
        }

        public async Task<StreamEntity?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var stream = await connection.QueryFirstOrDefaultAsync<StreamEntity>(
                    "[dbo].[Stream_GetByIdLocalized]",
                    new { Id = id, LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure);

                if (stream != null)
                {
                    await PopulateStreamDetailsAsync(connection, new[] { stream });
                }

                return stream;
            });
        }

        public async Task<IEnumerable<StreamEntity>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var streams = (await connection.QueryAsync<StreamEntity>(
                    "[dbo].[Stream_GetAll]",
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStreamDetailsAsync(connection, streams);
                return streams;
            });
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var streams = (await connection.QueryAsync<StreamEntity>(
                    "[dbo].[Stream_GetActive]",
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStreamDetailsAsync(connection, streams);
                return streams;
            });
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveLocalizedAsync(string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var streams = (await connection.QueryAsync<StreamEntity>(
                    "[dbo].[Stream_GetActiveLocalized]",
                    new { LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStreamDetailsAsync(connection, streams);
                return streams;
            });
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdAsync(int qualificationId)
        {
            return await WithConnectionAsync(async connection =>
            {
                var streams = (await connection.QueryAsync<StreamEntity>(
                    "[dbo].[Stream_GetActiveByQualificationId]",
                    new { QualificationId = qualificationId },
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStreamDetailsAsync(connection, streams);
                return streams;
            });
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdLocalizedAsync(int qualificationId, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                var streams = (await connection.QueryAsync<StreamEntity>(
                    "[dbo].[Stream_GetActiveByQualificationIdLocalized]",
                    new { QualificationId = qualificationId, LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure)).ToList();

                await PopulateStreamDetailsAsync(connection, streams);
                return streams;
            });
        }

        public async Task<StreamEntity> AddAsync(StreamEntity stream, string? namesJson = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", stream.Name);
            parameters.Add("@Description", stream.Description);
            parameters.Add("@QualificationId", stream.QualificationId);
            parameters.Add("@IsActive", stream.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Stream_Create]",
                    parameters,
                    commandType: CommandType.StoredProcedure));
            
            if (parameters.Get<int>("@Id") > 0)
            {
                stream.Id = parameters.Get<int>("@Id");
            }

            return stream;
        }

        public async Task UpdateAsync(StreamEntity stream, string? namesJson = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", stream.Id);
            parameters.Add("@Name", stream.Name);
            parameters.Add("@Description", stream.Description);
            parameters.Add("@QualificationId", stream.QualificationId);
            parameters.Add("@IsActive", stream.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Stream_Update]",
                    parameters,
                    commandType: CommandType.StoredProcedure));
        }

        public async Task DeleteAsync(StreamEntity stream)
        {
            await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Stream_Delete]",
                    new { Id = stream.Id },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var affected = await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Stream_SoftDelete]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure));

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var affected = await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Stream_SetActive]",
                    new { Id = id, IsActive = isActive },
                    commandType: CommandType.StoredProcedure));

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogDebug("SaveChangesAsync invoked on {Repository}. Dapper calls are already committed.", nameof(StreamDapperRepository));
            return await Task.FromResult(1);
        }

        private async Task PopulateStreamDetailsAsync(IDbConnection connection, IList<StreamEntity> streams)
        {
            if (streams.Count == 0)
            {
                return;
            }

            var streamIds = streams.Select(stream => stream.Id).Distinct().ToArray();
            var qualificationIds = streams
                .Select(stream => stream.QualificationId)
                .Where(id => id > 0)
                .Distinct()
                .ToArray();

            var languages = streamIds.Length == 0
                ? Array.Empty<StreamLanguage>()
                : (await connection.QueryAsync<StreamLanguage>(StreamLanguageSql, new { StreamIds = streamIds })).ToArray();

            var qualifications = qualificationIds.Length == 0
                ? Array.Empty<Qualification>()
                : (await connection.QueryAsync<Qualification>(QualificationSql, new { Ids = qualificationIds })).ToArray();

            RepositoryEntityMapper.AttachStreamDetails(streams, languages, qualifications);
        }
    }
}
