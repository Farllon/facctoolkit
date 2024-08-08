using FaccToolkit.Domain.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        TRepository GetRepository<TRepository, TEntity, TId>()
            where TRepository : IRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>;

        Task<bool> IsInTransactionAsync(CancellationToken cancellationToken);

        Task BeginTransactionAsync(CancellationToken cancellationToken);

        Task CommitTransactionAsync(CancellationToken cancellationToken);

        Task AbortTransactionAsync(CancellationToken cancellationToken);
    }
}
