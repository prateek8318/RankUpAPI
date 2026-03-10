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
            var connectionString = _context.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string is not initialized in MasterDbContext");
                
            return new SqlConnection(connectionString);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            try
            {
                var sql = "EXEC [dbo].[Category_GetById] @Id";
                return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting category by ID {id}: {ex.Message}", ex);
            }
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
                    @NameEn, @NameHi, @Key, @Type, @Description, 
                    @DisplayOrder, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@NameEn", category.NameEn);
            parameters.Add("@NameHi", category.NameHi ?? (object?)null);
            parameters.Add("@Key", category.Key);
            parameters.Add("@Type", category.Type);
            parameters.Add("@Description", category.Description ?? (object?)null);
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
            
            try
            {
                var sql = @"
                    DECLARE @Id INT = @IdParam;
                    DECLARE @NameEn NVARCHAR(100) = @NameEnParam;
                    DECLARE @NameHi NVARCHAR(100) = @NameHiParam;
                    DECLARE @Key NVARCHAR(50) = @KeyParam;
                    DECLARE @Type NVARCHAR(50) = @TypeParam;
                    DECLARE @Description NVARCHAR(500) = @DescriptionParam;
                    DECLARE @DisplayOrder INT = @DisplayOrderParam;
                    DECLARE @IsActive BIT = @IsActiveParam;
                    DECLARE @UpdatedAt DATETIME2 = GETUTCDATE();
                    
                    EXEC [dbo].[Category_Update] 
                        @Id, @NameEn, @NameHi, @Key, @Type, @Description, 
                        @DisplayOrder, @IsActive, @UpdatedAt";

                var parameters = new DynamicParameters();
                parameters.Add("@IdParam", category.Id);
                parameters.Add("@NameEnParam", category.NameEn);
                parameters.Add("@NameHiParam", category.NameHi ?? (object?)null);
                parameters.Add("@KeyParam", category.Key);
                parameters.Add("@TypeParam", category.Type);
                parameters.Add("@DescriptionParam", category.Description ?? (object?)null);
                parameters.Add("@DisplayOrderParam", category.DisplayOrder);
                parameters.Add("@IsActiveParam", category.IsActive);

                await connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(Category category)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            try
            {
                var sql = @"
                    UPDATE Categories
                    SET 
                        IsActive = 0,
                        UpdatedAt = GETUTCDATE()
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, new { Id = category.Id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting category: {ex.Message}", ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
