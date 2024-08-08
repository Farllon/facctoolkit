using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.RichDomain
{
    public class WriteRepository<TAggregateRoot, TId> : IWriteRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        protected readonly IMongoDbContext _context;
        protected readonly IDomainEventDispatcher _dispatcher;
        protected readonly IMongoCollection<TAggregateRoot> _collection;
        protected readonly IModelRepository<TAggregateRoot> _modelRepository;
        protected readonly ILogger<WriteRepository<TAggregateRoot, TId>> _logger;

        public WriteRepository(string collectionName, IMongoDbContext context, ILogger<WriteRepository<TAggregateRoot, TId>> logger, IDomainEventDispatcher dispatcher)
        {
            _logger = logger;
            _context = context;
            _dispatcher = dispatcher;
            _collection = context.GetCollection<TAggregateRoot, TId>(collectionName);
            _modelRepository = context.GetModelRepository<TAggregateRoot>(collectionName, logger);
        }

        public virtual async Task InsertAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            await _modelRepository.InsertAsync(ag => ag.Id, aggregate, cancellationToken);

            await DispatchAggregateEventsAsync(aggregate, cancellationToken);
        }

        public virtual async Task InsertAsync(IEnumerable<TAggregateRoot> aggregates, CancellationToken cancellationToken)
        {
            await _modelRepository.InsertAsync(ag => ag.Id, aggregates, cancellationToken);

            await Task.WhenAll(aggregates
                .Select(aggregate => DispatchAggregateEventsAsync(aggregate, cancellationToken)));
        }

        public virtual async Task UpdateAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            var updated = await _modelRepository.UpdateAsync(ag => ag.Id, aggregate, cancellationToken);

            if (updated != null)
                await DispatchAggregateEventsAsync(aggregate, cancellationToken);
        }

        public virtual async Task DeleteAsync(TAggregateRoot aggregate, CancellationToken cancellationToken)
        {
            var deleted = await _modelRepository.DeleteAsync(ag => ag.Id, aggregate.Id, cancellationToken);

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

            _logger.LogInformation("Clearing the {Aggregate} aggregate domain events");

            aggregateRoot.ClearEvents();
        }
    }
}
