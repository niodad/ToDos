using System.Linq.Expressions;
using ToDos.Api.Models;
using ToDos.Domain.Interfaces;

namespace ToDos.Api.Interfaces
{
    public interface IPaginatedRepository<T, TId> : IRepository<T, TId> where T : IBaseEntity<TId>
    {
        Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> filter, PagedQuery query);
        Task<int> GetCountAsync(Expression<Func<T, bool>> filter);
    }
}
