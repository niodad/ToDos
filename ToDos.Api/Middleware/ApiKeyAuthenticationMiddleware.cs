using ToDos.Api.Services;

namespace ToDos.Api.Middleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

        public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApiKeyAuthenticationService authService)
        {
            // Skip authentication for Swagger endpoints
            if (context.Request.Path.StartsWithSegments("/swagger") || 
                context.Request.Path.StartsWithSegments("/api-docs"))
            {
                await _next(context);
                return;
            }

            // Skip authentication for health checks
            if (context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            string? apiKey = null;

            // Check for API key in header
            if (context.Request.Headers.TryGetValue("X-API-Key", out var headerValue))
            {
                apiKey = headerValue.FirstOrDefault();
            }
            // Check for API key in query parameter (for testing purposes)
            else if (context.Request.Query.TryGetValue("apiKey", out var queryValue))
            {
                apiKey = queryValue.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("No API key provided for request to {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API key is required");
                return;
            }

            if (!authService.ValidateApiKey(apiKey))
            {
                _logger.LogWarning("Invalid API key provided for request to {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }

            // Set the user principal
            context.User = authService.CreatePrincipal(apiKey);

            _logger.LogInformation("API key authentication successful for request to {Path}", context.Request.Path);
            await _next(context);
        }
    }
}
