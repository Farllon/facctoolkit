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
    public class EntityRepository<TEntity, TId> : MongoDocumentRepository<TEntity>, IEntityRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public EntityRepository(string collectionName, IMongoDbContext context, ILogger<EntityRepository<TEntity, TId>> logger)
            : base(logger, context, context.GetCollection<TEntity, TId>(collectionName))
        {

        }

        public virtual Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken)
            => base.FindByIdAsync(entity => entity.Id, id, cancellationToken);

        public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
            => base.InsertAsync(entity => entity.Id, entity, cancellationToken);

        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
            => base.InsertAsync<TId>(entities, cancellationToken);

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
            => base.UpdateAsync(entity => entity.Id, entity, cancellationToken);

        public virtual Task DeleteAsync(TId id, CancellationToken cancellationToken)
            => base.DeleteAsync(entity => entity.Id, id, cancellationToken);
    }
}
