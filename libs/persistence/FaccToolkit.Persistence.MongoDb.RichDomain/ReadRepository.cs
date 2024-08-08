using FaccToolkit.Domain.Rich;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.RichDomain
{
    public class ReadRepository<TAggregateRoot, TId> : IReadRepository<TAggregateRoot, TId>
        where TAggregateRoot : class, IAggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        protected readonly IMongoDbContext _context;
        protected readonly IMongoCollection<TAggregateRoot> _collection;
        protected readonly IModelRepository<TAggregateRoot> _modelRepository;
        protected readonly ILogger<ReadRepository<TAggregateRoot, TId>> _logger;

        public ReadRepository(string collectionName, IMongoDbContext context, ILogger<ReadRepository<TAggregateRoot, TId>> logger)
        {
            _logger = logger;
            _context = context;
            _collection = context.GetCollection<TAggregateRoot, TId>(collectionName);
            _modelRepository = context.GetModelRepository<TAggregateRoot>(collectionName, logger);
        }

        public virtual Task<TAggregateRoot?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => _modelRepository.FindByIdAsync(aggregate => aggregate.Id, id, cancellationToken);
    }
}
