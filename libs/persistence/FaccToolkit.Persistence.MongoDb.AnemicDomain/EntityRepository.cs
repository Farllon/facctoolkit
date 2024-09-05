using FaccToolkit.Domain.Abstractions;
using FaccToolkit.Domain.Anemic;
using FaccToolkit.Persistence.MongoDb.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.AnemicDomain
{
    public class EntityRepository<TEntity, TId> : IEntityRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        protected readonly IMongoDbContext _context;
        protected readonly IMongoCollection<TEntity> _collection;
        protected readonly IModelRepository<TEntity> _modelRepository;
        protected readonly ILogger<EntityRepository<TEntity, TId>> _logger;
        
        public EntityRepository(string collectionName, IMongoDbContext context, ILogger<EntityRepository<TEntity, TId>> logger)
        {
            _logger = logger;
            _context = context;
            _collection = context.GetCollection<TEntity, TId>(collectionName);
            _modelRepository = context.GetModelRepository<TEntity>(collectionName, logger);
        }

        public virtual Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => _modelRepository.FindByIdAsync(entity => entity.Id, id, cancellationToken);

        public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
            => _modelRepository.InsertAsync(entity => entity.Id, entity, cancellationToken);

        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
            => _modelRepository.InsertAsync(entity => entity.Id, entities, cancellationToken);

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
            => _modelRepository.UpdateAsync(entity => entity.Id, entity, cancellationToken);

        public virtual Task DeleteAsync(TId id, CancellationToken cancellationToken)
            => _modelRepository.DeleteAsync(entity => entity.Id, id, cancellationToken);
    }
}
