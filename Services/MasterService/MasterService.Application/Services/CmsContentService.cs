using Common.Language;
using MasterService.Application.DTOs;
using MasterService.Application.Exceptions;
using MasterService.Application.Interfaces;
using MasterService.Domain;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MasterService.Application.Services
{
    public class CmsContentService : ICmsContentService
    {
        private readonly ICmsContentRepository _repository;
        private readonly ILogger<CmsContentService> _logger;

        public CmsContentService(ICmsContentRepository repository, ILogger<CmsContentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CmsContentDto?> GetByKeyAsync(string key, string language)
        {
            try
            {
                var keyTrimmed = key?.Trim() ?? "";
                if (!CmsContentKeys.IsValid(keyTrimmed))
                    throw new CmsContentKeyInvalidException(keyTrimmed, CmsContentKeys.InvalidKeyMessage(key));

                var normalizedLang = LanguageValidator.NormalizeLanguage(language);
                var entity = await _repository.GetByKeyAsync(keyTrimmed);
                if (entity == null || !entity.IsActive)
                    return null;

                return MapToDto(entity, normalizedLang, includeTranslations: false);
            }
            catch (CmsContentKeyInvalidException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CMS content for key {Key}", key);
                throw;
            }
        }

        public async Task<CmsContentDto?> GetByIdAsync(int id, string language)
        {
            try
            {
                var normalizedLang = LanguageValidator.NormalizeLanguage(language);
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null || !entity.IsActive)
                    return null;

                return MapToDto(entity, normalizedLang, includeTranslations: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CMS content for ID {Id}", id);
                throw;
            }
        }

        public IReadOnlyList<string> GetAllowedKeys() => CmsContentKeys.All;

        public async Task<IEnumerable<CmsContentDto>> GetAllAsync(string language)
        {
            try
            {
                var normalizedLang = LanguageValidator.NormalizeLanguage(language);
                var items = await _repository.GetActiveAsync();
                return items.Select(i => MapToDto(i, normalizedLang, includeTranslations: false)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all CMS contents");
                throw;
            }
        }

        public async Task<IEnumerable<CmsContentDto>> GetAllWithTranslationsAsync(string? language = null)
        {
            try
            {
                var normalizedLang = LanguageValidator.NormalizeLanguage(language ?? LanguageConstants.English);
                var items = await _repository.GetAllAsync();
                return items.Select(i => MapToDto(i, normalizedLang, includeTranslations: true)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all CMS contents for admin");
                throw;
            }
        }

        public async Task<CmsContentDto> CreateAsync(CreateCmsContentDto createDto)
        {
            var key = createDto.Key?.Trim() ?? "";
            if (!CmsContentKeys.IsValid(key))
                throw new CmsContentKeyInvalidException(key, CmsContentKeys.InvalidKeyMessage(createDto.Key));

            // Create translations list from direct properties if translations array is empty
            var translations = createDto.Translations.ToList();
            
            // Add English translation from direct properties if not already present
            if (!string.IsNullOrEmpty(createDto.TitleEn) || !string.IsNullOrEmpty(createDto.ContentEn))
            {
                var existingEn = translations.FirstOrDefault(t => 
                    string.Equals(t.LanguageCode, LanguageConstants.English, StringComparison.OrdinalIgnoreCase));
                
                if (existingEn == null)
                {
                    translations.Add(new CmsContentTranslationDto
                    {
                        LanguageCode = LanguageConstants.English,
                        Title = createDto.TitleEn ?? "",
                        Content = createDto.ContentEn ?? ""
                    });
                }
            }

            // Add Hindi translation from direct properties if not already present
            if (!string.IsNullOrEmpty(createDto.TitleHi) || !string.IsNullOrEmpty(createDto.ContentHi))
            {
                var existingHi = translations.FirstOrDefault(t => 
                    string.Equals(t.LanguageCode, LanguageConstants.Hindi, StringComparison.OrdinalIgnoreCase));
                
                if (existingHi == null)
                {
                    translations.Add(new CmsContentTranslationDto
                    {
                        LanguageCode = LanguageConstants.Hindi,
                        Title = createDto.TitleHi ?? "",
                        Content = createDto.ContentHi ?? ""
                    });
                }
            }

            // Validate that at least one translation exists
            if (translations.Count == 0)
            {
                throw new InvalidOperationException("At least one translation is required. Provide either Translations array or direct language properties (TitleEn, ContentEn, etc.).");
            }

            var enTranslation = translations.FirstOrDefault(t =>
                string.Equals(t.LanguageCode, LanguageConstants.English, StringComparison.OrdinalIgnoreCase));
            if (enTranslation == null)
                throw new InvalidOperationException("At least one 'en' (English) translation is required.");

            // Validate that English translation has content
            if (string.IsNullOrWhiteSpace(enTranslation.Title) && string.IsNullOrWhiteSpace(enTranslation.Content))
            {
                throw new InvalidOperationException("English translation must have either Title or Content.");
            }

            return await _repository.ExecuteInTransactionAsync(async () =>
            {
                // Double-check existence within transaction
                var alreadyExists = await _repository.ExistsByKeyAsync(key);
                if (alreadyExists)
                    throw new CmsContentKeyAlreadyDefinedException(key, CmsContentKeys.AlreadyDefinedMessage(key));

                var entity = new CmsContent
                {
                    Key = key,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var t in translations)
                {
                    var code = t.LanguageCode?.Trim().ToLowerInvariant() ?? "";
                    if (string.IsNullOrEmpty(code) || !LanguageConstants.SupportedLanguages.Contains(code))
                    {
                        _logger.LogWarning("Skipping translation for unsupported language code: {LanguageCode}", code);
                        continue;
                    }

                    // Skip empty translations
                    if (string.IsNullOrWhiteSpace(t.Title) && string.IsNullOrWhiteSpace(t.Content))
                    {
                        _logger.LogWarning("Skipping empty translation for language: {LanguageCode}", code);
                        continue;
                    }

                    entity.Translations.Add(new CmsContentTranslation
                    {
                        LanguageCode = code,
                        Title = t.Title ?? "",
                        Content = t.Content ?? ""
                    });
                }

                if (entity.Translations.Count == 0)
                {
                    throw new InvalidOperationException("At least one valid translation with content is required.");
                }

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return MapToDto(entity, LanguageConstants.English, includeTranslations: true);
            });
        }

        public async Task<CmsContentDto?> UpdateAsync(int id, UpdateCmsContentDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return null;

            var key = updateDto.Key?.Trim() ?? "";
            if (!CmsContentKeys.IsValid(key))
                throw new CmsContentKeyInvalidException(key, CmsContentKeys.InvalidKeyMessage(updateDto.Key));

            var keyTakenByOther = await _repository.ExistsByKeyExceptIdAsync(key, id);
            if (keyTakenByOther)
                throw new CmsContentKeyAlreadyDefinedException(key, CmsContentKeys.AlreadyDefinedMessage(key));

            existing.Key = key;
            existing.IsActive = updateDto.Status == CmsContentStatus.Active;
            existing.UpdatedAt = DateTime.UtcNow;

            // Create translations list from direct properties if translations array is empty
            var translations = updateDto.Translations.ToList();
            
            // Add English translation from direct properties if not already present
            if (!string.IsNullOrEmpty(updateDto.TitleEn) || !string.IsNullOrEmpty(updateDto.ContentEn))
            {
                var existingEn = translations.FirstOrDefault(t => 
                    string.Equals(t.LanguageCode, LanguageConstants.English, StringComparison.OrdinalIgnoreCase));
                
                if (existingEn == null)
                {
                    translations.Add(new CmsContentTranslationDto
                    {
                        LanguageCode = LanguageConstants.English,
                        Title = updateDto.TitleEn ?? "",
                        Content = updateDto.ContentEn ?? ""
                    });
                }
            }

            // Add Hindi translation from direct properties if not already present
            if (!string.IsNullOrEmpty(updateDto.TitleHi) || !string.IsNullOrEmpty(updateDto.ContentHi))
            {
                var existingHi = translations.FirstOrDefault(t => 
                    string.Equals(t.LanguageCode, LanguageConstants.Hindi, StringComparison.OrdinalIgnoreCase));
                
                if (existingHi == null)
                {
                    translations.Add(new CmsContentTranslationDto
                    {
                        LanguageCode = LanguageConstants.Hindi,
                        Title = updateDto.TitleHi ?? "",
                        Content = updateDto.ContentHi ?? ""
                    });
                }
            }

            // Update existing translations and add new ones - don't remove existing ones
            var existingTranslations = existing.Translations.ToList();
            
            foreach (var t in translations)
            {
                var code = t.LanguageCode?.Trim().ToLowerInvariant() ?? "";
                if (string.IsNullOrEmpty(code) || !LanguageConstants.SupportedLanguages.Contains(code))
                    continue;

                var existingTranslation = existingTranslations.FirstOrDefault(et => 
                    string.Equals(et.LanguageCode, code, StringComparison.OrdinalIgnoreCase));
                
                if (existingTranslation != null)
                {
                    // Update existing translation
                    existingTranslation.Title = t.Title ?? "";
                    existingTranslation.Content = t.Content ?? "";
                }
                else
                {
                    // Add new translation
                    existing.Translations.Add(new CmsContentTranslation
                    {
                        LanguageCode = code,
                        Title = t.Title ?? "",
                        Content = t.Content ?? ""
                    });
                }
            }

            // Ensure English translation exists
            var enExists = existing.Translations.Any(t =>
                string.Equals(t.LanguageCode, LanguageConstants.English, StringComparison.OrdinalIgnoreCase));
            if (!enExists)
                throw new InvalidOperationException("At least one 'en' (English) translation is required.");

            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();

            return MapToDto(existing, LanguageConstants.English, includeTranslations: true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return false;

            await _repository.DeleteAsync(existing);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, CmsContentStatus status)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.IsActive = status == CmsContentStatus.Active;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Picks translation for language; falls back to "en". Both Title and Content from same translation.
        /// Supports any language in LanguageConstants - future languages work without code change.
        /// </summary>
        private CmsContentDto MapToDto(CmsContent entity, string language, bool includeTranslations)
        {
            var translation = entity.Translations.FirstOrDefault(t =>
                    string.Equals(t.LanguageCode, language, StringComparison.OrdinalIgnoreCase))
                ?? entity.Translations.FirstOrDefault(t =>
                    string.Equals(t.LanguageCode, LanguageConstants.English, StringComparison.OrdinalIgnoreCase));

            var title = translation?.Title ?? "";
            var content = translation?.Content ?? "";

            return new CmsContentDto
            {
                Id = entity.Id,
                Key = entity.Key,
                Title = title,
                Content = content,
                Status = entity.IsActive ? CmsContentStatus.Active : CmsContentStatus.Inactive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Translations = includeTranslations
                    ? entity.Translations.Select(t => new CmsContentTranslationDto
                    {
                        LanguageCode = t.LanguageCode,
                        Title = t.Title,
                        Content = t.Content
                    }).ToList()
                    : null
            };
        }
    }
}
