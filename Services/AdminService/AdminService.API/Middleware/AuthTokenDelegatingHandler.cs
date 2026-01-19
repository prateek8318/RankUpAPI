using System.Net.Http.Headers;

namespace AdminService.API.Middleware
{
    public class AuthTokenDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
