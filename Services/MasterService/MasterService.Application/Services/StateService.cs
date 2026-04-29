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
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly ILanguageDataService _languageDataService;
        private readonly ILogger<StateService> _logger;

        public StateService(IStateRepository stateRepository, ICountryRepository countryRepository, IMapper mapper, ILanguageDataService languageDataService, ILogger<StateService> logger)
        {
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
            _languageDataService = languageDataService;
            _logger = logger;
        }

        public async Task<StateDto> CreateStateAsync(CreateStateDto createDto)
        {
            // Validate that at least one language name is provided
            if (createDto.Names == null || !createDto.Names.Any())
            {
                throw new ArgumentException("At least one language name must be provided for the state.");
            }

            // Validate country code exists
            if (string.IsNullOrWhiteSpace(createDto.CountryCode))
            {
                throw new ArgumentException("Country code is required.");
            }

            var country = await _countryRepository.GetByCodeAsync(createDto.CountryCode);
            if (country == null)
            {
                throw new ArgumentException($"Invalid country code: '{createDto.CountryCode}'. Country does not exist.");
            }

            if (!country.IsActive)
            {
                throw new ArgumentException($"Country '{createDto.CountryCode}' is not active.");
            }

            // Check for existing state with same code (case-insensitive)
            var existingStates = await _stateRepository.GetActiveAsync();
            var duplicateState = existingStates.FirstOrDefault(s => 
                s.Code != null && createDto.Code != null && s.Code.Equals(createDto.Code, StringComparison.OrdinalIgnoreCase));

            if (duplicateState != null)
            {
                throw new InvalidOperationException($"A state with code '{createDto.Code}' already exists.");
            }

            // Create state with the first language name as the primary name
            var primaryLanguage = createDto.Names.First();
            var state = new State
            {
                Name = primaryLanguage.Name,
                Code = createDto.Code,
                CountryCode = createDto.CountryCode,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Add all state languages
            foreach (var langDto in createDto.Names)
            {
                state.StateLanguages.Add(new StateLanguage
                {
                    LanguageId = langDto.LanguageId,
                    Name = langDto.Name,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            var namesJson = LanguagePayloadSerializer.SerializeItems(
                state.StateLanguages,
                language => new { language.LanguageId, language.Name });

            await _stateRepository.AddAsync(state, namesJson);
            await _stateRepository.SaveChangesAsync();

            return await GetStateByIdAsync(state.Id);
        }

        public async Task<StateDto?> UpdateStateAsync(int id, UpdateStateDto updateDto)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return null;

            // Validate country code exists if provided
            if (!string.IsNullOrWhiteSpace(updateDto.CountryCode))
            {
                var country = await _countryRepository.GetByCodeAsync(updateDto.CountryCode);
                if (country == null)
                {
                    throw new ArgumentException($"Invalid country code: '{updateDto.CountryCode}'. Country does not exist.");
                }

                if (!country.IsActive)
                {
                    throw new ArgumentException($"Country '{updateDto.CountryCode}' is not active.");
                }
            }

            // Check for existing state with same code (excluding current state)
            var existingStates = await _stateRepository.GetActiveAsync();
            var duplicateState = existingStates.FirstOrDefault(s => 
                s.Id != id && s.Code != null && updateDto.Code != null && 
                s.Code.Equals(updateDto.Code, StringComparison.OrdinalIgnoreCase));

            if (duplicateState != null)
            {
                throw new InvalidOperationException($"A state with code '{updateDto.Code}' already exists.");
            }

            // Update basic properties
            state.Code = updateDto.Code;
            state.CountryCode = updateDto.CountryCode;
            state.UpdatedAt = DateTime.UtcNow;

            // Update state languages if provided
            if (updateDto.Names != null && updateDto.Names.Any())
            {
                // Remove existing languages
                var existingLanguages = state.StateLanguages.ToList();
                foreach (var existingLang in existingLanguages)
                {
                    state.StateLanguages.Remove(existingLang);
                }

                // Update primary name to first language name
                var primaryLanguage = updateDto.Names.First();
                state.Name = primaryLanguage.Name;

                // Add new languages
                foreach (var langDto in updateDto.Names)
                {
                    state.StateLanguages.Add(new StateLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            var namesJson = LanguagePayloadSerializer.SerializeItems(
                state.StateLanguages,
                language => new { language.LanguageId, language.Name });

            await _stateRepository.UpdateAsync(state, namesJson);
            await _stateRepository.SaveChangesAsync();

            return await GetStateByIdAsync(state.Id);
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            return await _stateRepository.SoftDeleteByIdAsync(id);
        }

        public async Task<StateDto?> GetStateByIdAsync(int id, int? languageId = null)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return null;

            var stateDto = _mapper.Map<StateDto>(state);
            
            // Populate Names collection with all available languages
            stateDto.Names = state.StateLanguages?.Select(sl => new StateLanguageDto
            {
                LanguageId = sl.LanguageId,
                LanguageCode = sl.Language?.Code ?? string.Empty,
                LanguageName = sl.Language?.Name ?? string.Empty,
                Name = sl.Name
            }).ToList() ?? new List<StateLanguageDto>();
            
            // Set the primary name based on language preference
            if (languageId.HasValue)
            {
                var languageName = state.StateLanguages
                    .FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                
                if (!string.IsNullOrEmpty(languageName))
                    stateDto.Name = languageName;
            }

            return stateDto;
        }

        public async Task<IEnumerable<StateDto>> GetAllStatesAsync(int? languageId = null)
        {
            var states = await _stateRepository.GetActiveAsync();
            return MapStates(states, languageId);
        }

        public async Task<IEnumerable<StateDto>> GetAllStatesIncludingInactiveAsync(int? languageId = null)
        {
            var states = await _stateRepository.GetAllAsync();
            return MapStates(states, languageId);
        }

        public async Task<IEnumerable<StateDto>> GetAllStatesIncludingInactiveAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var states = await _stateRepository.GetAllAsync();
                return states
                    .OrderBy(s => s.Name)
                    .Select(s => MapToOptimizedStateDto(s, normalizedLanguage))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all states including inactive for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<StateDto>> GetStatesByCountryCodeIncludingInactiveAsync(string countryCode, int? languageId = null)
        {
            var states = await _stateRepository.GetAllAsync();
            return MapStates(states.Where(s => string.Equals(s.CountryCode, countryCode, StringComparison.OrdinalIgnoreCase)), languageId);
        }

        public async Task<IEnumerable<StateDto>> GetStatesByCountryCodeIncludingInactiveAsync(string countryCode, string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var states = await _stateRepository.GetAllAsync();
                return states
                    .Where(s => string.Equals(s.CountryCode, countryCode, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(s => s.Name)
                    .Select(s => MapToOptimizedStateDto(s, normalizedLanguage))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting states by country code including inactive for language {Language}", language);
                throw;
            }
        }

        private IEnumerable<StateDto> MapStates(IEnumerable<State> states, int? languageId)
        {
            var stateDtos = new List<StateDto>();

            foreach (var state in states.OrderBy(s => s.Name))
            {
                var stateDto = _mapper.Map<StateDto>(state);
                
                // Populate Names collection with all available languages
                stateDto.Names = state.StateLanguages?.Select(sl => new StateLanguageDto
                {
                    LanguageId = sl.LanguageId,
                    LanguageCode = sl.Language?.Code ?? string.Empty,
                    LanguageName = sl.Language?.Name ?? string.Empty,
                    Name = sl.Name
                }).ToList() ?? new List<StateLanguageDto>();
                
                // Set the primary name based on language preference
                if (languageId.HasValue)
                {
                    var languageName = state.StateLanguages
                        .FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                    
                    if (!string.IsNullOrEmpty(languageName))
                        stateDto.Name = languageName;
                }
                
                stateDtos.Add(stateDto);
            }

            return stateDtos;
        }

        public async Task<IEnumerable<StateDto>> GetAllStatesAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
                var states = await _stateRepository.GetActiveLocalizedAsync(normalizedLanguage);
                var stateList = states
                    .Select(s => MapToOptimizedStateDto(s, normalizedLanguage))
                    .ToList();

                return stateList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting states for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<StateDto>> GetStatesByCountryCodeAsync(string countryCode, string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
                var states = await _stateRepository.GetActiveByCountryCodeLocalizedAsync(countryCode, normalizedLanguage);
                var stateList = states
                    .Select(s => MapToOptimizedStateDto(s, normalizedLanguage))
                    .ToList();

                return stateList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting states by country code for language {Language}", language);
                throw;
            }
        }

        /// <summary>
        /// Maps State entity to optimized DTO with language support
        /// </summary>
        private StateDto MapToOptimizedStateDto(Domain.Entities.State state, string language)
        {
            var useHindi = language == LanguageConstants.Hindi;

            // For now, use the main Name field since State doesn't have NameEn/NameHi
            // This can be enhanced later to support NameEn/NameHi like Category
            var localizedName = state.Name;

            // Map StateLanguages to Names collection
            var names = state.StateLanguages?.Select(sl => new StateLanguageDto
            {
                LanguageId = sl.LanguageId,
                LanguageCode = sl.Language?.Code ?? string.Empty,
                LanguageName = sl.Language?.Name ?? string.Empty,
                Name = sl.Name
            }).ToList() ?? new List<StateLanguageDto>();

            return new StateDto
            {
                Id = state.Id,
                Name = localizedName,
                Code = state.Code,
                CountryCode = state.CountryCode,
                IsActive = state.IsActive,
                CreatedAt = state.CreatedAt,
                UpdatedAt = state.UpdatedAt,
                Names = names
            };
        }

        /// <summary>
        /// Gets default states from language data service
        /// Used when database has no states
        /// </summary>
        private async Task<IEnumerable<StateDto>> GetDefaultStatesOptimized(string language)
        {
            try
            {
                var data = await _languageDataService.GetLocalizedDataAsync(language, "states");
                
                if (data.TryGetValue("states", out var statesData) && statesData is IEnumerable<object> items)
                {
                    return items.Select(item => MapToOptimizedStateDto(item, language)).ToList();
                }

                return new List<StateDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default states for language {Language}", language);
                return new List<StateDto>();
            }
        }

        /// <summary>
        /// Maps object to optimized State DTO (for language data service)
        /// </summary>
        private StateDto MapToOptimizedStateDto(object item, string language)
        {
            try
            {
                _logger.LogInformation("Mapping state item of type: {ItemType}", item?.GetType().Name);
                
                // Handle JsonElement from System.Text.Json
                if (item is System.Text.Json.JsonElement element)
                {
                    var id = element.GetProperty("id").GetInt32();
                    var name = element.GetProperty("name").GetString();
                    var code = element.TryGetProperty("code", out var codeProp) ? codeProp.GetString() : null;
                    var countryCode = element.TryGetProperty("countryCode", out var ccProp) ? ccProp.GetString() : null;

                    return new StateDto
                    {
                        Id = id,
                        Name = name,
                        Code = code,
                        CountryCode = countryCode,
                        IsActive = true
                    };
                }

                // Handle dynamic objects
                if (item is System.Dynamic.ExpandoObject expando)
                {
                    var dict = (IDictionary<string, object>)expando;
                    return new StateDto
                    {
                        Id = dict.ContainsKey("id") ? Convert.ToInt32(dict["id"]) : 0,
                        Name = dict.ContainsKey("name") ? dict["name"]?.ToString() : "",
                        Code = dict.ContainsKey("code") ? dict["code"]?.ToString() : null,
                        CountryCode = dict.ContainsKey("countryCode") ? dict["countryCode"]?.ToString() : null,
                        IsActive = true
                    };
                }

                _logger.LogWarning("Unsupported state item type: {ItemType}", item?.GetType().Name);
                return new StateDto { IsActive = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping state item: {Item}", item);
                return new StateDto { IsActive = true };
            }
        }

        public async Task<IEnumerable<StateDto>> GetStatesByCountryCodeAsync(string countryCode, int? languageId = null)
        {
            var states = await _stateRepository.GetActiveByCountryCodeAsync(countryCode);
            return MapStates(states, languageId);
        }

        public async Task<bool> ToggleStateStatusAsync(int id, bool isActive)
        {
            return await _stateRepository.SetActiveAsync(id, isActive);
        }

        public async Task SeedStateLanguagesAsync()
        {
            // For now, just return - the actual seeding will be done via database migration
            // or a separate seeding service to avoid circular dependencies
            await Task.CompletedTask;
        }

        public async Task<int> DeleteStatesWithEmptyNamesAsync()
        {
            var statesToDelete = await _stateRepository.GetStatesWithEmptyNamesAsync();
            var deletedCount = 0;

            foreach (var state in statesToDelete)
            {
                await _stateRepository.DeleteAsync(state);
                deletedCount++;
            }

            await _stateRepository.SaveChangesAsync();
            return deletedCount;
        }
    }
}
