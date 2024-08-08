using FaccToolkit.Domain.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Rich
{
    public interface IReadRepository<TAggregateRoot, in TId> : IRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        Task<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken cancellationToken);
    }
}
