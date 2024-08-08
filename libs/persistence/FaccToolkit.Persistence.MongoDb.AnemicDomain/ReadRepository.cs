using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.AnemicDomain
{
    public class ReadRepository<TEntity, TId> : IReadRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        protected readonly IMongoDbContext _context;
        protected readonly IMongoCollection<TEntity> _collection;
        protected readonly IModelRepository<TEntity> _modelRepository;
        protected readonly ILogger<ReadRepository<TEntity, TId>> _logger;

        public ReadRepository(string collectionName, IMongoDbContext context, ILogger<ReadRepository<TEntity, TId>> logger)
        {
            _logger = logger;
            _context = context;
            _collection = context.GetCollection<TEntity, TId>(collectionName);
            _modelRepository = context.GetModelRepository<TEntity>(collectionName, logger);
        }

        public virtual Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => _modelRepository.FindByIdAsync(entity => entity.Id, id, cancellationToken);
    }
}
