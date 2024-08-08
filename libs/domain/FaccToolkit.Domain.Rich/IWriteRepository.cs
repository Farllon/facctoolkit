using FaccToolkit.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Rich
{
    public interface IWriteRepository<in TAggregateRoot, in TId> : IRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        Task InsertAsync(TAggregateRoot aggregate, CancellationToken cancellationToken);

        Task InsertAsync(IEnumerable<TAggregateRoot> aggregates, CancellationToken cancellationToken);

        Task UpdateAsync(TAggregateRoot aggregate, CancellationToken cancellationToken);

        Task DeleteAsync(TAggregateRoot aggregate, CancellationToken cancellationToken);
    }
}
