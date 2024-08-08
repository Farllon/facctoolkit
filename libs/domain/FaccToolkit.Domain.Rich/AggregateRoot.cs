using FaccToolkit.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Rich
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        private readonly List<IDomainEvent> _events;

        public IReadOnlyCollection<IDomainEvent> Events => _events;

        protected AggregateRoot(TId id) : base(id)
        {
            _events = new List<IDomainEvent>();
        }

        public void ClearEvents()
            => _events.Clear();

        public void EnqueueEvent<TDomainEvent>(TDomainEvent @event)
            where TDomainEvent : IDomainEvent
            => _events.Add(@event);

        public async Task DispatchAsync(IDomainEventDispatcher dispatcher, CancellationToken cancellationToken)
        {
            foreach (var @event in _events)
                await dispatcher.DispatchAsync(@event, cancellationToken);
        }
    }
}
