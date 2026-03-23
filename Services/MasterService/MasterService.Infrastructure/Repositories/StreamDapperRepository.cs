using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using System.Data;
using StreamEntity = MasterService.Domain.Entities.Stream;
using System.Text.Json;

namespace MasterService.Infrastructure.Repositories
{
    public class StreamDapperRepository : BaseDapperRepository, IStreamRepository
    {
        public StreamDapperRepository(string connectionString) : base(connectionString)
        {
        }


        public async Task<StreamEntity?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var stream = await connection.QueryFirstOrDefaultAsync<StreamEntity>(
                    "EXEC [dbo].[Stream_GetById] @Id", new { Id = id });
                
                if (stream != null)
                {
                    // Load StreamLanguages
                    var languages = await connection.QueryAsync<StreamLanguage>(
                        "SELECT * FROM StreamLanguages WHERE StreamId = @StreamId AND IsActive = 1",
                        new { StreamId = stream.Id });
                    stream.StreamLanguages = languages.ToList();

                    // Load Qualification
                    if (stream.QualificationId > 0)
                    {
                        stream.Qualification = await connection.QueryFirstOrDefaultAsync<Qualification>(
                            "SELECT Id, Name, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive FROM Qualifications WHERE Id = @Id AND IsActive = 1",
                            new { Id = stream.QualificationId });
                    }
                }
                
