using FaccToolkit.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Rich
{
    public interface IAggregateRoot<out TId> : IEntity<TId>
        where TId : IEquatable<TId>
    {
        IReadOnlyCollection<IDomainEvent> Events { get; }

        void EnqueueEvent<TDomainEvent>(TDomainEvent @event)
            where TDomainEvent : IDomainEvent;

        void ClearEvents();

        Task DispatchAsync(IDomainEventDispatcher dispatcher, CancellationToken cancellationToken);
    }
}
