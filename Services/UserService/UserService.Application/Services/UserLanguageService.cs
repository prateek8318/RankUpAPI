using Common.Language;
using Common.Services;
using UserService.Application.Interfaces;

namespace UserService.Application.Services
{
    public interface IUserLanguageService
    {
        Task<Dictionary<string, object>> GetStatesAsync(string? language = null);
        Task<Dictionary<string, object>> GetQualificationsAsync(string? language = null);
        Task<Dictionary<string, object>> GetExamCategoriesAsync(string? language = null);
        Task<Dictionary<string, object>> GetCategoriesAsync(string? language = null);
        Task<Dictionary<string, object>> GetStreamsAsync(string? language = null);
        Task<Dictionary<string, object>> GetAllUserDataAsync(string? language = null);
    }

    public class UserLanguageService : IUserLanguageService
    {
        private readonly IUserService _userService;
        private readonly ILanguageService _languageService;
        private readonly ILanguageDataService _languageDataService;

        public UserLanguageService(
            IUserService userService,
            ILanguageService languageService,
            ILanguageDataService languageDataService)
        {
            _userService = userService;
            _languageService = languageService;
            _languageDataService = languageDataService;
        }

        public async Task<Dictionary<string, object>> GetStatesAsync(string? language = null)
        {
            var currentLanguage = language ?? _languageService.GetCurrentLanguage();
            return await _languageDataService.GetLocalizedDataAsync(currentLanguage, "states");
        }

        public async Task<Dictionary<string, object>> GetQualificationsAsync(string? language = null)
        {
            var currentLanguage = language ?? _languageService.GetCurrentLanguage();
            return await _languageDataService.GetLocalizedDataAsync(currentLanguage, "qualifications");
        }

        public async Task<Dictionary<string, object>> GetExamCategoriesAsync(string? language = null)
        {
            var currentLanguage = language ?? _languageService.GetCurrentLanguage();
            return await _languageDataService.GetLocalizedDataAsync(currentLanguage, "examCategories");
        }

        public async Task<Dictionary<string, object>> GetCategoriesAsync(string? language = null)
        {
            var currentLanguage = language ?? _languageService.GetCurrentLanguage();
            return await _languageDataService.GetLocalizedDataAsync(currentLanguage, "categories");
        }

        public async Task<Dictionary<string, object>> GetStreamsAsync(string? language = null)
        {
            var currentLanguage = language ?? _languageService.GetCurrentLanguage();
            return await _languageDataService.GetLocalizedDataAsync(currentLanguage, "streams");
        }

        public async Task<Dictionary<string, object>> GetAllUserDataAsync(string? language = null)
        {
            var currentLanguage = language ?? _languageService.GetCurrentLanguage();
            
            var result = new Dictionary<string, object>();
            
            // Get all categories data
            var states = await _languageDataService.GetLocalizedDataAsync(currentLanguage, "states");
            var qualifications = await _languageDataService.GetLocalizedDataAsync(currentLanguage, "qualifications");
            var examCategories = await _languageDataService.GetLocalizedDataAsync(currentLanguage, "examCategories");
            
            result["states"] = states["states"];
            result["qualifications"] = qualifications["qualifications"];
            result["examCategories"] = examCategories["examCategories"];
            result["currentLanguage"] = currentLanguage;
            
            return result;
        }
    }
}
