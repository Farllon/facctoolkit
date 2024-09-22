using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.RichDomain
{
    public class AggregateRepository<TAggregateRoot, TId> : MongoDocumentRepository<TAggregateRoot>, IAggregateRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        protected readonly IDomainEventDispatcher _dispatcher;

        public AggregateRepository(string collectionName, IMongoDbContext context, ILogger<AggregateRepository<TAggregateRoot, TId>> logger, IDomainEventDispatcher dispatcher)
            : base(logger, context, context.GetCollection<TAggregateRoot, TId>(collectionName))
        {
            _dispatcher = dispatcher;
        }

        public virtual Task<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => base.FindByIdAsync(aggregate => aggregate.Id, id, cancellationToken);

        public virtual async Task InsertAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            await base.InsertAsync(ag => ag.Id, aggregate, cancellationToken);

            await DispatchAggregateEventsAsync(aggregate, cancellationToken);
        }

        public virtual async Task InsertAsync(IEnumerable<TAggregateRoot> aggregates, CancellationToken cancellationToken)
        {
            await base.InsertAsync<TId>(aggregates, cancellationToken);

            await Task.WhenAll(aggregates
                .Select(aggregate => DispatchAggregateEventsAsync(aggregate, cancellationToken)));
        }

        public virtual async Task UpdateAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            var updated = await base.UpdateAsync(ag => ag.Id, aggregate, cancellationToken);

            if (updated != null)
                await DispatchAggregateEventsAsync(aggregate, cancellationToken);
        }

        public virtual async Task DeleteAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            var deleted = await base.DeleteAsync(ag => ag.Id, aggregate.Id, cancellationToken);

            if (deleted != null)
                await DispatchAggregateEventsAsync(aggregate, cancellationToken);
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
