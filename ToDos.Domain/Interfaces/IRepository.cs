using System.Linq.Expressions;
using ToDos.Domain.Interfaces;

namespace ToDos.Domain.Interfaces
{
    public interface IRepository<T, TId> where T : IBaseEntity<TId>
    {
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter);
        Task<T> SaveAsync(T entity);
        Task<T> DeleteAsync(TId id);
    }
}
