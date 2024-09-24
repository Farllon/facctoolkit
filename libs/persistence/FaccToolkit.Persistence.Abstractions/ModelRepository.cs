using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Abstractions
{
    public abstract class ModelRepository<TModel, TId>
        where TModel : class
        where TId : IEquatable<TId>
    {
        protected readonly ILogger _logger;

        protected ModelRepository(ILogger logger)
        {
            _logger = logger;
        }

        public virtual async Task<TModel?> FindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding {Model} model with {Id} in database", typeof(TModel), id);

            try
            {
                var result = await InternalFindByIdAsync(id, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    _logger.LogInformation("The {Model} model with {Id} id was found in database", typeof(TModel), id);
                }
                else
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found in database", typeof(TModel), id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id find operation in database was failed", typeof(TModel), id);

                throw;
            }
        }

        public virtual async Task InsertAsync(TModel model, CancellationToken cancellationToken)
        {
            var id = GetId(model);

            _logger.LogInformation("Inserting one {Model} model with {Id} id in database", typeof(TModel), id);

            try
            {
                await InternalInsertAsync(model, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("The {Model} model with {Id} id insertion in database was completed", typeof(TModel), id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id insertion in database was failed", typeof(TModel), id);

                throw;
            }
        }

        public virtual async Task InsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Inserting many {Model} models in database", typeof(TModel));

            try
            {
                await InternalInsertAsync(models, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("The {Model} models insertion in database was completed", typeof(TModel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} models insertion in database was failed", typeof(TModel));

                throw;
            }
        }

        public virtual async Task<TModel?> UpdateAsync(TModel model, CancellationToken cancellationToken)
        {
            var id = GetId(model);

            _logger.LogInformation("Updating the {Model} model with {Id} id in database", typeof(TModel), id);

            try
            {
                var result = await InternalUpdateAsync(model, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    _logger.LogInformation("The {Model} model with {Id} id was updated in database", typeof(TModel), id);
                }
                else
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found in database", typeof(TModel), id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id update operation in database was failed", typeof(TModel), id);

                throw;
            }
        }

        public virtual async Task<TModel?> DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting the {Model} model with {Id} id in database", typeof(TModel), id);

            try
            {
                var result = await InternalDeleteAsync(id, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    _logger.LogInformation("The {Model} model with {Id} id was deleted in database", typeof(TModel), id);
                }
                else
                {
                    _logger.LogWarning("The {Model} model with {Id} id was not found in database", typeof(TModel), id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The {Model} model with {Id} id delete operation in database was failed", typeof(TModel), id);

                throw;
            }
        }

        protected abstract Task<TModel?> InternalFindByIdAsync(TId id, CancellationToken cancellationToken);

        protected abstract Task InternalInsertAsync(TModel model, CancellationToken cancellationToken);

        protected abstract Task InternalInsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken);

        protected abstract Task<TModel?> InternalUpdateAsync(TModel model, CancellationToken cancellationToken);

        protected abstract Task<TModel?> InternalDeleteAsync(TId id, CancellationToken cancellationToken);

        protected abstract TId GetId(TModel model);
    }
}
