using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    public class ModelRepository<TModel> : IModelRepository<TModel>
        where TModel : class
    {
        protected readonly ILogger _logger;
        protected readonly IMongoDbContext _context;
        protected readonly IMongoCollection<TModel> _collection;

        public ModelRepository(
            ILogger logger,
            IMongoDbContext context,
            IMongoCollection<TModel> collection)
        {
            _logger = logger;
            _context = context;
            _collection = collection;
        }

        public virtual Task<TModel?> FindByIdAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            return _collection
                .Find(_context.CurrentSession, Builders<TModel>.Filter.Eq(idSelctor, id))
                .FirstOrDefaultAsync(cancellationToken)
                .ContinueWith(task =>
                {
                    switch (task.IsCompletedSuccessfully)
                    {
                        case true when task.Result != null:
                            _logger.LogInformation("The {Model} model with {Id} id was found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                            break;
                        case true when task.Result == null:
                            _logger.LogWarning("The {Model} model with {Id} id was not found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                            break;
                        case false:
                            _logger.LogError(task.Exception, "The {Model} model with {Id} id find operation was failed", typeof(TModel), id);
                            break;
                    };

                    return task.Result;
                });
        }

        public virtual Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelctor.Compile().Invoke(model);

            _logger.LogInformation("Inserting one {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            return _collection
                .InsertOneAsync(_context.CurrentSession, model, cancellationToken: cancellationToken)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                        _logger.LogInformation("The {Model} model with {Id} id insertion in mongo {Collection} collection was completed", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                    else
                        _logger.LogError(task.Exception, "The {Model} model with {Id} id insertion in mongo {Collection} collection was failed", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

                    return task;
                });
        }

        public virtual Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelctor, IEnumerable<TModel> entities, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inserting many {Model} entities in mongo {Collection} collection", typeof(TModel), _collection.CollectionNamespace.CollectionName);

            return _collection
                .InsertManyAsync(_context.CurrentSession, entities, cancellationToken: cancellationToken)
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                        _logger.LogInformation("The {Model} entities insertion in mongo {Collection} collection was completed", typeof(TModel), _collection.CollectionNamespace.CollectionName);
                    else
                        _logger.LogError(task.Exception, "The {Model} entities insertion in mongo {Collection} collection was failed", typeof(TModel), _collection.CollectionNamespace.CollectionName);

                    return task;
                });
        }

        public virtual Task<TModel?> UpdateAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelctor.Compile().Invoke(model);

            _logger.LogInformation("Updating the {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            return _collection
                .FindOneAndReplaceAsync(
                    _context.CurrentSession,
                    Builders<TModel>.Filter.Eq(idSelctor, id),
                    model,
                    cancellationToken: cancellationToken)
                .ContinueWith(task =>
                {
                    switch (task.IsCompletedSuccessfully)
                    {
                        case true when task.Result != null:
                            _logger.LogInformation("The {Model} model with {Id} id was was updated in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                            break;
                        case true when task.Result == null:
                            _logger.LogWarning("The {Model} model with {Id} id was not found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                            break;
                        case false:
                            _logger.LogError(task.Exception, "The {Model} model with {Id} id update operation was failed", typeof(TModel), idSelctor);
                            break;
                    };

                    return task.Result;
                });
        }

        public virtual Task<TModel?> DeleteAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting the {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            return _collection
                .FindOneAndDeleteAsync(
                    _context.CurrentSession,
                    Builders<TModel>.Filter.Eq(idSelctor, id),
                    cancellationToken: cancellationToken)
                .ContinueWith(task =>
                {
                    switch (task.IsCompletedSuccessfully)
                    {
                        case true when task.Result != null:
                            _logger.LogInformation("The {Model} model with {Id} id was was deleted in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                            break;
                        case true when task.Result == null:
                            _logger.LogWarning("The {Model} model with {Id} id was not found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                            break;
                        case false:
                            _logger.LogError(task.Exception, "The {Model} model with {Id} id delete operation was failed", typeof(TModel), id);
                            break;
                    };

                    return task.Result;
                });
        }
    }
}
