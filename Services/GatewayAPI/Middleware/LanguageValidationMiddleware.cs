using Common.Language;

namespace GatewayAPI.Middleware
{
    public class LanguageValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LanguageValidationMiddleware> _logger;

        public LanguageValidationMiddleware(RequestDelegate next, ILogger<LanguageValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var languageHeader = context.Request.Headers[LanguageConstants.AcceptLanguageHeader].FirstOrDefault();
            
            if (!string.IsNullOrWhiteSpace(languageHeader))
            {
                if (!LanguageValidator.IsValidLanguage(languageHeader))
                {
                    _logger.LogWarning("Invalid language header received: {Language}", languageHeader);
                    
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        error = "Invalid language header",
                        message = $"Supported languages: {string.Join(", ", LanguageConstants.SupportedLanguages)}",
                        supportedLanguages = LanguageConstants.SupportedLanguages
                    });
                    return;
                }
            }
            
            await _next(context);
        }
    }

    public static class LanguageValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseLanguageValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LanguageValidationMiddleware>();
        }
    }
}
