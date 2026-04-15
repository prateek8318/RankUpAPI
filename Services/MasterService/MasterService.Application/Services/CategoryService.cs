using Microsoft.Extensions.Logging;
using AutoMapper;
using Common.Language;
using Common.DTOs;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.Application.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILanguageDataService _languageDataService;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ILanguageDataService languageDataService,
            IMapper mapper,
            ILogger<CategoryService> logger) : base(logger)
        {
            _categoryRepository = categoryRepository;
            _languageDataService = languageDataService;
            _mapper = mapper;
        }

        public async Task<object> GetAllCategoriesOptimizedAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                
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

                var categories = await _categoryRepository.GetActiveByTypeAsync("category");
                var categoryList = categories
                    .Select(c => MapToOptimizedCategoryDto(c, normalizedLanguage))
                    .ToList();

                return categoryList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories for language {Language}", language);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesIncludingInactiveAsync(string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);

                var categories = await _categoryRepository.GetAllIncludingInactiveAsync();
                return categories
                    .Where(c => string.Equals(c.Type, "category", StringComparison.OrdinalIgnoreCase))
                    .Select(c => MapToOptimizedCategoryDto(c, normalizedLanguage))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories including inactive for language {Language}", language);
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
            // Validation
            if (string.IsNullOrWhiteSpace(createDto.NameEn))
            {
                throw new ArgumentException("English name is required and cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(createDto.Key))
            {
                throw new ArgumentException("Key is required and cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(createDto.Type))
            {
                throw new ArgumentException("Type is required and cannot be empty.");
            }

            // Check for duplicate key within the same type only
            var existingCategoriesByType = await _categoryRepository.GetByTypeAsync(createDto.Type);
            var duplicateKey = existingCategoriesByType.FirstOrDefault(c => c.Key.Equals(createDto.Key, StringComparison.OrdinalIgnoreCase));
            
            if (duplicateKey != null)
            {
                throw new InvalidOperationException($"A category with key '{createDto.Key}' already exists in type '{createDto.Type}' (ID: {duplicateKey.Id}). Category keys must be unique within the same type.");
            }

            var entity = new Domain.Entities.Category
            {
                NameEn = createDto.NameEn,
                NameHi = createDto.NameHi,
                Key = createDto.Key,
                Type = createDto.Type,
                Description = createDto.Description,
                DisplayOrder = createDto.DisplayOrder,
                Status = createDto.Status ?? "active",
                CreatedAt = DateTime.UtcNow
            };

            var categoryEntity = await _categoryRepository.AddAsync(entity);

            // Default response English me
            return MapToCategoryDto(categoryEntity, LanguageConstants.English);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
        {
            try
            {
                var existing = await _categoryRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    return null;
                }

                // Validation
                if (string.IsNullOrWhiteSpace(updateDto.NameEn))
                {
                    throw new ArgumentException("English name is required and cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(updateDto.Key))
                {
                    throw new ArgumentException("Key is required and cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(updateDto.Type))
                {
                    throw new ArgumentException("Type is required and cannot be empty.");
                }

                // Check for duplicate key within the same type (excluding current entity)
                var existingCategoriesByType = await _categoryRepository.GetByTypeAsync(updateDto.Type);
                var duplicateKey = existingCategoriesByType.FirstOrDefault(c => 
                    c.Key.Equals(updateDto.Key, StringComparison.OrdinalIgnoreCase) && 
                    c.Id != id);
                
                if (duplicateKey != null)
                {
                    throw new InvalidOperationException($"A category with key '{updateDto.Key}' already exists in type '{updateDto.Type}' (ID: {duplicateKey.Id}). Category keys must be unique within the same type.");
                }

                existing.NameEn = updateDto.NameEn;
                existing.NameHi = updateDto.NameHi;
                existing.Key = updateDto.Key;
                existing.Type = updateDto.Type;
                existing.Description = updateDto.Description;
                existing.DisplayOrder = updateDto.DisplayOrder;
                existing.Status = updateDto.Status ?? existing.Status;
                existing.UpdatedAt = DateTime.UtcNow;

                await _categoryRepository.UpdateAsync(existing);

                return MapToCategoryDto(existing, LanguageConstants.English);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var existing = await _categoryRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    return false;
                }

                await _categoryRepository.DeleteAsync(existing);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> ToggleCategoryStatusAsync(int id, bool isActive)
        {
            try
            {
                _logger.LogInformation("Toggling category {CategoryId} status to {IsActive}", id, isActive);
                
                // Use GetAllIncludingInactiveAsync to get consistent data with admin endpoint
                var allCategories = await _categoryRepository.GetAllIncludingInactiveAsync();
                var category = allCategories.FirstOrDefault(c => c.Id == id);
                
                if (category == null)
                {
                    _logger.LogWarning("Category {CategoryId} not found for status toggle", id);
                    return false;
                }

                // Update the category status
                category.Status = isActive ? "active" : "inactive";
                category.IsActive = isActive;
                category.UpdatedAt = DateTime.UtcNow;
                
                await _categoryRepository.UpdateAsync(category);
                
                _logger.LogInformation("Category {CategoryId} status updated to {IsActive}", id, isActive);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category status for ID {Id}", id);
                throw;
            }
        }

        public async Task<CategoryDto?> GetCategoryAsync(int id, string language)
        {
            try
            {
                var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
                _logger.LogInformation("Getting category {CategoryId} for language {Language}", id, language);
                
                // Use GetAllIncludingInactiveAsync to get consistent data with admin endpoint
                var allCategories = await _categoryRepository.GetAllIncludingInactiveAsync();
                _logger.LogInformation("Total categories found: {Count}", allCategories.Count());
                
                var category = allCategories.FirstOrDefault(c => c.Id == id);
                _logger.LogInformation("Looking for category ID {Id}, found: {Found}", id, category != null);
                
                if (category == null)
                {
                    _logger.LogWarning("Category {CategoryId} not found in repository", id);
                    return null;
                }

                _logger.LogInformation("Category {CategoryId} found: {CategoryName}, Status: {Status}, IsActive: {IsActive}", 
                    id, category.NameEn, category.Status, category.IsActive);
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
                Status = category.Status,
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
                        Status = "active",
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
                              (dict.TryGetValue("category", out var categoryValue) ? categoryValue?.ToString() : string.Empty),
                        Status = "active",
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
                        Status = "active",
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
                    Status = "active",
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
                    Status = "active",
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
                    Status = "active",
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
                    Status = "active",
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
                    Status = "active",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        private IEnumerable<CategoryDto> GetDefaultQualificationsOptimized(string language)
        {
            var useHindi = language == LanguageConstants.Hindi;
            
            return new List<CategoryDto>
            {
                new() { Id = 1, Name = useHindi ? "१०वीं पास" : "10th Pass", NameEn = "10th Pass", NameHi = "१०वीं पास", Key = "10th", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = useHindi ? "१२वीं पास" : "12th Pass", NameEn = "12th Pass", NameHi = "१२वीं पास", Key = "12th", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 3, Name = useHindi ? "स्नातक" : "Graduate", NameEn = "Graduate", NameHi = "स्नातक", Key = "graduate", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 4, Name = useHindi ? "स्नातकोत्तर" : "Post Graduate", NameEn = "Post Graduate", NameHi = "स्नातकोत्तर", Key = "post_graduate", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 5, Name = useHindi ? "डिप्लोमा" : "Diploma", NameEn = "Diploma", NameHi = "डिप्लोमा", Key = "diploma", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 6, Name = useHindi ? "इंजीनियरिंग" : "Engineering", NameEn = "Engineering", NameHi = "इंजीनियरिंग", Key = "engineering", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 7, Name = useHindi ? "चिकित्सा" : "Medical", NameEn = "Medical", NameHi = "चिकित्सा", Key = "medical", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 8, Name = useHindi ? "प्रबंधन" : "Management", NameEn = "Management", NameHi = "प्रबंधन", Key = "management", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 9, Name = useHindi ? "वाणिज्य" : "Commerce", NameEn = "Commerce", NameHi = "वाणिज्य", Key = "commerce", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 10, Name = useHindi ? "विज्ञान" : "Science", NameEn = "Science", NameHi = "विज्ञान", Key = "science", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 11, Name = useHindi ? "कला" : "Arts", NameEn = "Arts", NameHi = "कला", Key = "arts", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 12, Name = useHindi ? "अन्य" : "Other", NameEn = "Other", NameHi = "अन्य", Key = "other", Type = "qualification", Status = "active", CreatedAt = DateTime.UtcNow }
            };
        }

        private IEnumerable<CategoryDto> GetDefaultExamCategoriesOptimized(string language)
        {
            var useHindi = language == LanguageConstants.Hindi;
            
            return new List<CategoryDto>
            {
                new() { Id = 1, Name = useHindi ? "सामान्य" : "General", NameEn = "General", NameHi = "सामान्य", Key = "general", Type = "exam_category", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = useHindi ? "अनुसूचित जाति" : "SC", NameEn = "SC", NameHi = "अनुसूचित जाति", Key = "sc", Type = "exam_category", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 3, Name = useHindi ? "अनुसूचित जनजाति" : "ST", NameEn = "ST", NameHi = "अनुसूचित जनजाति", Key = "st", Type = "exam_category", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 4, Name = useHindi ? "अन्य पिछड़ा वर्ग" : "OBC", NameEn = "OBC", NameHi = "अन्य पिछड़ा वर्ग", Key = "obc", Type = "exam_category", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 5, Name = useHindi ? "आर्थिक रूप से कमजोर" : "EWS", NameEn = "EWS", NameHi = "आर्थिक रूप से कमजोर", Key = "ews", Type = "exam_category", Status = "active", CreatedAt = DateTime.UtcNow }
            };
        }

        private IEnumerable<CategoryDto> GetDefaultStreamsOptimized(string language)
        {
            var useHindi = language == LanguageConstants.Hindi;
            
            return new List<CategoryDto>
            {
                new() { Id = 1, Name = useHindi ? "विज्ञान" : "Science", NameEn = "Science", NameHi = "विज्ञान", Key = "science", Type = "stream", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = useHindi ? "वाणिज्य" : "Commerce", NameEn = "Commerce", NameHi = "वाणिज्य", Key = "commerce", Type = "stream", Status = "active", CreatedAt = DateTime.UtcNow },
                new() { Id = 3, Name = useHindi ? "कला" : "Arts", NameEn = "Arts", NameHi = "कला", Key = "arts", Type = "stream", Status = "active", CreatedAt = DateTime.UtcNow }
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
                Status = category.Status,
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
                    Status = "active",
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
                    Status = "active",
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
                    Status = "active",
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
                    Status = "active",
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
                    Status = "active",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        // Pagination support methods
        public async Task<PaginatedResponse<CategoryDto>> GetAllCategoriesPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedCategories = await _categoryRepository.GetAllAsync(pagination);
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(paginatedCategories.Data);
                
                return new PaginatedResponse<CategoryDto>
                {
                    Data = categoryDtos,
                    TotalCount = paginatedCategories.TotalCount,
                    PageNumber = paginatedCategories.PageNumber,
                    PageSize = paginatedCategories.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated categories");
                throw;
            }
        }

        public async Task<PaginatedResponse<CategoryDto>> GetActiveCategoriesPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedCategories = await _categoryRepository.GetActiveAsync(pagination);
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(paginatedCategories.Data);
                
                return new PaginatedResponse<CategoryDto>
                {
                    Data = categoryDtos,
                    TotalCount = paginatedCategories.TotalCount,
                    PageNumber = paginatedCategories.PageNumber,
                    PageSize = paginatedCategories.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated active categories");
                throw;
            }
        }
    }
}