                return stream;
            });
        }

        public async Task<StreamEntity?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            var stream = await _connection.QueryFirstOrDefaultAsync<StreamEntity>(
                "[dbo].[Stream_GetByIdLocalized]",
                new { Id = id, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);

            if (stream != null)
            {
                // Load StreamLanguages
                var languages = await _connection.QueryAsync<StreamLanguage>(
                    "SELECT * FROM StreamLanguages WHERE StreamId = @StreamId AND IsActive = 1",
                    new { StreamId = stream.Id });
                stream.StreamLanguages = languages.ToList();

                // Load Qualification
                if (stream.QualificationId > 0)
                {
                    stream.Qualification = await _connection.QueryFirstOrDefaultAsync<Qualification>(
                        "SELECT Id, Name, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive FROM Qualifications WHERE Id = @Id AND IsActive = 1",
                        new { Id = stream.QualificationId });
                }
            }

            return stream;
        }

        public async Task<IEnumerable<StreamEntity>> GetAllAsync()
        {
            return await _connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetAll]",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveAsync()
        {
            var streams = await _connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActive]",
                commandType: CommandType.StoredProcedure);

            // Load StreamLanguages and Qualification for each stream
            var streamList = streams.ToList();
            var qualificationIds = streamList.Select(s => s.QualificationId).Distinct().ToList();
            
            // Load qualifications in bulk
            var qualifications = qualificationIds.Any() 
                ? (await _connection.QueryAsync<Qualification>(
                    "SELECT Id, Name, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive FROM Qualifications WHERE Id IN @Ids AND IsActive = 1",
                    new { Ids = qualificationIds })).ToDictionary(q => q.Id)
                : new Dictionary<int, Qualification>();

            foreach (var stream in streamList)
            {
                // Load StreamLanguages
                var languages = await _connection.QueryAsync<StreamLanguage>(
                    "SELECT * FROM StreamLanguages WHERE StreamId = @StreamId AND IsActive = 1",
                    new { StreamId = stream.Id });
                stream.StreamLanguages = languages.ToList();

                // Set Qualification
                if (qualifications.TryGetValue(stream.QualificationId, out var qualification))
                {
                    stream.Qualification = qualification;
                }
            }

            return streamList;
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveLocalizedAsync(string? languageCode)
        {
            var streams = await _connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActiveLocalized]",
                new { LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);

            // Load StreamLanguages and Qualification for each stream
            var streamList = streams.ToList();
            var qualificationIds = streamList.Select(s => s.QualificationId).Distinct().ToList();
            
            // Load qualifications in bulk
            var qualifications = qualificationIds.Any() 
                ? (await _connection.QueryAsync<Qualification>(
                    "SELECT Id, Name, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive FROM Qualifications WHERE Id IN @Ids AND IsActive = 1",
                    new { Ids = qualificationIds })).ToDictionary(q => q.Id)
                : new Dictionary<int, Qualification>();

            foreach (var stream in streamList)
            {
                // Load StreamLanguages
                var languages = await _connection.QueryAsync<StreamLanguage>(
                    "SELECT * FROM StreamLanguages WHERE StreamId = @StreamId AND IsActive = 1",
                    new { StreamId = stream.Id });
                stream.StreamLanguages = languages.ToList();

                // Set Qualification
                if (qualifications.TryGetValue(stream.QualificationId, out var qualification))
                {
                    stream.Qualification = qualification;
                }
            }

            return streamList;
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdAsync(int qualificationId)
        {
            var streams = await _connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActiveByQualificationId]",
                new { QualificationId = qualificationId },
                commandType: CommandType.StoredProcedure);

            // Load StreamLanguages and Qualification for each stream
            var streamList = streams.ToList();
            
            // Load the specific qualification
            var qualification = await _connection.QueryFirstOrDefaultAsync<Qualification>(
                "SELECT Id, Name, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive FROM Qualifications WHERE Id = @Id AND IsActive = 1",
                new { Id = qualificationId });

            foreach (var stream in streamList)
            {
                // Load StreamLanguages
                var languages = await _connection.QueryAsync<StreamLanguage>(
                    "SELECT * FROM StreamLanguages WHERE StreamId = @StreamId AND IsActive = 1",
                    new { StreamId = stream.Id });
                stream.StreamLanguages = languages.ToList();

                // Set Qualification
                stream.Qualification = qualification;
            }

            return streamList;
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdLocalizedAsync(int qualificationId, string? languageCode)
        {
            var streams = await _connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActiveByQualificationIdLocalized]",
                new { QualificationId = qualificationId, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);

            // Load StreamLanguages and Qualification for each stream
            var streamList = streams.ToList();
            
            // Load the specific qualification
            var qualification = await _connection.QueryFirstOrDefaultAsync<Qualification>(
                "SELECT Id, Name, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive FROM Qualifications WHERE Id = @Id AND IsActive = 1",
                new { Id = qualificationId });

            foreach (var stream in streamList)
            {
                // Load StreamLanguages
                var languages = await _connection.QueryAsync<StreamLanguage>(
                    "SELECT * FROM StreamLanguages WHERE StreamId = @StreamId AND IsActive = 1",
                    new { StreamId = stream.Id });
                stream.StreamLanguages = languages.ToList();

                // Set Qualification
                stream.Qualification = qualification;
            }

            return streamList;
        }

        public async Task<StreamEntity> AddAsync(StreamEntity stream)
        {
            var namesJson = stream.StreamLanguages != null && stream.StreamLanguages.Any()
                ? JsonSerializer.Serialize(stream.StreamLanguages.Select(x => new { x.LanguageId, x.Name, x.Description }))
                : null;

            var parameters = new DynamicParameters();
            parameters.Add("@Name", stream.Name);
            parameters.Add("@Description", stream.Description);
            parameters.Add("@QualificationId", stream.QualificationId);
            parameters.Add("@IsActive", stream.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(
                "[dbo].[Stream_Create]",
                parameters,
                commandType: CommandType.StoredProcedure);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                stream.Id = parameters.Get<int>("@Id");
            }

            return stream;
        }

        public async Task UpdateAsync(StreamEntity stream)
        {
            var namesJson = stream.StreamLanguages != null && stream.StreamLanguages.Any()
                ? JsonSerializer.Serialize(stream.StreamLanguages.Select(x => new { x.LanguageId, x.Name, x.Description }))
                : null;

            var parameters = new DynamicParameters();
            parameters.Add("@Id", stream.Id);
            parameters.Add("@Name", stream.Name);
            parameters.Add("@Description", stream.Description);
            parameters.Add("@QualificationId", stream.QualificationId);
            parameters.Add("@IsActive", stream.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await _connection.ExecuteAsync(
                "[dbo].[Stream_Update]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(StreamEntity stream)
        {
            await _connection.ExecuteAsync(
                "[dbo].[Stream_Delete]",
                new { Id = stream.Id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var affected = await _connection.ExecuteAsync(
                "[dbo].[Stream_SoftDelete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var affected = await _connection.ExecuteAsync(
                "[dbo].[Stream_SetActive]",
                new { Id = id, IsActive = isActive },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper commands are executed immediately; this exists for interface compatibility.
            return await Task.FromResult(1);
        }
    }
}
