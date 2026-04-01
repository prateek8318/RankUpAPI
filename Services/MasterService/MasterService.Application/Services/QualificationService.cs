using AutoMapper;
using Common.Language;
using Common.DTOs;
using MasterService.Application.DTOs;
using MasterService.Application.Helpers;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.Application.Services
{
    public class QualificationService : BaseService, IQualificationService
    {
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IMapper _mapper;
        private readonly ILanguageDataService _languageDataService;

        public QualificationService(
            IQualificationRepository qualificationRepository, 
            IMapper mapper,
            ILanguageDataService languageDataService,
            ILogger<QualificationService> logger) : base(logger)
        {
            _qualificationRepository = qualificationRepository;
            _mapper = mapper;
            _languageDataService = languageDataService;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationDto createDto)
        {
            return await ExecuteInTransactionAsync(
                _qualificationRepository,
                async (connection, transaction) =>
                {
                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(createDto.Name))
                    {
                        throw new ArgumentException("Qualification name is required and cannot be empty.");
                    }

                    var qualification = _mapper.Map<Qualification>(createDto);
                    qualification.CreatedAt = DateTime.UtcNow;
                    qualification.IsActive = true;
                    qualification.Description = createDto.Description;
                    qualification.NameHi = createDto.NameHi;

                    if (createDto.Names != null && createDto.Names.Any())
                    {
                        foreach (var langDto in createDto.Names)
                        {
                            qualification.QualificationLanguages.Add(new QualificationLanguage
                            {
                                LanguageId = langDto.LanguageId,
                                Name = langDto.Name,
                                Description = langDto.Description,
                                CreatedAt = DateTime.UtcNow,
                                IsActive = true
                            });
                        }
                    }

                    var namesJson = LanguagePayloadSerializer.SerializeNames(
                        createDto.Names,
                        lang => new { lang.LanguageId, lang.Name, lang.Description });

                    await _qualificationRepository.AddAsync(qualification, namesJson);
                    await _qualificationRepository.SaveChangesAsync();
                    return (await GetQualificationByIdAsync(qualification.Id))!;
                },
                "CreateQualification");
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto)
        {
            return await ExecuteInTransactionAsync(
                _qualificationRepository,
                async (connection, transaction) =>
                {
                    var qualification = await _qualificationRepository.GetByIdAsync(id);
                    if (qualification == null)
                        return null;

                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(updateDto.Name))
                    {
                        throw new ArgumentException("Qualification name is required and cannot be empty.");
                    }

                    qualification.Name = updateDto.Name;
                    qualification.Description = updateDto.Description;
                    qualification.CountryCode = updateDto.CountryCode;
                    qualification.NameHi = updateDto.NameHi;
                    qualification.UpdatedAt = DateTime.UtcNow;

                    if (updateDto.Names != null && updateDto.Names.Any())
                    {
                        var existingLanguages = qualification.QualificationLanguages.ToList();
                        foreach (var existingLang in existingLanguages)
                        {
                            qualification.QualificationLanguages.Remove(existingLang);
                        }
                        foreach (var langDto in updateDto.Names)
                        {
                            qualification.QualificationLanguages.Add(new QualificationLanguage
                            {
                                LanguageId = langDto.LanguageId,
                                Name = langDto.Name,
                                Description = langDto.Description,
                                CreatedAt = DateTime.UtcNow,
                                IsActive = true
                            });
                        }
                    }

                    var namesJson = LanguagePayloadSerializer.SerializeNames(
                        updateDto.Names,
                        lang => new { lang.LanguageId, lang.Name, lang.Description });

                    await _qualificationRepository.UpdateAsync(qualification, namesJson);
                    await _qualificationRepository.SaveChangesAsync();
                    return await GetQualificationByIdAsync(qualification.Id);
                },
                "UpdateQualification");
        }

        public async Task<bool> DeleteQualificationAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return false;
            
            return await _qualificationRepository.HardDeleteByIdAsync(id);
        }

        public async Task<QualificationDto?> GetQualificationByIdAsync(int id, int? languageId = null)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return null;

            var dto = _mapper.Map<QualificationDto>(qualification);
            dto.NameHi = qualification.NameHi;
            
            // Map QualificationLanguages to Names collection
            dto.Names = qualification.QualificationLanguages?.Select(ql => new QualificationLanguageDto
            {
                LanguageId = ql.LanguageId,
                LanguageCode = ql.Language?.Code ?? string.Empty,
                LanguageName = ql.Language?.Name ?? string.Empty,
                Name = ql.Name,
                Description = ql.Description
            }).ToList() ?? new List<QualificationLanguageDto>();
            
            if (languageId.HasValue)
            {
                var langName = qualification.QualificationLanguages
                    .FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                if (!string.IsNullOrEmpty(langName) && IsValidQualificationName(langName))
                    dto.Name = langName;
                var langDesc = qualification.QualificationLanguages
                    .FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                if (langDesc != null && IsValidQualificationName(langDesc))
                    dto.Description = langDesc;
            }
            else
            {
                // Use NameHi for Hindi if available, fallback to Name
                if (!string.IsNullOrWhiteSpace(qualification.NameHi))
                {
                    dto.Name = qualification.NameHi;
                }
            }
            
            return dto;
        }

        public async Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync(string language)
        {
            try
            {
                var qualifications = await _qualificationRepository.GetActiveAsync();
                var dtos = qualifications.OrderBy(q => q.Name).Select(q => _mapper.Map<QualificationDto>(q)).ToList();
                
                // Populate Names array and apply language filtering
                foreach (var dto in dtos)
                {
                    var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                    if (q != null)
                    {
                        // Map QualificationLanguages to Names collection
                        dto.Names = q.QualificationLanguages?.Select(ql => new QualificationLanguageDto
                        {
                            LanguageId = ql.LanguageId,
                            LanguageCode = ql.Language?.Code ?? string.Empty,
                            LanguageName = ql.Language?.Name ?? string.Empty,
                            Name = ql.Name,
                            Description = ql.Description
                        }).ToList() ?? new List<QualificationLanguageDto>();
                        
                        // Apply language filtering to name and description
                        if (language.ToLower() == "hi")
                        {
                            if (!string.IsNullOrEmpty(q.NameHi))
                            {
                                dto.Name = q.NameHi;
                            }
                            // Check QualificationLanguages for Hindi description
                            var hindiDesc = q.QualificationLanguages?.FirstOrDefault(ql => ql.LanguageId == 49)?.Description;
                            if (!string.IsNullOrEmpty(hindiDesc))
                            {
                                dto.Description = hindiDesc;
                            }
                        }
                        else
                        {
                            // Default to English
                            dto.Name = q.Name;
                            // Check QualificationLanguages for English description
                            var englishDesc = q.QualificationLanguages?.FirstOrDefault(ql => ql.LanguageId == 50)?.Description;
                            if (!string.IsNullOrEmpty(englishDesc))
                            {
                                dto.Description = englishDesc;
                            }
                        }
                    }
                }
                
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting qualifications for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync(int? languageId = null)
        {
            var qualifications = await _qualificationRepository.GetActiveAsync();
            var dtos = qualifications.OrderBy(q => q.Name).Select(q => _mapper.Map<QualificationDto>(q)).ToList();
            
            // Populate Names array for all qualifications
            foreach (var dto in dtos)
            {
                var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                if (q != null)
                {
                    // Map QualificationLanguages to Names collection
                    dto.Names = q.QualificationLanguages?.Select(ql => new QualificationLanguageDto
                    {
                        LanguageId = ql.LanguageId,
                        LanguageCode = ql.Language?.Code ?? string.Empty,
                        LanguageName = ql.Language?.Name ?? string.Empty,
                        Name = ql.Name,
                        Description = ql.Description
                    }).ToList() ?? new List<QualificationLanguageDto>();
                    
                    if (languageId.HasValue)
                    {
                        var langName = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName) && IsValidQualificationName(langName))
                            dto.Name = langName;
                        var langDesc = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                        if (langDesc != null && IsValidQualificationName(langDesc))
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
        }

        public async Task<IEnumerable<QualificationDto>> GetQualificationsByCountryCodeAsync(string countryCode, int? languageId = null)
        {
            var qualifications = await _qualificationRepository.GetActiveByCountryCodeAsync(countryCode);
            var dtos = qualifications.OrderBy(q => q.Name).Select(q => _mapper.Map<QualificationDto>(q)).ToList();
            
            // Populate Names array for all qualifications
            foreach (var dto in dtos)
            {
                var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                if (q != null)
                {
                    // Map QualificationLanguages to Names collection
                    dto.Names = q.QualificationLanguages?.Select(ql => new QualificationLanguageDto
                    {
                        LanguageId = ql.LanguageId,
                        LanguageCode = ql.Language?.Code ?? string.Empty,
                        LanguageName = ql.Language?.Name ?? string.Empty,
                        Name = ql.Name,
                        Description = ql.Description
                    }).ToList() ?? new List<QualificationLanguageDto>();
                    
                    if (languageId.HasValue)
                    {
                        var langName = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName) && IsValidQualificationName(langName))
                            dto.Name = langName;
                        var langDesc = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                        if (langDesc != null && IsValidQualificationName(langDesc))
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
        }

        public async Task<IEnumerable<QualificationDto>> GetQualificationsByCountryCodeAsync(string countryCode, string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var qualifications = await _qualificationRepository.GetActiveByCountryCodeAsync(countryCode);
                var list = qualifications.Select(q => MapToOptimizedQualificationDtoFromEntity(q, normalizedLanguage)).ToList();
                
                // Apply language filtering
                foreach (var dto in list)
                {
                    var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                    if (q != null)
                    {
                        if (language.ToLower() == "hi")
                        {
                            if (!string.IsNullOrEmpty(q.NameHi))
                            {
                                dto.Name = q.NameHi;
                            }
                            var hindiDesc = q.QualificationLanguages?.FirstOrDefault(ql => ql.LanguageId == 49)?.Description;
                            if (!string.IsNullOrEmpty(hindiDesc))
                            {
                                dto.Description = hindiDesc;
                            }
                        }
                        else
                        {
                            dto.Name = q.Name;
                            var englishDesc = q.QualificationLanguages?.FirstOrDefault(ql => ql.LanguageId == 50)?.Description;
                            if (!string.IsNullOrEmpty(englishDesc))
                            {
                                dto.Description = englishDesc;
                            }
                        }
                    }
                }
                
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting qualifications by country code for language {Language}", language);
                throw;
            }
        }

        private static bool IsValidQualificationName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
                
            // Check if the name looks like a language name rather than a qualification name
            var invalidNames = new[] { "English", "अंग्रेज़ी", "Hindi", "हिंदी", "English / अंग्रेज़ी", "अंग्रेज़ी / English" };
            
            return !invalidNames.Contains(name.Trim()) && 
                   !name.Contains("English") && 
                   !name.Contains("अंग्रेज़ी") &&
                   !name.Contains("Hindi") &&
                   !name.Contains("हिंदी");
        }

        public async Task<bool> ToggleQualificationStatusAsync(int id, bool isActive)
        {
            return await _qualificationRepository.SetActiveAsync(id, isActive);
        }

        // Pagination support methods
        public async Task<PaginatedResponse<QualificationDto>> GetAllQualificationsPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedQualifications = await _qualificationRepository.GetAllAsync(pagination);
                var qualificationDtos = paginatedQualifications.Data.Select(q => _mapper.Map<QualificationDto>(q));
                
                return new PaginatedResponse<QualificationDto>
                {
                    Data = qualificationDtos,
                    TotalCount = paginatedQualifications.TotalCount,
                    PageNumber = paginatedQualifications.PageNumber,
                    PageSize = paginatedQualifications.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated qualifications");
                throw;
            }
        }

        public async Task<PaginatedResponse<QualificationDto>> GetActiveQualificationsPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedQualifications = await _qualificationRepository.GetActiveAsync(pagination);
                var qualificationDtos = paginatedQualifications.Data.Select(q => _mapper.Map<QualificationDto>(q));
                
                return new PaginatedResponse<QualificationDto>
                {
                    Data = qualificationDtos,
                    TotalCount = paginatedQualifications.TotalCount,
                    PageNumber = paginatedQualifications.PageNumber,
                    PageSize = paginatedQualifications.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated active qualifications");
                throw;
            }
        }

        private QualificationDto MapToOptimizedQualificationDtoFromEntity(Domain.Entities.Qualification qualification, string language)
        {
            var useHindi = language == LanguageConstants.Hindi;

            // Use NameHi for Hindi, Name for English
            var localizedName = useHindi && !string.IsNullOrWhiteSpace(qualification.NameHi)
                ? qualification.NameHi!
                : qualification.Name;

            // Parse Names JSON if available
            var names = new List<QualificationLanguageDto>();
            if (!string.IsNullOrEmpty(qualification.Names))
            {
                try
                {
                    using var document = System.Text.Json.JsonDocument.Parse(qualification.Names);
                    if (document.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var element in document.RootElement.EnumerateArray())
                        {
                            var nameDto = new QualificationLanguageDto
                            {
                                LanguageId = element.TryGetProperty("LanguageId", out var langIdProp) ? langIdProp.GetInt32() : 0,
                                LanguageCode = element.TryGetProperty("LanguageCode", out var langCodeProp) ? langCodeProp.GetString() ?? "" : "",
                                LanguageName = element.TryGetProperty("LanguageName", out var langNameProp) ? langNameProp.GetString() ?? "" : "",
                                Name = element.TryGetProperty("Name", out var nameProp) ? nameProp.GetString() ?? "" : "",
                                Description = element.TryGetProperty("Description", out var descProp) ? descProp.GetString() : null
                            };
                            names.Add(nameDto);
                        }
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to parse Names JSON for qualification {QualificationId}", qualification.Id);
                }
            }

            // Fallback to QualificationLanguages if Names JSON is empty
            if (!names.Any() && qualification.QualificationLanguages != null)
            {
                names = qualification.QualificationLanguages.Select(ql => new QualificationLanguageDto
                {
                    LanguageId = ql.LanguageId,
                    LanguageCode = ql.Language?.Code ?? string.Empty,
                    LanguageName = ql.Language?.Name ?? string.Empty,
                    Name = ql.Name,
                    Description = ql.Description
                }).ToList();
            }

            return new QualificationDto
            {
                Id = qualification.Id,
                Name = localizedName,
                NameHi = qualification.NameHi,
                Description = qualification.Description,
                CountryCode = qualification.CountryCode,
                IsActive = qualification.IsActive,
                CreatedAt = qualification.CreatedAt,
                UpdatedAt = qualification.UpdatedAt,
                Names = names
            };
        }

        private async Task<IEnumerable<QualificationDto>> GetDefaultQualificationsOptimized(string language)
        {
            try
            {
                var data = await _languageDataService.GetLocalizedDataAsync(language, "qualifications");
                if (data.TryGetValue("qualifications", out var qualificationsData) && qualificationsData is IEnumerable<object> items)
                {
                    return items.Select(item => MapToOptimizedQualificationDto(item, language)).ToList();
                }

                return new List<QualificationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default qualifications for language {Language}", language);
                return new List<QualificationDto>();
            }
        }

        private QualificationDto MapToOptimizedQualificationDto(object item, string language)
        {
            try
            {
                _logger.LogInformation("Mapping qualification item of type: {ItemType}", item?.GetType().Name);
                
                // Handle anonymous types using reflection
                if (item != null && item.GetType().Name.StartsWith("<>f__AnonymousType"))
                {
                    var itemType = item.GetType();
                    var properties = itemType.GetProperties();
                    
                    var id = 0;
                    var name = "";
                    var description = (string?)null;
                    var countryCode = (string?)null;
                    var createdAt = DateTime.UtcNow;
                    
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item);
                        switch (prop.Name.ToLower())
                        {
                            case "id":
                                if (value != null) id = Convert.ToInt32(value);
                                break;
                            case "name":
                                name = value?.ToString() ?? "";
                                break;
                            case "description":
                                description = value?.ToString();
                                break;
                            case "countrycode":
                                countryCode = value?.ToString();
                                break;
                            case "createdat":
                                if (value != null) createdAt = Convert.ToDateTime(value);
                                break;
                        }
                    }
                    
                    return new QualificationDto
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                        CountryCode = countryCode,
                        IsActive = true,
                        CreatedAt = createdAt
                    };
                }
                
                // Handle JsonElement from System.Text.Json
                if (item is System.Text.Json.JsonElement element)
                {
                    var id = element.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0;
                    var name = element.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
                    var description = element.TryGetProperty("description", out var descProp) ? descProp.GetString() : null;
                    var countryCode = element.TryGetProperty("countryCode", out var ccProp) ? ccProp.GetString() : null;
                    var createdAt = element.TryGetProperty("createdAt", out var createdProp) ? createdProp.GetDateTime() : DateTime.UtcNow;

                    return new QualificationDto
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                        CountryCode = countryCode,
                        IsActive = true,
                        CreatedAt = createdAt
                    };
                }

                // Handle dynamic objects
                if (item is System.Dynamic.ExpandoObject expando)
                {
                    var dict = (IDictionary<string, object>)expando;
                    return new QualificationDto
                    {
                        Id = dict.ContainsKey("id") ? Convert.ToInt32(dict["id"]) : 0,
                        Name = dict.ContainsKey("name") ? dict["name"]?.ToString() ?? "" : "",
                        Description = dict.ContainsKey("description") ? dict["description"]?.ToString() : null,
                        CountryCode = dict.ContainsKey("countryCode") ? dict["countryCode"]?.ToString() : null,
                        IsActive = true,
                        CreatedAt = dict.ContainsKey("createdAt") ? Convert.ToDateTime(dict["createdAt"]) : DateTime.UtcNow
                    };
                }

                _logger.LogWarning("Unsupported qualification item type: {ItemType}", item?.GetType());
                return new QualificationDto { IsActive = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping qualification item: {Item}", item);
                return new QualificationDto { IsActive = true };
            }
        }
    }
}
