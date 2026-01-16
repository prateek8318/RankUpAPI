using AdminService.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace AdminService.API.Middleware
{
    // Service-to-service authentication middleware
    public class ServiceAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ServiceAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for login endpoints
            if (context.Request.Path.StartsWithSegments("/api/auth") || 
                context.Request.Path.StartsWithSegments("/api/admin/auth") ||
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Get AuthService from service provider
            var authService = context.RequestServices.GetRequiredService<AuthService>();

            // Check for Authorization header
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Missing Authorization header");
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].ToString();
            
            // Extract token from "Bearer <token>" format
            if (authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                // Validate user token
                if (await authService.ValidateUserTokenAsync(token))
                {
                    // Add user info to context for further use
                    context.Items["UserToken"] = token;
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Invalid or expired token");
                }
            }
            else
            {
                // Check for service token (internal communication)
                var serviceToken = authHeader;
                if (authService.ValidateServiceToken(serviceToken, "AdminService"))
                {
                    context.Items["ServiceToken"] = serviceToken;
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Invalid service token");
                }
            }
        }
    }
}
