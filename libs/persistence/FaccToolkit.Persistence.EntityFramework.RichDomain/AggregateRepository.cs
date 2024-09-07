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

        public async Task<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding {Model} model with {Id} id in database", typeof(TAggregateRoot), id);

            try
            {
                var entity = await _entities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogInformation("Successfully found {Model} model with {Id} id", typeof(TAggregateRoot), id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to found {Model} model with {Id} id", typeof(TAggregateRoot), id);
                
                throw;
            }
        }

        public async Task InsertAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await InsertAsync(e => e.Id, aggregateRoot, cancellationToken);
            await DispatchAggregateEventsAsync(aggregateRoot, cancellationToken);
        }

        public override async Task InsertAsync(IEnumerable<TAggregateRoot> aggregates, CancellationToken cancellationToken)
        {
            await base.InsertAsync(aggregates, cancellationToken);

            await Task.WhenAll(aggregates
                .Select(aggregate => DispatchAggregateEventsAsync(aggregate, cancellationToken)));
        }

        public async Task UpdateAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await UpdateAsync(e => e.Id, aggregateRoot, cancellationToken);
            await DispatchAggregateEventsAsync(aggregateRoot, cancellationToken);
        }

        public async Task DeleteAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            await DeleteAsync(aggregateRoot.Id, cancellationToken);
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
