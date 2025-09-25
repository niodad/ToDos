using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ToDos.Api.Services
{
    public interface ICachingService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }

    public class CachingService : ICachingService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachingService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public CachingService(IDistributedCache cache, ILogger<CachingService> logger)
        {
            _cache = cache;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var cachedValue = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cachedValue))
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return default(T);
                }

                _logger.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                var options = new DistributedCacheEntryOptions();

                if (expiration.HasValue)
                {
                    options.SetAbsoluteExpiration(expiration.Value);
                }
                else
                {
                    // Default expiration: 1 hour
                    options.SetAbsoluteExpiration(TimeSpan.FromHours(1));
                }

                await _cache.SetStringAsync(key, serializedValue, options);
                _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Removed cache for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                // Note: This is a simplified implementation
                // In production, you might want to use Redis SCAN or maintain a pattern registry
                _logger.LogWarning("RemoveByPatternAsync is not fully implemented for pattern: {Pattern}", pattern);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by pattern: {Pattern}", pattern);
            }
        }
    }
}
