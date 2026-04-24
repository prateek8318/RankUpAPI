using AutoMapper;
using Common.Language;
using MasterService.Application.DTOs;
using MasterService.Application.Helpers;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.Application.Services
{
    public class ExamService : BaseService, IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;
        private readonly ILanguageDataService _languageDataService;

        public ExamService(
            IExamRepository examRepository,
            ISubjectRepository subjectRepository,
            IMapper mapper,
            ILanguageDataService languageDataService,
            ILogger<ExamService> logger) : base(logger)
        {
            _examRepository = examRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
            _languageDataService = languageDataService;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto)
        {
            return await ExecuteInTransactionAsync(
                _examRepository,
                async (connection, transaction) =>
                {
                    if (createDto.SubjectIds == null || !createDto.SubjectIds.Any())
                    {
                        throw new ArgumentException("At least one subject is required.");
                    }

                    var exam = _mapper.Map<Exam>(createDto);
                    exam.CreatedAt = DateTime.UtcNow;
                    exam.IsActive = true;

                    // Set the base Name from the first language name to satisfy database constraint
                    if (createDto.Names != null && createDto.Names.Any())
                    {
                        exam.Name = createDto.Names.First().Name ?? string.Empty;
                    }

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

                    var namesJson = LanguagePayloadSerializer.SerializeItems(
                        exam.ExamLanguages,
                        language => new { language.LanguageId, language.Name, language.Description });
                    var relationsJson = LanguagePayloadSerializer.SerializeItems(
                        exam.ExamQualifications,
                        relation => new { relation.QualificationId, relation.StreamId });

                    await _examRepository.AddAsync(exam, namesJson, relationsJson);
                    await _examRepository.ReplaceExamSubjectsAsync(exam.Id, createDto.SubjectIds, transaction);
                    await _examRepository.SaveChangesAsync();

                    return (await GetExamByIdAsync(exam.Id))!;
                },
                "CreateExam");
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
        {
            return await ExecuteInTransactionAsync(
                _examRepository,
                async (connection, transaction) =>
                {
                    if (updateDto.SubjectIds == null || !updateDto.SubjectIds.Any())
                    {
                        throw new ArgumentException("At least one subject is required.");
                    }

                    var exam = await _examRepository.GetByIdAsync(id);
                    if (exam == null)
                        return null;

                    // Handle Name property - if null or empty, keep existing value
                    if (string.IsNullOrWhiteSpace(updateDto.Name))
                    {
                        // Keep previous value when client sends partial payload without Name.
                        exam.Name = exam.Name; // Keep existing name
                    }
                    else
                    {
                        exam.Name = updateDto.Name;
                    }
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

                    var namesJson = LanguagePayloadSerializer.SerializeItems(
                        exam.ExamLanguages,
                        language => new { language.LanguageId, language.Name, language.Description });
                    var relationsJson = LanguagePayloadSerializer.SerializeItems(
                        exam.ExamQualifications,
                        relation => new { relation.QualificationId, relation.StreamId });

                    // Use the UpdateAsync method that accepts transaction
                    await _examRepository.UpdateAsync(exam, namesJson, relationsJson, transaction);
                    await _examRepository.ReplaceExamSubjectsAsync(exam.Id, updateDto.SubjectIds, transaction);
                    await _examRepository.SaveChangesAsync();
                    return await GetExamByIdAsync(exam.Id);
                },
                "UpdateExam");
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete exam with ID {ExamId}", id);
                
                var exam = await _examRepository.GetByIdAsync(id);
                if (exam == null)
                {
                    _logger.LogWarning("Exam with ID {ExamId} not found", id);
                    return false;
                }

                _logger.LogInformation("Exam found: {ExamName} - proceeding with deletion", exam.Name);
                
                await _examRepository.DeleteAsync(exam);
                
                _logger.LogInformation("Exam with ID {ExamId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hard deleting exam with ID {ExamId}", id);
                return false;
            }
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
            dto.SubjectIds = exam.SubjectIds?.ToList() ?? new List<int>();
            dto.SubjectNames = await ResolveSubjectNamesAsync(dto.SubjectIds);
            
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
                    .Select(e => MapToExamDto(e, normalizedLanguage))
                    .ToList();

                if (!examList.Any())
                {
                    // Fallback to non-localized data if no localized data found
                    var fallbackExams = await _examRepository.GetActiveAsync();
                    return fallbackExams.Select(e => MapToExamDto(e)).ToList();
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
                .Select(e => MapToExamDto(e))
                .ToList();

            // Populate QualificationIds and StreamIds for all exams
            var subjectNameMap = await BuildSubjectNameMapAsync(exams.SelectMany(x => x.SubjectIds ?? new List<int>()));
            foreach (var dto in dtos)
            {
                var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                if (exam != null)
                {
                    dto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                    dto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                    dto.SubjectIds = exam.SubjectIds?.ToList() ?? new List<int>();
                    dto.SubjectNames = dto.SubjectIds
                        .Where(subjectId => subjectNameMap.ContainsKey(subjectId))
                        .Select(subjectId => new SubjectNameDto 
                        { 
                            Id = subjectId, 
                            Name = subjectNameMap[subjectId] 
                        })
                        .ToList();
                    
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
                .Select(e => MapToExamDto(e))
                .ToList();

            // Populate QualificationIds and StreamIds for all exams
            var subjectNameMap = await BuildSubjectNameMapAsync(exams.SelectMany(x => x.SubjectIds ?? new List<int>()));
            foreach (var dto in dtos)
            {
                var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                if (exam != null)
                {
                    dto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                    dto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                    dto.SubjectIds = exam.SubjectIds?.ToList() ?? new List<int>();
                    dto.SubjectNames = dto.SubjectIds
                        .Where(subjectId => subjectNameMap.ContainsKey(subjectId))
                        .Select(subjectId => new SubjectNameDto 
                        { 
                            Id = subjectId, 
                            Name = subjectNameMap[subjectId] 
                        })
                        .ToList();
                    
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
                var list = exams.Select(e => MapToExamDto(e, normalizedLanguage)).ToList();
                
                if (!list.Any())
                {
                    // Fallback to non-localized filter if no localized data found
                    var fallbackExams = await _examRepository.GetByFilterAsync(countryCode, qualificationId, streamId, minAge, maxAge);
                    return fallbackExams.Select(e => MapToExamDto(e)).ToList();
                }
                
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

        public async Task<IEnumerable<ExamDto>> GetAllExamsIncludingInactiveAsync(string? language = null, int? languageId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(language))
                {
                    var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                    var exams = await _examRepository.GetAllIncludingInactiveLocalizedAsync(normalizedLanguage);
                    var list = exams.Select(e => MapToExamDto(e, normalizedLanguage)).ToList();
                    
                    if (!list.Any())
                    {
                        // Fallback to non-localized data if no localized data found
                        var fallbackExams = await _examRepository.GetAllIncludingInactiveAsync();
                        return fallbackExams.Select(e => MapToExamDto(e)).ToList();
                    }
                    
                    return list;
                }
                else if (languageId.HasValue)
                {
                    var exams = await _examRepository.GetAllIncludingInactiveAsync();
                    var dtos = exams.OrderBy(e => e.Name)
                        .Select(e => MapToExamDto(e))
                        .ToList();

                    // Populate QualificationIds and StreamIds for all exams
                    var subjectNameMap = await BuildSubjectNameMapAsync(exams.SelectMany(x => x.SubjectIds ?? new List<int>()));
                    foreach (var dto in dtos)
                    {
                        var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                        if (exam != null)
                        {
                            dto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                            dto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                            dto.SubjectIds = exam.SubjectIds?.ToList() ?? new List<int>();
                            dto.SubjectNames = dto.SubjectIds
                                .Where(subjectId => subjectNameMap.ContainsKey(subjectId))
                                .Select(subjectId => new SubjectNameDto 
                                { 
                                    Id = subjectId, 
                                    Name = subjectNameMap[subjectId] 
                                })
                                .ToList();
                            
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
                else
                {
                    var exams = await _examRepository.GetAllIncludingInactiveAsync();
                    var dtos = exams.OrderBy(e => e.Name)
                        .Select(e => MapToExamDto(e))
                        .ToList();

                    // Populate QualificationIds and StreamIds for all exams
                    var subjectNameMap = await BuildSubjectNameMapAsync(exams.SelectMany(x => x.SubjectIds ?? new List<int>()));
                    foreach (var dto in dtos)
                    {
                        var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                        if (exam != null)
                        {
                            dto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                            dto.StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>();
                            dto.SubjectIds = exam.SubjectIds?.ToList() ?? new List<int>();
                            dto.SubjectNames = dto.SubjectIds
                                .Where(subjectId => subjectNameMap.ContainsKey(subjectId))
                                .Select(subjectId => new SubjectNameDto 
                                { 
                                    Id = subjectId, 
                                    Name = subjectNameMap[subjectId] 
                                })
                                .ToList();
                        }
                    }

                    return dtos;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all exams including inactive");
                throw;
            }
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

                var namesJson = LanguagePayloadSerializer.SerializeItems(
                    exam.ExamLanguages,
                    language => new { language.LanguageId, language.Name, language.Description });
                var relationsJson = LanguagePayloadSerializer.SerializeItems(
                    exam.ExamQualifications,
                    relation => new { relation.QualificationId, relation.StreamId });

                await _examRepository.UpdateAsync(exam, namesJson, relationsJson);
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

        private ExamDto MapToExamDto(Exam exam, string? language = null)
        {
            // Convert full URLs to relative paths
            var imageUrl = exam.ImageUrl;
            if (!string.IsNullOrEmpty(imageUrl) && (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")))
            {
                // Extract just the path from full URL
                var uri = new Uri(imageUrl);
                imageUrl = uri.PathAndQuery;
            }

            // Map exam languages to DTOs
            var names = exam.ExamLanguages?.Select(el => new ExamLanguageDto
            {
                LanguageId = el.LanguageId,
                LanguageCode = el.Language?.Code ?? string.Empty,
                LanguageName = el.Language?.Name ?? string.Empty,
                Name = el.Name,
                Description = el.Description
            }).ToList() ?? new List<ExamLanguageDto>();

            return new ExamDto
            {
                Id = exam.Id,
                Name = exam.Name,
                Description = exam.Description,
                CountryCode = exam.CountryCode,
                MinAge = exam.MinAge,
                MaxAge = exam.MaxAge,
                ImageUrl = imageUrl,
                IsInternational = exam.IsInternational,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt,
                QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>(),
                StreamIds = exam.ExamQualifications?.Select(eq => eq.StreamId).ToList() ?? new List<int?>(),
                SubjectIds = exam.SubjectIds?.ToList() ?? new List<int>(),
                SubjectNames = new List<SubjectNameDto>(),
                Names = names
            };
        }

        private async Task<List<SubjectNameDto>> ResolveSubjectNamesAsync(IEnumerable<int> subjectIds)
        {
            var subjectNameMap = await BuildSubjectNameMapAsync(subjectIds);
            return subjectIds
                .Where(subjectId => subjectNameMap.ContainsKey(subjectId))
                .Select(subjectId => new SubjectNameDto 
                { 
                    Id = subjectId, 
                    Name = subjectNameMap[subjectId] 
                })
                .ToList();
        }

        private async Task<Dictionary<int, string>> BuildSubjectNameMapAsync(IEnumerable<int> subjectIds)
        {
            var idSet = (subjectIds ?? Enumerable.Empty<int>()).Distinct().ToHashSet();
            if (idSet.Count == 0)
            {
                return new Dictionary<int, string>();
            }

            var subjects = await _subjectRepository.GetAllAsync();
            return subjects
                .Where(subject => idSet.Contains(subject.Id) && subject.IsActive)
                .ToDictionary(subject => subject.Id, subject => subject.Name);
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

                    // Convert full URLs to relative paths
                    if (!string.IsNullOrEmpty(imageUrl) && (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")))
                    {
                        var uri = new Uri(imageUrl);
                        imageUrl = uri.PathAndQuery;
                    }

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
                        IsActive = true,
                        Names = new List<ExamLanguageDto>() // Initialize empty names array
                    };
                }

                // Handle dynamic objects
                if (item is System.Dynamic.ExpandoObject expando)
                {
                    var dict = (IDictionary<string, object>)expando;
                    var imageUrl = dict.ContainsKey("imageUrl") ? dict["imageUrl"]?.ToString() : null;
                    
                    // Convert full URLs to relative paths
                    if (!string.IsNullOrEmpty(imageUrl) && (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")))
                    {
                        var uri = new Uri(imageUrl);
                        imageUrl = uri.PathAndQuery;
                    }
                    
                    return new ExamDto
                    {
                        Id = dict.ContainsKey("id") ? Convert.ToInt32(dict["id"]) : 0,
                        Name = dict.ContainsKey("name") ? dict["name"]?.ToString() : "",
                        Description = dict.ContainsKey("description") ? dict["description"]?.ToString() : null,
                        CountryCode = dict.ContainsKey("countryCode") ? dict["countryCode"]?.ToString() : null,
                        MinAge = dict.ContainsKey("minAge") ? Convert.ToInt32(dict["minAge"]) : (int?)null,
                        MaxAge = dict.ContainsKey("maxAge") ? Convert.ToInt32(dict["maxAge"]) : (int?)null,
                        ImageUrl = imageUrl,
                        IsInternational = dict.ContainsKey("isInternational") ? Convert.ToBoolean(dict["isInternational"]) : false,
                        IsActive = true,
                        Names = new List<ExamLanguageDto>() // Initialize empty names array
                    };
                }

                _logger.LogWarning("Unsupported exam item type: {ItemType}", item?.GetType());
                return new ExamDto { IsActive = true, Names = new List<ExamLanguageDto>() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping exam item: {Item}", item);
                return new ExamDto { IsActive = true, Names = new List<ExamLanguageDto>() };
            }
        }
    }
}
