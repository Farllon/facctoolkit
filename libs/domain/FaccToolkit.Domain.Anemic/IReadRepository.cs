using FaccToolkit.Domain.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Anemic
{
    public interface IReadRepository<TEntity, in TId> : IRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken);
    }
}
