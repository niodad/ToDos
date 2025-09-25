using ToDos.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace ToDos.Infrastructure.Data.Cosmos
{
    public class CosmosDbRepository<T, TId> : IRepository<T, TId> where T : IBaseEntity<TId>
    {
        private IMongoCollection<T>? _mongoCollection;
        private readonly IMongoDatabase _mongoDB;
        private readonly ILogger<CosmosDbRepository<T, TId>> _logger;

        public CosmosDbRepository(IConfiguration configuration, ILogger<CosmosDbRepository<T, TId>> logger)
        {
            _logger = logger;
            
            // Configure MongoDB client with connection pooling and performance optimizations
            var connectionString = configuration["MongoDBSettings:ConnectionString"];
            var databaseName = configuration["MongoDBSettings:DatabaseName"];
            
            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
            
            // Performance optimizations
            clientSettings.MaxConnectionPoolSize = 100;
            clientSettings.MinConnectionPoolSize = 5;
            clientSettings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);
            clientSettings.MaxConnectionLifeTime = TimeSpan.FromMinutes(30);
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(10);
            clientSettings.SocketTimeout = TimeSpan.FromSeconds(30);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
            
            // Enable compression (simplified for compatibility)
            // clientSettings.Compressors = new[] { new CompressorConfiguration(CompressorType.Zstd) };
            
            var client = new MongoClient(clientSettings);
            _mongoDB = client.GetDatabase(databaseName);
            
            _logger.LogInformation("MongoDB connection configured with optimized settings");
        }
        public async Task<T> SaveAsync(T entity)
        {
            try
            {
                _mongoCollection = _mongoDB.GetCollection<T>(typeof(T).Name);
                
                if (entity.Id == null || entity.Id.Equals(default(TId)))
                {
                    if (typeof(TId) == typeof(Guid))
                    {
                        entity.Id = (TId)(object)Guid.NewGuid();
                    }
                    await _mongoCollection.InsertOneAsync(entity);
                    _logger.LogDebug("Inserted new entity with ID: {Id}", entity.Id);
                }
                else
                {
                    var result = await _mongoCollection.ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity);
                    _logger.LogDebug("Updated entity with ID: {Id}, Matched: {MatchedCount}", entity.Id, result.MatchedCount);
                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving entity with ID: {Id}", entity.Id);
                throw;
            }
        }
        public async Task<T> DeleteAsync(TId id)
        {
            try
            {
                _mongoCollection = _mongoDB.GetCollection<T>(typeof(T).Name);
                var result = await _mongoCollection.FindOneAndDeleteAsync(e => e.Id.Equals(id));
                _logger.LogDebug("Deleted entity with ID: {Id}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity with ID: {Id}", id);
                throw;
            }
        }
        
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                _mongoCollection = _mongoDB.GetCollection<T>(typeof(T).Name);
                var cursor = await _mongoCollection.FindAsync(filter);
                var results = cursor.ToEnumerable();
                _logger.LogDebug("Retrieved {Count} entities", results.Count());
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entities with filter");
                throw;
            }
        }
    }
}
