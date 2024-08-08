using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Domain.Rich
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken)
            where TDomainEvent : IDomainEvent;
    }
}
