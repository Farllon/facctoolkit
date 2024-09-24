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
    public class EntityRepository<TEntity, TId> : MongoDocumentRepository<TEntity, TId>, IEntityRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public EntityRepository(string collectionName, IMongoDbContext context, ILogger<EntityRepository<TEntity, TId>> logger)
            : base(logger, context, context.GetCollection<TEntity, TId>(collectionName))
        {

        }

        Task IEntityRepository<TEntity, TId>.UpdateAsync(TEntity entity, CancellationToken cancellationToken)
            => base.UpdateAsync(entity, cancellationToken);

        Task IEntityRepository<TEntity, TId>.DeleteAsync(TId id, CancellationToken cancellationToken)
            => base.DeleteAsync(id, cancellationToken);

        protected override TId GetId(TEntity model)
            => model.Id;
    }
}
