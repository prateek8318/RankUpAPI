using Microsoft.AspNetCore.Http;
using Common.Language;

namespace Common.HttpClient
{
    public class LanguageHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items["Language"] is string language)
            {
                // Forward the Accept-Language header to downstream services
                request.Headers.Remove(LanguageConstants.AcceptLanguageHeader);
                request.Headers.Add(LanguageConstants.AcceptLanguageHeader, language);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
