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

        public virtual async Task<TModel?> FindByIdAsync<TId>(Expression<Func<TModel, TId>> idSelector, TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            try
            {
                var result = await _collection
                    .Find(_context.CurrentSession, Builders<TModel>.Filter.Eq(idSelector, id))
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (result != null)
                {
                    _logger.LogInformation("The {Model} model with {Id} id was found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                }
                else
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id find operation in mongo {Collection} collection failed", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                throw;
            }
        }

        public virtual async Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector.Compile().Invoke(model);

            _logger.LogInformation("Inserting one {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            try
            {
                await _collection
                    .InsertOneAsync(_context.CurrentSession, model, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogInformation("The {Model} model with {Id} id insertion in mongo {Collection} collection was completed", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id insertion in mongo {Collection} collection failed", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                throw;
            }
        }

        public virtual async Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelector, IEnumerable<TModel> models, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inserting many {Model} models in mongo {Collection} collection", typeof(TModel), _collection.CollectionNamespace.CollectionName);

            try
            {
                await _collection
                    .InsertManyAsync(_context.CurrentSession, models, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                _logger.LogInformation("The {Model} models insertion in mongo {Collection} collection was completed", typeof(TModel), _collection.CollectionNamespace.CollectionName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} models insertion in mongo {Collection} collection failed", typeof(TModel), _collection.CollectionNamespace.CollectionName);
                throw;
            }
        }

        public virtual async Task<TModel?> UpdateAsync<TId>(Expression<Func<TModel, TId>> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector.Compile().Invoke(model);

            _logger.LogInformation("Updating the {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            try
            {
                var result = await _collection
                    .FindOneAndReplaceAsync(
                        _context.CurrentSession,
                        Builders<TModel>.Filter.Eq(idSelector, id),
                        model,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (result != null)
                {
                    _logger.LogInformation("The {Model} model with {Id} id was updated in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                }
                else
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id update operation in mongo {Collection} collection failed", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                throw;
            }
        }

        public virtual async Task<TModel?> DeleteAsync<TId>(Expression<Func<TModel, TId>> idSelector, TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting the {Model} model with {Id} id in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);

            try
            {
                var result = await _collection
                    .FindOneAndDeleteAsync(
                        _context.CurrentSession,
                        Builders<TModel>.Filter.Eq(idSelector, id),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (result != null)
                {
                    _logger.LogInformation("The {Model} model with {Id} id was deleted in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                }
                else
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found in mongo {Collection} collection", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id delete operation in mongo {Collection} collection failed", typeof(TModel), id, _collection.CollectionNamespace.CollectionName);
                throw;
            }
        }
    }
}