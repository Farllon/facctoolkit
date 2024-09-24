using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.EntityFramework.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FaccToolkit.Persistence.EntityFramework.RichDomain
{
    public class AggregateRepository<TAggregateRoot, TId, TDbContext> : BaseRepository<TAggregateRoot, TId, TDbContext>, IAggregateRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
        where TDbContext : DbContext
    {
        protected readonly DbSet<TAggregateRoot> _entities;
        protected readonly IDomainEventDispatcher _dispatcher;

        public AggregateRepository(TDbContext context, ILogger<AggregateRepository<TAggregateRoot, TId, TDbContext>> logger, IDomainEventDispatcher domainEventDispatcher)
            : base(context, logger)
        {
            _entities = _context.Set<TAggregateRoot>();
            _dispatcher = domainEventDispatcher;
        }

        async Task IAggregateRepository<TAggregateRoot, TId>.InsertAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await base.InsertAsync(aggregateRoot, cancellationToken);
            await DispatchAggregateEventsAsync(aggregateRoot, cancellationToken);
        }

        async Task IAggregateRepository<TAggregateRoot, TId>.InsertAsync(IEnumerable<TAggregateRoot> aggregates, CancellationToken cancellationToken)
        {
            await base.InsertAsync(aggregates, cancellationToken);

            await Task.WhenAll(aggregates
                .Select(aggregate => DispatchAggregateEventsAsync(aggregate, cancellationToken)));
        }

        async Task IAggregateRepository<TAggregateRoot, TId>.UpdateAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await base.UpdateAsync(aggregateRoot, cancellationToken);
            await DispatchAggregateEventsAsync(aggregateRoot, cancellationToken);
        }

        async Task IAggregateRepository<TAggregateRoot, TId>.DeleteAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await base.DeleteAsync(aggregateRoot.Id, cancellationToken);
            await DispatchAggregateEventsAsync(aggregateRoot, cancellationToken);
        }

        protected async Task DispatchAggregateEventsAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting domain events dipatch of {Aggregate} aggregate with {Id} id", typeof(TAggregateRoot), aggregateRoot.Id);

            foreach (var @event in aggregateRoot.Events)
                try
                {
                    _logger.LogInformation("Dipatching {Event} event", @event.GetType());

                    await _dispatcher.DispatchAsync(@event, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error was occurred on {Event} event dispatch", @event.GetType());

                    throw;
                }

            _logger.LogInformation("Clearing the {Aggregate} aggregate domain events", typeof(TAggregateRoot));

            aggregateRoot.ClearEvents();
        }

        protected override TId GetId(TAggregateRoot model)
            => model.Id;
    }
}
