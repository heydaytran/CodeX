using Domain.Primitives;

namespace Persistence;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T, TKey> Repository<T, TKey>() where T : Entity<TKey> where TKey : notnull;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
}