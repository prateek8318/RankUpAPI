using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class CategoryDapperRepository : BaseDapperRepository, ICategoryRepository
    {
        public CategoryDapperRepository(string connectionString) : base(connectionString)
        {
        }


        public async Task<Category?> GetByIdAsync(int id)
        {
            try
            {
                return await WithConnectionAsync(async connection =>
                {
                    var sql = "EXEC [dbo].[Category_GetById] @Id";
                    return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting category by ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Category_GetAll]";
                return await connection.QueryAsync<Category>(sql);
            });
        }

        public async Task<IEnumerable<Category>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Category_GetActive]";
                return await connection.QueryAsync<Category>(sql);
            });
        }

        public async Task<IEnumerable<Category>> GetActiveByTypeAsync(string type)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Category_GetActiveByType] @Type";
                return await connection.QueryAsync<Category>(sql, new { Type = type });
            });
        }

        public async Task<Category> AddAsync(Category category)
        {
            return await WithConnectionAsync(async connection =>
            {
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
            });
        }

        public async Task UpdateAsync(Category category)
        {
            try
            {
                await WithConnectionAsync(async connection =>
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
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(Category category)
        {
            try
            {
                await WithConnectionAsync(async connection =>
                {
                    var sql = "EXEC [dbo].[Category_Delete] @Id";
                    await connection.ExecuteAsync(sql, new { Id = category.Id });
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting category: {ex.Message}", ex);
            }
        }

        public async Task<bool> ToggleCategoryStatusAsync(int id, bool isActive)
        {
            try
            {
                return await WithConnectionAsync(async connection =>
                {
                    var sql = "EXEC [dbo].[Category_ToggleStatus] @Id, @IsActive";
                    var result = await connection.ExecuteAsync(sql, new { Id = id, IsActive = isActive });
                    return result > 0;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error toggling category status: {ex.Message}", ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper commands are executed immediately; this exists for interface compatibility.
            return await Task.FromResult(1);
        }
    }
}
