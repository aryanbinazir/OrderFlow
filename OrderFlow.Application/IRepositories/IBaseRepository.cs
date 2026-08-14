using System.Linq.Expressions;

namespace OrderFlow.Infrastructure.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false);
        Task<T?> Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        Task Add(T entity);
        Task<bool> Any(Expression<Func<T, bool>> filter);
    }
}
