using AutoMapper;
using Common.Language;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly ILanguageDataService _languageDataService;
        private readonly ILogger<ExamService> _logger;

        public ExamService(IExamRepository examRepository, IMapper mapper, ILanguageDataService languageDataService, ILogger<ExamService> logger)
        {
            _examRepository = examRepository;
            _mapper = mapper;
            _languageDataService = languageDataService;
            _logger = logger;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto)
        {
            var exam = _mapper.Map<Exam>(createDto);
            exam.CreatedAt = DateTime.UtcNow;
            exam.IsActive = true;

            if (createDto.Names != null && createDto.Names.Any())
            {
                foreach (var langDto in createDto.Names)
                {
                    exam.ExamLanguages.Add(new ExamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            if (createDto.QualificationIds != null && createDto.QualificationIds.Any())
            {
                var streamIds = createDto.StreamIds ?? new List<int?>();
                for (int i = 0; i < createDto.QualificationIds.Count; i++)
                {
                    var qualificationId = createDto.QualificationIds[i];
                    var streamId = i < streamIds.Count ? streamIds[i] : null;

                    exam.ExamQualifications.Add(new ExamQualification
                    {
                        QualificationId = qualificationId,
                        StreamId = streamId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _examRepository.AddAsync(exam);
            await _examRepository.SaveChangesAsync();

            return (await GetExamByIdAsync(exam.Id))!;
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return null;

            exam.Name = updateDto.Name;
            exam.Description = updateDto.Description;
            exam.CountryCode = updateDto.CountryCode;
            exam.MinAge = updateDto.MinAge;
            exam.MaxAge = updateDto.MaxAge;
            exam.ImageUrl = updateDto.ImageUrl;
            exam.UpdatedAt = DateTime.UtcNow;

            if (updateDto.Names != null && updateDto.Names.Any())
            {
                var existingLanguages = exam.ExamLanguages.ToList();
                foreach (var existingLang in existingLanguages)
                {
                    exam.ExamLanguages.Remove(existingLang);
                }

                foreach (var langDto in updateDto.Names)
                {
                    exam.ExamLanguages.Add(new ExamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            if (updateDto.QualificationIds != null)
            {
                var existingRelations = exam.ExamQualifications.ToList();
                foreach (var rel in existingRelations)
                {
                    exam.ExamQualifications.Remove(rel);
                }

                var streamIds = updateDto.StreamIds ?? new List<int?>();
                for (int i = 0; i < updateDto.QualificationIds.Count; i++)
                {
                    var qualificationId = updateDto.QualificationIds[i];
                    var streamId = i < streamIds.Count ? streamIds[i] : null;

                    exam.ExamQualifications.Add(new ExamQualification
                    {
                        QualificationId = qualificationId,
                        StreamId = streamId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _examRepository.UpdateAsync(exam);
            await _examRepository.SaveChangesAsync();
            return await GetExamByIdAsync(exam.Id);
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            return await _examRepository.SoftDeleteByIdAsync(id);
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id, int? languageId = null)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return null;

            var dto = _mapper.Map<ExamDto>(exam);
            
            // Populate QualificationIds and StreamIds
            dto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            dto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
            
            if (languageId.HasValue)
            {
                var langName = exam.ExamLanguages
                    .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Name;
                if (!string.IsNullOrEmpty(langName))
                    dto.Name = langName;

                var langDesc = exam.ExamLanguages
                    .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Description;
                if (langDesc != null)
                    dto.Description = langDesc;
            }

            return dto;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
                var exams = await _examRepository.GetActiveLocalizedAsync(normalizedLanguage);
                var examList = exams
                    .Select(e => MapToOptimizedExamDto(e, normalizedLanguage))
                    .ToList();

                if (!examList.Any())
                {
                    return await GetDefaultExamsOptimized(normalizedLanguage);
                }

                return examList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exams for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync(int? languageId = null)
        {
            var exams = await _examRepository.GetActiveAsync();
            var dtos = exams.OrderBy(e => e.Name)
                .Select(e => _mapper.Map<ExamDto>(e))
                .ToList();

            // Populate QualificationIds and StreamIds for all exams
            foreach (var dto in dtos)
            {
                var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                if (exam != null)
                {
                    dto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                    dto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                    
                    if (languageId.HasValue)
                    {
                        var langName = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;

                        var langDesc = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }

            return dtos;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByFilterAsync(string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge, int? languageId = null)
        {
            var exams = await _examRepository.GetByFilterAsync(countryCode, qualificationId, streamId, minAge, maxAge);
            var dtos = exams.OrderBy(e => e.Name)
                .Select(e => _mapper.Map<ExamDto>(e))
                .ToList();

            // Populate QualificationIds and StreamIds for all exams
            foreach (var dto in dtos)
            {
                var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                if (exam != null)
                {
                    dto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                    dto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                    
                    if (languageId.HasValue)
                    {
                        var langName = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;

                        var langDesc = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }

            return dtos;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByFilterAsync(string language, string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var exams = await _examRepository.GetByFilterLocalizedAsync(normalizedLanguage, countryCode, qualificationId, streamId, minAge, maxAge);
                var list = exams.Select(e => MapToOptimizedExamDto(e, normalizedLanguage)).ToList();
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exams by filter for language {Language}", language);
                throw;
            }
        }

        public async Task<bool> ToggleExamStatusAsync(int id, bool isActive)
        {
            return await _examRepository.SetActiveAsync(id, isActive);
        }

        public async Task<bool> UpdateExamImageUrlAsync(int examId, string imageUrl)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(examId);
                if (exam == null)
                    return false;

                exam.ImageUrl = imageUrl;
                exam.UpdatedAt = DateTime.UtcNow;
                
                await _examRepository.UpdateAsync(exam);
                await _examRepository.SaveChangesAsync();
                
                _logger.LogInformation("Updated image URL for exam {ExamId}: {ImageUrl}", examId, imageUrl);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating image URL for exam {ExamId}", examId);
                return false;
            }
        }

        private ExamDto MapToOptimizedExamDto(Exam exam, string language)
        {
            var useHindi = language == LanguageConstants.Hindi;

            // For now, use the main Name field since Exam doesn't have NameEn/NameHi
            // This can be enhanced later to support NameEn/NameHi like Category
            var localizedName = exam.Name;

            return new ExamDto
            {
                Id = exam.Id,
                Name = localizedName,
                Description = exam.Description,
                CountryCode = exam.CountryCode,
                MinAge = exam.MinAge,
                MaxAge = exam.MaxAge,
                ImageUrl = exam.ImageUrl,
                IsInternational = exam.IsInternational,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt
            };
        }

        private async Task<IEnumerable<ExamDto>> GetDefaultExamsOptimized(string language)
        {
            try
            {
                var data = await _languageDataService.GetLocalizedDataAsync(language, "exams");
                if (data.TryGetValue("exams", out var examsData) && examsData is IEnumerable<object> items)
                {
                    return items.Select(item => MapToOptimizedExamDto(item, language)).ToList();
                }

                return new List<ExamDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default exams for language {Language}", language);
                return new List<ExamDto>();
            }
        }

        private ExamDto MapToOptimizedExamDto(object item, string language)
        {
            try
            {
                _logger.LogInformation("Mapping exam item of type: {ItemType}", item?.GetType().Name);
                
                // Handle JsonElement from System.Text.Json
                if (item is System.Text.Json.JsonElement element)
                {
                    var id = element.GetProperty("id").GetInt32();
                    var name = element.GetProperty("name").GetString();
                    var description = element.TryGetProperty("description", out var descProp) ? descProp.GetString() : null;
                    var countryCode = element.TryGetProperty("countryCode", out var ccProp) ? ccProp.GetString() : null;
                    var minAge = element.TryGetProperty("minAge", out var minAgeProp) ? minAgeProp.GetInt32() : (int?)null;
                    var maxAge = element.TryGetProperty("maxAge", out var maxAgeProp) ? maxAgeProp.GetInt32() : (int?)null;
                    var imageUrl = element.TryGetProperty("imageUrl", out var imgProp) ? imgProp.GetString() : null;
                    var isInternational = element.TryGetProperty("isInternational", out var intProp) ? intProp.GetBoolean() : false;

                    return new ExamDto
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                        CountryCode = countryCode,
                        MinAge = minAge,
                        MaxAge = maxAge,
                        ImageUrl = imageUrl,
                        IsInternational = isInternational,
                        IsActive = true
                    };
                }

                // Handle dynamic objects
                if (item is System.Dynamic.ExpandoObject expando)
                {
                    var dict = (IDictionary<string, object>)expando;
                    return new ExamDto
                    {
                        Id = dict.ContainsKey("id") ? Convert.ToInt32(dict["id"]) : 0,
                        Name = dict.ContainsKey("name") ? dict["name"]?.ToString() : "",
                        Description = dict.ContainsKey("description") ? dict["description"]?.ToString() : null,
                        CountryCode = dict.ContainsKey("countryCode") ? dict["countryCode"]?.ToString() : null,
                        MinAge = dict.ContainsKey("minAge") ? Convert.ToInt32(dict["minAge"]) : (int?)null,
                        MaxAge = dict.ContainsKey("maxAge") ? Convert.ToInt32(dict["maxAge"]) : (int?)null,
                        ImageUrl = dict.ContainsKey("imageUrl") ? dict["imageUrl"]?.ToString() : null,
                        IsInternational = dict.ContainsKey("isInternational") ? Convert.ToBoolean(dict["isInternational"]) : false,
                        IsActive = true
                    };
                }

                _logger.LogWarning("Unsupported exam item type: {ItemType}", item?.GetType());
                return new ExamDto { IsActive = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping exam item: {Item}", item);
                return new ExamDto { IsActive = true };
            }
        }
    }
}
