using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Repositories
{
    public class AdminDapperRepository : IAdminRepository
    {
        private readonly IDbConnection _connection;

        public AdminDapperRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<Admin?> GetByIdAsync(int id)
        {
            var sql = "EXEC [dbo].[Admin_GetById] @Id";
            return await _connection.QueryFirstOrDefaultAsync<Admin>(sql, new { Id = id });
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            var sql = "EXEC [dbo].[Admin_GetByUserId] @UserId";
            return await _connection.QueryFirstOrDefaultAsync<Admin>(sql, new { UserId = userId });
        }

        public async Task<Admin?> GetByIdWithRolesAsync(int id)
        {
            // For complex queries with includes, create separate SPs
            // For now, using basic GetById
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            var sql = "EXEC [dbo].[Admin_GetAll]";
            return await _connection.QueryAsync<Admin>(sql);
        }

        public async Task<Admin> AddAsync(Admin admin)
        {
            var sql = @"
                EXEC [dbo].[Admin_Insert] 
                    @UserId, @Name, @Email, @PhoneNumber, @IsActive, @CreatedAt, @UpdatedAt";

            await _connection.ExecuteAsync(sql, admin);
            return admin;
        }

        public Task UpdateAsync(Admin admin)
        {
            // For updates, create separate SP if needed
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }
    }
}
