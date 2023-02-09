using Gitos.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ToDos.Domain.Interfaces;

namespace ToDos.Infrastructure.Data.EntityFramework
{
    public class InmemoryRespository<T, Id> : IRepository<T, Guid> where T : class, IBaseEntity<Guid>
    {
        public ToDosDbContext _dbContext;

        public InmemoryRespository(ToDosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> DeleteAsync(Guid id)
        {
            var item = _dbContext.Set<T>().Single(x => x.Id == id);
            _dbContext.Set<T>().Remove(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbContext.Set<T>().Where(filter).ToListAsync();
        }

        public async Task<T> SaveAsync(T entity)
        {
            if (entity.Id == Guid.Empty)
            {
                await _dbContext.Set<T>().AddAsync(entity);
            }
            else
            {
                var dbEntity = _dbContext.Set<T>().Single(x => x.Id == entity.Id);
                _dbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
            }
            _dbContext.SaveChanges();
            return entity;
        }
    }
}
