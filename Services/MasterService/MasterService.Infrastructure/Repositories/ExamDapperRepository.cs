using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Infrastructure.Repositories
{
    public class ExamDapperRepository : BaseDapperRepository, IExamRepository
    {
        public ExamDapperRepository(string connectionString) : base(connectionString)
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
            using var multi = await _connection.QueryMultipleAsync(
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
                
                // Map languages to exams
                foreach (var exam in exams)
                {
                    exam.ExamLanguages = languages.Where(l => l.ExamId == exam.Id).ToList();
                }
                
                return exams;
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
                
                // Map languages to exams
                foreach (var exam in exams)
                {
                    exam.ExamLanguages = languages.Where(l => l.ExamId == exam.Id).ToList();
                }
                
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
                
                // Map languages to exams
                foreach (var exam in exams)
                {
                    exam.ExamLanguages = languages.Where(l => l.ExamId == exam.Id).ToList();
                }
                
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
                
                // Map languages to exams
                foreach (var exam in exams)
                {
                    exam.ExamLanguages = languages.Where(l => l.ExamId == exam.Id).ToList();
                }
                
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
                
                // Map languages to exams
                foreach (var exam in exams)
                {
                    exam.ExamLanguages = languages.Where(l => l.ExamId == exam.Id).ToList();
                }
                
                return exams;
            });
        }

        public async Task<Exam> AddAsync(Exam exam)
        {
            var namesJson = exam.ExamLanguages != null && exam.ExamLanguages.Any()
                ? JsonSerializer.Serialize(exam.ExamLanguages.Select(x => new { x.LanguageId, x.Name, x.Description }))
                : null;

            var relationsJson = exam.ExamQualifications != null && exam.ExamQualifications.Any()
                ? JsonSerializer.Serialize(exam.ExamQualifications.Select(x => new { x.QualificationId, x.StreamId }))
                : null;

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

            await _connection.ExecuteAsync(
                "[dbo].[Exam_Create]",
                parameters,
                commandType: CommandType.StoredProcedure);
            
            if (parameters.Get<int>("@Id") > 0)
            {
                exam.Id = parameters.Get<int>("@Id");
            }

            return exam;
        }

        public async Task UpdateAsync(Exam exam)
        {
            var namesJson = exam.ExamLanguages != null && exam.ExamLanguages.Any()
                ? JsonSerializer.Serialize(exam.ExamLanguages.Select(x => new { x.LanguageId, x.Name, x.Description }))
                : null;

            var relationsJson = exam.ExamQualifications != null && exam.ExamQualifications.Any()
                ? JsonSerializer.Serialize(exam.ExamQualifications.Select(x => new { x.QualificationId, x.StreamId }))
                : null;

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

            await _connection.ExecuteAsync(
                "[dbo].[Exam_Update]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(Exam exam)
        {
            await _connection.ExecuteAsync(
                "[dbo].[Exam_Delete]",
                new { Id = exam.Id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> SoftDeleteByIdAsync(int id)
        {
            var affected = await _connection.ExecuteAsync(
                "[dbo].[Exam_SoftDelete]",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var affected = await _connection.ExecuteAsync(
                "[dbo].[Exam_SetActive]",
                new { Id = id, IsActive = isActive },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            // Dapper commands are executed immediately; this exists for interface compatibility.
            return await Task.FromResult(1);
        }
    }
}
