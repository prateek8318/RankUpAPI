using AutoMapper;
using Common.Language;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using StreamEntity = MasterService.Domain.Entities.Stream;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.Application.Services
{
    public class StreamService : IStreamService
    {
        private readonly IStreamRepository _streamRepository;
        private readonly IMapper _mapper;
        private readonly ILanguageDataService _languageDataService;
        private readonly ILogger<StreamService> _logger;

        public StreamService(IStreamRepository streamRepository, IMapper mapper, ILanguageDataService languageDataService, ILogger<StreamService> logger)
        {
            _streamRepository = streamRepository;
            _mapper = mapper;
            _languageDataService = languageDataService;
            _logger = logger;
        }

        public async Task<StreamDto> CreateStreamAsync(CreateStreamDto createDto)
        {
            var stream = _mapper.Map<StreamEntity>(createDto);
            stream.CreatedAt = DateTime.UtcNow;
            stream.IsActive = true;

            if (createDto.Names != null && createDto.Names.Any())
            {
                foreach (var langDto in createDto.Names)
                {
                    stream.StreamLanguages.Add(new StreamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _streamRepository.AddAsync(stream);
            await _streamRepository.SaveChangesAsync();
            return (await GetStreamByIdAsync(stream.Id))!;
        }

        public async Task<StreamDto?> UpdateStreamAsync(int id, UpdateStreamDto updateDto)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            if (stream == null)
                return null;

            stream.Name = updateDto.Name;
            stream.Description = updateDto.Description;
            stream.QualificationId = updateDto.QualificationId;
            stream.UpdatedAt = DateTime.UtcNow;

            if (updateDto.Names != null && updateDto.Names.Any())
            {
                var existingLanguages = stream.StreamLanguages.ToList();
                foreach (var existingLang in existingLanguages)
                {
                    stream.StreamLanguages.Remove(existingLang);
                }
                foreach (var langDto in updateDto.Names)
                {
                    stream.StreamLanguages.Add(new StreamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _streamRepository.UpdateAsync(stream);
            await _streamRepository.SaveChangesAsync();
            return await GetStreamByIdAsync(stream.Id);
        }

        public async Task<bool> DeleteStreamAsync(int id)
        {
            return await _streamRepository.SoftDeleteByIdAsync(id);
        }

        public async Task<StreamDto?> GetStreamByIdAsync(int id, int? languageId = null)
        {
            var stream = await _streamRepository.GetByIdAsync(id);
            if (stream == null)
                return null;

            var dto = _mapper.Map<StreamDto>(stream);
            dto.QualificationName = stream.Qualification?.Name;
            if (languageId.HasValue)
            {
                var langName = stream.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                if (!string.IsNullOrEmpty(langName))
                    dto.Name = langName;
                var langDesc = stream.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Description;
                if (langDesc != null)
                    dto.Description = langDesc;
            }
            return dto;
        }

        public async Task<IEnumerable<StreamDto>> GetAllStreamsAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
                var streams = await _streamRepository.GetActiveLocalizedAsync(normalizedLanguage);
                var streamList = streams
                    .Select(s => MapToOptimizedStreamDto(s, normalizedLanguage))
                    .ToList();

                if (!streamList.Any())
                {
                    return await GetDefaultStreamsOptimized(normalizedLanguage);
                }

                return streamList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting streams for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<StreamDto>> GetAllStreamsAsync(int? languageId = null)
        {
            var streams = await _streamRepository.GetActiveAsync();
            var dtos = streams.OrderBy(s => s.Name).Select(s =>
            {
                var dto = _mapper.Map<StreamDto>(s);
                dto.QualificationName = s.Qualification?.Name;
                return dto;
            }).ToList();
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var s = streams.FirstOrDefault(x => x.Id == dto.Id);
                    if (s != null)
                    {
                        var langName = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
        }

        public async Task<IEnumerable<StreamDto>> GetStreamsByQualificationIdAsync(int qualificationId, int? languageId = null)
        {
            var streams = await _streamRepository.GetActiveByQualificationIdAsync(qualificationId);
            var dtos = streams.OrderBy(s => s.Name).Select(s =>
            {
                var dto = _mapper.Map<StreamDto>(s);
                dto.QualificationName = s.Qualification?.Name;
                return dto;
            }).ToList();
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var s = streams.FirstOrDefault(x => x.Id == dto.Id);
                    if (s != null)
                    {
                        var langName = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = s.StreamLanguages.FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
        }

        public async Task<IEnumerable<StreamDto>> GetStreamsByQualificationIdAsync(int qualificationId, string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var streams = await _streamRepository.GetActiveByQualificationIdLocalizedAsync(qualificationId, normalizedLanguage);
                var list = streams.Select(s => MapToOptimizedStreamDto(s, normalizedLanguage)).ToList();
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting streams by qualification id for language {Language}", language);
                throw;
            }
        }

        public async Task<bool> ToggleStreamStatusAsync(int id, bool isActive)
        {
            return await _streamRepository.SetActiveAsync(id, isActive);
        }

        private StreamDto MapToOptimizedStreamDto(StreamEntity stream, string language)
        {
            var useHindi = language == LanguageConstants.Hindi;

            // Use NameHi for Hindi, fallback to Name for English
            var localizedName = useHindi && !string.IsNullOrWhiteSpace(stream.NameHi)
                ? stream.NameHi!
                : stream.Name;

            // Map StreamLanguages to Names collection
            var names = stream.StreamLanguages?.Select(sl => new StreamLanguageDto
            {
                LanguageId = sl.LanguageId,
                LanguageCode = sl.Language?.Code ?? string.Empty,
                LanguageName = sl.Language?.Name ?? string.Empty,
                Name = sl.Name,
                Description = sl.Description
            }).ToList() ?? new List<StreamLanguageDto>();

            return new StreamDto
            {
                Id = stream.Id,
                Name = localizedName,
                Description = stream.Description,
                QualificationId = stream.QualificationId,
                QualificationName = stream.Qualification?.Name,
                IsActive = stream.IsActive,
                CreatedAt = stream.CreatedAt,
                UpdatedAt = stream.UpdatedAt,
                Names = names
            };
        }

        private async Task<IEnumerable<StreamDto>> GetDefaultStreamsOptimized(string language)
        {
            try
            {
                var data = await _languageDataService.GetLocalizedDataAsync(language, "streams");
                if (data.TryGetValue("streams", out var streamsData) && streamsData is IEnumerable<object> items)
                {
                    return items.Select(item => MapToOptimizedStreamDto(item, language)).ToList();
                }

                return new List<StreamDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default streams for language {Language}", language);
                return new List<StreamDto>();
            }
        }

        private StreamDto MapToOptimizedStreamDto(object item, string language)
        {
            try
            {
                _logger.LogInformation("Mapping stream item of type: {ItemType}", item?.GetType().Name);
                
                // Handle JsonElement from System.Text.Json
                if (item is System.Text.Json.JsonElement element)
                {
                    var id = element.GetProperty("id").GetInt32();
                    var name = element.GetProperty("name").GetString();
                    var description = element.TryGetProperty("description", out var descProp) ? descProp.GetString() : null;
                    var qualificationId = element.TryGetProperty("qualificationId", out var qualProp) ? qualProp.GetInt32() : 0;

                    return new StreamDto
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                        QualificationId = qualificationId,
                        IsActive = true
                    };
                }

                // Handle dynamic objects
                if (item is System.Dynamic.ExpandoObject expando)
                {
                    var dict = (IDictionary<string, object>)expando;
                    return new StreamDto
                    {
                        Id = dict.ContainsKey("id") ? Convert.ToInt32(dict["id"]) : 0,
                        Name = dict.ContainsKey("name") ? dict["name"]?.ToString() : "",
                        Description = dict.ContainsKey("description") ? dict["description"]?.ToString() : null,
                        QualificationId = dict.ContainsKey("qualificationId") ? Convert.ToInt32(dict["qualificationId"]) : 0,
                        IsActive = true
                    };
                }

                _logger.LogWarning("Unsupported stream item type: {ItemType}", item?.GetType());
                return new StreamDto { IsActive = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping stream item: {Item}", item);
                return new StreamDto { IsActive = true };
            }
        }
    }
}
