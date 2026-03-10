using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;
using System.Text.Json;

namespace MasterService.Infrastructure.Repositories
{
    public class QualificationDapperRepository : IQualificationRepository
    {
        private readonly MasterDbContext _context;

        public QualificationDapperRepository(MasterDbContext context)
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

        public async Task<Qualification?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            return await connection.QueryFirstOrDefaultAsync<Qualification>(
                "[dbo].[Qualification_GetById]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Qualification?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<Qualification>(
                "[dbo].[Qualification_GetByIdLocalized]",
                new { Id = id, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            return await connection.QueryAsync<Qualification>(
                "[dbo].[Qualification_GetAll]",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Qualification>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var qualifications = await connection.QueryAsync<Qualification>(
                "[dbo].[Qualification_GetActive]",
                commandType: CommandType.StoredProcedure);

            // Load QualificationLanguages for each qualification
            var qualificationList = qualifications.ToList();
            foreach (var qualification in qualificationList)
            {
                var languages = await connection.QueryAsync<QualificationLanguage>(
                    "SELECT ql.*, l.Code as LanguageCode, l.Name as LanguageName FROM QualificationLanguages ql " +
                    "LEFT JOIN Languages l ON ql.LanguageId = l.Id " +
                    "WHERE ql.QualificationId = @QualificationId AND ql.IsActive = 1",
                    new { QualificationId = qualification.Id });
                
                qualification.QualificationLanguages = languages.ToList();
            }

            return qualificationList;
        }

        public async Task<IEnumerable<Qualification>> GetActiveLocalizedAsync(string? languageCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<Qualification>(
                "[dbo].[Qualification_GetActiveLocalized]",
                new { LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Qualification>> GetActiveByCountryCodeAsync(string countryCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            return await connection.QueryAsync<Qualification>(
                "[dbo].[Qualification_GetActiveByCountryCode]",
                new { CountryCode = countryCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Qualification>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<Qualification>(
                "[dbo].[Qualification_GetActiveByCountryCodeLocalized]",
                new { CountryCode = countryCode, LanguageCode = languageCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Qualification> AddAsync(Qualification qualification)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var namesJson = qualification.QualificationLanguages != null && qualification.QualificationLanguages.Any()
                ? JsonSerializer.Serialize(qualification.QualificationLanguages.Select(x => new { x.LanguageId, x.Name, x.Description }))
                : null;

            var parameters = new DynamicParameters();
            parameters.Add("@Name", qualification.Name);
            parameters.Add("@Description", qualification.Description);
            parameters.Add("@CountryCode", qualification.CountryCode);
            parameters.Add("@IsActive", qualification.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "[dbo].[Qualification_Create]",
                parameters,
                commandType: CommandType.StoredProcedure);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                qualification.Id = parameters.Get<int>("@Id");
            }

            return qualification;
        }

        public async Task UpdateAsync(Qualification qualification)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var namesJson = qualification.QualificationLanguages != null && qualification.QualificationLanguages.Any()
                ? JsonSerializer.Serialize(qualification.QualificationLanguages.Select(x => new { x.LanguageId, x.Name, x.Description }))
                : null;

            var parameters = new DynamicParameters();
            parameters.Add("@Id", qualification.Id);
            parameters.Add("@Name", qualification.Name);
            parameters.Add("@Description", qualification.Description);
            parameters.Add("@CountryCode", qualification.CountryCode);
            parameters.Add("@IsActive", qualification.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await connection.ExecuteAsync(
                "[dbo].[Qualification_Update]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public Task DeleteAsync(Qualification qualification)
        {
            using var connection = GetConnection();
            connection.Open();
            
            connection.Execute(
                "[dbo].[Qualification_Delete]",
                new { Id = qualification.Id },
                commandType: CommandType.StoredProcedure);
            return Task.CompletedTask;
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var affected = await connection.ExecuteAsync(
                "[dbo].[Qualification_SoftDelete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var affected = await connection.ExecuteAsync(
                "[dbo].[Qualification_SetActive]",
                new { Id = id, IsActive = isActive },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper-based repo executes commands immediately; nothing to commit via EF here.
            return await Task.FromResult(0);
        }

        public async Task<bool> HasRelatedStreamsAsync(int qualificationId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "[dbo].[Qualification_HasRelatedStreams]",
                new { QualificationId = qualificationId },
                commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<bool> HardDeleteByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            var affected = await connection.ExecuteAsync(
                "[dbo].[Qualification_HardDelete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }
    }
}
