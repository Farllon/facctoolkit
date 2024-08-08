using FaccToolkit.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    public interface IMongoDbContext : IDisposable
    {
        IMongoCollection<TEntity> GetCollection<TEntity, TId>(string collectionName)
            where TEntity : IEntity<TId>
            where TId : IEquatable<TId>;

        IModelRepository<TModel> GetModelRepository<TModel>(string collectionName, ILogger? logger = null)
            where TModel : class;

        IClientSessionHandle CurrentSession { get; }

        void StartTransaction();

        Task CommitTransactionAsync(CancellationToken cancellationToken);

        Task AbortTransactionAsync(CancellationToken cancellationToken);
    }
}
