using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;

namespace TestService.Infrastructure.Repositories
{
    public class GenericDapperRepository<T> : BaseDapperRepository, IRepository<T> where T : BaseEntity
    {
        public GenericDapperRepository(string connectionString) : base(connectionString)
        {
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var entityName = typeof(T).Name;
            var sql = $"EXEC [dbo].[{entityName}_GetById] @Id";
            return await WithConnectionAsync(async connection => 
                await connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id }));
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var entityName = typeof(T).Name;
            var sql = $"EXEC [dbo].[{entityName}_GetAll]";
            return await WithConnectionAsync(async connection => 
                await connection.QueryAsync<T>(sql));
        }

        public virtual async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            // For complex predicates, create a specific stored procedure
            // This is a fallback for simple cases - consider creating SPs for complex queries
            throw new NotImplementedException("Use specific repository methods with stored procedures for complex queries");
        }

        public virtual async Task AddAsync(T entity)
        {
            var entityName = typeof(T).Name;
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "CreatedAt" && p.Name != "UpdatedAt")
                .ToList();
            
            var parameterNames = properties.Select(p => $"@{p.Name}").ToArray();
            
            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                var value = property.GetValue(entity);
                parameters.Add($"@{property.Name}", value ?? DBNull.Value);
            }
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            
            var sql = $"EXEC [dbo].[{entityName}_Create] {string.Join(", ", parameterNames)}, @Id OUTPUT";
            
            await WithConnectionAsync(async connection => 
            {
                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    entity.Id = parameters.Get<int>("@Id");
                }
            });
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var entityName = typeof(T).Name;
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "CreatedAt")
                .ToList();
            
            var parameterNames = properties.Select(p => $"@{p.Name}").ToArray();
            
            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                var value = property.GetValue(entity);
                parameters.Add($"@{property.Name}", value ?? DBNull.Value);
            }
            
            var sql = $"EXEC [dbo].[{entityName}_Update] {string.Join(", ", parameterNames)}";
            
            await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, parameters));
        }

        public virtual async Task DeleteAsync(T entity)
        {
            var entityName = typeof(T).Name;
            var sql = $"EXEC [dbo].[{entityName}_Delete] @Id";
            await WithConnectionAsync(async connection => 
                await connection.ExecuteAsync(sql, new { Id = entity.Id }));
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException("SaveChangesAsync is not supported in pure Dapper implementation. Use specific stored procedures for data operations.");
        }
    }
}
