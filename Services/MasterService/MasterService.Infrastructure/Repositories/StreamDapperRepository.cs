using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;
using StreamEntity = MasterService.Domain.Entities.Stream;
using System.Text.Json;

namespace MasterService.Infrastructure.Repositories
{
    public class StreamDapperRepository : IStreamRepository
    {
        private readonly MasterDbContext _context;

        public StreamDapperRepository(MasterDbContext context)
        {
            _context = context;
        }

        private SqlConnection GetConnection()
        {
            var connectionString = _context.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string is not initialized in MasterDbContext");
                
            return new SqlConnection(connectionString);
        }

        public async Task<StreamEntity?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            return await connection.QueryFirstOrDefaultAsync<StreamEntity>(
                "[dbo].[Stream_GetById]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<StreamEntity?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<StreamEntity>(
                "[dbo].[Stream_GetByIdLocalized]",
                new { Id = id, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StreamEntity>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            return await connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetAll]",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var streams = await connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActive]",
                commandType: CommandType.StoredProcedure);

            // Load StreamLanguages for each stream
            var streamList = streams.ToList();
            foreach (var stream in streamList)
            {
                var languages = await connection.QueryAsync<StreamLanguage>(
                    "SELECT * FROM StreamLanguages WHERE StreamId = @StreamId AND IsActive = 1",
                    new { StreamId = stream.Id });
                
                stream.StreamLanguages = languages.ToList();
            }

            return streamList;
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveLocalizedAsync(string? languageCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActiveLocalized]",
                new { LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdAsync(int qualificationId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            return await connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActiveByQualificationId]",
                new { QualificationId = qualificationId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdLocalizedAsync(int qualificationId, string? languageCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<StreamEntity>(
                "[dbo].[Stream_GetActiveByQualificationIdLocalized]",
                new { QualificationId = qualificationId, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<StreamEntity> AddAsync(StreamEntity stream)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
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

            await connection.ExecuteAsync(
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
            using var connection = GetConnection();
            await connection.OpenAsync();
            
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

            await connection.ExecuteAsync(
                "[dbo].[Stream_Update]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public Task DeleteAsync(StreamEntity stream)
        {
            using var connection = GetConnection();
            connection.Open();
            
            connection.Execute(
                "[dbo].[Stream_Delete]",
                new { Id = stream.Id },
                commandType: CommandType.StoredProcedure);
            return Task.CompletedTask;
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var affected = await connection.ExecuteAsync(
                "[dbo].[Stream_SoftDelete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var affected = await connection.ExecuteAsync(
                "[dbo].[Stream_SetActive]",
                new { Id = id, IsActive = isActive },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper-based repo executes commands immediately; nothing to commit via EF here.
            return await Task.FromResult(0);
        }
    }
}
