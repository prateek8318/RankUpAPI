using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class CountryDapperRepository : ICountryRepository
    {
        private readonly MasterDbContext _context;
        private readonly IDbConnection _connection;

        public CountryDapperRepository(MasterDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }


        public async Task<Country?> GetByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[Country_GetById] @Id";
            return await _connection.QueryFirstOrDefaultAsync<Country>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            var sql = "EXEC [dbo].[Country_GetAll]";
            return await _connection.QueryAsync<Country>(sql);
        }

        public async Task<Country?> GetByCodeAsync(string code)
        {
            var sql = "EXEC [dbo].[Country_GetByCode] @Code";
            return await _connection.QueryFirstOrDefaultAsync<Country>(sql, new { Code = code });
        }

        public async Task<Country> AddAsync(Country country)
        {
            var sql = @"
                EXEC [dbo].[Country_Create] 
                    @Name, @Code, @SubdivisionLabelEn, @SubdivisionLabelHi, 
                    @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", country.Name);
            parameters.Add("@Code", country.Code);
            parameters.Add("@SubdivisionLabelEn", country.SubdivisionLabelEn);
            parameters.Add("@SubdivisionLabelHi", country.SubdivisionLabelHi);
            parameters.Add("@IsActive", country.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                country.Id = parameters.Get<int>("@Id");
            }

            return country;
        }

        public async Task<Country> UpdateAsync(Country country)
        {
            var sql = @"
                EXEC [dbo].[Country_Update] 
                    @Id, @Name, @Code, @SubdivisionLabelEn, @SubdivisionLabelHi, 
                    @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", country.Id);
            parameters.Add("@Name", country.Name);
            parameters.Add("@Code", country.Code);
            parameters.Add("@SubdivisionLabelEn", country.SubdivisionLabelEn);
            parameters.Add("@SubdivisionLabelHi", country.SubdivisionLabelHi);
            parameters.Add("@IsActive", country.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await _connection.ExecuteAsync(sql, parameters);
            return country;
        }

        public async Task DeleteAsync(Country country)
        {
            var sql = "EXEC [dbo].[Country_Delete] @Id";
            await _connection.ExecuteAsync(sql, new { Id = country.Id });
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
