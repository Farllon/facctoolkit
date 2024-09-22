using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.EntityFramework.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FaccToolkit.Persistence.EntityFramework.RichDomain
{
    public class AggregateRepository<TAggregateRoot, TId, TDbContext> : BaseRepository<TAggregateRoot, TDbContext>, IAggregateRepository<TAggregateRoot, TId>
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

        public Task<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => base.FindByIdAsync(e => e.Id, id, cancellationToken);

        public async Task InsertAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await base.InsertAsync(e => e.Id, aggregateRoot, cancellationToken);
            await DispatchAggregateEventsAsync(aggregateRoot, cancellationToken);
        }

        public async Task InsertAsync(IEnumerable<TAggregateRoot> aggregates, CancellationToken cancellationToken)
        {
            await base.InsertAsync<TId>(aggregates, cancellationToken);

            await Task.WhenAll(aggregates
                .Select(aggregate => DispatchAggregateEventsAsync(aggregate, cancellationToken)));
        }

        public async Task UpdateAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await base.UpdateAsync(e => e.Id, aggregateRoot, cancellationToken);
            await DispatchAggregateEventsAsync(aggregateRoot, cancellationToken);
        }

        public async Task DeleteAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await base.DeleteAsync(e => e.Id, aggregateRoot.Id, cancellationToken);
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
    }
}
