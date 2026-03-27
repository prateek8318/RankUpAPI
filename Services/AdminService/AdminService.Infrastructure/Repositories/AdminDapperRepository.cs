using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Repositories
{
    public class AdminDapperRepository : BaseDapperRepository, IAdminRepository
    {
        public AdminDapperRepository(string connectionString, ILogger<AdminDapperRepository> logger)
            : base(connectionString, logger)
        {
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(connection =>
                connection.QueryFirstOrDefaultAsync<Admin>(
                    "[dbo].[Admin_GetById]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            return await WithConnectionAsync(connection =>
                connection.QueryFirstOrDefaultAsync<Admin>(
                    "[dbo].[Admin_GetByUserId]",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<Admin?> GetByIdWithRolesAsync(int id)
        {
            // For complex queries with includes, create separate SPs
            // For now, using basic GetById
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await WithConnectionAsync(connection =>
                connection.QueryAsync<Admin>(
                    "[dbo].[Admin_GetAll]",
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<Admin> AddAsync(Admin admin)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", admin.UserId);
            parameters.Add("@Role", admin.Role);
            parameters.Add("@IsTwoFactorEnabled", admin.IsTwoFactorEnabled);
            parameters.Add("@MobileNumber", admin.MobileNumber);
            parameters.Add("@AdminId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await WithConnectionAsync(connection =>
                connection.ExecuteAsync(
                    "[dbo].[Admin_Create]",
                    parameters,
                    commandType: CommandType.StoredProcedure));

            admin.Id = parameters.Get<int>("@AdminId");
            return admin;
        }

        public async Task UpdateAsync(Admin admin)
        {
            await WithConnectionAsync(connection =>
                connection.ExecuteAsync(
                    "[dbo].[Admin_Update]",
                    new
                    {
                        admin.Id,
                        admin.Role,
                        admin.IsTwoFactorEnabled,
                        admin.MobileNumber
                    },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogDebug("SaveChangesAsync invoked on {Repository}. Dapper calls are already committed.", nameof(AdminDapperRepository));
            return await Task.FromResult(0);
        }
    }
}
