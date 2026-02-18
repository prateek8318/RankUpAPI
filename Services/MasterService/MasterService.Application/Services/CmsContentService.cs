using Common.Language;
using MasterService.Application.DTOs;
using MasterService.Application.Exceptions;
using MasterService.Application.Interfaces;
using MasterService.Domain;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;

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

        public async Task<CmsContentDto> CreateAsync(CreateCmsContentDto createDto)
        {
            var key = createDto.Key?.Trim() ?? "";
            if (!CmsContentKeys.IsValid(key))
                throw new CmsContentKeyInvalidException(key, CmsContentKeys.InvalidKeyMessage(createDto.Key));

            var enTranslation = createDto.Translations.FirstOrDefault(t =>
                string.Equals(t.LanguageCode, LanguageConstants.English, StringComparison.OrdinalIgnoreCase));
            if (enTranslation == null)
                throw new InvalidOperationException("At least one 'en' (English) translation is required.");

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

                foreach (var t in createDto.Translations)
                {
                    var code = t.LanguageCode?.Trim().ToLowerInvariant() ?? "";
                    if (string.IsNullOrEmpty(code) || !LanguageConstants.SupportedLanguages.Contains(code))
                        continue;

                    entity.Translations.Add(new CmsContentTranslation
                    {
                        LanguageCode = code,
                        Title = t.Title ?? "",
                        Content = t.Content ?? ""
                    });
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

            // Replace translations
            existing.Translations.Clear();
            foreach (var t in updateDto.Translations)
            {
                var code = t.LanguageCode?.Trim().ToLowerInvariant() ?? "";
                if (string.IsNullOrEmpty(code) || !LanguageConstants.SupportedLanguages.Contains(code))
                    continue;

                existing.Translations.Add(new CmsContentTranslation
                {
                    LanguageCode = code,
                    Title = t.Title ?? "",
                    Content = t.Content ?? ""
                });
            }

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
