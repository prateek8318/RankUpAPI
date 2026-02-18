using Microsoft.AspNetCore.Http;
using Common.Language;

namespace Common.Services
{
    public interface ILanguageService
    {
        string GetCurrentLanguage();
        string GetCurrentLanguageOrDefault();
        bool IsValidLanguage(string language);
        string NormalizeLanguage(string language);
    }

    public class LanguageService : ILanguageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentLanguage()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items["Language"] is string language)
            {
                return language;
            }
            
            return LanguageConstants.DefaultLanguage;
        }

        public string GetCurrentLanguageOrDefault()
        {
            return GetCurrentLanguage();
        }

        public bool IsValidLanguage(string language)
        {
            return LanguageValidator.IsValidLanguage(language);
        }

        public string NormalizeLanguage(string language)
        {
            return LanguageValidator.NormalizeLanguage(language);
        }
    }
}
