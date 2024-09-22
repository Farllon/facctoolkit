using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.Abstractions
{
    public abstract class ModelRepository<TModel>
        where TModel : class
    {
        protected readonly ILogger _logger;
        protected Expression<Func<TModel, bool>>? _queryFilter;

        protected ModelRepository(ILogger logger)
        {
            _logger = logger;
        }

        public virtual async Task<TModel?> FindByIdAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Finding {Model} model with {Id} in database", typeof(TModel), id);

            try
            {
                var result = await InternalFindByIdAsync(idSelector, id, cancellationToken).ConfigureAwait(false);

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

        public virtual async Task InsertAsync<TId>(Func<TModel, TId> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector(model);

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

        public virtual async Task InsertAsync<TId>(IEnumerable<TModel> models, CancellationToken cancellationToken)
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

        public virtual async Task<TModel?> UpdateAsync<TId>(Func<TModel, TId> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector(model);

            _logger.LogInformation("Updating the {Model} model with {Id} id in database", typeof(TModel), id);

            try
            {
                var result = await InternalUpdateAsync(idSelector, model, cancellationToken).ConfigureAwait(false);

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

        public virtual async Task<TModel?> DeleteAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting the {Model} model with {Id} id in database", typeof(TModel), id);

            try
            {
                var result = await InternalDeleteAsync(idSelector, id, cancellationToken).ConfigureAwait(false);

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

        protected abstract Task<TModel?> InternalFindByIdAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken);

        protected abstract Task InternalInsertAsync(TModel model, CancellationToken cancellationToken);

        protected abstract Task InternalInsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken);

        protected abstract Task<TModel?> InternalUpdateAsync<TId>(Func<TModel, TId> idSelector, TModel model, CancellationToken cancellationToken);

        protected abstract Task<TModel?> InternalDeleteAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken);

        protected Expression<Func<TModel, bool>> GetFilter(Expression<Func<TModel, bool>> newExpression)
        {
            if (_queryFilter == null)
            {
                return newExpression;
            }

            var parameter = Expression.Parameter(typeof(TModel));

            var leftVisitor = new ReplaceExpressionVisitor(_queryFilter.Parameters[0], parameter);
            var left = leftVisitor.Visit(_queryFilter.Body);

            var rightVisitor = new ReplaceExpressionVisitor(newExpression.Parameters[0], parameter);
            var right = rightVisitor.Visit(newExpression.Body);

            var andExpression = Expression.AndAlso(left, right);
            return Expression.Lambda<Func<TModel, bool>>(andExpression, parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _oldValue)
                    return _newValue;

                return base.VisitParameter(node);
            }
        }
    }
}
