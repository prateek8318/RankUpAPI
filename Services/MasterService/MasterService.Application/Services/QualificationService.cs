using AutoMapper;
using Common.Language;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.Application.Services
{
    public class QualificationService : IQualificationService
    {
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IMapper _mapper;
        private readonly ILanguageDataService _languageDataService;
        private readonly ILogger<QualificationService> _logger;

        public QualificationService(
            IQualificationRepository qualificationRepository, 
            IMapper mapper,
            ILanguageDataService languageDataService,
            ILogger<QualificationService> logger)
        {
            _qualificationRepository = qualificationRepository;
            _mapper = mapper;
            _languageDataService = languageDataService;
            _logger = logger;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationDto createDto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                throw new ArgumentException("Qualification name is required and cannot be empty.");
            }

            var qualification = _mapper.Map<Qualification>(createDto);
            qualification.CreatedAt = DateTime.UtcNow;
            qualification.IsActive = true;

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

            await _qualificationRepository.AddAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return (await GetQualificationByIdAsync(qualification.Id))!;
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto)
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

            await _qualificationRepository.UpdateAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return await GetQualificationByIdAsync(qualification.Id);
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
            
            if (languageId.HasValue)
            {
                var langName = qualification.QualificationLanguages
                    .FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                if (!string.IsNullOrEmpty(langName))
                    dto.Name = langName;
                var langDesc = qualification.QualificationLanguages
                    .FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                if (langDesc != null)
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
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
                var qualifications = await _qualificationRepository.GetActiveLocalizedAsync(normalizedLanguage);
                var qualificationList = qualifications
                    .Select(q => MapToOptimizedQualificationDto(q, normalizedLanguage))
                    .ToList();

                if (!qualificationList.Any())
                {
                    return await GetDefaultQualificationsOptimized(normalizedLanguage);
                }

                return qualificationList;
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
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                    if (q != null)
                    {
                        var langName = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                        if (langDesc != null)
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
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                    if (q != null)
                    {
                        var langName = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                        if (langDesc != null)
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
                var qualifications = await _qualificationRepository.GetActiveByCountryCodeLocalizedAsync(countryCode, normalizedLanguage);
                var list = qualifications.Select(q => MapToOptimizedQualificationDto(q, normalizedLanguage)).ToList();
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting qualifications by country code for language {Language}", language);
                throw;
            }
        }

        public async Task<bool> ToggleQualificationStatusAsync(int id, bool isActive)
        {
            return await _qualificationRepository.SetActiveAsync(id, isActive);
        }

        private QualificationDto MapToOptimizedQualificationDto(Domain.Entities.Qualification qualification, string language)
        {
            var useHindi = language == LanguageConstants.Hindi;

            // Use NameHi for Hindi, Name for English
            var localizedName = useHindi && !string.IsNullOrWhiteSpace(qualification.NameHi)
                ? qualification.NameHi!
                : qualification.Name;

            // Map QualificationLanguages to Names collection
            var names = qualification.QualificationLanguages?.Select(ql => new QualificationLanguageDto
            {
                LanguageId = ql.LanguageId,
                LanguageCode = ql.Language?.Code ?? string.Empty,
                LanguageName = ql.Language?.Name ?? string.Empty,
                Name = ql.Name,
                Description = ql.Description
            }).ToList() ?? new List<QualificationLanguageDto>();

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
                
                // Handle JsonElement from System.Text.Json
                if (item is System.Text.Json.JsonElement element)
                {
                    var id = element.GetProperty("id").GetInt32();
                    var name = element.GetProperty("name").GetString();
                    var description = element.TryGetProperty("description", out var descProp) ? descProp.GetString() : null;
                    var countryCode = element.TryGetProperty("countryCode", out var ccProp) ? ccProp.GetString() : null;

                    return new QualificationDto
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                        CountryCode = countryCode,
                        IsActive = true
                    };
                }

                // Handle dynamic objects
                if (item is System.Dynamic.ExpandoObject expando)
                {
                    var dict = (IDictionary<string, object>)expando;
                    return new QualificationDto
                    {
                        Id = dict.ContainsKey("id") ? Convert.ToInt32(dict["id"]) : 0,
                        Name = dict.ContainsKey("name") ? dict["name"]?.ToString() : "",
                        Description = dict.ContainsKey("description") ? dict["description"]?.ToString() : null,
                        CountryCode = dict.ContainsKey("countryCode") ? dict["countryCode"]?.ToString() : null,
                        IsActive = true
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
