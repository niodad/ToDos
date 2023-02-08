using System.Linq.Expressions;
using ToDos.Domain.Interfaces;

namespace Gitos.Domain.Repository
{
    public interface IRepository<T, Id> where T : IBaseEntity<Id>
    {
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter);
        Task<T> SaveAsync(T entity);
        Task<T> DeleteAsync(Id id);
    }
}
