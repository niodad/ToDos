using Gitos.Domain.Repository;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq.Expressions;
using ToDos.Domain.Interfaces;

namespace ToDos.Infrastructure.Data.Cosmos
{
    public class CosmosDbRepository<T, Id> : IRepository<T, Guid> where T : IBaseEntity<Guid>
    {
        private IMongoCollection<T>? _mongoCollection;
        private readonly IMongoDatabase _mongoDB;

        public CosmosDbRepository(IConfiguration configuration)
        {
            _mongoDB = new MongoClient(configuration["MongoDBSettings:ConnectionString"])
                .GetDatabase(configuration["MongoDBSettings:DatabaseName"]);
        }
        public async Task<T> SaveAsync(T entity)
        {
            _mongoCollection = _mongoDB.GetCollection<T>(typeof(T).Name);
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                await _mongoCollection.InsertOneAsync(entity);
            }
            else
            {
                await _mongoCollection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
            }
            return entity;
        }
        public async Task<T> DeleteAsync(Guid id)
        {
            _mongoCollection = _mongoDB.GetCollection<T>(typeof(T).Name);
            return await _mongoCollection.FindOneAndDeleteAsync(e => e.Id == id);
        }
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            _mongoCollection = _mongoDB.GetCollection<T>(typeof(T).Name);
            var cursor = await _mongoCollection.FindAsync(filter);
            return cursor.ToEnumerable();

        }
    }
}
