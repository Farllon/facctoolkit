using FaccToolkit.Domain.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Retrieves an instance of a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TRepository">The type of the repository to retrieve.</typeparam>
        /// <typeparam name="TEntity">The type of the entities managed by the repository.</typeparam>
        /// <typeparam name="TId">The type of the identifier for the entities.</typeparam>
        /// <returns>An instance of <typeparamref name="TRepository"/> for managing the specified entity type.</returns>
        TRepository GetRepository<TRepository, TEntity, TId>()
            where TRepository : IRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>;

        /// <summary>
        /// Checks asynchronously if the unit of work is currently in a transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the unit of work is in a transaction.</returns>
        Task<bool> IsInTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Begins a new transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Commits the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Aborts the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AbortTransactionAsync(CancellationToken cancellationToken);
    }
}