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
        /// <summary>
        /// Retrieves a MongoDB collection for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities stored in the collection.</typeparam>
        /// <typeparam name="TId">The type of the identifier for the entities.</typeparam>
        /// <param name="collectionName">The name of the MongoDB collection to retrieve.</param>
        /// <returns>An <see cref="IMongoCollection{TEntity}"/> representing the MongoDB collection for the specified entity type.</returns>
        IMongoCollection<TEntity> GetCollection<TEntity, TId>(string collectionName)
            where TEntity : IEntity<TId>
            where TId : IEquatable<TId>;

        /// <summary>
        /// Creates an instance of a model repository for the specified model type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model managed by the repository.</typeparam>
        /// <param name="collectionName">The name of the MongoDB collection that stores the models.</param>
        /// <param name="logger">An optional logger instance to be used for logging operations within the repository.</param>
        /// <returns>An <see cref="IModelRepository{TModel}"/> instance for managing the specified model type.</returns>
        IModelRepository<TModel> GetModelRepository<TModel>(string collectionName, ILogger? logger = null)
            where TModel : class;

        /// <summary>
        /// Gets the current MongoDB client session, which is used for managing transactions and other session-related operations.
        /// </summary>
        IClientSessionHandle CurrentSession { get; }

        /// <summary>
        /// Starts a new transaction within the current session.
        /// </summary>
        /// <remarks>
        /// This method initiates a transaction that can be committed or aborted using the appropriate methods. Ensure that the session is active before starting a transaction.
        /// </remarks>
        void StartTransaction();

        /// <summary>
        /// Commits the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous commit operation.</returns>
        Task CommitTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Aborts the current transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous abort operation.</returns>
        Task AbortTransactionAsync(CancellationToken cancellationToken);
    }
}