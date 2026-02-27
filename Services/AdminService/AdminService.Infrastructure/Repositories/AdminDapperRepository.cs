using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Infrastructure.Data;

namespace AdminService.Infrastructure.Repositories
{
    public class AdminDapperRepository : IAdminRepository
    {
        private readonly AdminDbContext _context;
        
        public AdminDapperRepository(AdminDbContext context)
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

        public async Task<Admin?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Admin_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<Admin>(sql, new { Id = id });
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Admin_GetByUserId] @UserId";
            return await connection.QueryFirstOrDefaultAsync<Admin>(sql, new { UserId = userId });
        }

        public async Task<Admin?> GetByIdWithRolesAsync(int id)
        {
            // For complex queries with includes, create separate SPs
            // For now, using basic GetById
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Admin_GetAll]";
            return await connection.QueryAsync<Admin>(sql);
        }

        public async Task<Admin> AddAsync(Admin admin)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Admin_Insert] 
                    @UserId, @Name, @Email, @PhoneNumber, @IsActive, @CreatedAt, @UpdatedAt";

            await connection.ExecuteAsync(sql, admin);
            return admin;
        }

        public Task UpdateAsync(Admin admin)
        {
            // For updates, create separate SP if needed
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
