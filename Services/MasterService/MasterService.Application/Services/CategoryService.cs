using Microsoft.Extensions.Logging;
using Common.Language;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILanguageDataService _languageDataService;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ILanguageDataService languageDataService,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _languageDataService = languageDataService;
            _logger = logger;
        }

        public async Task<object> GetAllCategoriesOptimizedAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
                // Get all category types in parallel
                var categoriesTask = GetCategoriesAsync(normalizedLanguage);
                var qualificationsTask = GetQualificationsAsync(normalizedLanguage);
                var examCategoriesTask = GetExamCategoriesAsync(normalizedLanguage);
                var streamsTask = GetStreamsAsync(normalizedLanguage);

                await Task.WhenAll(categoriesTask, qualificationsTask, examCategoriesTask, streamsTask);

                return new
                {
                    categories = categoriesTask.Result,
                    qualifications = qualificationsTask.Result,
                    examCategories = examCategoriesTask.Result,
                    streams = streamsTask.Result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories data for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);

                // Primary source: DB categories
                var categories = await _categoryRepository.GetActiveByTypeAsync("category");
                var categoryList = categories
                    .Select(c => MapToOptimizedCategoryDto(c, normalizedLanguage))
                    .ToList();

                // If DB empty, fall back to in-memory defaults
                if (!categoryList.Any())
                {
                    return GetDefaultCategoriesOptimized(normalizedLanguage);
                }

                return categoryList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetQualificationsAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var data = await _languageDataService.GetLocalizedDataAsync(normalizedLanguage, "qualifications");
                
                if (data.TryGetValue("qualifications", out var qualificationsData) && qualificationsData is IEnumerable<object> items)
                {
                    return items.Select(item => MapToOptimizedCategoryDto(item, normalizedLanguage)).ToList();
                }

                // Fallback to default qualifications
                return GetDefaultQualificationsOptimized(normalizedLanguage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting qualifications for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetExamCategoriesAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var data = await _languageDataService.GetLocalizedDataAsync(normalizedLanguage, "examCategories");
                
                if (data.TryGetValue("examCategories", out var examCategoriesData) && examCategoriesData is IEnumerable<object> items)
                {
                    return items.Select(item => MapToOptimizedCategoryDto(item, normalizedLanguage)).ToList();
                }

                // Fallback to default exam categories
                return GetDefaultExamCategoriesOptimized(normalizedLanguage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exam categories for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetStreamsAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
                // For now, return default streams (can be extended later)
                return GetDefaultStreamsOptimized(normalizedLanguage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting streams for language {Language}", language);
                throw;
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            var entity = new Domain.Entities.Category
            {
                NameEn = createDto.NameEn,
                NameHi = createDto.NameHi,
                Key = createDto.Key,
                Type = createDto.Type,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(entity);
            await _categoryRepository.SaveChangesAsync();

            // Default response English me
            return MapToCategoryDto(entity, LanguageConstants.English);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return null;
            }

            existing.NameEn = updateDto.NameEn;
            existing.NameHi = updateDto.NameHi;
            existing.Key = updateDto.Key;
            existing.Type = updateDto.Type;
            existing.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(existing);
            await _categoryRepository.SaveChangesAsync();

            return MapToCategoryDto(existing, LanguageConstants.English);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            await _categoryRepository.DeleteAsync(existing);
            await _categoryRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleCategoryStatusAsync(int id, bool isActive)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            existing.IsActive = isActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(existing);
            await _categoryRepository.SaveChangesAsync();

            return true;
        }

        public async Task<CategoryDto> GetCategoryAsync(int id, string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    // Fallback: try from default/in-memory list (for legacy IDs)
                    var defaults = GetDefaultCategories();
                    var defaultCategory = defaults.FirstOrDefault(c => c.Id == id);
                    if (defaultCategory != null)
                    {
                        return defaultCategory;
                    }

                    throw new KeyNotFoundException($"Category with id {id} not found.");
                }

                return MapToCategoryDto(category, normalizedLanguage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category {CategoryId} for language {Language}", id, language);
                throw;
            }
        }

        private CategoryDto MapToOptimizedCategoryDto(Domain.Entities.Category category, string language)
        {
            var useHindi = language == LanguageConstants.Hindi;

            // Hindi select hone par agar NameHi null/empty hai to automatic English fallback
            var localizedName = useHindi && !string.IsNullOrWhiteSpace(category.NameHi)
                ? category.NameHi!
                : category.NameEn;

            return new CategoryDto
            {
                Id = category.Id,
                Name = localizedName,
                NameEn = category.NameEn,
                NameHi = category.NameHi,
                Key = category.Key,
                Type = category.Type,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        private CategoryDto MapToOptimizedCategoryDto(object item, string language)
        {
            try
            {
                _logger.LogInformation("Mapping item of type: {ItemType}", item?.GetType().Name);
                
                // Handle JsonElement from System.Text.Json
                if (item is System.Text.Json.JsonElement element)
                {
                    _logger.LogInformation("Processing JsonElement: {Element}", element);
                    
                    return new CategoryDto
                    {
                        Id = element.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0,
                        Name = element.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : string.Empty,
                        Key = element.TryGetProperty("key", out var keyProp) ? keyProp.GetString() : 
                              element.TryGetProperty("code", out var codeProp) ? codeProp.GetString() : string.Empty,
                        Type = element.TryGetProperty("type", out var typeProp) ? typeProp.GetString() : 
                               element.TryGetProperty("category", out var categoryProp) ? categoryProp.GetString() : string.Empty,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                
                // Handle Dictionary<string, object>
                if (item is Dictionary<string, object> dict)
                {
                    _logger.LogInformation("Processing Dictionary: {Dictionary}", string.Join(", ", dict.Select(kvp => $"{kvp.Key}={kvp.Value}")));
                    
                    return new CategoryDto
                    {
                        Id = dict.TryGetValue("id", out var idValue) ? Convert.ToInt32(idValue) : 0,
                        Name = dict.TryGetValue("name", out var nameValue) ? nameValue?.ToString() : string.Empty,
                        Key = dict.TryGetValue("key", out var keyValue) ? keyValue?.ToString() : 
                              dict.TryGetValue("code", out var codeValue) ? codeValue?.ToString() : string.Empty,
                        Type = dict.TryGetValue("type", out var typeValue) ? typeValue?.ToString() : 
                               dict.TryGetValue("category", out var categoryValue) ? categoryValue?.ToString() : string.Empty,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                
                // Handle anonymous objects using reflection
                var itemType = item?.GetType();
                if (itemType != null && itemType.Name.Contains("AnonymousType"))
                {
                    _logger.LogInformation("Processing Anonymous Type: {ItemType}", itemType.Name);
                    
                    var idProperty = itemType.GetProperty("id");
                    var nameProperty = itemType.GetProperty("name");
                    var keyProperty = itemType.GetProperty("key");
                    var typeProperty = itemType.GetProperty("type");
                    var codeProperty = itemType.GetProperty("code");
                    var categoryProperty = itemType.GetProperty("category");
                    
                    return new CategoryDto
                    {
                        Id = idProperty?.GetValue(item) != null ? Convert.ToInt32(idProperty.GetValue(item)) : 0,
                        Name = nameProperty?.GetValue(item)?.ToString() ?? string.Empty,
                        Key = keyProperty?.GetValue(item)?.ToString() ?? 
                              codeProperty?.GetValue(item)?.ToString() ?? string.Empty,
                        Type = typeProperty?.GetValue(item)?.ToString() ?? 
                               categoryProperty?.GetValue(item)?.ToString() ?? string.Empty,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                
                _logger.LogWarning("Unknown item type: {ItemType}", item?.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping category item: {Item}", item);
            }
            
            return new CategoryDto();
        }

        private IEnumerable<CategoryDto> GetDefaultCategoriesOptimized(string language)
        {
            var useHindi = language == LanguageConstants.Hindi;
            
            return new List<CategoryDto>
            {
                new()
                {
                    Id = 1,
                    Name = useHindi ? "सामान्य" : "General",
                    NameEn = "General",
                    NameHi = "सामान्य",
                    Key = "general",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    Name = useHindi ? "अन्य पिछड़ा वर्ग" : "OBC",
                    NameEn = "OBC",
                    NameHi = "अन्य पिछड़ा वर्ग",
                    Key = "obc",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 3,
                    Name = useHindi ? "अनुसूचित जाति" : "SC",
                    NameEn = "SC",
                    NameHi = "अनुसूचित जाति",
                    Key = "sc",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 4,
                    Name = useHindi ? "अनुसूचित जनजाति" : "ST",
                    NameEn = "ST",
                    NameHi = "अनुसूचित जनजाति",
                    Key = "st",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 5,
                    Name = useHindi ? "आर्थिक रूप से कमजोर" : "EWS",
                    NameEn = "EWS",
                    NameHi = "आर्थिक रूप से कमजोर",
                    Key = "ews",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        private IEnumerable<CategoryDto> GetDefaultQualificationsOptimized(string language)
        {
            var useHindi = language == LanguageConstants.Hindi;
            
            return new List<CategoryDto>
            {
                new() { Id = 1, Name = useHindi ? "१०वीं पास" : "10th Pass", NameEn = "10th Pass", NameHi = "१०वीं पास", Key = "10th", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = useHindi ? "१२वीं पास" : "12th Pass", NameEn = "12th Pass", NameHi = "१२वीं पास", Key = "12th", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 3, Name = useHindi ? "स्नातक" : "Graduate", NameEn = "Graduate", NameHi = "स्नातक", Key = "graduate", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 4, Name = useHindi ? "स्नातकोत्तर" : "Post Graduate", NameEn = "Post Graduate", NameHi = "स्नातकोत्तर", Key = "post_graduate", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 5, Name = useHindi ? "डिप्लोमा" : "Diploma", NameEn = "Diploma", NameHi = "डिप्लोमा", Key = "diploma", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 6, Name = useHindi ? "इंजीनियरिंग" : "Engineering", NameEn = "Engineering", NameHi = "इंजीनियरिंग", Key = "engineering", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 7, Name = useHindi ? "चिकित्सा" : "Medical", NameEn = "Medical", NameHi = "चिकित्सा", Key = "medical", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 8, Name = useHindi ? "प्रबंधन" : "Management", NameEn = "Management", NameHi = "प्रबंधन", Key = "management", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 9, Name = useHindi ? "वाणिज्य" : "Commerce", NameEn = "Commerce", NameHi = "वाणिज्य", Key = "commerce", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 10, Name = useHindi ? "विज्ञान" : "Science", NameEn = "Science", NameHi = "विज्ञान", Key = "science", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 11, Name = useHindi ? "कला" : "Arts", NameEn = "Arts", NameHi = "कला", Key = "arts", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 12, Name = useHindi ? "अन्य" : "Other", NameEn = "Other", NameHi = "अन्य", Key = "other", Type = "qualification", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
        }

        private IEnumerable<CategoryDto> GetDefaultExamCategoriesOptimized(string language)
        {
            var useHindi = language == LanguageConstants.Hindi;
            
            return new List<CategoryDto>
            {
                new() { Id = 1, Name = useHindi ? "सामान्य" : "General", NameEn = "General", NameHi = "सामान्य", Key = "general", Type = "exam_category", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = useHindi ? "अनुसूचित जाति" : "SC", NameEn = "SC", NameHi = "अनुसूचित जाति", Key = "sc", Type = "exam_category", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 3, Name = useHindi ? "अनुसूचित जनजाति" : "ST", NameEn = "ST", NameHi = "अनुसूचित जनजाति", Key = "st", Type = "exam_category", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 4, Name = useHindi ? "अन्य पिछड़ा वर्ग" : "OBC", NameEn = "OBC", NameHi = "अन्य पिछड़ा वर्ग", Key = "obc", Type = "exam_category", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 5, Name = useHindi ? "आर्थिक रूप से कमजोर" : "EWS", NameEn = "EWS", NameHi = "आर्थिक रूप से कमजोर", Key = "ews", Type = "exam_category", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
        }

        private IEnumerable<CategoryDto> GetDefaultStreamsOptimized(string language)
        {
            var useHindi = language == LanguageConstants.Hindi;
            
            return new List<CategoryDto>
            {
                new() { Id = 1, Name = useHindi ? "विज्ञान" : "Science", NameEn = "Science", NameHi = "विज्ञान", Key = "science", Type = "stream", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = useHindi ? "वाणिज्य" : "Commerce", NameEn = "Commerce", NameHi = "वाणिज्य", Key = "commerce", Type = "stream", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 3, Name = useHindi ? "कला" : "Arts", NameEn = "Arts", NameHi = "कला", Key = "arts", Type = "stream", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
        }

        private CategoryDto MapToCategoryDto(Domain.Entities.Category category, string language)
        {
            var useHindi = language == LanguageConstants.Hindi;

            // Hindi select hone par agar NameHi null/empty hai to automatic English fallback
            var localizedName = useHindi && !string.IsNullOrWhiteSpace(category.NameHi)
                ? category.NameHi!
                : category.NameEn;

            return new CategoryDto
            {
                Id = category.Id,
                Name = localizedName,
                NameEn = category.NameEn,
                NameHi = category.NameHi,
                Key = category.Key,
                Type = category.Type,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        private IEnumerable<CategoryDto> GetDefaultCategories()
        {
            return new List<CategoryDto>
            {
                new()
                {
                    Id = 1,
                    Name = "General",
                    NameEn = "General",
                    NameHi = "सामान्य",
                    Key = "general",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    Name = "OBC",
                    NameEn = "OBC",
                    NameHi = "अन्य पिछड़ा वर्ग",
                    Key = "obc",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 3,
                    Name = "SC",
                    NameEn = "SC",
                    NameHi = "अनुसूचित जाति",
                    Key = "sc",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 4,
                    Name = "ST",
                    NameEn = "ST",
                    NameHi = "अनुसूचित जनजाति",
                    Key = "st",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 5,
                    Name = "EWS",
                    NameEn = "EWS",
                    NameHi = "आर्थिक रूप से कमजोर",
                    Key = "ews",
                    Type = "category",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}
