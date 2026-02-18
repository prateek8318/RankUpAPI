using Common.Services;

namespace Common.Language
{
    public interface ILanguageDataService
    {
        Task<Dictionary<string, object>> GetLocalizedDataAsync(string language, string category);
        Task<T> GetLocalizedDataAsync<T>(string language, string category, string key);
        Task<Dictionary<string, object>> GetUserDataAsync(string language, string category);
        Task<bool> HasLanguageDataAsync(string language, string category);
    }

    public class LanguageDataService : ILanguageDataService
    {
        private readonly ILanguageService _languageService;
        private readonly IServiceProvider _serviceProvider;

        // Default data structure for when language-specific data is not available
        private readonly Dictionary<string, object> _defaultEnglishData = new()
        {
            ["states"] = new[]
            {
                new { id = 1, name = "Andhra Pradesh", code = "AP" },
                new { id = 2, name = "Arunachal Pradesh", code = "AR" },
                new { id = 3, name = "Assam", code = "AS" },
                new { id = 4, name = "Bihar", code = "BR" },
                new { id = 5, name = "Chhattisgarh", code = "CG" },
                new { id = 6, name = "Goa", code = "GA" },
                new { id = 7, name = "Gujarat", code = "GJ" },
                new { id = 8, name = "Haryana", code = "HR" },
                new { id = 9, name = "Himachal Pradesh", code = "HP" },
                new { id = 10, name = "Jharkhand", code = "JH" },
                new { id = 11, name = "Karnataka", code = "KA" },
                new { id = 12, name = "Kerala", code = "KL" },
                new { id = 13, name = "Madhya Pradesh", code = "MP" },
                new { id = 14, name = "Maharashtra", code = "MH" },
                new { id = 15, name = "Manipur", code = "MN" },
                new { id = 16, name = "Meghalaya", code = "ML" },
                new { id = 17, name = "Mizoram", code = "MZ" },
                new { id = 18, name = "Nagaland", code = "NL" },
                new { id = 19, name = "Odisha", code = "OR" },
                new { id = 20, name = "Punjab", code = "PB" },
                new { id = 21, name = "Rajasthan", code = "RJ" },
                new { id = 22, name = "Sikkim", code = "SK" },
                new { id = 23, name = "Tamil Nadu", code = "TN" },
                new { id = 24, name = "Telangana", code = "TG" },
                new { id = 25, name = "Tripura", code = "TR" },
                new { id = 26, name = "Uttar Pradesh", code = "UP" },
                new { id = 27, name = "Uttarakhand", code = "UT" },
                new { id = 28, name = "West Bengal", code = "WB" }
            },
            ["qualifications"] = new[]
            {
                new { id = 1, name = "10th Pass", category = "general" },
                new { id = 2, name = "12th Pass", category = "general" },
                new { id = 3, name = "Graduate", category = "general" },
                new { id = 4, name = "Post Graduate", category = "general" },
                new { id = 5, name = "Diploma", category = "technical" },
                new { id = 6, name = "Engineering", category = "technical" },
                new { id = 7, name = "Medical", category = "medical" },
                new { id = 8, name = "Management", category = "management" },
                new { id = 9, name = "Commerce", category = "commerce" },
                new { id = 10, name = "Science", category = "science" },
                new { id = 11, name = "Arts", category = "arts" },
                new { id = 12, name = "Other", category = "general" }
            },
            ["categories"] = new[]
            {
                new { id = 1, name = "General", key = "general", type = "category" },
                new { id = 2, name = "OBC", key = "obc", type = "category" },
                new { id = 3, name = "SC", key = "sc", type = "category" },
                new { id = 4, name = "ST", key = "st", type = "category" },
                new { id = 5, name = "EWS", key = "ews", type = "category" }
            },
            ["examCategories"] = new[]
            {
                new { id = 1, name = "General", key = "general" },
                new { id = 2, name = "SC", key = "sc" },
                new { id = 3, name = "ST", key = "st" },
                new { id = 4, name = "OBC", key = "obc" },
                new { id = 5, name = "EWS", key = "ews" }
            }
        };

