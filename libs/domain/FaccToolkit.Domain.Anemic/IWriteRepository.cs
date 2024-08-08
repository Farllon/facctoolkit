using FaccToolkit.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Anemic
{
    public interface IWriteRepository<in TEntity, in TId> : IRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken);

        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync(TId id, CancellationToken cancellationToken);
    }
}
