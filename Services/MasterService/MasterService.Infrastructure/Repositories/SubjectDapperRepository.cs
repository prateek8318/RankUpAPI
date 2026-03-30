using Dapper;
using Microsoft.Data.SqlClient;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;
using Common.DTOs;

namespace MasterService.Infrastructure.Repositories
{
    public class SubjectDapperRepository : BaseDapperRepository, ISubjectRepository
    {
        public SubjectDapperRepository(string connectionString, ILogger<SubjectDapperRepository> logger) : base(connectionString, logger)
        {
        }

        public async Task<IEnumerable<Subject>> GetAllAsync(int? languageId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var procedure = languageId.HasValue
                    ? "[dbo].[Subject_GetAllByLanguage]"
                    : "[dbo].[Subject_GetAll]";
                var subjectList = (await connection.QueryAsync<Subject>(
                    procedure,
                    new { LanguageId = languageId },
                    commandType: CommandType.StoredProcedure)).ToList();
                await PopulateSubjectLanguagesAsync(connection, subjectList, languageId);

                return subjectList;
            });
        }

        public async Task<Subject?> GetByIdAsync(int id, int? languageId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var procedure = languageId.HasValue
                    ? "[dbo].[Subject_GetByIdByLanguage]"
                    : "[dbo].[Subject_GetById]";
                
                var parameters = languageId.HasValue
                    ? (object)new { Id = id, LanguageId = languageId.Value }
                    : (object)new { Id = id };
                
                var subject = await connection.QueryFirstOrDefaultAsync<Subject>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure);
                
                if (subject != null)
                {
                    await PopulateSubjectLanguagesAsync(connection, new[] { subject }, languageId);
                }
                
                return subject;
            });
        }

        public async Task<PaginatedResponse<Subject>> GetAllAsync(PaginationRequest pagination)
        {
            var subjects = (await GetAllAsync()).ToList();
            return new PaginatedResponse<Subject>
            {
                Data = subjects.Skip(pagination.Skip).Take(pagination.PageSize).ToList(),
                TotalCount = subjects.Count,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync(int? languageId = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var procedure = languageId.HasValue
                    ? "[dbo].[Subject_GetActiveByLanguage]"
                    : "[dbo].[Subject_GetActive]";
                var subjectList = (await connection.QueryAsync<Subject>(
                    procedure,
                    new { LanguageId = languageId },
                    commandType: CommandType.StoredProcedure)).ToList();
                await PopulateSubjectLanguagesAsync(connection, subjectList, languageId);

                return subjectList;
            });
        }

        public async Task<PaginatedResponse<Subject>> GetActiveSubjectsAsync(PaginationRequest pagination)
        {
            var subjects = (await GetActiveSubjectsAsync()).ToList();
            return new PaginatedResponse<Subject>
            {
                Data = subjects.Skip(pagination.Skip).Take(pagination.PageSize).ToList(),
                TotalCount = subjects.Count,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<Subject> AddAsync(Subject subject, string? namesJson = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", subject.Name);
                parameters.Add("@Description", subject.Description ?? (object?)null);
                parameters.Add("@IsActive", subject.IsActive);
                parameters.Add("@NamesJson", namesJson);
                parameters.Add("@CreatedAt", DateTime.UtcNow);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("[dbo].[Subject_Create]", parameters, commandType: CommandType.StoredProcedure);
                
                if (parameters.Get<int>("@Id") > 0)
                {
                    subject.Id = parameters.Get<int>("@Id");
                }

                return subject;
            });
        }

        public async Task<Subject> UpdateAsync(Subject subject, string? namesJson = null)
        {
            return await WithConnectionAsync(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", subject.Id);
                parameters.Add("@Name", subject.Name);
                parameters.Add("@Description", subject.Description ?? (object?)null);
                parameters.Add("@IsActive", subject.IsActive);
                parameters.Add("@NamesJson", namesJson);
                parameters.Add("@UpdatedAt", DateTime.UtcNow);

                await connection.ExecuteAsync("[dbo].[Subject_Update]", parameters, commandType: CommandType.StoredProcedure);
                return subject;
            });
        }

        public async Task DeleteAsync(Subject subject)
        {
            await WithConnectionAsync(async connection =>
            {
                await connection.ExecuteAsync("[dbo].[Subject_Delete]", new { Id = subject.Id }, commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<bool> ToggleSubjectStatusAsync(int id, bool isActive)
        {
            try
            {
                return await WithConnectionAsync(async connection =>
                {
                    var result = await connection.ExecuteAsync("[dbo].[Subject_ToggleStatus]", new { Id = id, IsActive = isActive }, commandType: CommandType.StoredProcedure);
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
                var result = await connection.QueryFirstOrDefaultAsync<int>("[dbo].[Subject_Exists]", new { Id = id }, commandType: CommandType.StoredProcedure);
                return result > 0;
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogDebug("SaveChangesAsync invoked on {Repository}. Dapper calls are already committed.", nameof(SubjectDapperRepository));
            return await Task.FromResult(0);
        }

        private async Task PopulateSubjectLanguagesAsync(IDbConnection connection, IEnumerable<Subject> subjects, int? languageId = null)
        {
            var subjectList = subjects.ToList();
            if (subjectList.Count == 0)
            {
                return;
            }

            var subjectIds = subjectList.Select(subject => subject.Id).Distinct().ToArray();
            var languages = await connection.QueryAsync<SubjectLanguage>(
                "[dbo].[SubjectLanguage_GetBySubjectIds]",
                new
                {
                    SubjectIds = string.Join(",", subjectIds),
                    LanguageId = languageId
                },
                commandType: CommandType.StoredProcedure);
            RepositoryEntityMapper.AttachSubjectLanguages(subjectList, languages);
        }
    }
}
