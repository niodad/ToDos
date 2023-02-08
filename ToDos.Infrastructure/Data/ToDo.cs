using MongoDB.Bson.Serialization.Attributes;
using ToDos.Domain.Interfaces;

namespace ToDos.Infrastructure.Data
{
    public class ToDo : Domain.Entities.ToDo, IBaseEntity<Guid>
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}
