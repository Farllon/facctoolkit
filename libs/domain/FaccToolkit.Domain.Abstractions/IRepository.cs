using System;

namespace FaccToolkit.Domain.Abstractions
{
    public interface IRepository<in TEntity, in TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
    }
}
