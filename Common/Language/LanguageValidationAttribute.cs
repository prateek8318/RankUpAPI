using System.ComponentModel.DataAnnotations;

namespace Common.Language
{
    public class LanguageValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
                
            var language = value.ToString();
            if (LanguageValidator.IsValidLanguage(language))
                return ValidationResult.Success;
                
            return new ValidationResult($"Invalid language. Supported languages: {string.Join(", ", LanguageConstants.SupportedLanguages)}");
        }
    }
}
