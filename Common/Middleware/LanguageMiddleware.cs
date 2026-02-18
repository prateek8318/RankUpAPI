using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Common.Language;

namespace Common.Middleware
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LanguageMiddleware> _logger;

        public LanguageMiddleware(RequestDelegate next, ILogger<LanguageMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var languageHeader = context.Request.Headers[LanguageConstants.AcceptLanguageHeader].FirstOrDefault();
            var normalizedLanguage = LanguageValidator.NormalizeLanguage(languageHeader);
            
            // Store in HttpContext for access throughout the request
            context.Items["Language"] = normalizedLanguage;
            
            // Add to response header for debugging
            context.Response.Headers[LanguageConstants.AcceptLanguageHeader] = normalizedLanguage;
            
            _logger.LogDebug("Language set to: {Language}", normalizedLanguage);
            
            await _next(context);
        }
    }

    public static class LanguageMiddlewareExtensions
    {
        public static IApplicationBuilder UseLanguage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LanguageMiddleware>();
        }
    }
}
