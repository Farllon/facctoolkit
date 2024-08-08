using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Rich.Extensions.MediatR
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task DispatchAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken) 
            where TDomainEvent : IDomainEvent
            => _mediator.Publish(@event, cancellationToken);
    }
}
