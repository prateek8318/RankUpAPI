using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class CountryDapperRepository : BaseDapperRepository, ICountryRepository
    {
        public CountryDapperRepository(string connectionString) : base(connectionString)
        {
        }


        public async Task<Country?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Country_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<Country>(sql, new { Id = id });
            });
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Country_GetAll]";
                return await connection.QueryAsync<Country>(sql);
            });
        }

        public async Task<Country?> GetByCodeAsync(string code)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Country_GetByCode] @Code";
                return await connection.QueryFirstOrDefaultAsync<Country>(sql, new { Code = code });
            });
        }

        public async Task<Country> AddAsync(Country country)
        {
            return await WithConnectionAsync(async connection =>
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

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    country.Id = parameters.Get<int>("@Id");
                }

                return country;
            });
        }

        public async Task<Country> UpdateAsync(Country country)
        {
            return await WithConnectionAsync(async connection =>
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

                await connection.ExecuteAsync(sql, parameters);
                return country;
            });
        }

        public async Task DeleteAsync(Country country)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Country_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = country.Id });
                return Task.CompletedTask;
            });
        }

        public async Task<bool> ToggleCountryStatusAsync(int id, bool isActive)
        {
            try
            {
                return await WithConnectionAsync(async connection =>
                {
                    var sql = "EXEC [dbo].[Country_ToggleStatus] @Id, @IsActive";
                    var result = await connection.ExecuteAsync(sql, new { Id = id, IsActive = isActive });
                    return result > 0;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error toggling country status: {ex.Message}", ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper commands are executed immediately; this exists for interface compatibility.
            return await Task.FromResult(1);
        }
    }
}
