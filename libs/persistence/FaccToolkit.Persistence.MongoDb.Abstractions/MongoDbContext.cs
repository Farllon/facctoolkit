using FaccToolkit.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    public abstract class MongoDbContext : IMongoDbContext
    {
        protected readonly IMongoClient _client;
        protected readonly IMongoDatabase _database;
        private IClientSessionHandle? _session;
        public IClientSessionHandle CurrentSession => _session!;
        protected ILogger<MongoDbContext> _logger;
        
        protected MongoDbContext(
            string databaseName,
            IMongoClient client, 
            ILogger<MongoDbContext> logger)
        {
            _client = client;
            _logger = logger;
            _database = _client.GetDatabase(databaseName);

            StartNewSession();
        }

        private void StartNewSession()
        {
            using var _ = _logger.BeginScope("Start new mongodb session flow");

            try
            {
                _logger.LogInformation("Starting the new session");
                _session = _client.StartSession();
                _logger.LogInformation("The new session was started");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error was occured in the start mongodb session flow");
                throw;
            }
        }

        public virtual IMongoCollection<TEntity> GetCollection<TEntity, TId>(string collectionName)
            where TEntity : IEntity<TId>
            where TId : IEquatable<TId>
            => _database.GetCollection<TEntity>(collectionName);

        public virtual IModelRepository<TModel> GetModelRepository<TModel>(string collectionName, ILogger? logger = null)
            where TModel : class
        {
            var collection = _database.GetCollection<TModel>(collectionName);

            return new ModelRepository<TModel>(logger ?? _logger, this, collection);
        }

        public virtual void StartTransaction()
        {
            using var _ = _logger.BeginScope("Start new mongodb transaction flow");

            try
            {
                if (CurrentSession == null)
                    throw new InvalidOperationException("The mongodb session was not start yet");

                _logger.LogInformation("Starting the new transaction");
                CurrentSession.StartTransaction();
                _logger.LogInformation("The new transaction was started");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error was occured in the start mongodb transaction flow");
                throw;
            }
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Commit mongodb transaction flow");
            
            try
            {
                if (CurrentSession == null)
                    throw new InvalidOperationException("The mongodb session was not start yet");

                _logger.LogInformation("Committing the mongodb transaction");
                await CurrentSession!.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("The transaction was committed");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error was occured in the commit mongodb transaction flow");
                throw;
            }
        }

        public virtual async Task AbortTransactionAsync(CancellationToken cancellationToken)
        {
            using var _ = _logger.BeginScope("Abort mongodb transaction flow");
            
            try
            {
                if (CurrentSession == null)
                    throw new InvalidOperationException("The mongodb session was not start yet");

                _logger.LogInformation("Aborting the mongodb transaction");
                await CurrentSession!.AbortTransactionAsync(cancellationToken);
                _logger.LogInformation("The transaction was aborted");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error was occured in the abort mongodb transaction flow");
                throw;
            }
        }
        
        public virtual void Dispose()
        {
            _logger.LogInformation("Disposing the mongodb session");
            _session!.Dispose();
        }
    }
}
