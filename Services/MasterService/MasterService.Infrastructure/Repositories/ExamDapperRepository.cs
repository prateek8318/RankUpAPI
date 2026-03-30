using Dapper;
using System.Data;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Common.DTOs;

namespace MasterService.Infrastructure.Repositories
{
    public class ExamDapperRepository : BaseDapperRepository, IExamRepository
    {
        public ExamDapperRepository(string connectionString, ILogger<ExamDapperRepository> logger) : base(connectionString, logger)
        {
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetByIdWithRelations]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure);

                var exam = await multi.ReadFirstOrDefaultAsync<Exam>();
                if (exam == null)
                    return null;

                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var relations = (await multi.ReadAsync<ExamQualification>()).ToList();
                
                exam.ExamLanguages = languages;
                exam.ExamQualifications = relations;
                return exam;
            });
        }

        public async Task<Exam?> GetByIdLocalizedAsync(int id, string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetByIdWithRelationsLocalized]",
                    new { Id = id, LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure);

                var exam = await multi.ReadFirstOrDefaultAsync<Exam>();
                if (exam == null)
                    return null;

                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var relations = (await multi.ReadAsync<ExamQualification>()).ToList();

                exam.ExamLanguages = languages;
                exam.ExamQualifications = relations;
                return exam;
            });
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetAllWithLanguages]",
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages);
                return exams;
            });
        }

        public async Task<PaginatedResponse<Exam>> GetAllAsync(PaginationRequest pagination)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetAllWithLanguagesPaginated]",
                    new { Skip = pagination.Skip, Take = pagination.PageSize },
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var totalCount = await multi.ReadFirstOrDefaultAsync<int>();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages);
                
                return new PaginatedResponse<Exam>
                {
                    Data = exams,
                    TotalCount = totalCount,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            });
        }

        public async Task<IEnumerable<Exam>> GetActiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetActiveWithLanguages]",
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var qualifications = (await multi.ReadAsync<ExamQualification>()).ToList();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages, qualifications);
                return exams;
            });
        }

        public async Task<PaginatedResponse<Exam>> GetActiveAsync(PaginationRequest pagination)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetActiveWithLanguagesPaginated]",
                    new { Skip = pagination.Skip, Take = pagination.PageSize },
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var qualifications = (await multi.ReadAsync<ExamQualification>()).ToList();
                var totalCount = await multi.ReadFirstOrDefaultAsync<int>();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages, qualifications);
                
                return new PaginatedResponse<Exam>
                {
                    Data = exams,
                    TotalCount = totalCount,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            });
        }

        public async Task<IEnumerable<Exam>> GetAllIncludingInactiveAsync()
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetAllWithLanguagesIncludingInactive]",
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var qualifications = (await multi.ReadAsync<ExamQualification>()).ToList();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages, qualifications);
                return exams;
            });
        }

        public async Task<IEnumerable<Exam>> GetAllIncludingInactiveLocalizedAsync(string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetAllWithLanguagesIncludingInactiveLocalized]",
                    new { LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var qualifications = (await multi.ReadAsync<ExamQualification>()).ToList();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages, qualifications);
                return exams;
            });
        }

        public async Task<IEnumerable<Exam>> GetActiveLocalizedAsync(string? languageCode)
        {
            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetActiveWithLanguagesLocalized]",
                    new { LanguageCode = languageCode },
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                var qualifications = (await multi.ReadAsync<ExamQualification>()).ToList();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages, qualifications);
                return exams;
            });
        }

        public async Task<IEnumerable<Exam>> GetByFilterAsync(string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CountryCode", countryCode);
            parameters.Add("@QualificationId", qualificationId);
            parameters.Add("@StreamId", streamId);
            parameters.Add("@MinAge", minAge);
            parameters.Add("@MaxAge", maxAge);

            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetByFilterWithLanguages]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages);
                return exams;
            });
        }

        public async Task<IEnumerable<Exam>> GetByFilterLocalizedAsync(string? languageCode, string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@LanguageCode", languageCode);
            parameters.Add("@CountryCode", countryCode);
            parameters.Add("@QualificationId", qualificationId);
            parameters.Add("@StreamId", streamId);
            parameters.Add("@MinAge", minAge);
            parameters.Add("@MaxAge", maxAge);

            return await WithConnectionAsync(async connection =>
            {
                using var multi = await connection.QueryMultipleAsync(
                    "[dbo].[Exam_GetByFilterWithLanguagesLocalized]",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var exams = (await multi.ReadAsync<Exam>()).ToList();
                var languages = (await multi.ReadAsync<ExamLanguage>()).ToList();
                
                RepositoryEntityMapper.AttachExamRelations(exams, languages);
                return exams;
            });
        }

        public async Task<Exam> AddAsync(Exam exam, string? namesJson = null, string? relationsJson = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", exam.Name);
            parameters.Add("@Description", exam.Description);
            parameters.Add("@CountryCode", exam.CountryCode);
            parameters.Add("@MinAge", exam.MinAge);
            parameters.Add("@MaxAge", exam.MaxAge);
            parameters.Add("@ImageUrl", exam.ImageUrl);
            parameters.Add("@IsInternational", exam.IsInternational);
            parameters.Add("@IsActive", exam.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@RelationsJson", relationsJson);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Exam_Create]",
                    parameters,
                    commandType: CommandType.StoredProcedure));
            
            if (parameters.Get<int>("@Id") > 0)
            {
                exam.Id = parameters.Get<int>("@Id");
            }

            return exam;
        }

        public async Task<Exam> AddAsync(Exam exam, string? namesJson = null, string? relationsJson = null, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Name", exam.Name);
            parameters.Add("@Description", exam.Description);
            parameters.Add("@CountryCode", exam.CountryCode);
            parameters.Add("@MinAge", exam.MinAge);
            parameters.Add("@MaxAge", exam.MaxAge);
            parameters.Add("@ImageUrl", exam.ImageUrl);
            parameters.Add("@IsInternational", exam.IsInternational);
            parameters.Add("@IsActive", exam.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@RelationsJson", relationsJson);
            parameters.Add("@CreatedAt", DateTime.UtcNow);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            if (transaction != null)
            {
                await transaction.Connection.ExecuteAsync(
                    "[dbo].[Exam_Create]",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                await WithConnectionAsync(async connection =>
                    await connection.ExecuteAsync(
                        "[dbo].[Exam_Create]",
                        parameters,
                        commandType: CommandType.StoredProcedure));
            }
            
            if (parameters.Get<int>("@Id") > 0)
            {
                exam.Id = parameters.Get<int>("@Id");
            }

            return exam;
        }

        public async Task UpdateAsync(Exam exam, string? namesJson = null, string? relationsJson = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", exam.Id);
            parameters.Add("@Name", exam.Name);
            parameters.Add("@Description", exam.Description);
            parameters.Add("@CountryCode", exam.CountryCode);
            parameters.Add("@MinAge", exam.MinAge);
            parameters.Add("@MaxAge", exam.MaxAge);
            parameters.Add("@ImageUrl", exam.ImageUrl);
            parameters.Add("@IsInternational", exam.IsInternational);
            parameters.Add("@IsActive", exam.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@RelationsJson", relationsJson);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Exam_Update]",
                    parameters,
                    commandType: CommandType.StoredProcedure));
        }

        public async Task UpdateAsync(Exam exam, string? namesJson = null, string? relationsJson = null, IDbTransaction? transaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", exam.Id);
            parameters.Add("@Name", exam.Name);
            parameters.Add("@Description", exam.Description);
            parameters.Add("@CountryCode", exam.CountryCode);
            parameters.Add("@MinAge", exam.MinAge);
            parameters.Add("@MaxAge", exam.MaxAge);
            parameters.Add("@ImageUrl", exam.ImageUrl);
            parameters.Add("@IsInternational", exam.IsInternational);
            parameters.Add("@IsActive", exam.IsActive);
            parameters.Add("@NamesJson", namesJson);
            parameters.Add("@RelationsJson", relationsJson);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            if (transaction != null)
            {
                await transaction.Connection.ExecuteAsync(
                    "[dbo].[Exam_Update]",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                await WithConnectionAsync(async connection =>
                    await connection.ExecuteAsync(
                        "[dbo].[Exam_Update]",
                        parameters,
                        commandType: CommandType.StoredProcedure));
            }
        }

        public async Task DeleteAsync(Exam exam)
        {
            await WithConnectionAsync(connection =>
                connection.ExecuteAsync(
                    "[dbo].[Exam_Delete]",
                    new { Id = exam.Id },
                    commandType: CommandType.StoredProcedure));
        }

        public async Task DeleteAsync(Exam exam, IDbTransaction? transaction = null)
        {
            if (transaction != null)
            {
                await transaction.Connection.ExecuteAsync(
                    "[dbo].[Exam_Delete]",
                    new { Id = exam.Id },
                    transaction,
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                await WithConnectionAsync(connection =>
                    connection.ExecuteAsync(
                        "[dbo].[Exam_Delete]",
                        new { Id = exam.Id },
                        commandType: CommandType.StoredProcedure));
            }
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var affected = await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Exam_SoftDelete]",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure));

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var affected = await WithConnectionAsync(async connection =>
                await connection.ExecuteAsync(
                    "[dbo].[Exam_SetActive]",
                    new { Id = id, IsActive = isActive },
                    commandType: CommandType.StoredProcedure));

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogDebug("SaveChangesAsync invoked on {Repository}. Dapper calls are already committed.", nameof(ExamDapperRepository));
            return await Task.FromResult(0);
        }
    }
}