        // Hindi data structure
        private readonly Dictionary<string, object> _hindiData = new()
        {
            ["states"] = new[]
            {
                new { id = 1, name = "आंध्र प्रदेश", code = "AP" },
                new { id = 2, name = "अरुणाचल प्रदेश", code = "AR" },
                new { id = 3, name = "असम", code = "AS" },
                new { id = 4, name = "बिहार", code = "BR" },
                new { id = 5, name = "छत्तीसगढ़", code = "CG" },
                new { id = 6, name = "गोवा", code = "GA" },
                new { id = 7, name = "गुजरात", code = "GJ" },
                new { id = 8, name = "हरियाणा", code = "HR" },
                new { id = 9, name = "हिमाचल प्रदेश", code = "HP" },
                new { id = 10, name = "झारखंड", code = "JH" },
                new { id = 11, name = "कर्नाटक", code = "KA" },
                new { id = 12, name = "केरल", code = "KL" },
                new { id = 13, name = "मध्य प्रदेश", code = "MP" },
                new { id = 14, name = "महाराष्ट्र", code = "MH" },
                new { id = 15, name = "मणिपुर", code = "MN" },
                new { id = 16, name = "मेघालय", code = "ML" },
                new { id = 17, name = "मिजोरम", code = "MZ" },
                new { id = 18, name = "नागालैंड", code = "NL" },
                new { id = 19, name = "ओडिशा", code = "OR" },
                new { id = 20, name = "पंजाब", code = "PB" },
                new { id = 21, name = "राजस्थान", code = "RJ" },
                new { id = 22, name = "सिक्किम", code = "SK" },
                new { id = 23, name = "तमिल नाडु", code = "TN" },
                new { id = 24, name = "तेलंगाना", code = "TG" },
                new { id = 25, name = "त्रिपुरा", code = "TR" },
                new { id = 26, name = "उत्तर प्रदेश", code = "UP" },
                new { id = 27, name = "उत्तराखंड", code = "UT" },
                new { id = 28, name = "पश्चिम बंगाल", code = "WB" }
            },
            ["qualifications"] = new[]
            {
                new { id = 1, name = "१०वीं पास", category = "general" },
                new { id = 2, name = "१२वीं पास", category = "general" },
                new { id = 3, name = "स्नातक", category = "general" },
                new { id = 4, name = "स्नातकोत्तर", category = "general" },
                new { id = 5, name = "डिप्लोमा", category = "technical" },
                new { id = 6, name = "इंजीनियरिंग", category = "technical" },
                new { id = 7, name = "मेडिकल", category = "medical" },
                new { id = 8, name = "प्रबंधन", category = "management" },
                new { id = 9, name = "वाणिज्य", category = "commerce" },
                new { id = 10, name = "विज्ञान", category = "science" },
                new { id = 11, name = "कला", category = "arts" },
                new { id = 12, name = "अन्य", category = "general" }
            },
            ["categories"] = new[]
            {
                new { id = 1, name = "सामान्य", key = "general", type = "category" },
                new { id = 2, name = "अन्य पिछड़ा वर्ग", key = "obc", type = "category" },
                new { id = 3, name = "अनुसूचित जाति", key = "sc", type = "category" },
                new { id = 4, name = "अनुसूचित जनजाति", key = "st", type = "category" },
                new { id = 5, name = "आर्थिक रूप से कमजोर", key = "ews", type = "category" }
            },
            ["examCategories"] = new[]
            {
                new { id = 1, name = "सामान्य", key = "general" },
                new { id = 2, name = "अनुसूचित जनजाति", key = "sc" },
                new { id = 3, name = "अनुसूचित जनजाति", key = "st" },
                new { id = 4, name = "अन्य पिछड़ा वर्ग", key = "obc" },
                new { id = 5, name = "आर्थिक रूप से कमजोर", key = "ews" }
            }
        };

        public LanguageDataService(ILanguageService languageService, IServiceProvider serviceProvider)
        {
            _languageService = languageService;
            _serviceProvider = serviceProvider;
        }

        public async Task<Dictionary<string, object>> GetLocalizedDataAsync(string language, string category)
        {
            var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
            var dataSource = GetLanguageDataSource(normalizedLanguage);

            if (dataSource.ContainsKey(category))
            {
                return new Dictionary<string, object>
                {
                    [category] = dataSource[category]
                };
            }

            // Fallback to English if category not found
            return new Dictionary<string, object>
            {
                [category] = _defaultEnglishData[category]
            };
        }

        public async Task<T> GetLocalizedDataAsync<T>(string language, string category, string key)
        {
            var data = await GetLocalizedDataAsync(language, category);
            
            if (data.TryGetValue(category, out var categoryData) && categoryData is IEnumerable<object> items)
            {
                var itemList = items.ToList();
                var result = itemList.FirstOrDefault(item => 
                {
                    var dict = item as Dictionary<string, object>;
                    return dict?.ContainsKey(key) == true && dict[key]?.ToString() == key;
                });
                
                if (result != null)
                {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(result));
                }
            }

            throw new KeyNotFoundException($"Data not found for category: {category}, key: {key}");
        }

        public async Task<Dictionary<string, object>> GetUserDataAsync(string language, string category)
        {
            // This method can be extended to fetch user-specific data
            // For now, return general localized data
            return await GetLocalizedDataAsync(language, category);
        }

        public async Task<bool> HasLanguageDataAsync(string language, string category)
        {
            var normalizedLanguage = LanguageValidator.NormalizeLanguage(language);
            var dataSource = GetLanguageDataSource(normalizedLanguage);
            return dataSource.ContainsKey(category);
        }

        private Dictionary<string, object> GetLanguageDataSource(string language)
        {
            return language switch
            {
                LanguageConstants.Hindi => _hindiData,
                LanguageConstants.Tamil => _defaultEnglishData, // Can add Tamil data
                LanguageConstants.Gujarati => _defaultEnglishData, // Can add Gujarati data
                _ => _defaultEnglishData
            };
        }
    }
}
