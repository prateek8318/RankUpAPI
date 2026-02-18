using System.ComponentModel.DataAnnotations;

namespace Common.Language
{
    public static class LanguageValidator
    {
        public static bool IsValidLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return false;
                
            return LanguageConstants.SupportedLanguages.Contains(language.ToLowerInvariant());
        }
        
        public static string NormalizeLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return LanguageConstants.DefaultLanguage;
                
            var normalized = language.ToLowerInvariant().Trim();
            return LanguageValidator.IsValidLanguage(normalized) ? normalized : LanguageConstants.DefaultLanguage;
        }
        
        public static void ValidateLanguage(string? language, string fieldName = "Language")
        {
            if (!IsValidLanguage(language))
            {
                throw new ValidationException($"Invalid {fieldName}. Supported languages: {string.Join(", ", LanguageConstants.SupportedLanguages)}");
            }
        }
    }
}
