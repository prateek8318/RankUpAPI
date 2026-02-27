using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

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
            var connection = _context.Database.GetDbConnection();
            if (connection is SqlConnection sqlConnection)
                return sqlConnection;
            throw new InvalidOperationException("Database connection is not a SqlConnection");
        }

        public async Task<Qualification?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Qualification_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<Qualification>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Qualification_GetAll]";
            return await connection.QueryAsync<Qualification>(sql);
        }

        public async Task<IEnumerable<Qualification>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Qualification_GetActive]";
            return await connection.QueryAsync<Qualification>(sql);
        }

        public async Task<IEnumerable<Qualification>> GetActiveByCountryCodeAsync(string countryCode)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Qualification_GetActiveByCountryCode] @CountryCode";
            return await connection.QueryAsync<Qualification>(sql, new { CountryCode = countryCode });
        }

        public async Task<Qualification> AddAsync(Qualification qualification)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Qualification_Create] 
                    @Name, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", qualification.Name);
            parameters.Add("@CountryCode", qualification.CountryCode);
            parameters.Add("@IsActive", qualification.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                qualification.Id = parameters.Get<int>("@Id");
            }

            return qualification;
        }

        public Task UpdateAsync(Qualification qualification)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = @"
                EXEC [dbo].[Qualification_Update] 
                    @Id, @Name, @CountryCode, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", qualification.Id);
            parameters.Add("@Name", qualification.Name);
            parameters.Add("@CountryCode", qualification.CountryCode);
            parameters.Add("@IsActive", qualification.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            connection.Execute(sql, parameters);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Qualification qualification)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = "EXEC [dbo].[Qualification_Delete] @Id";
            connection.Execute(sql, new { Id = qualification.Id });
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> HasRelatedStreamsAsync(int qualificationId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Qualification_HasRelatedStreams] @QualificationId";
            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { QualificationId = qualificationId });
            return result > 0;
        }
    }
}
