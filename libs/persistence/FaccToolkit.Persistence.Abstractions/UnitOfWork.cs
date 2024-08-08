using FaccToolkit.Domain.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FaccToolkit.Persistence.Abstractions
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected readonly IServiceProvider _serviceProvider;

        protected UnitOfWork(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        public virtual TRepository GetRepository<TRepository, TEntity, TId>()
            where TRepository : IRepository<TEntity, TId>
            where TEntity : class, IEntity<TId>
            where TId : IEquatable<TId>
            => _serviceProvider.GetRequiredService<TRepository>();

        public abstract Task<bool> IsInTransactionAsync(CancellationToken cancellationToken);

        public abstract Task BeginTransactionAsync(CancellationToken cancellationToken);

        public abstract Task CommitTransactionAsync(CancellationToken cancellationToken);

        public abstract Task AbortTransactionAsync(CancellationToken cancellationToken);

        public abstract void Dispose();
    }
}
