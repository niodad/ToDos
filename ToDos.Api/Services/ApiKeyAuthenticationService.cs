using System.Security.Claims;

namespace ToDos.Api.Services
{
    public class ApiKeyAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyAuthenticationService> _logger;

        public ApiKeyAuthenticationService(IConfiguration configuration, ILogger<ApiKeyAuthenticationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool ValidateApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("API key is null or empty");
                return false;
            }

            var validApiKey = _configuration["ApiSettings:ApiKey"];
            if (string.IsNullOrEmpty(validApiKey))
            {
                _logger.LogError("No valid API key configured");
                return false;
            }

            var isValid = apiKey.Equals(validApiKey, StringComparison.OrdinalIgnoreCase);
            if (!isValid)
            {
                _logger.LogWarning("Invalid API key provided");
            }

            return isValid;
        }

        public ClaimsPrincipal CreatePrincipal(string apiKey)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "ApiUser"),
                new(ClaimTypes.AuthenticationMethod, "ApiKey"),
                new("ApiKey", apiKey)
            };

            var identity = new ClaimsIdentity(claims, "ApiKey");
            return new ClaimsPrincipal(identity);
        }
    }
}
