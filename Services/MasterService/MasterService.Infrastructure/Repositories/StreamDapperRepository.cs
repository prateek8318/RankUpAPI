using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using MasterService.Infrastructure.Data;
using System.Data;
using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Infrastructure.Repositories
{
    public class StreamDapperRepository : IStreamRepository
    {
        private readonly MasterDbContext _context;

        public StreamDapperRepository(MasterDbContext context)
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

        public async Task<StreamEntity?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Stream_GetById] @Id";
            return await connection.QueryFirstOrDefaultAsync<StreamEntity>(sql, new { Id = id });
        }

        public async Task<IEnumerable<StreamEntity>> GetAllAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Stream_GetAll]";
            return await connection.QueryAsync<StreamEntity>(sql);
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveAsync()
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Stream_GetActive]";
            return await connection.QueryAsync<StreamEntity>(sql);
        }

        public async Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdAsync(int qualificationId)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = "EXEC [dbo].[Stream_GetActiveByQualificationId] @QualificationId";
            return await connection.QueryAsync<StreamEntity>(sql, new { QualificationId = qualificationId });
        }

        public async Task<StreamEntity> AddAsync(StreamEntity stream)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            
            var sql = @"
                EXEC [dbo].[Stream_Create] 
                    @Name, @QualificationId, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", stream.Name);
            parameters.Add("@QualificationId", stream.QualificationId);
            parameters.Add("@IsActive", stream.IsActive);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(sql, parameters);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                stream.Id = parameters.Get<int>("@Id");
            }

            return stream;
        }

        public Task UpdateAsync(StreamEntity stream)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = @"
                EXEC [dbo].[Stream_Update] 
                    @Id, @Name, @QualificationId, @IsActive, @UpdatedAt";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", stream.Id);
            parameters.Add("@Name", stream.Name);
            parameters.Add("@QualificationId", stream.QualificationId);
            parameters.Add("@IsActive", stream.IsActive);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            connection.Execute(sql, parameters);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(StreamEntity stream)
        {
            using var connection = GetConnection();
            connection.Open();
            
            var sql = "EXEC [dbo].[Stream_Delete] @Id";
            connection.Execute(sql, new { Id = stream.Id });
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
