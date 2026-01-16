using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Application.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;

        public LanguageService(ILanguageRepository languageRepository, IMapper mapper)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
        }

        public async Task<LanguageDto> CreateLanguageAsync(CreateLanguageDto createDto)
        {
            var language = _mapper.Map<Language>(createDto);
            language.CreatedAt = DateTime.UtcNow;
            language.IsActive = true;

            await _languageRepository.AddAsync(language);
            await _languageRepository.SaveChangesAsync();

            return _mapper.Map<LanguageDto>(language);
        }

        public async Task<LanguageDto?> UpdateLanguageAsync(int id, UpdateLanguageDto updateDto)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
                return null;

            _mapper.Map(updateDto, language);
            language.UpdatedAt = DateTime.UtcNow;
            await _languageRepository.UpdateAsync(language);
            await _languageRepository.SaveChangesAsync();

            return _mapper.Map<LanguageDto>(language);
        }

        public async Task<bool> DeleteLanguageAsync(int id)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
                return false;

            await _languageRepository.DeleteAsync(language);
            await _languageRepository.SaveChangesAsync();
            return true;
        }

        public async Task<LanguageDto?> GetLanguageByIdAsync(int id)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            return language == null ? null : _mapper.Map<LanguageDto>(language);
        }

        public async Task<IEnumerable<LanguageDto>> GetAllLanguagesAsync()
        {
            var languages = await _languageRepository.GetActiveAsync();
            return languages.OrderBy(l => l.Name).Select(l => _mapper.Map<LanguageDto>(l));
        }

        public async Task<bool> ToggleLanguageStatusAsync(int id, bool isActive)
        {
            var language = await _languageRepository.GetByIdAsync(id);
            if (language == null)
                return false;

            language.IsActive = isActive;
            language.UpdatedAt = DateTime.UtcNow;
            await _languageRepository.UpdateAsync(language);
            await _languageRepository.SaveChangesAsync();
            return true;
        }
    }
}
