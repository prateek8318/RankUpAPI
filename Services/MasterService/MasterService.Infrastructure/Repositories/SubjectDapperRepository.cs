using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using System.Data;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectDapperRepository : BaseDapperRepository, ISubjectRepository
    {
        public SubjectDapperRepository(string connectionString) : base(connectionString)
        {
        }


        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Subject_GetAll]";
                var subjects = await connection.QueryAsync<Subject>(sql);

                // Load SubjectLanguages for each subject
                var subjectList = subjects.ToList();
                foreach (var subject in subjectList)
                {
                    var languagesSql = "EXEC [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId";
                    subject.SubjectLanguages = (await connection.QueryAsync<SubjectLanguage>(languagesSql, new { SubjectId = subject.Id })).ToList();
                }

                return subjectList;
            });
        }

        public async Task<Subject> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Subject_GetById] @Id";
                var subject = await connection.QueryFirstOrDefaultAsync<Subject>(sql, new { Id = id });
                
                if (subject != null)
                {
                    // Load SubjectLanguages for the subject
                    var languagesSql = "EXEC [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId";
                    subject.SubjectLanguages = (await connection.QueryAsync<SubjectLanguage>(languagesSql, new { SubjectId = id })).ToList();
                }
                
                return subject ?? new Subject();
            });
        }

        public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Subject_GetActive]";
                var subjects = await connection.QueryAsync<Subject>(sql);

                // Load SubjectLanguages for each subject
                var subjectList = subjects.ToList();
                foreach (var subject in subjectList)
                {
                    var languagesSql = "EXEC [dbo].[SubjectLanguage_GetBySubjectId] @SubjectId";
                    subject.SubjectLanguages = (await connection.QueryAsync<SubjectLanguage>(languagesSql, new { SubjectId = subject.Id })).ToList();
                }

                return subjectList;
            });
        }

        public async Task<Subject> AddAsync(Subject subject)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Subject_Create] 
                        @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt, @Id OUTPUT";

                var parameters = new DynamicParameters();
                parameters.Add("@Name", subject.Name);
                parameters.Add("@Description", subject.Description ?? (object?)null);
                parameters.Add("@IsActive", subject.IsActive);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    subject.Id = parameters.Get<int>("@Id");
                }

                return subject;
            });
        }

        public async Task<Subject> UpdateAsync(Subject subject)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = @"
                    EXEC [dbo].[Subject_Update] 
                        @Id, @Name, @Description, @IsActive, @UpdatedAt";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", subject.Id);
                parameters.Add("@Name", subject.Name);
                parameters.Add("@Description", subject.Description ?? (object?)null);
                parameters.Add("@IsActive", subject.IsActive);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync(sql, parameters);
                return subject;
            });
        }

        public async Task DeleteAsync(Subject subject)
        {
            await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Subject_Delete] @Id";
                await connection.ExecuteAsync(sql, new { Id = subject.Id });
            });
        }

        public async Task<bool> ToggleSubjectStatusAsync(int id, bool isActive)
        {
            try
            {
                return await WithConnectionAsync(async connection =>
                {
                    var sql = "EXEC [dbo].[Subject_ToggleStatus] @Id, @IsActive";
                    var result = await connection.ExecuteAsync(sql, new { Id = id, IsActive = isActive });
                    return result > 0;
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error toggling subject status: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                var sql = "EXEC [dbo].[Subject_Exists] @Id";
                var result = await connection.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });
                return result > 0;
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper operations are executed immediately. Returning 1 to indicate success.
            return await Task.FromResult(1);
        }
    }
}
