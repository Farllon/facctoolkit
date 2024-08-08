using System;

namespace FaccToolkit.Domain.Abstractions
{
    public interface IEntity<out TId>
        where TId : IEquatable<TId>
    {
        TId Id { get; }
    }
}
