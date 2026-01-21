using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Application.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public StateService(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
        }

        public async Task<StateDto> CreateStateAsync(CreateStateDto createDto)
        {
            var state = _mapper.Map<State>(createDto);
            state.CreatedAt = DateTime.UtcNow;
            state.IsActive = true;

            // Add state languages if provided
            if (createDto.Names != null && createDto.Names.Any())
            {
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
            }

            await _stateRepository.AddAsync(state);
            await _stateRepository.SaveChangesAsync();

            return await GetStateByIdAsync(state.Id);
        }

        public async Task<StateDto?> UpdateStateAsync(int id, UpdateStateDto updateDto)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return null;

            _mapper.Map(updateDto, state);
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

            await _stateRepository.UpdateAsync(state);
            await _stateRepository.SaveChangesAsync();

            return await GetStateByIdAsync(state.Id);
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return false;

            await _stateRepository.DeleteAsync(state);
            await _stateRepository.SaveChangesAsync();
            return true;
        }

        public async Task<StateDto?> GetStateByIdAsync(int id, int? languageId = null)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return null;

            var stateDto = _mapper.Map<StateDto>(state);
            
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
            var stateDtos = states.OrderBy(s => s.Name).Select(s => _mapper.Map<StateDto>(s)).ToList();

            // Set names based on language preference
            if (languageId.HasValue)
            {
                foreach (var stateDto in stateDtos)
                {
                    var state = states.FirstOrDefault(s => s.Id == stateDto.Id);
                    if (state != null)
                    {
                        var languageName = state.StateLanguages
                            .FirstOrDefault(sl => sl.LanguageId == languageId.Value && sl.IsActive)?.Name;
                        
                        if (!string.IsNullOrEmpty(languageName))
                            stateDto.Name = languageName;
                    }
                }
            }

            return stateDtos;
        }

        public async Task<bool> ToggleStateStatusAsync(int id, bool isActive)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return false;

            state.IsActive = isActive;
            state.UpdatedAt = DateTime.UtcNow;
            await _stateRepository.UpdateAsync(state);
            await _stateRepository.SaveChangesAsync();
            return true;
        }

        public async Task SeedStateLanguagesAsync()
        {
            // For now, just return - the actual seeding will be done via database migration
            // or a separate seeding service to avoid circular dependencies
            await Task.CompletedTask;
        }
    }
}
