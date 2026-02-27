using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class CategoryDapperRepository : ICategoryRepository
    {
        private readonly MasterDbContext _context;

        public CategoryDapperRepository(MasterDbContext context)
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

        public async Task<Category?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Category_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Category_GetAll]";
            return await connection.QueryAsync<Category>(sql);
        }

        public async Task<IEnumerable<Category>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Category_GetActive]";
            return await connection.QueryAsync<Category>(sql);
        }

        public async Task<IEnumerable<Category>> GetActiveByTypeAsync(string type)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Category_GetActiveByType] @Type";
            return await connection.QueryAsync<Category>(sql, new { Type = type });
        }

        public async Task<Category> AddAsync(Category category)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Category_Create] 
                    @Name, @NameEn, @NameHi, @Type, @Description, 
                    @DisplayOrder, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", category.Name);
            parameters.Add("@NameEn", category.NameEn);
            parameters.Add("@NameHi", category.NameHi);
            parameters.Add("@Type", category.Type);
            parameters.Add("@Description", category.Description);
            parameters.Add("@DisplayOrder", category.DisplayOrder);
            parameters.Add("@IsActive", category.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                category.Id = parameters.Get<int>("@Id");
            }

            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Category_Update] 
                    @Id, @Name, @NameEn, @NameHi, @Type, @Description, 
                    @DisplayOrder, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", category.Id);
            parameters.Add("@Name", category.Name);
            parameters.Add("@NameEn", category.NameEn);
            parameters.Add("@NameHi", category.NameHi);
            parameters.Add("@Type", category.Type);
            parameters.Add("@Description", category.Description);
            parameters.Add("@DisplayOrder", category.DisplayOrder);
            parameters.Add("@IsActive", category.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await connection.ExecuteAsync(sql, parameters);
        }

        public async Task DeleteAsync(Category category)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Category_Delete] @Id";
            await connection.ExecuteAsync(sql, new { Id = category.Id });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
