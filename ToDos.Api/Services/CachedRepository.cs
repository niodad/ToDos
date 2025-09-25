using System.Linq.Expressions;
using ToDos.Domain.Interfaces;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Services
{
    public class CachedRepository<T, TId> : IRepository<T, TId> where T : IBaseEntity<TId>
    {
        private readonly IRepository<T, TId> _repository;
        private readonly ICachingService _cachingService;
        private readonly ILogger<CachedRepository<T, TId>> _logger;

        public CachedRepository(
            IRepository<T, TId> repository, 
            ICachingService cachingService, 
            ILogger<CachedRepository<T, TId>> logger)
        {
            _repository = repository;
            _cachingService = cachingService;
            _logger = logger;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            // For complex queries, we don't cache by default
            // In a real implementation, you might want to hash the expression
            return await _repository.GetAsync(filter);
        }

        public async Task<T> SaveAsync(T entity)
        {
            var result = await _repository.SaveAsync(entity);
            
            // Cache the saved entity
            var cacheKey = GetCacheKey(result.Id);
            await _cachingService.SetAsync(cacheKey, result, TimeSpan.FromHours(1));
            
            // Invalidate related caches (e.g., user's todo list)
            await InvalidateRelatedCaches(entity);
            
            _logger.LogDebug("Cached entity with key: {CacheKey}", cacheKey);
            return result;
        }

        public async Task<T> DeleteAsync(TId id)
        {
            var result = await _repository.DeleteAsync(id);
            
            if (result != null)
            {
                // Remove from cache
                var cacheKey = GetCacheKey(id);
                await _cachingService.RemoveAsync(cacheKey);
                
                // Invalidate related caches
                await InvalidateRelatedCaches(result);
                
                _logger.LogDebug("Removed entity from cache with key: {CacheKey}", cacheKey);
            }
            
            return result;
        }

        public async Task<T?> GetByIdAsync(TId id)
        {
            var cacheKey = GetCacheKey(id);
            var cachedEntity = await _cachingService.GetAsync<T>(cacheKey);
            
            if (cachedEntity != null)
            {
                _logger.LogDebug("Cache hit for entity with key: {CacheKey}", cacheKey);
                return cachedEntity;
            }

            _logger.LogDebug("Cache miss for entity with key: {CacheKey}", cacheKey);
            
            // Get from repository and cache it
            var entities = await _repository.GetAsync(e => e.Id.Equals(id));
            var entity = entities.FirstOrDefault();
            
            if (entity != null)
            {
                await _cachingService.SetAsync(cacheKey, entity, TimeSpan.FromHours(1));
                _logger.LogDebug("Cached entity with key: {CacheKey}", cacheKey);
            }
            
            return entity;
        }

        private string GetCacheKey(TId id)
        {
            return $"todo:{typeof(T).Name.ToLower()}:{id}";
        }

        private async Task InvalidateRelatedCaches(T entity)
        {
            // Invalidate user's todo list cache
            if (entity is ToDo todo)
            {
                var userTodosKey = $"user_todos:{todo.Email}";
                await _cachingService.RemoveAsync(userTodosKey);
                _logger.LogDebug("Invalidated user todos cache for email: {Email}", todo.Email);
            }
        }
    }
}
