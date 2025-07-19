using Application.Common;
using Domain.Primitives;

namespace Persistence;

public interface IGenericRepository<T, in TKey> where T : Entity<TKey> where TKey : notnull
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(TKey id);
    Task AddAsync(T entity);
    void Update(T entity);
    Task UpsertAsync(T entity);
    void Delete(T entity);
    Task AddRangeAsync(IEnumerable<T> entities); 
    Task<PagedResult<T>> GetPagedAsync(QueryParameters<T> queryParameters);
    Task<ErrorOr<Unit>> SaveChangesAsync(bool entityEvents = true, CancellationToken cancellationToken = default(CancellationToken));

}
