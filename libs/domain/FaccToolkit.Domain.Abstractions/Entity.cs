using System;

namespace FaccToolkit.Domain.Abstractions
{
    public abstract class Entity<TId> : IEntity<TId>
        where TId : IEquatable<TId>
    {
        public TId Id { get; protected set; }

        protected Entity(TId id) 
        {
            Id = id;
        }
    }
}
